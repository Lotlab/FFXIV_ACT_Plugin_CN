#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Advanced_Combat_Tracker;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Memory;
using FFXIV_ACT_Plugin.Memory.MemoryProcessors;
using FFXIV_ACT_Plugin.Memory.MemoryReader;
using FFXIV_ACT_Plugin.Network;
using Machina.FFXIV.Deucalion;

namespace FFXIV_ACT_Plugin;

public class ProblemDiagnosisHelper
{
	private readonly IProcessManager _processManager;

	private readonly IReadProcesses _readProcesses;

	private readonly ISignatureManager _signatureManager;

	private readonly ISettingsMediator _settingsManager;

	private readonly IScanPackets _scanPackets;

	private readonly IMobArrayProcessor _mobArrayProcessor;

	private readonly IReadAntiVirus _readAntiVirus;

	public ProblemDiagnosisHelper(ISettingsMediator settingsManager, IProcessManager processManager, IReadProcesses readProceses, ISignatureManager signatureManager, IScanPackets scanPackets, IMobArrayProcessor mobArrayProcessor, IReadAntiVirus readAntiVirus)
	{
		_settingsManager = settingsManager;
		_processManager = processManager;
		_readProcesses = readProceses;
		_signatureManager = signatureManager;
		_scanPackets = scanPackets;
		_mobArrayProcessor = mobArrayProcessor;
		_readAntiVirus = readAntiVirus;
	}

	public void Diagnose(bool isAutomatedScan)
	{
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		string text = "";
		int num = -1;
		if (_settingsManager.DataCollectionSettings == null)
		{
			return;
		}
		if (!isAutomatedScan)
		{
			string[] array = _readAntiVirus.Read();
			foreach (string text2 in array)
			{
				Trace.WriteLine("FFXIV解析插件：Windows安全中心中注册了以下反病毒软件：" + text2 + ".", "FFXIV_ACT_Plugin");
			}
		}
		try
		{
			string text3;
			if (!_processManager.Verify())
			{
				List<int> source = _readProcesses.Read64(false);
				Process[] processesByName = Process.GetProcessesByName("ffxiv_dx11.exe");
				if (_settingsManager.DataCollectionSettings == null)
				{
					text = "游戏数据捕获失败：FFXIV解析插件尚未初始化。";
				}
				else if (_settingsManager.DataCollectionSettings.ProcessID != 0)
				{
					text = "游戏数据捕获失败：当前指定的进程无法找到，请在解析插件中点击刷新列表后重新选择，或者改为自动识别。";
				}
				else
				{
					text = ((!processesByName.Any() || source.Any()) ? "游戏数据捕获失败：找不到正在运行的最终幻想14游戏进程。" : "游戏数据捕获失败：找到了游戏进程，但无法进行读取，请检查是否在运行其他游戏的反作弊软件。");
				}
			}
			else if ((text3 = _signatureManager.ValidateWithMessage()) != string.Empty)
			{
				text = text + "游戏数据捕获失败：" + text3 + "。如果游戏近期有更新，这可能需要您更新解析插件。";
			}
			else if ((num = _scanPackets.IsNetworkDataValid()) == 0)
			{
				text = "游戏数据捕获成功：内存标志全部识别，数据正在正常解析中。";
			}
			else if (_mobArrayProcessor.PrimaryPlayerPointer == IntPtr.Zero)
			{
				if (isAutomatedScan)
				{
					return;
				}
				text = "请在游戏内登陆进服务器后再试，并确保ACT以管理员权限运行。";
			}
			else if (num == 1)
			{
				if (isAutomatedScan)
				{
					return;
				}
				text = "游戏数据解析失败：收到了游戏数据，但未正常解析，请先重新登录游戏/重启（而非关机再开机）电脑再试，如仍未解决，请到支持群求助。";
			}
			else
			{
				text = "游戏数据捕获失败：没有接收到任何数据。" + Environment.NewLine + "如果问题持续出现，请重启游戏再试，如果仍然如此，请查看ACT“关于”页面中的“帮助文档”链接。" + Environment.NewLine + "其他游戏的反作弊软件可能干扰了正常解析，请使用管理员模式运行后再试。" + Environment.NewLine + "最后一条注入日志是：" + DeucalionInjector.LastInjectionError;
			}
		}
		catch (Exception ex)
		{
			text = "游戏数据捕获失败：检测是否能获取《最终幻想14》数据时发生了错误：" + ex.Message;
		}
		TraySlider val = new TraySlider
		{
			ButtonLayout = (TraySlider.ButtonLayoutEnum)0
		};
		((Control)val.TrayText).Text = text;
		((Control)val.TrayTitle).Text = "FFXIV解析插件游戏数据解析测试";
		val.ShowTraySlider();
	}
}
