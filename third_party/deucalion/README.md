# deucalion (upstream source, as submodules)

This directory holds the upstream source for **deucalion**, the Rust library that the ACT
FFXIV plugin injects into the game process to capture decoded packets. It is the source
behind the two native DLLs that the plugin zip shipped, which cannot be decompiled (they
carry no managed metadata):

| Submodule | Upstream tag | Commit | Corresponds to |
| --- | --- | --- | --- |
| `1.5.0/` | [`1.5.0`](https://github.com/ff14wed/deucalion/releases/tag/1.5.0) | `5cc714ad4cc66aced1519042660026edff68e850` | `deucalion-1.5.0.distrib.dll` (2,975,744 B, sha256 `326be4db261064ebb51b8c46d2940f55701d79887e2c65070aff00de4c8c65ef`) |
| `1.2.1/` | [`1.2.1`](https://github.com/ff14wed/deucalion/releases/tag/1.2.1) | `8b63caa5e842aa32345c7a3cb29040ed52502541` | `deucalion-1.2.1.distrib.dll` (3,153,144 B, sha256 `eabfff9e6cf5842f97781bae669d5b2b4056a5881bede29c9424b4626c8f566e`) |

* Upstream: <https://github.com/ff14wed/deucalion>, license **GPL-3.0** (`LICENSE.md`).
* Two entries of the same URL are used because two tagged revisions are needed; neither is
  a build input for the managed plugin.

## These are not needed to build the plugin

`FFXIV_ACT_Plugin.dll` does not reference deucalion at compile time. At runtime
`Machina.FFXIV.Deucalion.DeucalionInjector` loads a native `deucalion-<version>.dll` from
the plugin folder, and **Machina.FFXIV itself carries those DLLs as content items** - so
`dotnet build` already stages them into `artifacts/dist/<Configuration>/` without anything
from this directory.

They are kept here purely so the vendored version -> shipped binary mapping is on record
and reproducible. The CN build of the plugin cannot fetch a deucalion DLL on its own:
`ACT_PluginUpdate.UpdateCheckClicked_Version()` is empty and there is no download code
anywhere in the plugin.

## Building

```powershell
python tools/build_deucalion.py            # both versions
python tools/build_deucalion.py 1.5.0      # one version
python tools/build_deucalion.py --lib-only # upstream CI's smaller second step
```

The script pins `tokio-util` (0.7.16 for 1.5.0, 0.7.14 for 1.2.1) before building, because
upstream commits no `Cargo.lock` and the current resolve picks 0.7.20, whose extra
`impl Encoder<Bytes> for LengthDelimitedCodec` makes `ld_codec.encode(data.into(), dst)`
in `deucalion/src/rpc.rs:97` ambiguous (E0283/E0284). A lock file cannot be committed here
because it would live inside the submodule; upstream's `.gitignore` ignores it, so the pin
leaves the submodule clean.

Manual equivalent:

```powershell
rustup toolchain install nightly
cd third_party/deucalion/1.5.0
cargo update -p tokio-util --precise 0.7.16
cargo build --release          # -> target/release/deucalion.dll
```

### Gotchas

* **Nightly Rust is mandatory** - `deucalion/src/lib.rs` starts with
  `#![feature(link_llvm_intrinsics)]`; stable fails.
* **MSVC + Windows SDK** - `build.rs` runs `winres`, which needs `rc.exe`.
* `build.rs` reads the commit via `vergen-git2` and propagates the error, so a git-less
  source tarball fails to build. Submodules satisfy this.
* Upstream produces a plain `deucalion.dll`. The `.distrib` infix is ACT packaging: FFXIV
  keeps a lock on the injected DLL, so the ZIP ships it under another name and
  `ACT_PluginUpdate.FinalizeDeucalionDistrib` renames it at injection time.

### Verified sizes

| Tag | Build | `deucalion.dll` | Upstream release asset |
| --- | --- | --- | --- |
| 1.5.0 | workspace root | 3,002,880 B | 2,975,744 B |
| 1.5.0 | `-p deucalion` | 2,376,192 B | - |
| 1.2.1 | `-p deucalion` | 2,366,976 B | 3,153,144 B |

Rebuilds are not byte-identical to the released assets: those came from older nightlies
(1.95.0-nightly 2026-01-26 for 1.5.0, 1.88.0-nightly 2025-04-07 for 1.2.1) against the
dependency versions current at the time. The sources are unmodified.

## Not to be confused with

* **Machina / Machina.FFXIV** - a separate .NET library, built from the
  `third_party/machina` submodule. Its `Machina.FFXIV.Deucalion.DeucalionInjector` is the
  managed side that loads and injects the native deucalion DLL.
* `region-3.0.2` and `127.0.0.1:34254`, both of which appear in the binaries' string
  tables: the former is a transitive dependency of `retour`, the latter is a rustdoc
  example literal inside `tokio`. Deucalion's only transport is the named pipe
  `\.\pipe\deucalion-<FFXIV PID>`.
