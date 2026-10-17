"""Populate the (git-ignored) refs/ directory with the compile-time reference assemblies.

This repository deliberately contains **no binaries**, so the four assemblies the build
needs are materialised here on demand:

    refs/Advanced Combat Tracker.exe   ACT host assembly   (compile-time reference only)
    refs/Machina.dll                   network capture
    refs/Machina.FFXIV.dll             FFXIV network headers
    refs/Newtonsoft.Json.dll           JSON

Where they can come from
------------------------
`Machina` and `Machina.FFXIV` are **CN-fork builds** that ship inside the plugin
distribution and are not published anywhere:

* nuget.org's newest `Machina.FFXIV` (2.4.7.6/2.4.7.9/2.4.8.1) all differ from the
  embedded 2.4.7.1 by 82 declarations, and are missing exactly the Chinese-client types
  this build uses (`Headers.Chinese.Server_SkillType`,
  `Headers.Chinese.Server_StatusEffectListForay3`, `FFXIVNetworkMonitor.DUMB_DEUCALION_CONNECTION`);
* nuget.org's `Machina` 2.3.1.3 also differs (it lacks
  `Machina.Infrastructure.ConcurrentList` and uses `_monitorTask` for `_processThread`);
* no other public NuGet feed carries `Machina.FFXIV` 2.4.7.1 either (checked
  nuget.azure.cn, MyGet/ravahn, GitHub Packages).

They are therefore taken from the plugin distribution itself, where they are embedded
in `FFXIV_ACT_Plugin.dll` as Costura payloads.  By default that archive is **downloaded
from the public release it came from** and verified against a known SHA-256:

    TundraWork/FFXIV_ACT_Plugin_CN, tag 26.6.9.0
    FFXIV_ACT_Plugin_3.0.2.1_CN.zip   5,591,612 bytes
    sha256 94a9364bd3a714dc1eaa355d3f3d75ea63de1a2bd3dbaffbe21b2f2dbebde844

(The equivalent sources, if you would rather build them: `Machina` and `Machina.FFXIV`
2.4.7.1 are `ravahn/machina` commit `7a1e7c4620fbbf40d68927239413848a0f50b423`, built with
`-p:SignAssembly=false`.)

`Newtonsoft.Json` from that archive is byte-identical to nuget.org 13.0.1, and the ACT
host assembly comes from nuget.org (`AdvancedCombatTracker`).

Usage
-----
    python tools/fetch_references.py                     # download + unpack + nuget
    python tools/fetch_references.py --archive path/to/FFXIV_ACT_Plugin_3.0.2.1_CN.zip
    python tools/fetch_references.py --from-dir D:/Some/PluginFolder
    python tools/fetch_references.py --act-path "C:/.../Advanced Combat Tracker.exe"
    python tools/fetch_references.py --no-download       # never touch the network archive
    python tools/fetch_references.py --allow-inexact     # let nuget supply Machina*
    python tools/fetch_references.py --check             # only report what is there
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

# The public release the shipped binaries came from, used when no local copy is given.
DISTRIBUTION_TAG = "26.6.9.0"
DISTRIBUTION_REPO = "TundraWork/FFXIV_ACT_Plugin_CN"
DISTRIBUTION_ASSET = "FFXIV_ACT_Plugin_3.0.2.1_CN.zip"
DISTRIBUTION_URL = "https://github.com/%s/releases/download/%s/%s" % (
    DISTRIBUTION_REPO, DISTRIBUTION_TAG, DISTRIBUTION_ASSET)
DISTRIBUTION_SHA256 = "94a9364bd3a714dc1eaa355d3f3d75ea63de1a2bd3dbaffbe21b2f2dbebde844"
DISTRIBUTION_CACHE = os.path.join(REPO, ".work", "reference", DISTRIBUTION_ASSET)

ACT_PACKAGE = ("advancedcombattracker", "3.8.4.287")
# name -> (file name in refs/, assembly version the plugin was compiled against)
EXPECTED = {
    "Advanced Combat Tracker.exe": "3.8.5.288 (any 3.8.x accepted; not strong-named)",
    "Machina.dll": "2.3.1.3 (fork build)",
    "Machina.FFXIV.dll": "2.4.7.1 (fork build)",
    "Newtonsoft.Json.dll": "13.0.0.0",
}
# assemblies whose Costura payload we can pull straight out of the distribution
COSTURA_SOURCES = {
    "machina.dll": "Machina.dll",
    "machina.ffxiv.dll": "Machina.FFXIV.dll",
    "newtonsoft.json.dll": "Newtonsoft.Json.dll",
}

ILSPY = os.path.join(os.path.expanduser("~"), ".dotnet", "tools", "ilspycmd.exe")


def log(msg):
    print(msg, flush=True)


def assembly_version(path):
    """Best-effort AssemblyName.Version via PowerShell (Windows-only project)."""
    if os.name != "nt":
        return None
    try:
        out = subprocess.run(
            ["powershell", "-NoProfile", "-Command",
             "[System.Reflection.AssemblyName]::GetAssemblyName('%s').Version.ToString()"
             % path.replace("'", "''")],
            capture_output=True)
        text = out.stdout.decode("utf-8", "replace").strip()
        return text or None
    except OSError:
        return None


# --------------------------------------------------------------------------- sources

def from_directory(src, out):
    """Copy the reference assemblies out of an existing plugin / ACT folder."""
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
                got = subprocess.run([ILSPY, "--resource", resource, "-o", tmp, plugin],
                                     capture_output=True)
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
                log("    %-22s %8d B -> %-20s" % (payload, len(raw), target))
            return copied
    finally:
        shutil.rmtree(tmp, ignore_errors=True)


def download(url):
    log("    GET %s" % url)
    with urllib.request.urlopen(url) as response:
        return response.read()


def act_from_nuget(out):
    name, version = ACT_PACKAGE
    data = download("https://api.nuget.org/v3-flatcontainer/%s/%s/%s.%s.nupkg"
                    % (name, version, name, version))
    tmp = tempfile.mkdtemp(prefix="act-")
    try:
        nupkg = os.path.join(tmp, "act.nupkg")
        open(nupkg, "wb").write(data)
        with zipfile.ZipFile(nupkg) as zf:
            for entry in zf.namelist():
                if entry.lower().endswith("advanced combat tracker.exe"):
                    target = os.path.join(out, "Advanced Combat Tracker.exe")
                    with zf.open(entry) as src, open(target, "wb") as dst:
                        shutil.copyfileobj(src, dst)
                    log("    AdvancedCombatTracker %s -> Advanced Combat Tracker.exe" % version)
                    return True
    finally:
        shutil.rmtree(tmp, ignore_errors=True)
    return False


def machina_from_nuget(out):
    """Fallback only: the public packages are close, but not what the plugin was built on."""
    ok = True
    for pkg_id, version, entry_suffix, target in [
        ("machina", "2.3.1.3", "machina.dll", "Machina.dll"),
        ("machina.ffxiv", "2.4.7.6", "machina.ffxiv.dll", "Machina.FFXIV.dll"),
    ]:
        try:
            data = download("https://api.nuget.org/v3-flatcontainer/%s/%s/%s.%s.nupkg"
                            % (pkg_id, version, pkg_id, version))
        except Exception as exc:                       # noqa: BLE001
            log("    %s %s: %s" % (pkg_id, version, exc))
            ok = False
            continue
        tmp = tempfile.mkdtemp(prefix="mp-")
        try:
            nupkg = os.path.join(tmp, "p.nupkg")
            open(nupkg, "wb").write(data)
            found = None
            with zipfile.ZipFile(nupkg) as zf:
                for entry in zf.namelist():
                    if entry.lower().endswith("/" + entry_suffix):
                        found = entry
                        break
                if found:
                    with zf.open(found) as src, open(os.path.join(out, target), "wb") as dst:
                        shutil.copyfileobj(src, dst)
                    log("    %s %s -> %s" % (pkg_id, version, target))
                else:
                    ok = False
        finally:
            shutil.rmtree(tmp, ignore_errors=True)
    return ok


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
        version = assembly_version(path)
        log("  OK       %-28s %-8s %9d B   expected %s"
            % (name, version or "?", os.path.getsize(path), expected))
    return have_all


def fetch_distribution(cache):
    """Download the published plugin archive once and verify it against the known hash."""
    if os.path.exists(cache) and sha256(cache) == DISTRIBUTION_SHA256:
        log("  using cached %s" % cache)
        return cache
    os.makedirs(os.path.dirname(cache), exist_ok=True)
    log("  downloading %s" % DISTRIBUTION_URL)
    data = download(DISTRIBUTION_URL)
    digest = hashlib.sha256(data).hexdigest()
    if digest != DISTRIBUTION_SHA256:
        raise SystemExit("downloaded archive has sha256 %s, expected %s\n"
                         "(refusing to use it; pass --archive to supply one yourself)"
                         % (digest, DISTRIBUTION_SHA256))
    open(cache, "wb").write(data)
    log("  verified sha256 %s (%d bytes)" % (digest[:16] + "...", len(data)))
    return cache


def sha256(path):
    h = hashlib.sha256()
    with open(path, "rb") as fh:
        for chunk in iter(lambda: fh.read(1 << 20), b""):
            h.update(chunk)
    return h.hexdigest()


def default_archive():
    found = sorted(glob.glob(os.path.join(REPO, "reference", "*.zip")))
    return found[0] if found else None


def main():
    ap = argparse.ArgumentParser(description=__doc__,
                                 formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--out", default=REFS, help="output directory (default: refs/)")
    ap.add_argument("--archive", help="plugin distribution ZIP to unpack "
                                      "(default: reference/*.zip, else download)")
    ap.add_argument("--from-dir", help="copy the assemblies from this directory instead")
    ap.add_argument("--act-path", help="Advanced Combat Tracker.exe to copy")
    ap.add_argument("--no-download", action="store_true",
                    help="never download the distribution archive")
    ap.add_argument("--allow-inexact", action="store_true",
                    help="let nuget.org supply Machina/Machina.FFXIV when the distribution "
                         "is unavailable (the Chinese-client types differ - see the docstring)")
    ap.add_argument("--check", action="store_true", help="only report what is present")
    args = ap.parse_args()

    out = os.path.abspath(args.out)
    os.makedirs(out, exist_ok=True)

    if args.check:
        return 0 if report(out) else 1

    log("populating %s" % out)

    if args.from_dir:
        log("  from directory %s" % args.from_dir)
        from_directory(args.from_dir, out)
    else:
        archive = args.archive or default_archive() or DISTRIBUTION_CACHE
        if not os.path.exists(archive) and not args.no_download:
            log("  no local distribution archive; fetching the published one")
            archive = fetch_distribution(DISTRIBUTION_CACHE)
        if os.path.exists(archive):
            log("  from distribution archive %s" % archive)
            from_archive(archive, out)
        else:
            log("  no distribution archive available")
            if args.allow_inexact:
                log("  falling back to nuget.org for Machina/Machina.FFXIV (INEXACT)")
                machina_from_nuget(out)
            else:
                log("  pass --archive/--from-dir, allow the download, or --allow-inexact")

    if not os.path.exists(os.path.join(out, "Advanced Combat Tracker.exe")):
        if args.act_path and os.path.exists(args.act_path):
            shutil.copyfile(args.act_path, os.path.join(out, "Advanced Combat Tracker.exe"))
            log("    ACT from --act-path")
        else:
            log("  ACT host assembly from nuget.org (%s %s)"
                % (ACT_PACKAGE[0], ACT_PACKAGE[1]))
            act_from_nuget(out)

    ok = report(out)
    if not ok:
        log("")
        log("The build needs all four files.  Machina.dll / Machina.FFXIV.dll are fork")
        log("builds that only ship inside the plugin distribution - supply it with")
        log("--archive or --from-dir (see the module docstring).")
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
