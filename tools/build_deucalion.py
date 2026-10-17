"""Build the deucalion DLLs from the third_party/deucalion submodules.

    third_party/deucalion/1.5.0   ff14wed/deucalion @ tag 1.5.0
    third_party/deucalion/1.2.1   ff14wed/deucalion @ tag 1.2.1

These are the upstream sources for the two native `deucalion-*.dll` files that the
plugin zip shipped (the `.distrib` suffix is ACT packaging, not a build product).  They
are **not** a build input for the managed plugin: Machina.FFXIV loads a deucalion DLL
from the plugin folder at runtime, and the build already stages the copies that
Machina.FFXIV itself carries.  This script is here for anyone who wants to rebuild them
from source, and to make the vendored version -> binary mapping reproducible.

Requirements
------------
* **Nightly Rust** - `deucalion/src/lib.rs` uses `#![feature(link_llvm_intrinsics)]`.
* MSVC toolchain and the Windows SDK (`build.rs` runs `winres`, which needs `rc.exe`).
* A git checkout: `build.rs` reads the commit through `vergen-git2`, so building from a
  plain source tarball fails.  Submodules satisfy this.

Caveat
------
Upstream commits no `Cargo.lock` and no lock is committed here (it would live inside the
submodule).  With today's crates.io the resolve picks `tokio-util 0.7.20`, whose extra
`impl Encoder<Bytes> for LengthDelimitedCodec` makes `ld_codec.encode(data.into(), dst)`
in `deucalion/src/rpc.rs:97` ambiguous (E0283/E0284).  This script pins `tokio-util`
back to the version each tag was released with before building.  Upstream's own
`.gitignore` ignores `Cargo.lock`, so the submodule stays clean.

Usage
-----
    python tools/build_deucalion.py                # both versions, release
    python tools/build_deucalion.py 1.5.0
    python tools/build_deucalion.py --lib-only     # upstream CI's smaller second step
"""
import argparse
import os
import subprocess
import sys

REPO = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
DEUCALION = os.path.join(REPO, "third_party", "deucalion")

# tag -> (tokio-util version that tag was released with, expected dll size in bytes)
VERSIONS = {
    "1.5.0": ("0.7.16", 3_002_880),
    "1.2.1": ("0.7.14", 2_366_976),
}


def rustup_cargo():
    """Prefer the nightly toolchain, which deucalion requires."""
    probe = subprocess.run(["rustup", "run", "nightly", "cargo", "--version"],
                           capture_output=True)
    if probe.returncode == 0:
        return ["rustup", "run", "nightly", "cargo"]
    return ["cargo"]


def run(args, cwd):
    print("    $ %s" % " ".join(args))
    result = subprocess.run(args, cwd=cwd)
    return result.returncode == 0


def build(version, lib_only):
    pin, expected = VERSIONS[version]
    root = os.path.join(DEUCALION, version)
    if not os.path.exists(os.path.join(root, "Cargo.toml")):
        print("  %-6s submodule is not initialised" % version)
        print("         git submodule update --init --recursive third_party/deucalion")
        return False

    print("  %-6s pinning tokio-util to %s, then building" % (version, pin))
    cargo = rustup_cargo()
    if not run(cargo + ["update", "-p", "tokio-util", "--precise", pin], root):
        return False
    args = cargo + ["build", "--release"]
    if lib_only:
        args += ["-p", "deucalion"]
    if not run(args, root):
        return False

    dll = os.path.join(root, "target", "release", "deucalion.dll")
    if not os.path.exists(dll):
        print("  %-6s FAILED - %s was not produced" % (version, dll))
        return False
    size = os.path.getsize(dll)
    note = "" if lib_only else "  (upstream release asset was 2,975,744 B for 1.5.0)"
    print("  %-6s OK  %s  %d B%s" % (version, dll, size, note))
    return True


def main():
    ap = argparse.ArgumentParser(description=__doc__,
                                 formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("version", nargs="?", choices=sorted(VERSIONS),
                    help="build only this version (default: both)")
    ap.add_argument("--lib-only", action="store_true",
                    help="build just the library (the smaller DLL upstream CI's second step makes)")
    args = ap.parse_args()

    versions = [args.version] if args.version else sorted(VERSIONS, reverse=True)
    ok = True
    for version in versions:
        ok &= build(version, args.lib_only)
    return 0 if ok else 1


if __name__ == "__main__":
    sys.exit(main())
