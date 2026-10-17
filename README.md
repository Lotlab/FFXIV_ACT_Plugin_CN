# FFXIV_ACT_Plugin 3.0.2.1 (CN) — 反编译还原源码

本仓库保存 **ACT（Advanced Combat Tracker）FFXIV 解析插件 3.0.2.1 简体中文版**的分发
二进制反编译还原出的源代码，反编译结果可以正常编译，并且能产出与原版一致的**单文件插件**。

反编译对象是 [TundraWork/FFXIV_ACT_Plugin_CN](https://github.com/TundraWork/FFXIV_ACT_Plugin_CN)
release `26.6.9.0` 的 `FFXIV_ACT_Plugin_3.0.2.1_CN.zip`（5,591,612 字节，
sha256 `94a9364b…bde844`），内含三个文件：

| 压缩包内的文件 | 类型 | 还原方式 |
| --- | --- | --- |
| `FFXIV_ACT_Plugin.dll` | .NET Framework 4.8 托管程序集 | 反编译为 C# 源码（`src/`，8 个工程） |
| `deucalion-1.2.1.distrib.dll` | Rust 原生 DLL | 上游源码 vendored 到 `third_party/deucalion/1.2.1/` |
| `deucalion-1.5.0.distrib.dll` | Rust 原生 DLL | 上游源码 vendored 到 `third_party/deucalion/1.5.0/` |

## 0. 本仓库不提交任何二进制

仓库里只有源码、脚本和文本资源，**没有任何 .dll / .exe / .zip / .resources**。

编译所需的外部程序集和原始发行包不放在 git 里，而是由
[`tools/fetch_references.py`](tools/fetch_references.py) 在本地按需获取（见 §2），
获取结果写入被 gitignore 的 `refs/` 与 `.work/`。

## 1. 目录结构

```
FFXIV_ACT_Plugin.sln                解决方案（8 个工程）
Directory.Build.props               公共 MSBuild 设置（net48、统一输出目录、引用缺失检查）
src/
  FFXIV_ACT_Plugin/                 主插件（原 FFXIV_ACT_Plugin.dll）
    FodyWeavers.xml                 Costura.Fody 配置（单文件打包）
    SettingsPropertyPage.resx       空的 WinForms 设计器资源（见 §4）
    BenchmarkPropertyPage.resx
  FFXIV_ACT_Plugin.Common/          公共接口与数据模型
  FFXIV_ACT_Plugin.Config/          配置
  FFXIV_ACT_Plugin.Logfile/         日志格式化 / 输出
  FFXIV_ACT_Plugin.Memory/          内存读取（特征码、角色数据）
  FFXIV_ACT_Plugin.Network/         网络包捕获与解析（Machina / Deucalion）
  FFXIV_ACT_Plugin.Parse/           战斗日志解析
  FFXIV_ACT_Plugin.Resource/        内置资源（技能/状态/地图/副本等多语言表）
third_party/machina/                git 子模块：ravahn/machina @ v2.4.7.1，见 §2
third_party/deucalion/              vendored 的上游 Rust 源码，见 §6
tools/                              反编译、引用获取与校验脚本，见 §5
  PluginLoadTest/                   单文件投放的加载测试
artifacts/                          构建输出（gitignore）
refs/                               编译用外部程序集，由脚本生成（gitignore）
reference/ .work/                   原始发行包缓存与反编译中间产物（gitignore）
```

## 2. 编译

### 环境要求

* .NET SDK（本仓库使用 10.0.401 验证）
* .NET Framework 4.8 targeting pack
* Windows（`net48` + WinForms）
* 首次编译需要联网（取 `refs/` 与 NuGet 上的 Fody/Costura）

### 第一步：准备引用程序集

```powershell
python tools/fetch_references.py
```

它会填好 `refs/` 里的四个文件：

| 文件 | 版本 | 来源 |
| --- | --- | --- |
| `Advanced Combat Tracker.exe` | 3.8.4.287 | nuget.org `AdvancedCombatTracker`（仅编译期引用） |
| `Machina.dll` | 2.3.1.3 | 由 `third_party/machina` 子模块源码编译 |
| `Machina.FFXIV.dll` | 2.4.7.1 | 由 `third_party/machina` 子模块源码编译 |
| `Newtonsoft.Json.dll` | 13.0.0.0 | nuget.org 13.0.1（与原内嵌件逐字节相同） |

默认路径不需要任何二进制输入，也不需要下载发行包：

```powershell
python tools/fetch_references.py
```

其他来源（用于对照）：

```powershell
python tools/fetch_references.py --from-distribution   # 用发行包内嵌的原版 fork 程序集
python tools/fetch_references.py --archive path/to/FFXIV_ACT_Plugin_3.0.2.1_CN.zip
python tools/fetch_references.py --from-dir D:/Some/PluginFolder   # 从已有安装目录拷
python tools/fetch_references.py --check                           # 只报告现状
```

省略这一步直接编译会得到明确提示：

```
error DSH0001: Reference assemblies are missing from ...\refs\.
              Run: python tools/fetch_references.py  (see README section 2).
```

### `Machina` / `Machina.FFXIV` 从子模块源码编译

两者由 git 子模块 [ravahn/machina](https://github.com/ravahn/machina) 编译，固定在
产出所需版本的提交：

```
ravahn/machina @ 7a1e7c4620fbbf40d68927239413848a0f50b423   "v2.4.7.1"
  Machina/Machina.csproj         -> 2.3.1.3   (netstandard2.0)
  Machina.FFXIV/Machina.FFXIV.csproj -> 2.4.7.1 (netstandard2.0)
```

两个工程都是 SDK 风格；签名只发生在 `NuGet` 配置下，因此 `Release` 产物是**未签名**的，
与原插件编译时一致（原内嵌件的 `PublicKeyToken` 都是 `null`）。`Machina` 在其它配置下会
引用仓库里没有的 `.snk`，所以只能走 `Release`。
[`third_party/Directory.Build.props`](third_party/Directory.Build.props) 是一份空文件，
用来阻止本仓库根部的构建设置（net48、统一输出目录等）渗进子模块。

克隆后如果子模块是空的，脚本会自动 `git submodule update --init`。

> **保真度说明：** 原版插件内嵌的其实是**中文分支自己的 fork 构建**——该 fork 给
> `Machina.FFXIV` 增加了 `Headers.Chinese.Server_SkillType`、
> `Headers.Chinese.Server_StatusEffectListForay3`，并替换了 `Server_ActionEffect*` 的字段名；
> 给 `Machina` 增加了 `Infrastructure.ConcurrentList`。这些改动**不在上游仓库里**，其源码也
> 未公开（`TundraWork/FFXIV_ACT_Plugin_CN` 只是发布宿主，只有一个 README 和 SDK 压缩包）。
> 因此用上游源码编译出来的程序集与原内嵌件有 82 + 37 处声明差异。
>
> 插件自身的源码没有引用这些成员（它绑定的是上游也有的 `Headers.Server_StatusEffectListForay3`），
> 所以能正常编译运行；受影响的只有简体中文客户端下这两种包的解码路径。需要与原版逐位一致时，
> 用 `--from-distribution` 切换到发行包内嵌的 fork 程序集即可。

### 第二步：编译

```powershell
dotnet build FFXIV_ACT_Plugin.sln -c Release
```

当前状态：**0 error / 4 warning**（两个 `CS0169`、两个 `CS0414`，都是反编译代码里
未被使用的字段，与原始二进制行为一致）。

### 产物

```
artifacts/dist/Release/          ★ 单文件投放产物
  FFXIV_ACT_Plugin.dll
artifacts/bin/Release/           各工程原始输出（诊断用）
```

把 `artifacts/dist/Release/FFXIV_ACT_Plugin.dll` **单独一个文件**，加上
`deucalion-<版本>.distrib.dll` 放进 ACT 插件目录即可运行——不需要同级放置
`FFXIV_ACT_Plugin.*.dll`、`Machina*.dll` 或 `Newtonsoft.Json.dll`，它们都已织入主 DLL。

### 关于 ACT 版本

原插件针对 **ACT 3.8.5.288** 编译。可获取的 NuGet 包最高到 **3.8.4.287**，脚本默认用它。
ACT 程序集未强命名，且 `VerifyAssemblyVersions()` 是空实现，因此可以编译并正常运行；
若要严格对齐，用 `--act-path` 指向 3.8.5.288 的同名文件即可。

## 3. 单文件投放验证

[`tools/PluginLoadTest`](tools/PluginLoadTest/Program.cs) 会把主 DLL 单独复制到一个空目录、
以 ACT 加载插件的方式加载它，并强制解析全部内嵌程序集（`Assembly.Load(byte[])` 载入的
程序集 `Location` 为空，以此判定确实来自内嵌资源）：

```powershell
dotnet build tools/PluginLoadTest/PluginLoadTest.csproj -c Release
artifacts/loadtest/Release/PluginLoadTest.exe artifacts/dist/Release/FFXIV_ACT_Plugin.dll
```

```
siblings      : 0 other file(s) in the folder
bundled assemblies
  OK   Advanced Combat Tracker          not bundled, supplied by the host
  OK   FFXIV_ACT_Plugin.Common          v3.0.0.0  <embedded>
  ...  (7 个 FFXIV_ACT_Plugin.* 全部 <embedded>)
  OK   Machina                          v2.3.1.3  <embedded>
  OK   Machina.FFXIV                    v2.4.7.1  <embedded>
  OK   Newtonsoft.Json                  v13.0.0.0 <embedded>
  OK   Newtonsoft.Json round-trip       {"hello":"world"}

PASS - 10 assemblies resolved from the embedded resources, nothing on disk
```

## 4. 相对原始二进制所做的改动

反编译产物与原始二进制在声明层面完全一致（见 §5），只有以下几处是**有意**的调整；
每一处都在源码/工程文件中留有注释：

1. **Costura 在构建期重新织入，而不是提交内嵌产物。**
   原 `FFXIV_ACT_Plugin.dll` 用 Costura/Fody 把 10 个程序集 deflate 压缩内嵌。
   本仓库保留这一分发方式，但用 `PackageReference` + `FodyWeavers.xml` 让 Fody 在
   **构建时**把 `ProjectReference` 的输出织进去——单文件投放能力与原版一致，内嵌的每一份
   又都是本仓库源码编译出来的，仓库里不需要存 3.2 MB 的压缩二进制。
   与原始的差异仅在生成物层面：Fody 6.x 的 loader 实现不同，并额外内嵌了它自己的运行时
   辅助程序集 `Costura.dll` 与索引资源 `costura.metadata`；Fody 标记类由 `ProcessedByFody`
   改名为 `FFXIV_ACT_Plugin_ProcessedByFody`。
2. **设计器资源以 `.resx` 而非 `.resources` 提交。**
   主程序集清单里有两个空的 WinForms 设计器资源容器
   （`FFXIV_ACT_Plugin.{Settings,Benchmark}PropertyPage.resources`，各 180 字节）。
   它们无法从 IL 到达，但确实存在于原始清单中。这里提交两份**空的 `.resx` 文本文件**，
   由 MSBuild 的 GenerateResource 在构建时编译成同名资源——实测产物与原始 180 字节
   逐字节相同（md5 `cd7dbc7abeda9893ce25793744443958`），从而避免在仓库里放二进制。
3. **去重内嵌资源。**
   原始 `FFXIV_ACT_Plugin.Resource.dll` 中
   `...Generated.ActionList_Chinese.txt` 与 `...Generated.StatusList_Chinese.txt`
   各自被**重复内嵌了两份完全相同的字节**（`ManifestResource` 表里是两条同名记录）。
   MSBuild 会因此报 `CS1508`，而且 .NET 本身也只能解析到其中一份，所以只保留一份，
   删除了 ILSpy 命名出来的 `*_Chinese_2.txt`。
4. **移除了构造函数里冗余的 `AssemblyLoader.Attach();`。**
   Costura.Fody 已从织入的模块初始化函数（`.cctor`）里调用它，且 `Attach()` 幂等，
   因此不再在人工源码中引用生成代码。

除此之外没有对反编译代码做任何功能性修改。

## 5. 还原过程与校验

`tools/` 下的脚本可以完整复现这套源码：

| 脚本 | 作用 |
| --- | --- |
| [`tools/fetch_references.py`](tools/fetch_references.py) | 获取编译所需的四个引用程序集到 `refs/`：Machina 由子模块源码编译，其余取自 nuget.org，`--from-distribution` 可切换到发行包内嵌的原版 fork 程序集（见 §2） |
| [`tools/decompile.py`](tools/decompile.py) | 解压发行包 → 用 `ilspycmd --resource` + zlib 解出 10 个 Costura 内嵌程序集 → 用 `ilspycmd -p` 把 8 个托管程序集反编译成可编译工程（输出到 `.work/out/`） |
| [`tools/gen_projects.py`](tools/gen_projects.py) | 把 ilspycmd 生成的工程改写成最终的 `src/**/*.csproj`：同级程序集改为 `ProjectReference`，第三方程序集改为指向 `refs/` 的 copy-local `Reference`（以便 Costura 织入），并写入 Costura.Fody 配置、单文件 staging 目标与资源去重 |
| [`tools/verify_reconstruction.py`](tools/verify_reconstruction.py) | 校验：把原始 DLL 与重新编译出的 DLL 的元数据表（TypeDef / MethodDef / Field / Property / Event / ManifestResource）逐表比对 |
| [`tools/compare_pe_flags.py`](tools/compare_pe_flags.py) | 校验：比对原始与重新编译产物的 PE / CLR 头标志（目标平台、ILONLY、32 位优先、强名称等） |
| [`tools/PluginLoadTest`](tools/PluginLoadTest/Program.cs) | 功能校验：单文件产物放进空目录加载，强制解析全部内嵌程序集并实测 Newtonsoft.Json 可用 |
| [`tools/analyze_build.py`](tools/analyze_build.py) | 汇总 `dotnet build` 的报错/告警 |

复现步骤（需要 `dotnet tool install -g ilspycmd`）：

```powershell
python tools/fetch_references.py
python tools/decompile.py path/to/FFXIV_ACT_Plugin_3.0.2.1_CN.zip
python tools/gen_projects.py
dotnet build FFXIV_ACT_Plugin.sln -c Release
python tools/verify_reconstruction.py
python tools/compare_pe_flags.py

dotnet build tools/PluginLoadTest/PluginLoadTest.csproj -c Release
artifacts/loadtest/Release/PluginLoadTest.exe artifacts/dist/Release/FFXIV_ACT_Plugin.dll
```

> `decompile.py` 与 `verify_reconstruction.py` 需要原始发行包（仓库不保存它）；
> 把 zip 放到 `reference/` 或 `.work/reference/`，或在命令行给出路径。

校验结果：

```
FFXIV_ACT_Plugin           types=38    methods=327   fields=284   synth=112  OK
FFXIV_ACT_Plugin.Common    types=27    methods=280   fields=112   synth=48   OK
FFXIV_ACT_Plugin.Config    types=7     methods=21    fields=27    synth=0    OK
FFXIV_ACT_Plugin.Logfile   types=10    methods=85    fields=58    synth=24   OK
FFXIV_ACT_Plugin.Memory    types=96    methods=384   fields=368   synth=8    OK
FFXIV_ACT_Plugin.Network   types=37    methods=162   fields=114   synth=0    OK
FFXIV_ACT_Plugin.Parse     types=126   methods=727   fields=537   synth=59   OK
FFXIV_ACT_Plugin.Resource  types=59    methods=350   fields=173   synth=3    OK

8/8 assemblies verified with an identical declaration surface
8/8 assemblies with identical PE/CLR flags
```

* 校验允许的差异只有三类：被去重的两条 `*_Chinese.txt`、当前 Roslyn 自动注入的
  `EmbeddedAttribute` / `RefSafetyRulesAttribute`，以及 Fody 6.x 重新生成的
  `Costura.AssemblyLoader` 与 `FFXIV_ACT_Plugin_ProcessedByFody` 标记类。
* `synth=` 一列是编译器自动生成的名字（闭包类 `<>c__DisplayClass*`、lambda
  `<...>b__*`、缓存委托字段等）。重新编译时 Roslyn 会重新编号这些名字
  （例如 `foreach` 变量在反编译源码里必须改名为 `x2` 才能通过作用域检查），
  这些差异单独统计、不参与判定。
* 重新编译出的子程序集体积与原始一致（如 `FFXIV_ACT_Plugin.Common.dll`
  24576 → 24576、`FFXIV_ACT_Plugin.Network.dll` 40448 → 40448；
  `FFXIV_ACT_Plugin.Resource.dll` 由 9223168 变为 8356864，正好等于去掉的那份重复
  `Chinese` 资源）。单文件主 DLL 为 3,138,048 B，原始为 3,348,480 B——差异来自
  Costura 6 更好的压缩率与它自带的 loader 程序集。

## 6. `deucalion`（Rust 原生 DLL）

`deucalion-<版本>.distrib.dll` 是 **Rust 编译出的原生 DLL**，没有托管元数据，
无法“反编译成可编译源码”。因此 vendoring 与之**逐字节对应**的上游源码：

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

详见 [`third_party/deucalion/README.md`](third_party/deucalion/README.md)。

## 7. 许可与来源

* `src/` 的 C# 源码是 [TundraWork/FFXIV_ACT_Plugin_CN](https://github.com/TundraWork/FFXIV_ACT_Plugin_CN)
  release `26.6.9.0` 二进制的反编译产物。
* `third_party/machina/` 为 [ravahn/machina](https://github.com/ravahn/machina) 的 git
  子模块（固定在上表提交），许可证见该目录下的 `LICENSE.md`。
* `third_party/deucalion/` 为 [ff14wed/deucalion](https://github.com/ff14wed/deucalion)
  上游 GPL-3.0 源码，许可证见各版本目录下的 `LICENSE.md`。
* 编译用外部程序集（ACT、Machina、Machina.FFXIV、Newtonsoft.Json）由
  `tools/fetch_references.py` 在本地获取，仓库不保存，请遵循各自许可证。
