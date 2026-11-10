# FFXIV_ACT_Plugin CN — 上游二进制 + CN 补丁

本仓库构建 **ACT（Advanced Combat Tracker）FFXIV 解析插件的简体中文版**。

六个区域无关的程序集直接使用上游发行版，构建时剥离强名称并应用 CN 补丁；只有主插件
`FFXIV_ACT_Plugin` 和 `FFXIV_ACT_Plugin.Network` 由本仓库编译——中文界面和我们自己的功能
都在这两个程序集里。上游版本与 CN 数据都不写死在工程文件中：版本由
`upstream/lock.json` 选择，中文资源在 `cn/resource/` 维护。

## 0. 为什么只编译两个程序集

对当前基线（见 `upstream/lock.json`）逐方法做 IL 哈希比对（操作码 + 规范化操作数 +
异常处理），CN 版与上游版的差异集中在数据，而不是代码：

| 程序集 | 代码差异 |
| --- | --- |
| Common | 无 |
| Config | 无 |
| Logfile | 仅 `FormatVersion()`：CN 构建未签名，读取自身 token 会抛异常 |
| Memory | 无 |
| Network | 无 |
| Parse | 仅 `DoTSimulator.ProcessTick` 里两条字符串 |
| Resource | 无；差异全在内嵌资源（CN 专属 43 个） |
| main | UI 中文化、语言/区域默认值、updater 移除、本仓库的功能 |

因此“中文适配”主要是 **Resource 数据 + Machina + 少量 UI 字符串**。用上游二进制当底座、
把 CN 差异做成补丁，比维护八份反编译代码更小、也更容易跟随上游更新。

## 1. 仓库结构

```
upstream/
  lock.json                          选中的上游版本、release 资产名、zip sha256
  patch.json                         CN 补丁清单（字段 / 字符串 / 枚举 / 资源目录）
cn/
  resource/                          CN 专属内嵌资源（与上游不同的部分）
  resource-overrides.json            CSV 生成之后的手工补丁（见 §3.4）
src/
  FFXIV_ACT_Plugin/                  主插件（CN UI + 我们的功能）
  FFXIV_ACT_Plugin.Network/          抓包路径 + 反混淆
  FFXIV_ACT_Plugin.Unscrambler/      Unscrambler 的 net48 编译工程
third_party/
  machina/                           Lotlab CN fork（子模块）
  Unscrambler/ Unscrambler.CN/       密钥推导 + CN 常量（子模块）
  deucalion/                         原生 payload 对应源码（子模块）
tools/
  prepare_upstream.py                下载 → 解包 → 剥离 → 打补丁（--version 可换版本）
  generate_cn_resources.py           从 ffxiv-datamining-cn 的 CSV 生成 cn/resource
  refresh_cn_resources.py            从 CN 发行包重建 cn/resource（现在只用于比对基准）
  extract_cn_reference.py            解出 CN 原始程序集，供元数据比对
  build_deucalion.py                 从子模块重建原生 DLL（见 §6）
  AssemblyPatcher/                   Mono.Cecil 工具（extract / strip / patch / resources / dump）
  PluginLoadTest/                    单文件投放加载测试
  UnscramblerSmokeTest/              反混淆端到端冒烟测试
  verify_reconstruction.py           主插件 / Network 的元数据表比对
```

构建产物（`.gitignore`）：`artifacts/upstream/`、`artifacts/bin/`、`artifacts/dist/`、
`.work/`。

## 2. 构建

### 环境

* .NET SDK 10（验证用 10.0.401）
* .NET Framework 4.8 targeting pack
* Windows（`net48` + WinForms）
* 首次构建需要联网：NuGet、以及从 GitHub 拉取上游发行包

### 步骤

```powershell
python tools/prepare_upstream.py
dotnet build FFXIV_ACT_Plugin.sln -c Release
```

`prepare_upstream.py` 做五件事：

