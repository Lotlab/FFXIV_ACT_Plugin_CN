#!/usr/bin/env python3
"""Rebuild ``cn/resource`` from a CN release zip.

``cn/resource`` is normally *generated* from the ffxiv-datamining-cn CSV dump by
``tools/generate_cn_resources.py``; use that for routine updates.  This script is the
bootstrap / cross-check path: it answers "which payloads does a CN build replace?" for
whatever CN release you point it at, which is what ``generate_cn_resources.py --check``
compares against.

1. unpack ``FFXIV_ACT_Plugin.dll`` from the CN zip and extract its Costura payloads;
2. dump every embedded resource of the CN ``FFXIV_ACT_Plugin.Resource.dll``;
3. dump the same resources from the *raw* upstream Resource assembly that
   ``tools/prepare_upstream.py`` left in ``.work/upstream/<version>/raw``;
4. copy the differing (and CN-only) payloads into ``cn/resource/`` and delete files
   that are no longer part of the CN set.

Typical use when moving to a new CN client release:

    python tools/generate_cn_resources.py --csv ../ffxiv-automatic-data/csv
    python tools/prepare_upstream.py

and, when you want to diff the generated data against a real CN build:

    python tools/extract_cn_reference.py path/to/FFXIV_ACT_Plugin_CN.zip
    python tools/generate_cn_resources.py --check .work/bin
"""
import argparse
import json
import os
import shutil
import subprocess
import sys
import zipfile

REPO = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
UPSTREAM = os.path.join(REPO, "upstream")
CN_RESOURCE = os.path.join(REPO, "cn", "resource")
WORK = os.path.join(REPO, ".work", "cn-resources")
PATCHER_PROJECT = os.path.join(REPO, "tools", "AssemblyPatcher", "AssemblyPatcher.csproj")
PATCHER = os.path.join(REPO, "artifacts", "bin", "Release", "AssemblyPatcher.dll")


def log(message):
    print(message, flush=True)


def run(args):
    result = subprocess.run(args)
    if result.returncode != 0:
        raise SystemExit("command failed: %s" % " ".join(args))


def read_tree(directory):
    out = {}
    for name in os.listdir(directory):
        path = os.path.join(directory, name)
        if os.path.isfile(path):
            with open(path, "rb") as handle:
                out[name] = handle.read()
    return out


def main():
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("cn_zip", help="CN release zip containing FFXIV_ACT_Plugin.dll")
    parser.add_argument("--version", help="upstream version whose raw assemblies to compare against")
    parser.add_argument("--dry-run", action="store_true", help="report without writing cn/resource")
    args = parser.parse_args()

    with open(os.path.join(UPSTREAM, "lock.json"), encoding="utf-8") as handle:
        lock = json.load(handle)
    version = args.version or lock["version"]

    stamp = os.path.join(REPO, "artifacts", "upstream", "current", "stamp.json")
    if os.path.exists(stamp):
        with open(stamp, encoding="utf-8") as handle:
            version = args.version or json.load(handle)["version"]

    raw_resource = os.path.join(
        REPO, ".work", "upstream", version, "raw", "ffxiv_act_plugin.resource.dll")
    if not os.path.exists(raw_resource):
        raise SystemExit(
            "missing %s - run: python tools/prepare_upstream.py%s"
            % (os.path.relpath(raw_resource, REPO), "" if args.version is None else " --version " + version))

    shutil.rmtree(WORK, ignore_errors=True)
    os.makedirs(WORK)
    plugin = os.path.join(WORK, "FFXIV_ACT_Plugin.dll")
    with zipfile.ZipFile(args.cn_zip) as zip_file:
        with zip_file.open("FFXIV_ACT_Plugin.dll") as source, open(plugin, "wb") as target:
            shutil.copyfileobj(source, target)

    run(["dotnet", "build", PATCHER_PROJECT, "-c", "Release", "-v", "q"])
    if not os.path.exists(PATCHER):
        raise SystemExit("AssemblyPatcher not found at %s" % PATCHER)

    run(["dotnet", PATCHER, "extract", plugin, os.path.join(WORK, "raw")])
    cn_resource = os.path.join(WORK, "raw", "ffxiv_act_plugin.resource.dll")
    run(["dotnet", PATCHER, "resources", cn_resource, os.path.join(WORK, "cn")])
    run(["dotnet", PATCHER, "resources", raw_resource, os.path.join(WORK, "upstream")])

    cn = read_tree(os.path.join(WORK, "cn"))
    upstream = read_tree(os.path.join(WORK, "upstream"))
    changed = sorted(name for name, data in cn.items() if upstream.get(name) != data)
    removed = sorted(set(upstream) - set(cn))
    stale = sorted(
        name for name in os.listdir(CN_RESOURCE)
        if os.path.isfile(os.path.join(CN_RESOURCE, name)) and name not in changed
    )

    log("cn resources              %4d" % len(cn))
    log("upstream resources        %4d" % len(upstream))
    log("cn-specific (write)       %4d" % len(changed))
    log("removed by CN (warn)      %4d" % len(removed))
    log("stale in cn/resource      %4d" % len(stale))
    for name in changed:
        log("  write %s" % name)
    for name in removed:
        log("  WARN  %s exists upstream but not in the CN build; the patcher cannot delete it yet" % name)
    for name in stale:
        log("  remove %s" % name)

    if args.dry_run:
        log("\ndry run: nothing written")
        return

    os.makedirs(CN_RESOURCE, exist_ok=True)
    for name in stale:
        os.remove(os.path.join(CN_RESOURCE, name))
    for name in changed:
        with open(os.path.join(CN_RESOURCE, name), "wb") as handle:
            handle.write(cn[name])

    log("\ncn/resource now has %d payloads" % len(os.listdir(CN_RESOURCE)))


if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        sys.exit(130)
