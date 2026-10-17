"""Recreate the decompiler input/output tree under .work/ from reference/.

Pipeline
--------
1. `reference/FFXIV_ACT_Plugin_3.0.2.1_CN.zip` is unpacked into `.work/bin/`.
2. The ten Costura payloads embedded in `FFXIV_ACT_Plugin.dll`
   (`costura.<name>.dll.compressed`) are extracted with `ilspycmd --resource` and
   inflated into plain assemblies, so the sibling `FFXIV_ACT_Plugin.*`, `Machina*`
   and `Newtonsoft.Json` modules are available as real files to decompile against.
3. The two WinForms designer `.resources` blobs of the main assembly are extracted
   verbatim (they exist in the original manifest but not in the IL).
4. Every managed assembly is decompiled to a compilable project with
   `ilspycmd -p` into `.work/out/<key>/`.

After this script finishes, run `python tools/gen_projects.py` to rewrite the
generated projects in `src/`.

Requirements: the `ilspycmd` .NET global tool (`dotnet tool install -g ilspycmd`).
"""
import os
import shutil
import subprocess
import sys
import zipfile
import zlib

REPO = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
WORK = os.path.join(REPO, ".work")
BIN = os.path.join(WORK, "bin")
OUT = os.path.join(WORK, "out")
ZIP = os.path.join(REPO, "reference", "FFXIV_ACT_Plugin_3.0.2.1_CN.zip")

ILSPY = os.path.join(os.path.expanduser("~"), ".dotnet", "tools", "ilspycmd.exe")

# decompiler key -> (file in .work/bin, project folder generated into .work/out)
MODULES = [
    ("main", "ffxiv_act_plugin_main.dll"),
    ("common", "ffxiv_act_plugin.common.dll"),
    ("config", "ffxiv_act_plugin.config.dll"),
    ("logfile", "ffxiv_act_plugin.logfile.dll"),
    ("memory", "ffxiv_act_plugin.memory.dll"),
    ("network", "ffxiv_act_plugin.network.dll"),
    ("parse", "ffxiv_act_plugin.parse.dll"),
    ("resource", "ffxiv_act_plugin.resource.dll"),
]

COSTURA = [
    "machina.dll", "machina.ffxiv.dll", "newtonsoft.json.dll",
    "ffxiv_act_plugin.common.dll", "ffxiv_act_plugin.config.dll",
    "ffxiv_act_plugin.logfile.dll", "ffxiv_act_plugin.memory.dll",
    "ffxiv_act_plugin.network.dll", "ffxiv_act_plugin.parse.dll",
    "ffxiv_act_plugin.resource.dll",
]

DESIGNER_RESOURCES = [
    "FFXIV_ACT_Plugin.BenchmarkPropertyPage.resources",
    "FFXIV_ACT_Plugin.SettingsPropertyPage.resources",
]


def run(args, **kw):
    result = subprocess.run(args, capture_output=True, **kw)
    if result.returncode != 0:
        sys.stderr.write(result.stdout.decode("utf-8", "replace"))
        sys.stderr.write(result.stderr.decode("utf-8", "replace"))
        raise SystemExit("command failed: %s" % " ".join(args))
    return result.stdout


def unpack():
    if not os.path.exists(ZIP):
        raise SystemExit("missing %s" % ZIP)
    os.makedirs(BIN, exist_ok=True)
    with zipfile.ZipFile(ZIP) as zf:
        for name in zf.namelist():
            print("  unpack", name)
            zf.extract(name, BIN)
    # the plugin assembly is decompiled under a distinct file name so that the
    # freshly built output never shadows the original while decompiling
    shutil.copyfile(os.path.join(BIN, "FFXIV_ACT_Plugin.dll"),
                    os.path.join(BIN, "ffxiv_act_plugin_main.dll"))


def extract_costura():
    tmp = os.path.join(WORK, "costura")
    os.makedirs(tmp, exist_ok=True)
    main = os.path.join(BIN, "ffxiv_act_plugin_main.dll")
    for name in COSTURA:
        resource = "costura.%s.compressed" % name
        run([ILSPY, "--resource", resource, "-o", tmp, main])
    for name in COSTURA:
        blob = open(os.path.join(tmp, "costura.%s.compressed" % name), "rb").read()
        for wbits in (-15, 15 + 16, 15):
            try:
                raw = zlib.decompress(blob, wbits)
                break
            except zlib.error:
                continue
        else:
            raise SystemExit("cannot inflate %s" % name)
        with open(os.path.join(BIN, name), "wb") as fh:
            fh.write(raw)
        print("  inflated %-45s %8d -> %d" % (name, len(blob), len(raw)))


def extract_designer_resources():
    tmp = os.path.join(WORK, "res")
    os.makedirs(tmp, exist_ok=True)
    main = os.path.join(BIN, "ffxiv_act_plugin_main.dll")
    for name in DESIGNER_RESOURCES:
        run([ILSPY, "--resource", name, "-o", tmp, main])
        shutil.copyfile(os.path.join(tmp, name),
                        os.path.join(REPO, "src", "FFXIV_ACT_Plugin", name))
        print("  extracted designer resource", name)


def decompile():
    for key, filename in MODULES:
        target = os.path.join(OUT, key)
        if os.path.isdir(target):
            shutil.rmtree(target)
        os.makedirs(target, exist_ok=True)
        print("  decompile %-12s -> .work/out/%s" % (filename, key))
        run([ILSPY, "-p", "-o", target, os.path.join(BIN, filename)])


def main():
    print("[1/4] unpacking reference archive")
    unpack()
    print("[2/4] extracting Costura payloads")
    extract_costura()
    print("[3/4] extracting WinForms designer resources")
    extract_designer_resources()
    print("[4/4] decompiling managed assemblies")
    decompile()
    print("\ndone - now run: python tools/gen_projects.py")


if __name__ == "__main__":
    main()
