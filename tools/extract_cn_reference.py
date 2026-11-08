#!/usr/bin/env python3
"""Unpack a CN release into ``.work/bin`` for tools/verify_reconstruction.py.

The verifier compares the two source-built assemblies against the CN originals, so it
needs the CN plugin DLL and its embedded assemblies on disk:

    python tools/extract_cn_reference.py path/to/CN.zip
    python tools/verify_reconstruction.py

This only extracts; it does not decompile.
"""
import argparse
import os
import subprocess
import sys
import zipfile

REPO = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
BIN = os.path.join(REPO, ".work", "bin")
PATCHER_PROJECT = os.path.join(REPO, "tools", "AssemblyPatcher", "AssemblyPatcher.csproj")
PATCHER = os.path.join(REPO, "artifacts", "bin", "Release", "AssemblyPatcher.dll")


def run(args):
    result = subprocess.run(args)
    if result.returncode != 0:
        raise SystemExit("command failed: %s" % " ".join(args))


def main():
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("cn_zip", help="CN release zip containing FFXIV_ACT_Plugin.dll")
    args = parser.parse_args()

    os.makedirs(BIN, exist_ok=True)
    main_dll = os.path.join(BIN, "ffxiv_act_plugin_main.dll")
    with zipfile.ZipFile(args.cn_zip) as zip_file:
        with zip_file.open("FFXIV_ACT_Plugin.dll") as source, open(main_dll, "wb") as target:
            target.write(source.read())

    run(["dotnet", "build", PATCHER_PROJECT, "-c", "Release", "-v", "q"])
    if not os.path.exists(PATCHER):
        raise SystemExit("AssemblyPatcher not found at %s" % PATCHER)
    run(["dotnet", PATCHER, "extract", main_dll, BIN])

    print("reference assemblies ready in %s" % os.path.relpath(BIN, REPO))


if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        sys.exit(130)
