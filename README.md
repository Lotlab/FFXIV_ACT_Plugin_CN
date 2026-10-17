# FFXIV_ACT_Plugin 3.0.2.1 (CN) — 反编译还原源码

本仓库保存 **ACT（Advanced Combat Tracker）FFXIV 解析插件 3.0.2.1 简体中文版**的分发
二进制文件反编译还原出的源代码，反编译结果可以正常编译通过。

原始二进制位于 [`reference/FFXIV_ACT_Plugin_3.0.2.1_CN.zip`](reference/FFXIV_ACT_Plugin_3.0.2.1_CN.zip)，
包含三个文件：

| 压缩包内的文件 | 类型 | 还原方式 |
| --- | --- | --- |
| `FFXIV_ACT_Plugin.dll` | .NET Framework 4.8 托管程序集 | 反编译为 C# 源码（本仓库 `src/`，8 个工程） |
| `deucalion-1.2.1.distrib.dll` | Rust 原生 DLL | 对应的上游源码已 vendored 到 `third_party/deucalion/1.2.1/` |
| `deucalion-1.5.0.distrib.dll` | Rust 原生 DLL | 对应的上游源码已 vendored 到 `third_party/deucalion/1.5.0/` |

---

## 1. 目录结构

```
FFXIV_ACT_Plugin.sln                解决方案（8 个工程）
Directory.Build.props               公共 MSBuild 设置（net48、统一输出目录 …）
src/
  FFXIV_ACT_Plugin/                 主插件（原 FFXIV_ACT_Plugin.dll）
  FFXIV_ACT_Plugin.Common/          公共接口与数据模型
  FFXIV_ACT_Plugin.Config/          配置
  FFXIV_ACT_Plugin.Logfile/         日志格式化 / 输出
  FFXIV_ACT_Plugin.Memory/          内存读取（特征码、角色数据）
  FFXIV_ACT_Plugin.Network/         网络包捕获与解析（Machina / Deucalion）
  FFXIV_ACT_Plugin.Parse/           战斗日志解析
  FFXIV_ACT_Plugin.Resource/        内置资源（技能/状态/地图/副本等多语言表）
refs/                               编译所需的外部程序集（第三方，非本仓库源码）
  Advanced Combat Tracker.exe       ACT 宿主程序集
  Machina.dll / Machina.FFXIV.dll   网络捕获库
  Newtonsoft.Json.dll               JSON 库
third_party/deucalion/              vendored 的上游 Rust 源码，见下方 §5
tools/                              反编译与校验脚本，见下方 §4
  PluginLoadTest/                   单文件投放加载测试
artifacts/dist/Release/             ★ 单文件投放产物（已被 .gitignore 忽略）
artifacts/bin/Release/              各工程原始输出（已被 .gitignore 忽略）
.work/                              反编译中间产物（已被 .gitignore 忽略）
```

## 2. 编译

### 环境要求

* .NET SDK（本仓库使用 10.0.401 验证，`dotnet build` 需要能解析 `net48`）
* .NET Framework 4.8 targeting pack
* Windows（`net48` + WinForms）

### 命令

```powershell
dotnet build FFXIV_ACT_Plugin.sln -c Release
```

构建产物分两处：

```
artifacts/bin/Release/          各工程原始输出（诊断用）
artifacts/dist/Release/         ★ 单文件投放产物
  FFXIV_ACT_Plugin.dll
```

把 `artifacts/dist/Release/FFXIV_ACT_Plugin.dll` 单独一个文件，加上
`deucalion-<版本>.distrib.dll` 放进 ACT 插件目录即可运行——**不需要**同级放置
`FFXIV_ACT_Plugin.*.dll`、`Machina*.dll` 或 `Newtonsoft.Json.dll`，它们都已经织入主 DLL。

当前状态：**0 error / 4 warning**（两个 `CS0169`、两个 `CS0414`，都是反编译代码里
未被使用的字段，与原始二进制行为一致）。

### 单文件投放验证

[`tools/PluginLoadTest`](tools/PluginLoadTest/Program.cs) 会把主 DLL 单独复制到一个空目录、
以 ACT 加载插件的方式加载它，并强制解析全部内嵌程序集（从 `Assembly.Load(byte[])` 载入的
程序集 `Location` 为空，以此判定它确实来自内嵌资源）：

