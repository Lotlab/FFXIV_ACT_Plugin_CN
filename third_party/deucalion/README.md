# deucalion (vendored upstream source)

This directory contains unmodified upstream source for **deucalion**, the Rust library
that the ACT FFXIV plugin injects into the game to capture decoded packets.

It is vendored because the two native DLLs shipped in
[`reference/FFXIV_ACT_Plugin_3.0.2.1_CN.zip`](../../reference/FFXIV_ACT_Plugin_3.0.2.1_CN.zip)
are compiled Rust binaries with no managed metadata, so they cannot be decompiled into
compilable source. The upstream sources below are the exact revisions those binaries
were built from.

| Directory | Upstream tag | Commit | Corresponds to |
| --- | --- | --- | --- |
| `1.2.1/` | [`1.2.1`](https://github.com/ff14wed/deucalion/releases/tag/1.2.1) | `8b63caa5e842aa32345c7a3cb29040ed52502541` | `deucalion-1.2.1.distrib.dll` (3,153,144 B, sha256 `eabfff9e6cf5842f97781bae669d5b2b4056a5881bede29c9424b4626c8f566e`) |
| `1.5.0/` | [`1.5.0`](https://github.com/ff14wed/deucalion/releases/tag/1.5.0) | `5cc714ad4cc66aced1519042660026edff68e850` | `deucalion-1.5.0.distrib.dll` (2,975,744 B, sha256 `326be4db261064ebb51b8c46d2940f55701d79887e2c65070aff00de4c8c65ef`) |

* Upstream: <https://github.com/ff14wed/deucalion>
* License: **GPL-3.0** (`LICENSE.md` inside each version directory)
* Downloaded from `https://codeload.github.com/ff14wed/deucalion/zip/refs/tags/<tag>`
* Nothing in these directories has been modified.

## Why the files are called `*.distrib.dll`

Upstream builds a plain `deucalion.dll`. The `.distrib` infix is added by the ACT plugin's
own packaging: FFXIV keeps a lock on the injected DLL, so the ZIP ships it under a
different name and the plugin renames it to `deucalion-<version>.dll` right before
injection (`ACT_PluginUpdate.FinalizeDeucalionDistrib` in
`src/FFXIV_ACT_Plugin/FFXIV_ACT_Plugin/ACT_PluginUpdate.cs`). There is no `distrib` Cargo
profile, crate or `.cargo/config.toml` upstream.

## Building

```powershell
cd 1.5.0
cargo build --release --locked              # whole workspace
# or just the library, which yields a smaller DLL:
cargo build --release --locked -p deucalion
```

Both write `target/release/deucalion.dll` (the workspace shares one target directory).

Requirements and gotchas:

* **Nightly Rust is mandatory** — `deucalion/src/lib.rs` starts with
  `#![feature(link_llvm_intrinsics)]`. A stable toolchain fails to compile.
* **MSVC toolchain + Windows SDK** (the build script runs `winres`, which needs `rc.exe`).
* `deucalion/build.rs` calls `vergen_git2::Git2Builder::default()…build()?`, so building
  from a `.git`-less copy of the source fails with a git error. Building inside a git
  checkout (as in this repository) works; otherwise run
  `git init && git add -A && git commit -m vendor` in the version directory first, or
  patch `build.rs` to emit fixed `VERGEN_GIT_SHA` / `VERGEN_GIT_DIRTY` values.
* To reproduce ACT's packaging, rename the result:
  `deucalion.dll` → `deucalion-1.5.0.distrib.dll`.

The upstream CI recipe (`.github/workflows/release.yml`) is:
`cargo build --release` at the repo root, then again inside `deucalion/` (the CI notes
that building inside its own directory yields a smaller binary), then
`rustc shims/bcryptprimitives.rs -Copt-level=3 -Clto=fat --out-dir target/release`.

### Verified builds in this repository

```
rustup toolchain install nightly          # rustc 1.97.1-nightly, x86_64-pc-windows-msvc
cd 1.5.0
cargo build --release --locked              # workspace build -> target/release/deucalion.dll
cargo build --release --locked -p deucalion # library-only  -> target/release/deucalion.dll
cd ../1.2.1
cargo build --release --locked -p deucalion # library-only  -> target/release/deucalion.dll
```

| Tag | Build | `deucalion.dll` | Upstream release asset |
| --- | --- | --- | --- |
| 1.5.0 | workspace root | 3,002,880 B | `deucalion.dll` 2,975,744 B |
| 1.5.0 | `-p deucalion` | 2,376,192 B | — |
| 1.2.1 | `-p deucalion` | 2,366,976 B | `deucalion.dll` 3,153,144 B |

The workspace-root build of 1.5.0 lands within 1 % of the released asset, which confirms
the vendored source reproduces the shipped build mode (the smaller `-p deucalion` result
is the CI's second, size-optimised step).

Upstream commits **no `Cargo.lock`**, so dependency versions float. With today's
crates.io the resolution pulls `tokio-util 0.7.20`, whose additional
`impl Encoder<Bytes> for LengthDelimitedCodec` makes
`self.ld_codec.encode(data.into(), dst)` at `deucalion/src/rpc.rs:97` ambiguous, and the
build fails with `E0283` / `E0284` — for both tags. The `Cargo.lock` committed next to
each source tree pins `tokio-util` back to the version that tag was released with
(`0.7.16` for 1.5.0, `0.7.14` for 1.2.1), so a plain `cargo build --release` works.
(To redo it by hand: `cargo update -p tokio-util --precise <version>`.)

The rebuilds are **not byte-identical** to the released `.distrib.dll` files: upstream's
assets were produced by older nightlies (1.95.0-nightly 2026-01-26 for 1.5.0,
1.88.0-nightly 2025-04-07 for 1.2.1) against the dependency versions current at those
dates. The vendored sources are unmodified and both crates build and link cleanly into
x86-64 native DLLs.

## Not to be confused with

* **Machina / Machina.FFXIV** — a separate .NET library, decompiled under `src/`.
  `Machina.FFXIV.Deucalion.DeucalionInjector` is the managed side that loads and injects
  the native `deucalion-<version>.dll`.
* `region-3.0.2` and `127.0.0.1:34254`, both of which show up in the binaries' string
  tables: the former is a transitive dependency of `retour`, the latter is a rustdoc
  example literal inside `tokio`. Deucalion's only transport is the named pipe
  `\\.\pipe\deucalion-<FFXIV PID>`.