1. 按 `upstream/lock.json` 下载上游发行包并校验 sha256；
2. 解压出 `FFXIV_ACT_Plugin.dll`，把它内嵌的 Costura 程序集解压到 `.work/`；
3. 对六个 `FFXIV_ACT_Plugin.*` 程序集剥离强名称，并重写它们之间的 AssemblyRef；
4. 按 `upstream/patch.json` 打补丁（Config 字段、Logfile IL、Parse 字符串/枚举、
   `cn/resource` 资源）；
5. 输出到 `artifacts/upstream/current/`（路径不带版本号），同时写 `stamp.json` 记录版本、
   zip sha256 和每个程序集的 sha256。

重复执行是幂等的：每次都从上游原始包重新生成。

### 产物：`artifacts/dist/<Configuration>/`

```
FFXIV_ACT_Plugin.dll        单文件托管插件（织入六个上游件 + Network + Unscrambler +
                            Machina* + Newtonsoft.Json + WindowsFirewallHelper）
deucalion-1.5.1.dll         原生 payload（从上游发行包解出，见 §6）
```

这个目录就是完整的投放目录。上游自 3.0.3.4 起只发 `deucalion-1.5.1`，更早的版本不再随包；
`StagePluginFolder` 按 `deucalion-*.dll` 兜底，Machina 端按“目录里存在的最新版本”选择
（繁体中文区仍固定在 1.2.1，需要时自己把旧 payload 放进目录）。

### 依赖

| 依赖 | 方式 |
| --- | --- |
| `FFXIV_ACT_Plugin.Common/Config/Logfile/Memory/Parse/Resource` | `Reference` → `artifacts/upstream/current/`（上游件 + CN 补丁） |
| `FFXIV_ACT_Plugin.Network` | `ProjectReference`（本仓库源码） |
| `Machina` / `Machina.FFXIV` | `ProjectReference` → `third_party/machina`（Lotlab CN fork） |
| `Unscrambler` | `ProjectReference` → `src/FFXIV_ACT_Plugin.Unscrambler`（net48 适配） |
| `Advanced Combat Tracker` | `PackageReference`，仅编译期引用 |
| `Newtonsoft.Json` | `PackageReference` |
| `WindowsFirewallHelper` | `PackageReference` |
| `deucalion-1.5.1.dll` | `prepare_upstream.py` 从上游发行包解出 → `artifacts/upstream/current/` |
| `Fody` / `Costura.Fody` | `PackageReference`，单文件打包 |

上游 `Common` 针对较高版本的 ACT 编译，本仓库用的是 NuGet 上可获得的版本，构建时会有一条
`MSB3277` 版本冲突警告。ACT 程序集未强命名，插件里的 `VerifyAssemblyVersions()` 是空实现，
可以正常运行；要严格对齐就换成本地 ACT 的引用。

## 3. 上游程序集的处理

### 3.1 剥离强名称

上游程序集带 `PublicKeyToken`，而本仓库编译的 main / Network 是 unsigned。带签名的程序集
不能引用未签名的重建件，所以 `prepare_upstream.py` 把它们统一剥成 unsigned：

```csharp
asm.Name.HasPublicKey = false;
asm.Name.PublicKey = Array.Empty<byte>();
asm.Name.Attributes &= ~AssemblyAttributes.PublicKey;
asm.MainModule.Attributes &= ~ModuleAttributes.StrongNameSigned;   // CLI header flag 0x8
asm.Write(path, new WriterParameters { StrongNameKeyPair = null });
```

* 只清公钥、不清 `StrongNameSigned` 标志时，`Assembly.LoadFile` 会报
  `BadImageFormatException 0x80131018`；Cecil 不会自动清这个标志。
