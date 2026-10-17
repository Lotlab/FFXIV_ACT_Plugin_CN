# FFXIV_ACT_Plugin_CN — restored source tree

Source tree for the Chinese-client build of `FFXIV_ACT_Plugin`, reconstructed
from the released binary `FFXIV_ACT_Plugin_3.0.2.1_CN.zip` (plugin version
`3.0.2.1`, upstream `ravahn/FFXIV_ACT_Plugin` 3.0.2.1 with the CN patches).

## Layout

```
FFXIV_ACT_Plugin_CN.sln
lib/                           build-time references, fetched by scripts/fetch-deps.sh
scripts/
  fetch-deps.sh                downloads Advanced Combat Tracker
  package.sh                   produces the release archive
src/
  Directory.Build.props        shared build settings (net48, LangVersion, ACT path)
  FFXIV_ACT_Plugin/            host plugin: IActPluginV1 implementation, settings UI
  FFXIV_ACT_Plugin.Common/     shared interfaces/contracts (+ models)
  FFXIV_ACT_Plugin.Config/     settings mediator
  FFXIV_ACT_Plugin.Logfile/    ACT log-line formatting
  FFXIV_ACT_Plugin.Memory/     process memory scanning
  FFXIV_ACT_Plugin.Network/    packet handlers
  FFXIV_ACT_Plugin.Parse/      combat log parsing / DoT simulation
  FFXIV_ACT_Plugin.Resource/   game data tables (actions, statuses, zones, ...)
third_party/
  machina/                     Lotlab fork, branch feature/oodle-state-recovery
```

## Building

```sh
git submodule update --init --recursive
scripts/fetch-deps.sh          # once: Advanced Combat Tracker.exe into lib/
dotnet build -c Release FFXIV_ACT_Plugin_CN.sln
```

`net48` target framework reference assemblies and `Newtonsoft.Json` are restored
from NuGet, so the build works on Linux/macOS as well as Windows.

The plugin project uses [Costura.Fody](https://github.com/Fody/Costura) to embed
every managed dependency — including the Machina libraries built from
`third_party/machina` — into `FFXIV_ACT_Plugin.dll`, so the build output is the
single redistributable file that ACT loads.

`Advanced Combat Tracker.exe` is only a compile-time reference; the plugin is
loaded *by* ACT, so it is never embedded.

## Packaging

```sh
scripts/package.sh             # writes artifacts/FFXIV_ACT_Plugin_<version>_CN.zip
```

The archive contains `FFXIV_ACT_Plugin.dll` and the `deucalion-<version>.distrib.dll`
files that `DeucalionInjector` loads from the plugin directory.

## Notes on the restoration

The sources were recovered from the shipped assembly with ILSpy. A handful of
constructs that ILSpy renders in a form the C# compiler rejects had to be
rewritten; the mechanical ones (implicit conversion operators, nested types
referenced by simple name, fixed-size-buffer access, ambiguous type names) are
documented in the commit that introduced them.

The embedded resource tables under
`src/FFXIV_ACT_Plugin.Resource/{Definitions,Generated}` are byte-identical to the
ones in the released assembly and are loaded by name at runtime
(see `FFXIV_ACT_Plugin.Resource.ResourceFile`), so their file names and embedded
logical names must not be changed casually.

`src/FFXIV_ACT_Plugin.Network/Compat/Server_StatusEffectListForay3.cs` supplies a
header type that the Machina fork only defines for the other game regions.
