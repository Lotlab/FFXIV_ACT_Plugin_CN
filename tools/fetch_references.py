"""Populate the (git-ignored) refs/ directory with the compile-time reference assemblies.

This repository deliberately contains no binaries, so the four assemblies the build
needs are materialised here on demand:

    refs/Advanced Combat Tracker.exe   ACT host assembly   (compile-time reference only)
    refs/Machina.dll                   network capture
    refs/Machina.FFXIV.dll             FFXIV network headers
    refs/Newtonsoft.Json.dll           JSON

Where they come from
--------------------
* **Machina / Machina.FFXIV** are built from the `third_party/machina` git submodule,
  pinned to the commit that produces the versions the plugin expects:

      ravahn/machina @ 7a1e7c4620fbbf40d68927239413848a0f50b423   "v2.4.7.1"
      Machina.csproj       -> 2.3.1.3   (netstandard2.0)
      Machina.FFXIV.csproj -> 2.4.7.1   (netstandard2.0)

  A Release build is unsigned, matching what the plugin was compiled against (both
  embedded assemblies have `PublicKeyToken=null`).  Signing only happens in the
  `NuGet` configuration; `Machina` also references a `.snk` that is not in the
  repository, so the Release configuration is the only one that builds as-is.

* **Newtonsoft.Json** is taken from nuget.org; 13.0.1 is byte-identical to what the
  plugin shipped with.
* **Advanced Combat Tracker** comes from nuget.org (`AdvancedCombatTracker`).

Exactness caveat
----------------
The plugin that was shipped embeds *fork builds* of Machina and Machina.FFXIV: the
Chinese-client fork adds `Machina.FFXIV.Headers.Chinese.Server_SkillType` and
`...Server_StatusEffectListForay3`, and `Machina.Infrastructure.ConcurrentList`, none of
which exist upstream.  Building upstream therefore differs from the shipped binaries by
82 + 37 declarations.  The plugin's own source does not reference any of them (it binds
`Headers.Server_StatusEffectListForay3`, which upstream does have) so it compiles and
runs, but Simplified-Chinese packet decoding would go through upstream's headers.

Pass `--from-distribution` to extract the exact fork assemblies from the shipped
distribution archive instead, e.g. to compare behaviour:

    https://github.com/TundraWork/FFXIV_ACT_Plugin_CN/releases/download/26.6.9.0/FFXIV_ACT_Plugin_3.0.2.1_CN.zip
    sha256 94a9364bd3a714dc1eaa355d3f3d75ea63de1a2bd3dbaffbe21b2f2dbebde844

Usage
-----
    python tools/fetch_references.py                       # submodule + nuget
    python tools/fetch_references.py --from-distribution   # exact fork assemblies instead
    python tools/fetch_references.py --archive path/to.zip
    python tools/fetch_references.py --from-dir D:/Some/PluginFolder
    python tools/fetch_references.py --act-path "C:/.../Advanced Combat Tracker.exe"
    python tools/fetch_references.py --check               # only report what is there
"""
import argparse
import glob
import hashlib
import os
import shutil
import subprocess
import sys
import tempfile
import urllib.request
import zipfile
import zlib

REPO = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
REFS = os.path.join(REPO, "refs")

# --------------------------------------------------------------------------- machina
MACHINA_SUBMODULE = os.path.join(REPO, "third_party", "machina")
MACHINA_URL = "https://github.com/ravahn/machina.git"
MACHINA_COMMIT = "7a1e7c4620fbbf40d68927239413848a0f50b423"
MACHINA_PROJECT = os.path.join("Machina.FFXIV", "Machina.FFXIV.csproj")
MACHINA_OUTPUTS = {
    "Machina.dll": os.path.join("Machina", "bin", "Release", "netstandard2.0", "Machina.dll"),
    "Machina.FFXIV.dll": os.path.join("Machina.FFXIV", "bin", "Release",
                                      "netstandard2.0", "Machina.FFXIV.dll"),
}