* 所有引用被剥离程序集的 `AssemblyRef` 也要把 token 置空（`strip` 命令一并处理）。
* 剥离后 `GetPublicKeyToken()` 返回空数组，因此上游 `LogFormat.FormatVersion()` 必须打补丁。
* 插件通过 Costura 的 `Assembly.Load(byte[])` 加载，.NET Framework 不校验强名称签名；
  这些程序集也不进 GAC。

### 3.2 补丁清单

`upstream/patch.json` 是唯一的 CN 补丁声明，`AssemblyPatcher patch` 负责执行：

| 程序集 | 补丁 | 原因 |
| --- | --- | --- |
| Config | `DataCollectionSettingsEventArgs` 增加 `NetworkIP`、`UseWinPCap`、`UseSocketFilter`、`UseDeucalion` 四个字段 | 非注入抓包需要的设置（见 §5） |
| Logfile | 把 `FormatVersion()` 里的 `BitConverter.ToUInt64(GetPublicKeyToken(), 0)` 改成 `0` | 未签名程序集的 token 为空 |
| Parse | `Real DoT Tick` / `Real HoT Tick` → 中文；`Job` 增加 `Bst = 45` | 中文本地化；新职业 |
| Resource | 用 `cn/resource/` 替换与上游不同的内嵌资源 | CN 技能/状态/地图/副本表与翻译 |

`cn/resource/` 只保存与上游不同的资源；其余跟随上游，上游更新时自动继承。

**为什么替换资源，而不是重建 Resource 程序集**：Resource 的代码与上游完全一致，它只是按
名字在运行时加载内嵌资源的容器，重建不会带来任何编译期校验（资源名是字符串），只会把
反编译代码重新拉回维护面。替换 blob 保留上游代码，数据更新退化成文件拷贝；patcher 也支持
新增上游没有的资源名。

### 3.3 为什么修改签过名的程序集是可接受的

强名称签名在 .NET Framework 上默认**不校验**（自 3.5 SP1 起，非 GAC、完全信任）。
Cecil 改写后签名块会失效或缺失，但 CLR 加载不受影响；实测：

* 写入资源后，`Assembly.Load` 正常，全部资源可读；
* 增加字段后，字段读写正常；
* 剥离 + 改写引用后，Config/Logfile 的跨程序集调用正常。

代价是这些程序集不再是“原样”的上游件。`prepare_upstream.py` 每次都从原始包重新生成，
`upstream/lock.json` 的 sha256 保证底座可追溯。

### 3.4 `cn/resource` 的数据从哪来

