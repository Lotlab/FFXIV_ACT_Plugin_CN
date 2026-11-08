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
  refresh_cn_resources.py            从 CN 发行包重建 cn/resource
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
deucalion-1.2.1.dll         原生 payload
deucalion-1.5.0.dll
```

这个目录就是完整的投放目录。

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
新增上游没有的资源名。将来若改成从游戏数据生成资源，仍然只需把结果放进 `cn/resource/`。

### 3.3 为什么修改签过名的程序集是可接受的

强名称签名在 .NET Framework 上默认**不校验**（自 3.5 SP1 起，非 GAC、完全信任）。
Cecil 改写后签名块会失效或缺失，但 CLR 加载不受影响；实测：

* 写入资源后，`Assembly.Load` 正常，全部资源可读；
* 增加字段后，字段读写正常；
* 剥离 + 改写引用后，Config/Logfile 的跨程序集调用正常。

代价是这些程序集不再是“原样”的上游件。`prepare_upstream.py` 每次都从原始包重新生成，
`upstream/lock.json` 的 sha256 保证底座可追溯。

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
* 上游提供 `Derivation/` 和 `KeyGeneratorFactory`；CN fork 本身没有这部分，因为 IINACT 是
  注入式插件，可以直接 hook 拿到 key，而外部抓包必须从初始化包推导；
* 客户端版本取自 `ffxivgame.ver`，密钥在初始化包（`InitZone` 或混淆初始化 opcode）到达时
  生成；
* 只在非注入模式下启用；版本不在 CN fork 里时关闭反混淆并在“测试游戏连接”里说明原因。

Unscrambler 官方程序集只针对 net10.0，因此 `FFXIV_ACT_Plugin.Unscrambler` 把两个子模块的
相关源码编成 net48 程序集，并用 `Compat/` 补齐缺失 API。CN 客户端更新后，把
`third_party/Unscrambler.CN` 子模块更新到新提交即可。

## 6. `deucalion`（Rust 原生 DLL）

`deucalion-<版本>.distrib.dll` 是 Rust 编译的原生 DLL，没有托管元数据，因此以子模块引入
逐字节对应的上游源码：

* <https://github.com/ff14wed/deucalion>（GPL-3.0）
* `third_party/deucalion/1.5.0`、`1.2.1` 分别对应上游同名 tag
* 上游产物名为 `deucalion.dll`；`.distrib` 后缀是 ACT 打包时加的，插件注入前会改名
* 二进制与上游 release 资源 sha256 一致

它不是本插件的编译输入，保留是为了让“子模块版本 → 发行二进制”可查、可复现。重建：

```powershell
python tools/build_deucalion.py
python tools/build_deucalion.py --lib-only
```

脚本会把 `tokio-util` 钉回发版时的版本，并需要 nightly Rust、MSVC 与 Windows SDK。详见
`third_party/deucalion/README.md`。

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

## 8. 切换上游版本

上游版本没有写死在任何工程文件里：`artifacts/upstream/current/` 只是“当前选中的那一版”，
换版本不需要改 csproj。

1. 选与 CN 客户端基线同号的上游 release；
2. 先试跑 `python tools/prepare_upstream.py --version <版本>`。它会跳过 lock 里的哈希校验、
   打印 zip 的 sha256，并直接刷新 `current/`；
3. 把打印出的 sha256 连同 `version` / `asset` 写进 `upstream/lock.json`，之后的构建即可复现；
4. 用新的 CN 发行包重建资源：
   `python tools/refresh_cn_resources.py path/to/CN.zip`。它对比 CN 与上游原始 Resource，
   重写 `cn/resource/`（新增 / 变更 / 清理），并报告 CN 删除但 patcher 还不能删除的资源；
5. 如果上游改了补丁涉及的成员，调整 `upstream/patch.json`（补丁目标不存在时 patcher 会直接
   报错，不会静默跳过）；
6. 对比新的 CN 主程序集与上游 main（`ilspycmd` 反编译即可），把 CN 的 UI/默认值差异合并进
   `src/FFXIV_ACT_Plugin`；
7. `dotnet build`，跑 §7 的验证。

## 9. 许可与来源

* 六个上游程序集来自 [ravahn/FFXIV_ACT_Plugin](https://github.com/ravahn/FFXIV_ACT_Plugin)
  的发行版（该仓库不提供源码，只有发行物、`Definitions/`、`Overrides/`）。
* `src/` 的 main 与 Network 源码、以及 `cn/resource/` 的 CN 数据，来自
  [TundraWork/FFXIV_ACT_Plugin_CN](https://github.com/TundraWork/FFXIV_ACT_Plugin_CN)
  发行包的反编译与提取。
* `third_party/machina/` 为 [Lotlab/machina](https://github.com/Lotlab/machina) 子模块，
  上游 [ravahn/machina](https://github.com/ravahn/machina)（GPL-3.0）。
* `third_party/Unscrambler.CN/` 为 [Latihas/Unscrambler](https://github.com/Latihas/Unscrambler)
  （WTFPL），`third_party/Unscrambler/` 为
  [perchbirdd/Unscrambler](https://github.com/perchbirdd/Unscrambler)（WTFPL）。
* `third_party/deucalion/` 为 [ff14wed/deucalion](https://github.com/ff14wed/deucalion)
  （GPL-3.0）。
* ACT、Newtonsoft.Json、WindowsFirewallHelper 由 NuGet 还原，遵循各自许可证。