```powershell
dotnet build tools/PluginLoadTest/PluginLoadTest.csproj -c Release
artifacts/loadtest/Release/PluginLoadTest.exe artifacts/dist/Release/FFXIV_ACT_Plugin.dll
```

```
siblings      : 0 other file(s) in the folder
bundled assemblies
  OK   Advanced Combat Tracker          not bundled, supplied by the host
  OK   FFXIV_ACT_Plugin.Common          v3.0.0.0  <embedded>
  ... (7 个 FFXIV_ACT_Plugin.* 全部 <embedded>)
  OK   Machina                          v2.3.1.3  <embedded>
  OK   Machina.FFXIV                    v2.4.7.1  <embedded>
  OK   Newtonsoft.Json                  v13.0.0.0 <embedded>
  OK   Newtonsoft.Json round-trip       {"hello":"world"}

PASS - 10 assemblies resolved from the embedded resources, nothing on disk
```

### 关于 `refs/Advanced Combat Tracker.exe`

原始插件是针对 **ACT 3.8.5.288** 编译的（`AssemblyRef` 中的版本号）。仓库中的
`refs/Advanced Combat Tracker.exe` 是手头可用的 **3.8.0.283**，引用时显式设置了
`<SpecificVersion>false</SpecificVersion>`，用于替代编译。插件实际使用的 ACT API
在这两个版本之间没有变化，因此可以编译通过；若要严格对齐，可换成 3.8.5.288 的同名文件。
ACT 是宿主程序集，引用时标记 `<Private>false</Private>`，不会被复制或织入。

`refs/Machina.dll`（2.3.1.3）、`refs/Machina.FFXIV.dll`（2.4.7.1）与
`refs/Newtonsoft.Json.dll`（13.0.0.0）是从原插件内嵌资源中提取的**完全相同**的版本，
标记为 copy-local 以便 Costura 织入。

## 3. 相对原始二进制所做的改动

反编译产物与原始二进制在声明层面完全一致（见 §4 的校验结果），只有以下几处是**有意**
的调整；每一处都在源码/工程文件中留有注释：

1. **Costura 打包器改为在构建期重新织入。**
   原始 `FFXIV_ACT_Plugin.dll` 通过 Costura/Fody 把 `FFXIV_ACT_Plugin.*`、`Machina*`、
   `Newtonsoft.Json` 共 10 个程序集以 deflate 压缩内嵌，运行时由
   `Costura.AssemblyLoader` 在 `AssemblyResolve` 中解出。

   本仓库保留了这一分发方式，但在主工程用 `PackageReference` + `FodyWeavers.xml`
   重新启用 Costura.Fody，让它在**构建时**把 `ProjectReference` 的输出织进去，而不是
   把原来的 `costura.*.compressed` 二进制和反编译出来的 `AssemblyLoader.cs` 提交进仓库。
   这样单文件投放能力与原版一致，同时内嵌的每一份都是本仓库源码编译出来的。

   与原始二进制的差异仅在生成物层面：Fody 6.x 的 loader 实现与 3.3.2 不同，并额外内嵌了
   它自己的运行时辅助程序集 `Costura.dll` 与索引资源 `costura.metadata`；Fody 标记类也从
   `ProcessedByFody` 改名为 `FFXIV_ACT_Plugin_ProcessedByFody`。这三项都不影响行为
   （§4 的校验已把它们列为允许差异，加载测试也验证了实际可加载）。
2. **去重内嵌资源。**
   原始 `FFXIV_ACT_Plugin.Resource.dll` 中
   `...Generated.ActionList_Chinese.txt` 与 `...Generated.StatusList_Chinese.txt`
   各自被**重复内嵌了两份完全相同的字节**（`ManifestResource` 表里是两条同名记录）。
   MSBuild 会因此报 `CS1508`，而且 .NET 本身也只能解析到其中一份，所以只保留一份，
   删除了 ILSpy 命名出来的 `*_Chinese_2.txt`。
3. **补回 WinForms 设计器资源。**
   主程序集清单里还有两个空的 `*.resources`
   （`FFXIV_ACT_Plugin.{Benchmark,Settings}PropertyPage.resources`），ILSpy 的项目
   模板没有生成它们。已用 `ilspycmd --resource` 原样提取并作为 `EmbeddedResource`
   重新嵌入，保证清单与原始一致。