`cn/resource/` 里的 36 个文件由 `tools/generate_cn_resources.py` 从
[ffxiv-datamining-cn](https://github.com/thewakingsands/ffxiv-datamining-cn) 的 CSV 生成，
不再依赖任何 CN 发行包：

| 内嵌资源 | 来源 sheet | 键 / 值 |
| --- | --- | --- |
| `Generated.ActionList_{Chinese,English}.txt` | `Action` | 十六进制 action id / `Name`（空名行不输出） |
| `Generated.StatusList_{Chinese,English}.txt` | `Status` | 十六进制 status id / `Name` |
| `Generated.MapList_English.txt` | `Map` + `PlaceName` | 十六进制 map id / `PlaceName{Region}`、`PlaceName`、`PlaceName{Sub}` |
| `Generated.TerritoryList_English.txt` | `TerritoryType` + `PlaceName` | 十进制 territory id / `PlaceName` |
| `Generated.ContentFinderList_English.txt` | `ContentFinderCondition` | 十进制 **territory** id / `Name` |
| `Generated.WorldList_English.txt` | `World` + `WorldDCGroupType` | 十进制 world id / `Name`（只留 CN 大区） |
| `Generated.ActionCategoryList_English.txt` | `Action` | 十六进制 action id / `ActionCategory`（十进制数字串） |
| `Definitions.*.json` | `Action` / `Status` | 只替换每个条目的技能名 / 状态名，威力与效果数据保持原值 |

几个反推出来的细节：

* `ContentFinderList` 的键是 `ContentFinderCondition.TerritoryType`，不是副本 id。运行时
  `TerritoryList` 会把这张表垫在 territory 名表下面，于是副本区域显示副本名、其余显示地名；
  因此 PvP（`PvP=True`）与副本外内容（金碟、无人岛、多变迷宫等 `ContentType`）不写入，
  同一 territory 有多条时取 id 最小的一条。
* `ActionCategoryList` 的值上游写的是十进制数字串，而 `ActionList.LoadActionCategories` 用
  `int.Parse(..., NumberStyles.HexNumber)` 解析。生成器刻意照抄这个写法，保证行为不漂移。
* `Definitions.*.json` 的威力 / 连击 / DoT 系数无法从 CSV 得到（`Action` sheet 早已没有威力
  列），这部分继续沿用已有手写数据，生成器只做名字本地化；`LimitTo` 是按名字匹配的，所以
  名字必须和 `ActionList_*` 用同一份 CSV，这一点由生成器保证。
* CSV 与 CN 客户端不一致的极少数条目放在 `cn/resource-overrides.json`，例如 `World` 1168
  （客户端显示“黄金港”，sheet 里是未翻译的“クガネ”且 `Region` 为 0）。

更新流程：

```
python tools/generate_cn_resources.py --csv <ffxiv-datamining-cn>/csv
python tools/prepare_upstream.py          # 把 cn/resource 打进上游 Resource
dotnet build FFXIV_ACT_Plugin.sln -c Release
```

`--check` 会和从 CN 发行包解出的资源逐条比对，输出 `same / drift / new / lost`：正常情况是
`lost` 为 0，`drift`/`new` 全部来自版本更新（新职业、新副本、改名）。`--dry-run` 只报告不写盘。

### 3.5 `Generated.Override.global_*.txt` 为什么保留

`cn/resource/` 里另有 8 个 override 载荷（`global_P5S`–`global_P12S`），它们是唯一无法从
CSV 生成、只能取自 CN 发行包的数据，**有意保留**：

* 用途是替换“加密字符串”：sheet 里部分条目的名字本身是
  `_rsv_<id>_<语言>_<参数>_S<hash>_E<hash>` 令牌，客户端运行时用字符串表解析，而 CSV 里
  只有令牌。`ResourceFile.LoadResourceDictionary` 加载 `ActionList_*` / `StatusList_*` 时，
  会用 override 的**键**去匹配条目名字，命中就把名字换成 override 的值——所以它是按
  **当期版本 / 当期副本**生效的，不是常量表。
* 上游（ravahn）也维护同一套文件（3.0.3.4 共 23 个，含 `7.51` 五语言与全部 P/M/R 副本），
  我们的 8 个是其中 P5S–P12S 的中文本地化对应物。令牌里带语言位（中文 `-1_4_`、英文
  `-1_1_`），上游英文那份匹配不上中文名，所以不能用上游替代。
* 现状：这 3795 个键与当前 CSV 里的 129 个令牌（118 技能 + 11 状态，当期 R12S 那批）
  **一个都不匹配**，因此当前不改变任何显示；但只要客户端再次把对应 id 变成令牌，它们就
  立即生效，成本仅 281 KB。
* 当前 R12S 的 128 条令牌明文只有 NewMoe 的 `global_R12S.txt` 提供（中文明文，CSV 里
  不存在），本项目**不采纳**：那属于每期副本都要跟进的额外维护面。因此这些名字在 ACT 里
  仍显示为 `_rsv_...` 令牌，与上游英文版行为一致。

## 4. 本仓库编译的部分

### main

`src/FFXIV_ACT_Plugin` 是主程序集源码，包含中文界面与默认值、移除 updater、恢复的抓包
设置页，以及反混淆器的创建与注入。`FodyWeavers.xml` 配置 Costura，把全部依赖织进单文件。

### Network

`src/FFXIV_ACT_Plugin.Network` 与上游代码一致，额外提供：

* `INetworkPacketUnscrambler` / `NetworkPacketUnscrambler`：非注入抓包的反混淆；
* `FirewallManager`：原始套接字模式需要的防火墙规则；
* `ScanPackets` 按 `UseWinPCap` / `UseDeucalion` 选择抓包路径。

### 单文件投放

`FFXIV_ACT_Plugin.dll` 把六个上游件、Network、Unscrambler、Machina、Machina.FFXIV、
Newtonsoft.Json、WindowsFirewallHelper 全部织进内部资源；运行时仍需要同目录的
`deucalion-*.dll`（`DeucalionInjector` 按路径加载原生库，且 CN 构建没有下载逻辑）。

## 5. 网络抓包与反混淆

### 5.1 抓包设置

设置页「FFXIV解析插件」分组包含：

| 控件 | 配置字段 | 作用 |
| --- | --- | --- |
| 本机IP过滤 + 刷新网卡列表 | `NetworkIP` | 只监听一个本机地址 |
| 使用Npcap捕获游戏数据 | `UseWinPCap` | 用 Npcap 驱动抓包（需安装驱动） |
| 使用进程注入捕获游戏数据 | `UseDeucalion` | 注入 deucalion 抓包 |
| 禁用高性能网络数据过滤器 | `UseSocketFilter` | 关闭“只抓游戏服务器地址”的内核过滤 |
| 修复防火墙规则 | — | 删除并重建名为 `Advanced Combat Tracker` 的放行规则 |

注入默认不勾选，也就是默认使用非注入抓包。`SettingsMediator` 在字段变化时立即重启抓包，
不需要重开 ACT。

### 5.2 反混淆

游戏对部分服务器包（PlayerSpawn、NpcSpawn、ActionEffect、StatusEffectList、ActorControl、
ActorCast 等）做异或/减法混淆。deucalion 读到的是游戏已经解密的数据；原始套接字和 Npcap
读到的仍是混淆数据，不还原的话伤害、状态等字段都是错的。

`NetworkPacketUnscrambler` 使用 [Latihas/Unscrambler](https://github.com/Latihas/Unscrambler)
（CN fork）和 [perchbirdd/Unscrambler](https://github.com/perchbirdd/Unscrambler)（密钥推导）：

* CN fork 提供 `VersionConstants`、被混淆 opcode 表和 `Data/<版本>/` 密钥推导表；
* 上游提供 `Derivation/`（各版本的 `KeyGenerator72/73/74`）；CN fork 本身没有这部分，因为
  IINACT 是注入式插件，可以直接 hook 拿到 key，而外部抓包必须从初始化包推导；
* 客户端版本取自 `ffxivgame.ver`，密钥在初始化包（`InitZone` 或混淆初始化 opcode）到达时
  生成；7.4 起 `InitZone` 不再能推导密钥，只用混淆初始化包；
* 只在非注入模式下启用；版本在 `VersionConstants` 里找不到时关闭反混淆并在“测试游戏连接”
  里说明原因；
* 用哪个 `KeyGenerator` 由客户端**日期**决定（`NetworkPacketUnscrambler.CreateKeyGenerator`），
  不走上游那个按版本字符串硬编码的 `KeyGeneratorFactory`，否则每个补丁都要改子模块。

Unscrambler 官方程序集只针对 net10.0，因此 `FFXIV_ACT_Plugin.Unscrambler` 把两个子模块的
相关源码编成 net48 程序集，并用 `Compat/` 补齐缺失 API。CN 客户端更新后，给
`third_party/Unscrambler.CN` 补一份 `Constants/Constants<补丁>.cs` + `Data/<客户端版本>/*.bin`
（做法见 §8.3），本仓库的 csproj 用通配符引用，不需要改工程文件。

## 6. `deucalion`（Rust 原生 DLL）

`deucalion-<版本>.dll` 是 Rust 编译的原生 DLL，没有托管元数据。**它不是本插件编译出来的**，
也不需要 Rust 工具链：`tools/prepare_upstream.py` 直接从已 pin 的上游发行包里解出
`deucalion-<版本>.distrib.dll`，改名为 `deucalion-<版本>.dll` 放进
`artifacts/upstream/current/`，构建时由 `StagePluginFolder` 拷到投放目录。这样原生 payload
的字节由 `upstream/lock.json` 的 sha256 锚定，而不是取决于 machina 子模块指针指向哪个提交；
`stamp.json` 的 `native` 字段记录解出的文件哈希。

细节：

* 上游产物名为 `deucalion.dll`；`.distrib` 后缀是 ACT 打包时加的，插件注入前会改名；
* `Machina.FFXIV` 的 `DeucalionInjector` 在运行时从插件目录加载它们，而本构建没有能自动
  下载它们的更新器（CN 版 `ACT_PluginUpdate.UpdateCheckClicked_Version` 是空实现）；
* 上游会轮换 payload 版本（3.0.3.4 只带 1.5.1，3.0.2.x 带 1.2.1 + 1.5.0）。CN fork 的
  `DeucalionInjector` 因此改成“取 `DeucalionPath` 里存在的最新版本”，并按版本核 SHA-256
  （`_payloadChecksums`）；新增 payload 时把文件名和哈希补进那两张表即可。繁体中文区仍固定
  用 1.2.1；
* 同名二进制在上游 release、CN release 与 machina 子模块的 `Deucalion/Distrib/` 里目前
  sha256 完全一致，我们以上游 release 为准。

源码与重建（**可选**，只用于核对二进制）：

* <https://github.com/ff14wed/deucalion>（GPL-3.0），
  `third_party/deucalion/1.5.0`、`1.2.1` 对应上游同名 tag
* `python tools/build_deucalion.py`（需要 nightly Rust、MSVC 与 Windows SDK），脚本会把
  `tokio-util` 钉回发版时的版本；详见 `third_party/deucalion/README.md`

## 7. 验证

```powershell
python tools/prepare_upstream.py
dotnet build FFXIV_ACT_Plugin.sln -c Release

python tools/extract_cn_reference.py path/to/CN.zip   # 仅元数据比对需要
python tools/verify_reconstruction.py
dotnet build tools/PluginLoadTest/PluginLoadTest.csproj -c Release
artifacts/loadtest/Release/PluginLoadTest.exe artifacts/dist/Release/FFXIV_ACT_Plugin.dll

dotnet build tools/UnscramblerSmokeTest/UnscramblerSmokeTest.csproj -c Release
artifacts/smoketest/Release/UnscramblerSmokeTest.exe
```

预期输出：

```
2/2 assemblies verified with an identical declaration surface
PASS - 13 assemblies resolved from the embedded resources, nothing on disk
PASS - CN packet deobfuscation works end to end
```

`verify_reconstruction.py` 只比对仍然从源码编译的两个程序集；其余六个的“正确性”由
`prepare_upstream.py` 的 sha256 + 补丁清单保证。该脚本需要 `ilspycmd`：

```powershell
dotnet tool install -g ilspycmd
```

## 8. 跟进新的客户端补丁

补丁日要动的东西固定是四类，全部是数据，`src/` 的代码只在反混淆的密钥选择上有一部分耦合。
下面按 7.58（CN `2026.11.03.0000.0000`）为例。

### 8.1 四类随客户端变的数据

| 数据 | 位置 | 来源 |
| --- | --- | --- |
| 中文资源（技能/状态/地图/副本/大区名） | `cn/resource/` | `tools/generate_cn_resources.py` 从最新 CSV dump 生成 |
| CN opcode 表 | `third_party/machina/Machina.FFXIV/Headers/Opcodes/Chinese.txt` | <https://github.com/Yarukon/FFXIVNetworkOpcodes> `output/CN_<版本>/machina.txt` |
| Oodle 函数签名 | `third_party/machina/Machina.FFXIV/Memory/SigScan.cs` | 对 `ffxiv_dx11.exe` 重新扫；签名写错会导致非注入抓包拿不到 Oodle 状态 |
| 反混淆常量 + 六张表 | `third_party/Unscrambler.CN` 的 `Constants/Constants<版本>.cs` 与 `Data/<客户端版本>/*.bin` | 反汇编 + 从 exe dump；见 8.3 |

FFXIV 每个补丁都会重新随机 opcode，所以 opcode 表必然要换；资源多数补丁只改几条名字；
Oodle 与反混淆常量则看编译器有没有改写法。

### 8.2 上游二进制

上游版本没有写死在任何工程文件里：`artifacts/upstream/current/` 只是“当前选中的那一版”，
换版本不需要改 csproj。

1. 选与 CN 客户端基线同号的上游 release；
2. 先试跑 `python tools/prepare_upstream.py --version <版本>`。它会跳过 lock 里的哈希校验、
   打印 zip 的 sha256，并直接刷新 `current/`；
3. 把打印出的 sha256 连同 `version` / `asset` 写进 `upstream/lock.json`，之后的构建即可复现；
4. 如果上游改了补丁涉及的成员，调整 `upstream/patch.json`（补丁目标不存在时 patcher 会直接
   报错，不会静默跳过）；
5. 核对上游包里的 `deucalion-*.distrib.dll`：版本变了要给 Machina 的 `DeucalionInjector`
   补文件名和 SHA-256（见 §6）；
6. 对比新的 CN 主程序集与上游 main（`ilspycmd` 反编译即可），把 CN 的 UI/默认值差异合并进
   `src/FFXIV_ACT_Plugin`。

### 8.3 客户端相关的数据怎么来

1. **CSV**：`ffxiv-automatic-data-fetcher` 把新版本的 `ffxiv_dx11.exe`、`ffxivgame.ver` 和
   sqpack 拉到 `history/<客户端版本>/game/`，再由 `ffxiv_csv_loader` 展开成
   `ffxiv-automatic-data/csv/`；`tools/generate_cn_resources.py` 的默认 `--csv` 就指向它。
2. **opcode**：同一套工具会产出 `ffxiv-automatic-data/opcode/<客户端版本>.json`
   （旧 opcode → 新 opcode 的置换表）。Named 表以 Yarukon 的 `machina.txt` 为准；两者应
   一致。旧的具名表可以用这份 diff 推导新一版，再和 Yarukon 对一下。
3. **Oodle 签名**：对 exe 跑一遍 12 条签名（按 `SigScan.cs` 的语义：`**` 通配、匹配末尾
   紧跟 `e8` 的 rel32）。FFXIV 会换寄存器，例如 7.58 把 UDP/TCP train 的判据从 `r13d` 改成
   `r12d`。`SigScan` 支持每类签名的候选列表（`FallbackSignatures`），旧写法保留为 primary，
   新写法作为 fallback，这样同一份代码能同时支持前后两个补丁。
4. **反混淆常量**：`Unscrambler.CN` 里的 `CodeGenerator` / `DataGenerator` 是作者的工具，
   但它的启发式是逐补丁写的，补丁一变就会崩（7.58 就崩在 `imul` 扫描上）。实际做法是直接
   读 `ffxiv_dx11.exe` 的反汇编：

   * 派生函数按 `set` 分成三个分支，每个分支里 `cmp $max` / `imul $radix` 给出该表的
     `TableMax` 与 `TableRadixes`，末尾 `mov 0x<addr>(...,%r8,4)` 给出 `TableOffsets`；
   * 共享尾段给出 `MidTable`（`8 * (seed % N)` 里的 N → `N*8` 字节）与 `DayTable`
     （`% N` → `N*4` 字节）；
   * `OpcodeKeyTable` 在包派发函数里：`% N` 后接 `mov 0x<addr>(...%rcx,4)`；
   * 自检办法：`TableOffset[i] + radix[i]*max[i]*4` 应正好等于下一张表的起始地址，
     `MidTableOffset + MidTableSize` 应等于 `TableOffsets[0]`，`DayTableOffset + DayTableSize`
     应落在 `OpcodeKeyTableOffset` 之前；
   * `ObfuscationEnabledMode` / `UnknownObfuscationInitOpcode` 用包派发函数里的
     `cmp $<值>,%bl` / `cmp $<opcode>,%r15d` 确认；
   * `InitZone` / `FateInfo` 的 opcode 与各包长度取 Yarukon 的 `opcodes.json` 与
     `thewakingsands/matcha`（`NetworkMonitor.cs` 里的 `packet.Length != N`）。
5. **验证数据**：用 `KeyGenerator74` 从合成初始化包派生密钥，再用一份独立的
   “按反汇编重写的派生函数”算一遍，两边密钥必须一致；opcode key 也必须落在表内。

### 8.4 密钥实现的选择

上游 `KeyGeneratorFactory` 的 `switch` 是按客户端版本字符串硬编码的，每来一个补丁都要改
一次子模块，否则 `ForGameVersion` 直接抛异常。`NetworkPacketUnscrambler` 现在按客户端日期
选实现（7.3 起用 opcode key、7.4 起换初始化包布局），只有派生算法真的变时才需要动代码。

### 8.5 收尾

1. `python tools/prepare_upstream.py`（用 lock 里的版本，跳版本后不带 `--version`）；
2. `dotnet build`；
3. 跑 §7 的验证：`verify_reconstruction.py`、`PluginLoadTest`、`UnscramblerSmokeTest`；
4. 到游戏里各抓一次非注入（Npcap/原始套接字，走反混淆）与注入（Deucalion）路径，
   确认伤害、状态、副本名正常。

## 9. 许可与来源

## 9. 许可与来源

* 六个上游程序集来自 [ravahn/FFXIV_ACT_Plugin](https://github.com/ravahn/FFXIV_ACT_Plugin)
  的发行版（该仓库不提供源码，只有发行物、`Definitions/`、`Overrides/`）。
* `src/` 的 main 与 Network 源码来自
  [TundraWork/FFXIV_ACT_Plugin_CN](https://github.com/TundraWork/FFXIV_ACT_Plugin_CN)
  发行包的反编译。
* `cn/resource/` 的技能 / 状态 / 地图 / 区域 / 副本 / 大区名表由
  `tools/generate_cn_resources.py` 从
  [thewakingsands/ffxiv-datamining-cn](https://github.com/thewakingsands/ffxiv-datamining-cn)
  的 CSV 生成（见 §3.4）；`Definitions.*.json` 的威力 / 连击 / DoT 系数与
  `Generated.Override.*` 仍取自 TundraWork 发行包——前者 CSV 里已无对应列，后者按游戏字符串
  id 索引，两者都无法从 CSV 还原。
* `third_party/machina/` 为 [Lotlab/machina](https://github.com/Lotlab/machina) 子模块，
  上游 [ravahn/machina](https://github.com/ravahn/machina)（GPL-3.0）。
* `third_party/Unscrambler.CN/` 为 [Latihas/Unscrambler](https://github.com/Latihas/Unscrambler)
  （WTFPL），`third_party/Unscrambler/` 为
  [perchbirdd/Unscrambler](https://github.com/perchbirdd/Unscrambler)（WTFPL）。
* `third_party/deucalion/` 为 [ff14wed/deucalion](https://github.com/ff14wed/deucalion)
  （GPL-3.0）。
* ACT、Newtonsoft.Json、WindowsFirewallHelper 由 NuGet 还原，遵循各自许可证。