# --------------------------------------------------------------------------- distribution
DISTRIBUTION_TAG = "26.6.9.0"
DISTRIBUTION_REPO = "TundraWork/FFXIV_ACT_Plugin_CN"
DISTRIBUTION_ASSET = "FFXIV_ACT_Plugin_3.0.2.1_CN.zip"
DISTRIBUTION_URL = "https://github.com/%s/releases/download/%s/%s" % (
    DISTRIBUTION_REPO, DISTRIBUTION_TAG, DISTRIBUTION_ASSET)
DISTRIBUTION_SHA256 = "94a9364bd3a714dc1eaa355d3f3d75ea63de1a2bd3dbaffbe21b2f2dbebde844"
DISTRIBUTION_CACHE = os.path.join(REPO, ".work", "reference", DISTRIBUTION_ASSET)

# --------------------------------------------------------------------------- nuget
ACT_PACKAGE = ("advancedcombattracker", "3.8.4.287")
JSON_PACKAGE = ("newtonsoft.json", "13.0.1")

# Costura payload name inside the distribution -> refs/ file name
COSTURA_SOURCES = {
    "machina.dll": "Machina.dll",
    "machina.ffxiv.dll": "Machina.FFXIV.dll",
    "newtonsoft.json.dll": "Newtonsoft.Json.dll",
}

EXPECTED = {
    "Advanced Combat Tracker.exe": "3.8.5.288 (any 3.8.x accepted; not strong-named)",
    "Machina.dll": "2.3.1.3",
    "Machina.FFXIV.dll": "2.4.7.1",
    "Newtonsoft.Json.dll": "13.0.0.0",
}

ILSPY = os.path.join(os.path.expanduser("~"), ".dotnet", "tools", "ilspycmd.exe")


def log(msg):
    print(msg, flush=True)


def run(args, **kw):
    return subprocess.run(args, capture_output=True, **kw)


def assembly_version(path):
    """Best-effort AssemblyName.Version via PowerShell (Windows-only project)."""
    if os.name != "nt":
        return None
    try:
        out = run(["powershell", "-NoProfile", "-Command",
                   "[System.Reflection.AssemblyName]::GetAssemblyName('%s').Version.ToString()"
                   % path.replace("'", "''")])
        return out.stdout.decode("utf-8", "replace").strip() or None
    except OSError:
        return None


def sha256_file(path):
    h = hashlib.sha256()
    with open(path, "rb") as fh:
        for chunk in iter(lambda: fh.read(1 << 20), b""):
            h.update(chunk)
    return h.hexdigest()


def download(url):
    log("    GET %s" % url)
    with urllib.request.urlopen(url) as response:
        return response.read()


# --------------------------------------------------------------------------- machina

def ensure_submodule():
    """Make sure third_party/machina is checked out at the pinned commit."""
    if not os.path.isdir(MACHINA_SUBMODULE):
        run(["git", "submodule", "update", "--init", "--recursive", "third_party/machina"],
            cwd=REPO)
    if not os.path.exists(os.path.join(MACHINA_SUBMODULE, "Machina.sln")):
        log("  initialising the machina submodule")
        run(["git", "submodule", "update", "--init", "--recursive", "third_party/machina"],
            cwd=REPO)
    if not os.path.exists(os.path.join(MACHINA_SUBMODULE, "Machina.sln")):
        return False
    head = run(["git", "rev-parse", "HEAD"], cwd=MACHINA_SUBMODULE).stdout.decode().strip()
    if head != MACHINA_COMMIT:
        log("  WARNING submodule is at %s, expected %s" % (head[:12], MACHINA_COMMIT[:12]))
    return True