除此之外没有对反编译代码做任何功能性修改。

## 4. 还原过程与校验

`tools/` 下的脚本可以完整复现这套源码：

| 脚本 | 作用 |
| --- | --- |
| [`tools/decompile.py`](tools/decompile.py) | 解压 `reference/` 的 zip → 用 `ilspycmd --resource` + zlib 解出 10 个 Costura 内嵌程序集 → 用 `ilspycmd -p` 把 8 个托管程序集反编译成可编译工程（输出到 `.work/out/`） |
| [`tools/gen_projects.py`](tools/gen_projects.py) | 把 ilspycmd 生成的工程改写成最终的 `src/**/*.csproj`：同级程序集改为 `ProjectReference`，第三方程序集改为指向 `refs/` 的 copy-local `Reference`（以便 Costura 织入），并写入 Costura.Fody 配置、单文件 staging 目标、资源去重与设计器资源嵌入 |
| [`tools/verify_reconstruction.py`](tools/verify_reconstruction.py) | 校验：把原始 DLL 与重新编译出的 DLL 的元数据表（TypeDef / MethodDef / Field / Property / Event / ManifestResource）逐表比对 |
| [`tools/compare_pe_flags.py`](tools/compare_pe_flags.py) | 校验：比对原始与重新编译产物的 PE / CLR 头标志（目标平台、ILONLY、32 位优先、强名称等） |
| [`tools/PluginLoadTest`](tools/PluginLoadTest/Program.cs) | 功能校验：把单文件产物放进空目录加载，强制解析全部内嵌程序集并实测 Newtonsoft.Json 可用 |
| [`tools/analyze_build.py`](tools/analyze_build.py) | 汇总 `dotnet build` 的报错/告警 |

复现步骤（需要 `dotnet tool install -g ilspycmd`）：

```powershell
python tools/decompile.py
python tools/gen_projects.py
dotnet build FFXIV_ACT_Plugin.sln -c Release
python tools/verify_reconstruction.py
python tools/compare_pe_flags.py

dotnet build tools/PluginLoadTest/PluginLoadTest.csproj -c Release
artifacts/loadtest/Release/PluginLoadTest.exe artifacts/dist/Release/FFXIV_ACT_Plugin.dll
```

校验结果：

```
FFXIV_ACT_Plugin           types=38    methods=327   fields=284   synth=111  OK
FFXIV_ACT_Plugin.Common    types=27    methods=280   fields=112   synth=48   OK
FFXIV_ACT_Plugin.Config    types=7     methods=21    fields=27    synth=0    OK
FFXIV_ACT_Plugin.Logfile   types=10    methods=85    fields=58    synth=24   OK
FFXIV_ACT_Plugin.Memory    types=96    methods=384   fields=368   synth=8    OK
FFXIV_ACT_Plugin.Network   types=37    methods=162   fields=114   synth=0    OK
FFXIV_ACT_Plugin.Parse     types=126   methods=727   fields=537   synth=59   OK
FFXIV_ACT_Plugin.Resource  types=59    methods=350   fields=173   synth=3    OK

8/8 assemblies verified with an identical declaration surface
```

* 校验允许的差异只有三类：被去重的两条 `*_Chinese.txt`、当前 Roslyn 自动注入的
  `EmbeddedAttribute` / `RefSafetyRulesAttribute`，以及 Fody 6.x 重新生成的
  `Costura.AssemblyLoader` 与 `FFXIV_ACT_Plugin_ProcessedByFody` 标记类。
* `synth=` 一列是编译器自动生成的名字（闭包类 `<>c__DisplayClass*`、lambda
  `<...>b__*`、缓存委托字段等）。因为重新编译时 Roslyn 会重新编号这些名字
  （例如 `foreach` 变量在反编译源码里必须改名为 `x2` 才能通过作用域检查），
  这些差异单独统计、不参与判定。
* 重新编译出的程序集体积与原始基本一致（如 `FFXIV_ACT_Plugin.Common.dll`
  24576 → 24576、`FFXIV_ACT_Plugin.Network.dll` 40448 → 40448；
  `FFXIV_ACT_Plugin.Resource.dll` 由 9223168 变为 8356864，正好等于去掉的那份重复
  `Chinese` 资源；主程序集变小则是因为不再内嵌 10 个 Costura 负载）。

