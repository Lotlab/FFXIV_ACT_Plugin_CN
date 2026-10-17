# FFXIV_ACT_Plugin 3.0.2.1 (CN) — 反编译还原源码

本仓库保存 **ACT（Advanced Combat Tracker）FFXIV 解析插件 3.0.2.1 简体中文版**的分发
二进制反编译还原出的源代码，可以正常编译，并产出与原件一致的插件投放目录。

反编译对象是 [TundraWork/FFXIV_ACT_Plugin_CN](https://github.com/TundraWork/FFXIV_ACT_Plugin_CN)
release `26.6.9.0` 的 `FFXIV_ACT_Plugin_3.0.2.1_CN.zip`（5,591,612 字节，
sha256 `94a9364b…bde844`），内含三个文件：

| 压缩包内的文件 | 类型 | 还原方式 |
| --- | --- | --- |
| `FFXIV_ACT_Plugin.dll` | .NET Framework 4.8 托管程序集 | 反编译为 C# 源码（`src/`，8 个工程） |
| `deucalion-1.2.1.distrib.dll` | Rust 原生 DLL | 上游源码子模块 `third_party/deucalion/1.2.1` |
| `deucalion-1.5.0.distrib.dll` | Rust 原生 DLL | 上游源码子模块 `third_party/deucalion/1.5.0` |

## 0. 仓库约定

* **不提交任何二进制**（没有 .dll / .exe / .zip / .resources）。
* **依赖全部走标准机制**：能上 NuGet 的用 `PackageReference`，需要源码的用 git 子模块 +
  `ProjectReference`。因此没有 `refs/` 目录，也没有自定义的取件脚本。

## 1. 目录结构

```
FFXIV_ACT_Plugin.sln                解决方案（11 个工程：8 个插件工程 + Unscrambler + 2 个 machina）
Directory.Build.props               公共 MSBuild 设置（net48、统一输出目录）
src/
  FFXIV_ACT_Plugin/                 主插件（原 FFXIV_ACT_Plugin.dll）
    FodyWeavers.xml                 Costura.Fody 配置（单文件打包）
    SettingsPropertyPage.resx       空的 WinForms 设计器资源（见 §5）
    BenchmarkPropertyPage.resx
  FFXIV_ACT_Plugin.Common/          公共接口与数据模型
  FFXIV_ACT_Plugin.Config/          配置
  FFXIV_ACT_Plugin.Logfile/         日志格式化 / 输出
  FFXIV_ACT_Plugin.Memory/          内存读取（特征码、角色数据）
  FFXIV_ACT_Plugin.Network/         网络包捕获、反混淆与解析
  FFXIV_ACT_Plugin.Parse/           战斗日志解析
  FFXIV_ACT_Plugin.Resource/        内置资源（技能/状态/地图/副本等多语言表）
  FFXIV_ACT_Plugin.Unscrambler/     Unscrambler 的 net48 编译工程（见 §4）
third_party/
  Directory.Build.props/.targets    隔离与降噪，见 §2
  machina/                          子模块：Lotlab/machina @ feature/oodle-state-recovery-7.57
  Unscrambler/                      子模块：perchbirdd/Unscrambler @ main（密钥推导算法）
  Unscrambler.CN/                   子模块：Latihas/Unscrambler @ main（CN 常量 + 内嵌密钥表）
  deucalion/1.5.0/ 1.2.1/           子模块：ff14wed/deucalion @ 对应 tag
tools/                              反编译与校验脚本，见 §6
  PluginLoadTest/                   单文件投放的加载测试
  UnscramblerSmokeTest/             反混淆端到端冒烟测试（临时 ffxivgame.ver + 真实 CN 表）
artifacts/                          构建输出（gitignore）
reference/ .work/                   发行包缓存与反编译中间产物（gitignore）
```

## 2. 编译

### 环境要求

* .NET SDK（本仓库使用 10.0.401 验证）
* .NET Framework 4.8 targeting pack
* Windows（`net48` + WinForms）
* 首次编译需要联网（NuGet 上的 ACT / Newtonsoft.Json / Fody）

### 步骤

```powershell
git clone --recurse-submodules <this repo>
# 已经 clone 过的话：
git submodule update --init --recursive

dotnet build FFXIV_ACT_Plugin.sln -c Release
```

当前状态：**0 error / 2 warning**（一个 `CS0169`、一个 `CS0414`，都是反编译代码里未被使用
的字段，与原始二进制行为一致）。

### 依赖是怎么接上的

| 依赖 | 方式 | 说明 |
| --- | --- | --- |
| `Advanced Combat Tracker` | `PackageReference` `AdvancedCombatTracker` 3.8.4.287 | 宿主进程程序集：`ExcludeAssets=runtime`，只在编译期引用，不会拷到插件旁边、也不会被 Costura 织入 |
| `Newtonsoft.Json` | `PackageReference` 13.0.1 | 其 `net45` 程序集与原插件内嵌的**逐字节相同**（md5 `081d9558…`），copy-local 以便被织入 |
| `Machina` / `Machina.FFXIV` | `ProjectReference` → `third_party/machina` 子模块 | **Lotlab fork**，分支 `feature/oodle-state-recovery-7.57`（见 §7）。两个都是 netstandard2.0，Release 构建未签名 |
| `Unscrambler` | `ProjectReference` → `src/FFXIV_ACT_Plugin.Unscrambler`（源码来自 `third_party/Unscrambler.CN` + `third_party/Unscrambler`） | 官方包只有 net10.0 程序集，net48 无法加载，因此在本地按源码编译：CN fork 提供常量、内嵌表和 `IUnscrambler`，上游提供密钥推导算法（见 §4） |
| `WindowsFirewallHelper` | `PackageReference` 1.6.3.46 | “修复防火墙规则”按钮用；2.7.2.6 用的是 1.6.3.40 |
| `Fody` / `Costura.Fody` | `PackageReference` | 单文件打包，见 §5 |

`third_party/Directory.Build.props` 是一份空文件，用来阻止本仓库根部的构建设置（net48、
统一输出目录）渗进子模块；`third_party/Directory.Build.targets` 关掉子模块自带的代码风格
分析器——否则构建会刷出 200+ 条 IDE0073 警告，把本工程自己的 4 条淹没。两者都只影响诊断，
不改变产物。

### 关于 ACT 版本

原插件针对 **ACT 3.8.5.288** 编译，NuGet 上可获得的最高版本是 **3.8.4.287**。ACT 程序集
未强命名，且插件里的 `VerifyAssemblyVersions()` 是空实现，因此可以编译并正常运行；要严格
对齐就把 `AdvancedCombatTracker` 换成本地 3.8.5.288 的引用。

### 产物：`artifacts/dist/<Configuration>/`

```
FFXIV_ACT_Plugin.dll        单文件托管插件（已织入 7 个子程序集 + Machina* + Newtonsoft.Json）
deucalion-1.2.1.dll         原生 payload
deucalion-1.5.0.dll
```

这个目录就是**完整的投放目录**，直接放进 ACT 插件目录即可。两个 deucalion DLL 不需要单独
处理：它们是 `Machina.FFXIV` 自己的 `Content` 项，构建时随之产出，且与原件 **sha256 完全
一致**（`eabfff9e…` / `326be4db…`）。

## 3. 单文件投放验证

`FFXIV_ACT_Plugin.dll` 自身是自洽的：它把全部托管依赖（8 个 FFXIV_ACT_Plugin.*、
Machina、Machina.FFXIV、Unscrambler 及其 `System.Memory` 等依赖、Newtonsoft.Json）
织进了内部资源。但仍需要同目录的 `deucalion-*.dll`——`Machina.FFXIV.Deucalion.DeucalionInjector`
在运行时会按路径加载原生 DLL，而**这个 CN 构建没有任何下载逻辑**
（`ACT_PluginUpdate.UpdateCheckClicked_Version()` 是空方法，全工程也搜不到
`WebClient`/`HttpClient`/`DownloadFile`），所以必须随包投放。

[`tools/PluginLoadTest`](tools/PluginLoadTest/Program.cs) 会把主 DLL 单独复制到一个空目录、
以 ACT 加载插件的方式加载它，并强制解析全部内嵌程序集（`Assembly.Load(byte[])` 载入的程序集
`Location` 为空，以此判定确实来自内嵌资源）：

```powershell
dotnet build tools/PluginLoadTest/PluginLoadTest.csproj -c Release
artifacts/loadtest/Release/PluginLoadTest.exe artifacts/dist/Release/FFXIV_ACT_Plugin.dll
```

```
siblings      : 0 other file(s) in the folder
bundled assemblies
  OK   Advanced Combat Tracker          not bundled, supplied by the host
  OK   FFXIV_ACT_Plugin.Common          v3.0.0.0  <embedded>
  ...  (8 个 FFXIV_ACT_Plugin.* 全部 <embedded>)
  OK   Machina                          v2.3.1.3  <embedded>
  OK   Machina.FFXIV                    v2.4.8.1  <embedded>
  OK   Unscrambler                      <embedded>
  OK   WindowsFirewallHelper            <embedded>
  OK   Newtonsoft.Json                  v13.0.0.0 <embedded>
  OK   Newtonsoft.Json round-trip       {"hello":"world"}
  OK   Unscrambler constants            1 CN client version(s)

PASS - 13 assemblies resolved from the embedded resources, nothing on disk
```

## 4. 网络抓包模式与反混淆

3.0.2.1 的发行版只会把 deucalion 注入游戏进程来抓包。本节恢复 2.7.2.6 及更早版本提供的
抓包设置，并补上非注入抓包必需的反混淆。

### 4.1 抓包设置

设置页「FFXIV解析插件」分组恢复了这些控件（与 2.7.2.6 相同）：

| 控件 | 配置字段 | 作用 |
| --- | --- | --- |
| 本机IP过滤 + 刷新网卡列表 | `NetworkIP` | 只监听一个本机地址，而不是全部网卡 |
| 使用Npcap捕获游戏数据 | `UseWinPCap` | 用 Npcap 驱动抓包（需安装驱动）；不勾选则用原始套接字 |
| 使用进程注入捕获游戏数据 | `UseDeucalion` | 注入 deucalion 抓包，即 3.0.2.1 原来的固定行为 |
| 禁用高性能网络数据过滤器 | `UseSocketFilter` | 关闭“只抓游戏服务器地址”的内核过滤 |
| 修复防火墙规则 | —（按钮） | 删除并重建名为 `Advanced Combat Tracker` 的放行规则，供原始套接字模式使用 |

注入**默认不勾选**，也就是默认回到非注入抓包。`DataCollectionSettingsEventArgs` 恢复了
`NetworkIP` / `UseWinPCap` / `UseSocketFilter` / `UseDeucalion` 四个字段；
`SettingsMediator` 在这四个字段任一变化时立即重启抓包，不需要重开 ACT。
`ScanPackets` 据此在原始套接字 / Npcap / deucalion 三条路径中选择。原始套接字模式依赖
Windows 防火墙放行，因此设置页同时恢复了 2.7.2.6 的“修复防火墙规则”按钮
（[`FirewallManager`](src/FFXIV_ACT_Plugin.Network/FFXIV_ACT_Plugin.Network/FirewallManager.cs)，
基于 `WindowsFirewallHelper` 1.6.3.46）。

### 4.2 反混淆

游戏自 7.2 起对一部分服务器包（PlayerSpawn、NpcSpawn、ActionEffect、StatusEffectList、
ActorControl、ActorCast 等）做异或/减法混淆。deucalion 读到的是游戏已经解密的数据；
原始套接字和 Npcap 读到的仍是混淆后的数据，不还原的话伤害、状态等字段都是错的。

[`NetworkPacketUnscrambler`](src/FFXIV_ACT_Plugin.Network/FFXIV_ACT_Plugin.Network/NetworkPacketUnscrambler.cs)
使用 [Latihas/Unscrambler](https://github.com/Latihas/Unscrambler)（CN fork）和
[perchbirdd/Unscrambler](https://github.com/perchbirdd/Unscrambler)（密钥推导）完成还原。
两个仓库缺一不可：

* **Latihas/Unscrambler（CN fork）**提供 CN 客户端的 `VersionConstants`、被混淆 opcode 表，
  以及 `Data/<版本>/` 下的六张密钥推导表；构建时这些表作为内嵌资源编进 `Unscrambler.dll`。
  它只保留作者最近一次更新的客户端版本（当前 `2026.10.06`）。
* **perchbirdd/Unscrambler（上游）**提供 `Derivation/` 和 `KeyGeneratorFactory`。CN fork
  本身没有这部分代码，因为它的消费者 IINACT 是注入式插件：它 hook
  `PacketDispatcher_OnReceivePacket`，直接从 hook 参数读 key0/1/2、从模块内存读 opcode
  key table。我们的插件是外部抓包，拿不到这些 key，所以用上游算法从服务器下发的初始化包
  推导密钥。

工作流程：

* 客户端版本取自游戏主程序同目录的 `ffxivgame.ver`；
* `KeyGeneratorFactory.ForGameVersion` 用 CN fork 内嵌的六张表初始化密钥推导器，
  `UnscramblerFactory.ForGameVersion` 得到 CN fork 的 `IUnscrambler`；
* 密钥在收到服务器的初始化包时生成（7.4 以前是 InitZone，7.4 起是
  `UnknownObfuscationInitOpcode` 对应的包）；混淆包在 padded 副本里还原，避免库按整包
  偏移写入时越界；
* 只在非注入模式下启用。客户端版本不在 CN fork 里时只是关闭反混淆并在“测试游戏连接”
  里说明原因，不会用错误的密钥改包。

Unscrambler 官方程序集只针对 net10.0，ACT（net48）无法加载，因此
[`FFXIV_ACT_Plugin.Unscrambler`](src/FFXIV_ACT_Plugin.Unscrambler/FFXIV_ACT_Plugin.Unscrambler.csproj)
把两个子模块的相关源码编成 net48 程序集，并用 `Compat/` 里的 `Span<byte>`
`BitConverter` 重载和 `Index`/`Range` shim 补齐缺失 API。

> CN 客户端更新后，把 `third_party/Unscrambler.CN` 子模块更新到 Latihas 的新提交即可：
> 他的 fork 每次发版会替换 `Constants*.cs` 和 `Data/<版本>/`，不需要再生成或手改常量。
> 代价是反混淆只覆盖 fork 里当前那一个版本；若要在旧客户端上继续用，需要保留旧版本的
> 常量/表，或者回退到“官方 CN NuGet 常量 + 运行时读 exe”的方案。

> 提示：Machina 的 opcode 表（`Headers/Opcodes/Chinese.txt`）与 Unscrambler 的 CN 常量
> 来源不同，需要各自随补丁更新。当前 `feature/oodle-state-recovery-7.57` 基于上游
> `ravahn/machina` 的 `e92320c`（v2.4.8.1，CN 7.57，`StatusEffectList = 0x2CE`），
> 与 CN fork 的常量 `2026.10.06` 对齐。

## 5. 相对原始二进制所做的改动

反编译产物与原始二进制在声明层面完全一致（见 §6），只有以下几处是**有意**的调整；每一处
都在源码/工程文件中留有注释：

1. **Costura 在构建期重新织入，而不是提交内嵌产物。**
   原 `FFXIV_ACT_Plugin.dll` 用 Costura/Fody 把 10 个程序集 deflate 压缩内嵌。本仓库保留
   这一分发方式，但用 `PackageReference` + `FodyWeavers.xml` 让 Fody 在**构建时**把
   `ProjectReference`/`PackageReference` 的输出织进去——单文件投放能力与原版一致，内嵌的
   每一份又都是本仓库编译出来的，仓库里不需要存 3.2 MB 压缩二进制。
   差异仅在生成物层面：Fody 6.x 的 loader 实现不同，并额外内嵌了它自己的运行时辅助程序集
   `Costura.dll` 与索引资源 `costura.metadata`；Fody 标记类由 `ProcessedByFody` 改名为
   `FFXIV_ACT_Plugin_ProcessedByFody`。
2. **设计器资源以 `.resx` 而非 `.resources` 提交。**
   主程序集清单里有两个空的 WinForms 设计器资源容器
   （`FFXIV_ACT_Plugin.{Settings,Benchmark}PropertyPage.resources`，各 180 字节）。它们无法
   从 IL 到达，但确实存在于原始清单中。这里提交两份**空的 `.resx` 文本文件**，由 MSBuild 的
   GenerateResource 在构建时编译成同名资源——实测产物与原始 180 字节逐字节相同
   （md5 `cd7dbc7abeda9893ce25793744443958`）。
3. **去重内嵌资源。**
   原始 `FFXIV_ACT_Plugin.Resource.dll` 中 `...Generated.ActionList_Chinese.txt` 与
   `...Generated.StatusList_Chinese.txt` 各自被**重复内嵌了两份完全相同的字节**
   （`ManifestResource` 表里是两条同名记录）。MSBuild 会因此报 `CS1508`，而且 .NET 本身
   也只能解析到其中一份，所以只保留一份。
4. **移除了构造函数里冗余的 `AssemblyLoader.Attach();`。**
   Costura.Fody 已从织入的模块初始化函数（`.cctor`）里调用它，且 `Attach()` 幂等。
5. **恢复了非注入抓包设置并新增反混淆。**
   `SettingsPropertyPage` 找回 2.7.2.6 的抓包控件（含“修复防火墙规则”按钮与
   `FirewallManager`），`DataCollectionSettingsEventArgs` 找回对应字段，`ScanPackets`
   支持原始套接字 / Npcap / deucalion 三种抓包路径；`FFXIV_ACT_Plugin.Network` 新增
   `INetworkPacketUnscrambler` 与 `NetworkPacketUnscrambler`。
   这些是**功能性新增**，不与 3.0.2.1 的声明面一致，已在 §4 说明，并在
   `verify_reconstruction.py` 的 `FEATURE_TYPES` / `FEATURE_MEMBERS` 白名单里逐条登记。

除上述改动外，没有对反编译代码做任何功能性修改；`verify_reconstruction.py` 的白名单机制
仍能拦截任何未登记的声明漂移。

### 已知的元数据差异（不影响运行）

* `AssemblyRef` 里 `Advanced Combat Tracker` 是 3.8.4.287（原件 3.8.5.288）；该程序集未强
  命名，运行期绑定不受影响。
* 重建的主程序集多一条**空的** `netstandard` 引用（无 TypeRef，GAC 中可按完整标识解析）。
* 重建的主程序集少一条 `Machina` 引用——原件的这条引用在插件源码里找不到任何对应使用，
  是编译器裁掉的未使用引用；`Machina` 仍被织入并可从内嵌资源解析（见 §3 的加载测试）。

## 6. 还原过程与校验

| 脚本 | 作用 |
| --- | --- |
| [`tools/decompile.py`](tools/decompile.py) | 解压发行包 → 用 `ilspycmd --resource` + zlib 解出 10 个 Costura 内嵌程序集 → 用 `ilspycmd -p` 把 8 个托管程序集反编译成可编译工程（输出到 `.work/out/`）。发行包不入库，用命令行参数或放到 `reference/`、`.work/reference/` |
| [`tools/gen_projects.py`](tools/gen_projects.py) | 把 ilspycmd 生成的工程改写成最终的 `src/**/*.csproj`：同级程序集 → `ProjectReference`，第三方 → `PackageReference` 或子模块 `ProjectReference`，并写入 Costura 配置、投放目录 staging 目标、资源去重 |
| [`tools/verify_reconstruction.py`](tools/verify_reconstruction.py) | 校验：原始 DLL 与重新编译 DLL 的元数据表（TypeDef / MethodDef / Field / Property / Event / ManifestResource）逐表比对 |
| [`tools/compare_pe_flags.py`](tools/compare_pe_flags.py) | 校验：PE / CLR 头标志（目标平台、ILONLY、32 位优先、强名称等） |
| [`tools/PluginLoadTest`](tools/PluginLoadTest/Program.cs) | 功能校验：单文件产物放进空目录加载，强制解析全部内嵌程序集，实测 Newtonsoft.Json 与 Unscrambler（含 System.Memory 等依赖和 CN 常量表）可用 |
| [`tools/UnscramblerSmokeTest`](tools/UnscramblerSmokeTest/Program.cs) | 反混淆端到端冒烟测试：临时目录里放 `ffxivgame.ver`，用 CN fork 内嵌的真实密钥表走完初始化、密钥生成与一次解包 |
| [`tools/build_deucalion.py`](tools/build_deucalion.py) | 从 deucalion 子模块重建两个原生 DLL（见 §7） |
| [`tools/analyze_build.py`](tools/analyze_build.py) | 汇总 `dotnet build` 的报错/告警 |

复现步骤（需要 `dotnet tool install -g ilspycmd`）：

```powershell
python tools/decompile.py path/to/FFXIV_ACT_Plugin_3.0.2.1_CN.zip
python tools/gen_projects.py
dotnet build FFXIV_ACT_Plugin.sln -c Release
python tools/verify_reconstruction.py
python tools/compare_pe_flags.py

dotnet build tools/PluginLoadTest/PluginLoadTest.csproj -c Release
artifacts/loadtest/Release/PluginLoadTest.exe artifacts/dist/Release/FFXIV_ACT_Plugin.dll

dotnet build tools/UnscramblerSmokeTest/UnscramblerSmokeTest.csproj -c Release
artifacts/smoketest/Release/UnscramblerSmokeTest.exe
```

> `decompile.py` 与 `verify_reconstruction.py` 需要原始发行包（仓库不保存它）。
> `decompile.py` + `gen_projects.py` 只重建反编译**基线**；§4 的反混淆功能是在基线之上
> 加入的，重新运行 `gen_projects.py` 会覆盖 `src/**/*.csproj`，需要重新补上
> `FFXIV_ACT_Plugin.Network` → `FFXIV_ACT_Plugin.Unscrambler` 的 `ProjectReference`、
> 两个 Unscrambler 子模块的源码/资源项，以及解决方案里的两个 machina 工程。

校验结果：

```
FFXIV_ACT_Plugin           types=38    methods=327   fields=284   synth=112  OK
FFXIV_ACT_Plugin.Common    types=27    methods=280   fields=112   synth=48   OK
FFXIV_ACT_Plugin.Config    types=7     methods=21    fields=27    synth=0    OK
FFXIV_ACT_Plugin.Logfile   types=10    methods=85    fields=58    synth=24   OK
FFXIV_ACT_Plugin.Memory    types=96    methods=384   fields=368   synth=8    OK
FFXIV_ACT_Plugin.Network   types=37    methods=162   fields=114   synth=4    OK
FFXIV_ACT_Plugin.Parse     types=126   methods=727   fields=537   synth=59   OK
FFXIV_ACT_Plugin.Resource  types=59    methods=350   fields=173   synth=3    OK

8/8 assemblies verified with an identical declaration surface
8/8 assemblies with identical PE/CLR flags
```

* 校验允许的差异分两类：一类是还原本身的（被去重的两条 `*_Chinese.txt`、当前 Roslyn
  自动注入的 `EmbeddedAttribute` / `RefSafetyRulesAttribute`、Fody 6.x 重新生成的
  `Costura.AssemblyLoader` 与 `FFXIV_ACT_Plugin_ProcessedByFody` 标记类）；另一类是
  §4/§5 记录的反混淆功能新增，逐条列在 `verify_reconstruction.py` 的
  `FEATURE_TYPES` / `FEATURE_MEMBERS`，其余任何声明漂移都会让校验失败。
* `synth=` 一列是编译器自动生成的名字（闭包类、lambda、缓存委托字段等）。重新编译时
  Roslyn 会重新编号（例如 `foreach` 变量在反编译源码里必须改名为 `x2` 才能通过作用域检查），
  这些差异单独统计、不参与判定。
* 未改动子程序集的体积与原始一致（`FFXIV_ACT_Plugin.Common.dll` 24576 → 24576；
  `Resource.dll` 9223168 → 8356864，正好等于去掉的那份重复 `Chinese` 资源）。
  `Network.dll` 因为新增了反混淆代码而变大。

### `Machina` 的说明

`third_party/machina` 固定为 **Lotlab/machina** 的 `feature/oodle-state-recovery-7.57`
（基于上游 `ravahn/machina` 的 `e92320c` v2.4.8.1，重放 VLAN、Oodle 非托管缓冲、Oodle
状态恢复三个提交）。相对上游 `ravahn/machina`，这个分支带来两件事：

* **Oodle 状态恢复**：丢失 Oodle 解压状态（切图、字典重训）时，从游戏进程内存读回
  zone / chat 两个通道的 Oodle state 交给本地解码器，使非注入抓包不必等下一个训练包；
  相关代码在 `Machina.FFXIV/Memory/`，并给 `TCPNetworkMonitor` 增加了跨程序集用的
  `Connections`。
* **中文区服务器包头**：`Machina.FFXIV/Headers/Chinese/*`（`Server_ActionEffect`、
  `Server_StatusEffectList*` 等）以及中文 opcode 表，这正是 3.0.2.1 内嵌 Machina 的
  分支特性。

原版插件内嵌的 Machina 与上游声明一致（CN fork 还给 `Machina.FFXIV` 增加了
`Headers.Chinese.Server_SkillType`、`Headers.Chinese.Server_StatusEffectListForay3`
并替换了 `Server_ActionEffect*` 的字段名，这些在 `e92320c` 中已经包含）。插件本身的
源码没有引用这些成员（`StatusEffectListForay3` 处理器绑定的是根命名空间下的 Global
布局，与 CN 布局一致），因此用公开的 Lotlab 分支编译可以通过、加载测试也通过。

## 7. `deucalion`（Rust 原生 DLL）

`deucalion-<版本>.distrib.dll` 是 Rust 编译出的原生 DLL，没有托管元数据，无法反编译成
可编译源码，因此以**子模块**形式引入与之逐字节对应的上游源码：

* <https://github.com/ff14wed/deucalion>（GPL-3.0）
* `third_party/deucalion/1.5.0` ⇔ tag `1.5.0`（commit `5cc714ad…`）
* `third_party/deucalion/1.2.1` ⇔ tag `1.2.1`（commit `8b63caa5…`）
* 上游产物名为 `deucalion.dll`；`.distrib` 后缀是 ACT 打包时加的，插件注入前会改名
  （这样游戏锁定 DLL 时 ZIP 仍可重复解压）。二进制与上游 release 资源 sha256 完全一致。

**它不是本插件的编译输入**（`FFXIV_ACT_Plugin.dll` 不引用 deucalion），保留它只是为了让
“子模块版本 → 对应发行二进制”这一映射可查、可复现。需要重建时：

```powershell
python tools/build_deucalion.py            # 两个版本
python tools/build_deucalion.py --lib-only # 上游 CI 的第二道更小的构建
```

脚本会先把 `tokio-util` 钉回该 tag 发布时的版本（1.5.0 → 0.7.16、1.2.1 → 0.7.14），因为
上游不提交 `Cargo.lock`，按今天的 crates.io 会解析到 0.7.20，它新增的
`impl Encoder<Bytes> for LengthDelimitedCodec` 会让 `deucalion/src/rpc.rs:97` 的
`encode(data.into(), dst)` 产生 `E0283`/`E0284` 二义性错误。

还需要 **nightly Rust**（`lib.rs` 用了 `#![feature(link_llvm_intrinsics)]`）、MSVC 与
Windows SDK（`build.rs` 跑 `winres` 需要 `rc.exe`）。详见
[`third_party/deucalion/README.md`](third_party/deucalion/README.md)。

## 8. 许可与来源

* `src/` 的 C# 源码是 [TundraWork/FFXIV_ACT_Plugin_CN](https://github.com/TundraWork/FFXIV_ACT_Plugin_CN)
  release `26.6.9.0` 二进制的反编译产物。
* `third_party/machina/` 为 [Lotlab/machina](https://github.com/Lotlab/machina)
  子模块，分支 `feature/oodle-state-recovery-7.57`，上游为
  [ravahn/machina](https://github.com/ravahn/machina)（GPL-3.0，许可证见该目录下
  `LICENSE.md`）。
* `third_party/Unscrambler.CN/` 为 [Latihas/Unscrambler](https://github.com/Latihas/Unscrambler)
  子模块（CN fork，WTFPL，作者 perchbird / Latihas），提供 CN 常量与内嵌密钥表。
* `third_party/Unscrambler/` 为 [perchbirdd/Unscrambler](https://github.com/perchbirdd/Unscrambler)
  子模块（WTFPL），提供与客户端无关的密钥推导算法。`src/FFXIV_ACT_Plugin.Unscrambler`
  只做 net48 编译适配，把两者合成一个程序集。
* `third_party/deucalion/` 为 [ff14wed/deucalion](https://github.com/ff14wed/deucalion)
  子模块，上游 GPL-3.0。
* `WindowsFirewallHelper` 由 NuGet 还原（MIT），用于“修复防火墙规则”按钮。
* ACT、Newtonsoft.Json 等第三方程序集由 NuGet 在本地还原，仓库不保存，请遵循各自许可证。