def machina_from_source(out):
    if not ensure_submodule():
        log("  machina submodule is not available; run:")
        log("    git submodule update --init --recursive third_party/machina")
        return False
    log("  building %s @ %s" % (MACHINA_URL, MACHINA_COMMIT[:12]))
    result = run(["dotnet", "build", MACHINA_PROJECT, "-c", "Release", "--nologo", "-v", "q"],
                 cwd=MACHINA_SUBMODULE)
    if result.returncode != 0:
        sys.stderr.write(result.stdout.decode("utf-8", "replace"))
        sys.stderr.write(result.stderr.decode("utf-8", "replace"))
        log("  machina build FAILED")
        return False
    ok = True
    for target, relative in MACHINA_OUTPUTS.items():
        source = os.path.join(MACHINA_SUBMODULE, relative)
        if not os.path.exists(source):
            log("    %-22s not produced" % target)
            ok = False
            continue
        shutil.copyfile(source, os.path.join(out, target))
        log("    %-22s %8d B -> %s" % (os.path.basename(relative), os.path.getsize(source), target))
    return ok


# --------------------------------------------------------------------------- distribution

def from_directory(src, out):
    copied = []
    for name in EXPECTED:
        candidate = os.path.join(src, name)
        if os.path.exists(candidate):
            shutil.copyfile(candidate, os.path.join(out, name))
            copied.append(name)
    return copied


def from_archive(archive, out):
    """Extract the Costura payloads embedded in a shipped FFXIV_ACT_Plugin.dll."""
    if not os.path.exists(ILSPY):
        raise SystemExit("need the ilspycmd .NET tool to unpack %s\n"
                         "  dotnet tool install --global ilspycmd" % archive)
    tmp = tempfile.mkdtemp(prefix="refs-")
    try:
        with zipfile.ZipFile(archive) as zf:
            plugin = None
            for entry in zf.namelist():
                if os.path.basename(entry).lower() == "ffxiv_act_plugin.dll":
                    plugin = os.path.join(tmp, os.path.basename(entry))
                    with zf.open(entry) as src, open(plugin, "wb") as dst:
                        shutil.copyfileobj(src, dst)
                    break
            if plugin is None:
                raise SystemExit("%s does not contain FFXIV_ACT_Plugin.dll" % archive)

            copied = []
            for payload, target in COSTURA_SOURCES.items():
                resource = "costura.%s.compressed" % payload
                run([ILSPY, "--resource", resource, "-o", tmp, plugin])
                blob = os.path.join(tmp, resource)
                if not os.path.exists(blob):
                    log("    %-22s not embedded in this build" % payload)
                    continue
                data = open(blob, "rb").read()
                for wbits in (-15, 15 + 16, 15):
                    try:
                        raw = zlib.decompress(data, wbits)
                        break
                    except zlib.error:
                        continue
                else:
                    log("    %-22s could not be inflated" % payload)
                    continue
                open(os.path.join(out, target), "wb").write(raw)
                copied.append(target)
                log("    %-22s %8d B -> %s" % (payload, len(raw), target))
            return copied
    finally:
        shutil.rmtree(tmp, ignore_errors=True)


def archive_from_github():
    if os.path.exists(DISTRIBUTION_CACHE) and sha256_file(DISTRIBUTION_CACHE) == DISTRIBUTION_SHA256:
        log("  using cached %s" % DISTRIBUTION_CACHE)
        return DISTRIBUTION_CACHE
    os.makedirs(os.path.dirname(DISTRIBUTION_CACHE), exist_ok=True)
    log("  downloading the shipped distribution")
    data = download(DISTRIBUTION_URL)
    digest = hashlib.sha256(data).hexdigest()
    if digest != DISTRIBUTION_SHA256:
        raise SystemExit("downloaded archive has sha256 %s, expected %s\n"
                         "(refusing to use it; pass --archive to supply one yourself)"
                         % (digest, DISTRIBUTION_SHA256))
    open(DISTRIBUTION_CACHE, "wb").write(data)
    log("  verified sha256 %s (%d bytes)" % (digest[:16] + "...", len(data)))
    return DISTRIBUTION_CACHE


def default_archive():
    found = sorted(glob.glob(os.path.join(REPO, "reference", "*.zip")))
    return found[0] if found else None


# --------------------------------------------------------------------------- nuget