另外 `tools/compare_pe_flags.py` 确认 8 个程序集的 PE / CLR 头标志（`PE32`/`PE32+`、
ILONLY、32 位必需/优先、强名称签名等）与原始完全一致：

```
8/8 assemblies with identical PE/CLR flags
```

## 5. `deucalion`（Rust 原生 DLL）

`reference/` 里的 `deucalion-<版本>.distrib.dll` 是 **Rust 编译出的原生 DLL**，
没有托管元数据，无法“反编译成可编译源码”。因此改为 vendoring 与之**逐字节对应**的
上游源码：

* 上游仓库：<https://github.com/ff14wed/deucalion>（GPL-3.0）
* `deucalion-1.2.1.distrib.dll` ⇔ tag `1.2.1`（commit `8b63caa5e842aa32345c7a3cb29040ed52502541`）
* `deucalion-1.5.0.distrib.dll` ⇔ tag `1.5.0`（commit `5cc714ad4cc66aced1519042660026edff68e850`）
* 上游产物名为 `deucalion.dll`；`.distrib` 后缀是 ACT 插件打包时加的，插件在注入前
  会把它改名（这样游戏锁定 DLL 时 ZIP 仍可重复解压）。二进制内容与上游 release 资源
  **SHA-256 完全一致**。

构建方式（需要 **nightly** Rust + MSVC + Windows SDK）：

```powershell
rustup toolchain install nightly
cd third_party/deucalion/1.5.0
cargo build --release --locked              # 整个 workspace
cargo build --release --locked -p deucalion # 只编译库，产物更小
```

两个版本都已在本仓库内**实际编译通过**（rustc 1.97.1-nightly、x86_64-pc-windows-msvc）：

| 版本 | 构建方式 | 产物 `deucalion.dll` | 上游 release 资源 |
| --- | --- | --- | --- |
| 1.5.0 | workspace 根目录 | 3,002,880 B | 2,975,744 B |
| 1.5.0 | `-p deucalion` | 2,376,192 B | — |
| 1.2.1 | `-p deucalion` | 2,366,976 B | 3,153,144 B |

1.5.0 在 workspace 根目录构建出的体积与上游发布资源相差不到 1%，说明 vendored 源码对应
的就是发行时的构建方式。

三点需要注意：

* `deucalion/src/lib.rs` 使用 `#![feature(link_llvm_intrinsics)]`，**必须用 nightly**。
* `deucalion/build.rs` 通过 `vergen-git2` 读取 git 提交信息并 `?` 传播错误，
  所以**在没有 `.git` 的源码压缩包里直接 `cargo build` 会失败**。本仓库把源码放进
  了自己的 git 仓库，因此在仓库内构建可以正常工作；若单独拷贝该目录，请先
  `git init && git add -A && git commit`，或自行修改 `build.rs`。
* 上游没有提交 `Cargo.lock`。按今天的 crates.io 解析会把 `tokio-util` 拉到
  `0.7.20`，它新增的 `impl Encoder<Bytes> for LengthDelimitedCodec` 会让
  `deucalion/src/rpc.rs:97` 的 `encode(data.into(), dst)` 产生 `E0283`/`E0284`
  二义性错误。本仓库在每个版本目录下提交了一份 `Cargo.lock`，把 `tokio-util`
  钉回该版本发布时的 `0.7.16` / `0.7.14`，因此可以直接 `cargo build`。

重新编译出的 DLL **不是逐字节相同**的（上游 release 用的是更早的 nightly 与当时的
依赖版本），但源码未作任何修改且可正常编译链接。

详见 [`third_party/deucalion/README.md`](third_party/deucalion/README.md)。

## 6. 许可与来源

* 本仓库的 C# 源码是 ravahn/FFXIV_ACT_Plugin 中文发行版二进制的反编译产物。
* `third_party/deucalion/` 为上游 GPL-3.0 源码，许可证见各版本目录下的 `LICENSE.md`。
* `refs/` 中为编译所需第三方二进制（ACT、Machina、Newtonsoft.Json），仅作编译引用，
  请遵循各自许可证。
