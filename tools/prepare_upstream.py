#!/usr/bin/env python3
"""Fetch a ravahn release and turn it into the assemblies we compile against.

Pipeline
--------
1. Download ``FFXIV_ACT_Plugin_<version>.zip`` (cached under ``.work/cache``) and
   verify the sha256 recorded in ``upstream/lock.json``.
2. Unpack ``FFXIV_ACT_Plugin.dll`` and extract the ten Costura payloads with
   ``tools/AssemblyPatcher``.
3. Strip the strong name from the six ``FFXIV_ACT_Plugin.*`` assemblies and rewrite
   the references between them (see README section 3).
4. Apply ``upstream/patch.json``: Config fields, Logfile FormatVersion, Parse
   strings/enum, and the CN resource payloads from ``cn/resource``.
5. Copy the result to ``artifacts/upstream/current/`` where the project files expect
   it (``$(UpstreamDir)``).  The path has no version in it, so moving to a newer
   upstream release only touches ``upstream/lock.json`` (or ``--version``).

``--version`` is a trial switch: it downloads that release, skips the recorded hash
check and prints the sha256 so you can put it into ``upstream/lock.json``.

Requires the .NET SDK (10.x) and network access on the first run.
"""
import argparse
import hashlib
import json
import os
import shutil
import subprocess
import sys
import urllib.request
import zipfile

REPO = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
UPSTREAM = os.path.join(REPO, "upstream")
WORK = os.path.join(REPO, ".work", "upstream")
CACHE = os.path.join(REPO, ".work", "cache")
PATCHER_PROJECT = os.path.join(REPO, "tools", "AssemblyPatcher", "AssemblyPatcher.csproj")
PATCHER = os.path.join(REPO, "artifacts", "bin", "Release", "AssemblyPatcher.dll")

# assembly name -> file name inside the Costura payload set
ASSEMBLY_FILES = {
    "FFXIV_ACT_Plugin.Common": "ffxiv_act_plugin.common.dll",
    "FFXIV_ACT_Plugin.Config": "ffxiv_act_plugin.config.dll",
    "FFXIV_ACT_Plugin.Logfile": "ffxiv_act_plugin.logfile.dll",
    "FFXIV_ACT_Plugin.Memory": "ffxiv_act_plugin.memory.dll",
    "FFXIV_ACT_Plugin.Network": "ffxiv_act_plugin.network.dll",
    "FFXIV_ACT_Plugin.Parse": "ffxiv_act_plugin.parse.dll",
    "FFXIV_ACT_Plugin.Resource": "ffxiv_act_plugin.resource.dll",
}


def log(message):
    print(message, flush=True)


def run(args, **kwargs):
    result = subprocess.run(args, **kwargs)
    if result.returncode != 0:
        raise SystemExit("command failed: %s" % " ".join(args))


def sha256(path):
    digest = hashlib.sha256()
    with open(path, "rb") as handle:
        for chunk in iter(lambda: handle.read(1 << 20), b""):
            digest.update(chunk)
    return digest.hexdigest()


def download(repo, version, asset_name, expected, target):
    """GitHub release assets are fetched through the API; the browser URL is unreliable."""
    if os.path.exists(target):
        actual = sha256(target)
        if expected is None or actual == expected:
            log("  cache hit: %s" % os.path.relpath(target, REPO))
            return actual
        os.remove(target)

    os.makedirs(os.path.dirname(target), exist_ok=True)
    api = "https://api.github.com/repos/%s/releases/tags/%s" % (repo, version)
    headers = {"Accept": "application/vnd.github+json", "User-Agent": "prepare-upstream"}
    with urllib.request.urlopen(urllib.request.Request(api, headers=headers), timeout=60) as response:
        release = json.load(response)

    assets = [a for a in release.get("assets", []) if a["name"] == asset_name]
    if not assets:
        raise SystemExit("release %s has no asset %s" % (version, asset_name))

    log("  downloading %s (%d bytes)" % (asset_name, assets[0]["size"]))
    request = urllib.request.Request(
        assets[0]["url"],
        headers={"Accept": "application/octet-stream", "User-Agent": "prepare-upstream"},
    )
    with urllib.request.urlopen(request, timeout=300) as response, open(target, "wb") as handle:
        shutil.copyfileobj(response, handle)

    actual = sha256(target)
    if expected is not None and actual != expected:
        os.remove(target)
        raise SystemExit("sha256 mismatch: expected %s, got %s" % (expected, actual))
    return actual