def nupkg_assembly(package_id, version, match, out_name, out):
    log("    %s %s from nuget.org" % (package_id, version))
    data = download("https://api.nuget.org/v3-flatcontainer/%s/%s/%s.%s.nupkg"
                    % (package_id, version, package_id, version))
    tmp = tempfile.mkdtemp(prefix="nuget-")
    try:
        nupkg = os.path.join(tmp, "p.nupkg")
        open(nupkg, "wb").write(data)
        with zipfile.ZipFile(nupkg) as zf:
            for entry in zf.namelist():
                if entry.lower().endswith(match):
                    with zf.open(entry) as src, open(os.path.join(out, out_name), "wb") as dst:
                        shutil.copyfileobj(src, dst)
                    return True
    finally:
        shutil.rmtree(tmp, ignore_errors=True)
    return False


# --------------------------------------------------------------------------- report

def report(out):
    log("")
    log("refs/ contents")
    have_all = True
    for name, expected in EXPECTED.items():
        path = os.path.join(out, name)
        if not os.path.exists(path):
            have_all = False
            log("  MISSING  %-28s expected %s" % (name, expected))
            continue
        log("  OK       %-28s %-8s %9d B   expected %s"
            % (name, assembly_version(path) or "?", os.path.getsize(path), expected))
    return have_all


def main():
    ap = argparse.ArgumentParser(description=__doc__,
                                 formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--out", default=REFS, help="output directory (default: refs/)")
    ap.add_argument("--from-distribution", action="store_true",
                    help="take Machina/Machina.FFXIV (and Newtonsoft.Json) from the shipped "
                         "distribution instead of building the submodule - exact fork builds")
    ap.add_argument("--from-dir", help="copy the assemblies from this directory instead")
    ap.add_argument("--archive", help="plugin distribution ZIP to use with --from-distribution")
    ap.add_argument("--no-download", action="store_true",
                    help="never download the distribution archive")
    ap.add_argument("--act-path", help="Advanced Combat Tracker.exe to copy")
    ap.add_argument("--check", action="store_true", help="only report what is present")
    args = ap.parse_args()

    out = os.path.abspath(args.out)
    os.makedirs(out, exist_ok=True)

    if args.check:
        return 0 if report(out) else 1

    log("populating %s" % out)

    want_distribution = args.from_distribution or bool(args.archive) or bool(args.from_dir)

    if args.from_dir:
        log("  from directory %s" % args.from_dir)
        from_directory(args.from_dir, out)
    elif want_distribution:
        archive = args.archive or default_archive()
        if not archive and not args.no_download:
            archive = archive_from_github()
        if archive and os.path.exists(archive):
            log("  from distribution archive %s" % archive)
            from_archive(archive, out)
        else:
            log("  no distribution archive available")
    else:
        log("  Machina / Machina.FFXIV from the submodule source")
        machina_from_source(out)
        nupkg_assembly(JSON_PACKAGE[0], JSON_PACKAGE[1],
                       "lib/net45/newtonsoft.json.dll", "Newtonsoft.Json.dll", out)

    if not os.path.exists(os.path.join(out, "Newtonsoft.Json.dll")):
        nupkg_assembly(JSON_PACKAGE[0], JSON_PACKAGE[1],
                       "lib/net45/newtonsoft.json.dll", "Newtonsoft.Json.dll", out)

    if not os.path.exists(os.path.join(out, "Advanced Combat Tracker.exe")):
        if args.act_path and os.path.exists(args.act_path):
            shutil.copyfile(args.act_path, os.path.join(out, "Advanced Combat Tracker.exe"))
            log("    ACT from --act-path")
        else:
            log("  ACT host assembly")
            nupkg_assembly(ACT_PACKAGE[0], ACT_PACKAGE[1],
                           "lib/net45/advanced combat tracker.exe", "Advanced Combat Tracker.exe", out)

    if not report(out):
        log("")
        log("The build needs all four files.  See the module docstring for the sources.")
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