def main():
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument(
        "--version",
        help="upstream release to build instead of the one in upstream/lock.json "
             "(the sha256 is printed so it can be recorded)",
    )
    args = parser.parse_args()

    with open(os.path.join(UPSTREAM, "lock.json"), encoding="utf-8") as handle:
        lock = json.load(handle)
    with open(os.path.join(UPSTREAM, "patch.json"), encoding="utf-8") as handle:
        patch = json.load(handle)

    version = args.version or lock["version"]
    pinned = version == lock["version"]
    asset = lock["asset"] if pinned else "FFXIV_ACT_Plugin_%s.zip" % version
    expected = lock["sha256"] if pinned else None

    work = os.path.join(WORK, version)
    out = os.path.join(REPO, "artifacts", "upstream", "current")
    raw = os.path.join(work, "raw")
    stripped = os.path.join(work, "stripped")

    for directory in (raw, stripped):
        shutil.rmtree(directory, ignore_errors=True)

    log("[1/5] upstream release %s%s" % (version, "" if pinned else " (unpinned trial)"))
    archive = os.path.join(CACHE, asset)
    archive_sha = download(lock["repo"], version, asset, expected, archive)
    if not pinned:
        log("  sha256 %s (update upstream/lock.json to pin this release)" % archive_sha)

    plugin = os.path.join(work, "plugin", "FFXIV_ACT_Plugin.dll")
    os.makedirs(os.path.dirname(plugin), exist_ok=True)
    with zipfile.ZipFile(archive) as zip_file:
        with zip_file.open("FFXIV_ACT_Plugin.dll") as source, open(plugin, "wb") as target:
            shutil.copyfileobj(source, target)
    log("  plugin: %s" % os.path.relpath(plugin, REPO))

    log("[2/5] build AssemblyPatcher")
    run(["dotnet", "build", PATCHER_PROJECT, "-c", "Release", "-v", "q"])
    if not os.path.exists(PATCHER):
        raise SystemExit("AssemblyPatcher not found at %s" % PATCHER)

    log("[3/5] extract Costura payloads")
    run(["dotnet", PATCHER, "extract", plugin, raw])

    log("[4/5] strip strong names")
    run(["dotnet", PATCHER, "strip", raw, stripped, ",".join(patch["strip"])])

    log("[5/5] apply CN patch")
    manifest = {"assemblies": []}
    for entry in patch["assemblies"]:
        assembly = entry["assembly"]
        item = {"path": os.path.join(stripped, ASSEMBLY_FILES[assembly])}
        for key, value in entry.items():
            if key == "assembly":
                continue
            if key == "replaceResourcesFrom":
                directory = os.path.join(UPSTREAM, value)
                item["replaceResources"] = [
                    {"name": name, "file": os.path.join(directory, name)}
                    for name in sorted(os.listdir(directory))
                ]
            else:
                item[key] = value
        manifest["assemblies"].append(item)

    runtime_manifest = os.path.join(work, "patch.json")
    with open(runtime_manifest, "w", encoding="utf-8") as handle:
        json.dump(manifest, handle, ensure_ascii=False, indent=1)
    run(["dotnet", PATCHER, "patch", runtime_manifest])

    shutil.rmtree(out, ignore_errors=True)
    os.makedirs(out, exist_ok=True)
    stamp = {"version": version, "asset": asset, "sha256": archive_sha, "assemblies": {}}
    for assembly in patch["strip"]:
        source = os.path.join(stripped, ASSEMBLY_FILES[assembly])
        target = os.path.join(out, assembly + ".dll")
        shutil.copyfile(source, target)
        digest = sha256(target)
        stamp["assemblies"][assembly] = digest
        log("  %-34s %10d bytes  %s" % (assembly, os.path.getsize(target), digest[:16]))

    with open(os.path.join(out, "stamp.json"), "w", encoding="utf-8") as handle:
        json.dump(stamp, handle, indent=2)

    log("\nready: %s" % os.path.relpath(out, REPO))


if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        sys.exit(130)
