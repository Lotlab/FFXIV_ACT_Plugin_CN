using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Advanced_Combat_Tracker;
using FFXIV_ACT_Plugin.Common;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory.MemoryReader;
using Machina.Infrastructure;

namespace FFXIV_ACT_Plugin;

public class SettingsPropertyPage : UserControl
{
	private readonly string settingsFile = Path.Combine(ACTWrapper.AppDataFolder.FullName, "Config\\FFXIV_ACT_Plugin.config.xml");

	private SettingsSerializer xmlSettings;

	private bool m_Initialized;

	private TabPage _benchmarkTabPage;

	private readonly ISettingsMediator _settingsManager;

	private readonly IReadProcesses _readProcesses;

	private readonly ILogOutput _logOutput;

	private readonly ILogFormat _logFormat;

	private readonly ProblemDiagnosisHelper _problemDiagnosisHelper;

	private readonly BenchmarkPropertyPage _benchmarkPropertyPage;

	private FormActMain.AttackTypeGraphGenerator m_oldGenerator;

	private IContainer components;

	private Label lblEventRollover;

	private CheckBox chkDisableDamageShield;

	private ComboBox cboLanguage;

	private Label label2;

	private GroupBox groupBox1;

	private Label label6;

	private ComboBox cboParseFilter;

	private CheckBox chkDisableCombinePets;

	private Button cmdValidateAddress;

	private ComboBox cboProcessID;

	private Label label7;

	private Button cmdReloadProcess;

	private GroupBox groupBox3;

	private FolderBrowserDialog folderBrowserDialog1;

	private GroupBox grpDebug;

	private CheckBox chkShowRealDoTs;

	private CheckBox chkSimulateDoTCrits;

	private CheckBox chkShowDebug;

	private CheckBox chkLogAllNetwork;

	private CheckBox chkGraphPotency;

	private GroupBox groupBox2;

	private Button cmdResetLogPath;

	private Button cmdChangeFolder;

	private TextBox txtLogFileDirectory;

	private Button cmdOpenExportFolder;

	private Label label1;

	private Button cmdClearMessages;

	private Button cmdCopyProblematic;

	private ListBox lstMessages;

	private LinkLabel linkLabel1;

	private CheckBox chkDisableCombatLog;

	private CheckBox chkEnableBenchmark;

	private ComboBox cboRegion;

	private Label label3;

	private Label label4;

	private ComboBox cboFilterIP;

	private Button cboRefreshNetwork;

	private CheckBox chkUsePcap;

	private CheckBox chkUseDeucalion;

	private CheckBox chkDisableSocketFilter;

	public ParseSettings ParseSettings
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Expected Obj, but got Unknown
			return new ParseSettings
			{
				DisableCombinePets = chkDisableCombinePets.Checked,
				DisableDamageShield = chkDisableDamageShield.Checked,
				LanguageID = (Language)(((ListControl)cboLanguage).SelectedIndex + 1),
				ParseFilter = (ParseFilterMode)((((ListControl)cboParseFilter).SelectedIndex == 1) ? 1 : ((((ListControl)cboParseFilter).SelectedIndex == 2) ? 2 : ((((ListControl)cboParseFilter).SelectedIndex == 3) ? 3 : 0))),
				SimulateIndividualDoTCrits = chkSimulateDoTCrits.Checked,
				ShowRealDoTTicks = chkShowRealDoTs.Checked,
				ShowDebug = chkShowDebug.Checked,
				EnableBenchmarks = chkEnableBenchmark.Checked
			};
		}
	}

	public SettingsPropertyPage(ISettingsMediator settingsManager, IReadProcesses readProcesses, ProblemDiagnosisHelper problemDiagnosisHelper, ILogOutput logOutput, ILogFormat logFormat, BenchmarkPropertyPage benchmarkPropertyPage)
	{
		InitializeComponent();
		_settingsManager = settingsManager;
		_readProcesses = readProcesses;
		_logOutput = logOutput;
		_logFormat = logFormat;
		_problemDiagnosisHelper = problemDiagnosisHelper;
		_benchmarkPropertyPage = benchmarkPropertyPage;
		_settingsManager.ProcessException = (DateTime timestamp, string message) =>
		{
			AddParserMessage(timestamp, message);
		};
	}

	public SettingsSerializer InitializeSettingsSerializer()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected Obj, but got Unknown
		xmlSettings = new SettingsSerializer((object)this);
		xmlSettings.AddControlSetting(((Control)chkDisableDamageShield).Name, (Control)(object)chkDisableDamageShield);
		xmlSettings.AddControlSetting(((Control)cboLanguage).Name, (Control)(object)cboLanguage);
		xmlSettings.AddControlSetting(((Control)cboParseFilter).Name, (Control)(object)cboParseFilter);
		xmlSettings.AddControlSetting(((Control)chkDisableCombinePets).Name, (Control)(object)chkDisableCombinePets);
		xmlSettings.AddControlSetting(((Control)chkDisableCombatLog).Name, (Control)(object)chkDisableCombatLog);
		xmlSettings.AddControlSetting(((Control)txtLogFileDirectory).Name, (Control)(object)txtLogFileDirectory);
		xmlSettings.AddControlSetting(((Control)chkSimulateDoTCrits).Name, (Control)(object)chkSimulateDoTCrits);
		xmlSettings.AddControlSetting(((Control)chkShowRealDoTs).Name, (Control)(object)chkShowRealDoTs);
		xmlSettings.AddControlSetting(((Control)chkShowDebug).Name, (Control)(object)chkShowDebug);
		xmlSettings.AddControlSetting(((Control)chkEnableBenchmark).Name, (Control)(object)chkEnableBenchmark);
		xmlSettings.AddControlSetting(((Control)cboRegion).Name, (Control)(object)cboRegion);
		xmlSettings.AddControlSetting(((Control)cboFilterIP).Name, (Control)(object)cboFilterIP);
		xmlSettings.AddControlSetting(((Control)chkUsePcap).Name, (Control)(object)chkUsePcap);
		xmlSettings.AddControlSetting(((Control)chkDisableSocketFilter).Name, (Control)(object)chkDisableSocketFilter);
		xmlSettings.AddControlSetting(((Control)chkUseDeucalion).Name, (Control)(object)chkUseDeucalion);
		return xmlSettings;
	}

	public void LoadSettings(SettingsSerializer xmlSettings)
	{
		RefreshProcessList();
		cboProcessID.SelectedItem = "自动识别";
		RefreshNetworkList();
		if (File.Exists(settingsFile))
		{
			using XmlTextReader xmlTextReader = new XmlTextReader(new FileStream(settingsFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
			while (xmlTextReader.Read())
			{
				if (xmlTextReader.NodeType == XmlNodeType.Element && xmlTextReader.LocalName == "SettingsSerializer")
				{
					xmlSettings.ImportFromXml(xmlTextReader);
				}
			}
			xmlTextReader.Close();
		}
		xmlSettings.FinializeComboBoxes();
		if (cboRegion.SelectedItem == null)
		{
			((ListControl)cboRegion).SelectedIndex = 0;
		}
		if (cboLanguage.SelectedItem == null)
		{
			((ListControl)cboLanguage).SelectedIndex = 0;
		}
		if (cboParseFilter.SelectedItem == null)
		{
			((ListControl)cboParseFilter).SelectedIndex = 0;
		}
		if (cboFilterIP.SelectedItem == null || cboFilterIP.SelectedItem.ToString() == "")
		{
			cboFilterIP.SelectedItem = "不过滤[默认]";
		}
		if (string.IsNullOrWhiteSpace(((Control)txtLogFileDirectory).Text))
		{
			((Control)txtLogFileDirectory).Text = Path.Combine(ACTWrapper.AppDataFolder.FullName, "FFXIVLogs");
		}
		chkShowDebug_CheckedChanged(null, null);
		((Control)chkGraphPotency).Visible = true;
		((ListControl)cboLanguage).SelectedIndex = 4;
		((ListControl)cboRegion).SelectedIndex = 1;
		m_Initialized = true;
		OnParseSettingsChanged(ParseSettings);
		OnDataCollectionSettingsChanged(GetDataCollectionSettings());
	}

	public void SaveSettings()
	{
		XmlTextWriter xmlTextWriter = new XmlTextWriter(new FileStream(settingsFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8);
		xmlTextWriter.Formatting = Formatting.Indented;
		xmlTextWriter.Indentation = 1;
		xmlTextWriter.IndentChar = '\t';
		xmlTextWriter.WriteStartDocument(standalone: true);
		xmlTextWriter.WriteStartElement("Config");
		xmlTextWriter.WriteStartElement("SettingsSerializer");
		xmlSettings.ExportToXml(xmlTextWriter);
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteEndDocument();
		xmlTextWriter.Flush();
		xmlTextWriter.Close();
	}

	private void cmdValidateAddress_Click(object sender, EventArgs e)
	{
		_problemDiagnosisHelper.Diagnose(isAutomatedScan: false);
	}

	private void cmdOpenExportFolder_Click(object sender, EventArgs e)
	{
		if (!string.IsNullOrWhiteSpace(((Control)txtLogFileDirectory).Text))
		{
			Process.Start(((Control)txtLogFileDirectory).Text);
		}
	}

	private void cmdReloadProcess_Click(object sender, EventArgs e)
	{
		object selectedItem = cboProcessID.SelectedItem;
		RefreshProcessList();
		if (cboProcessID.Items.Contains(selectedItem))
		{
			cboProcessID.SelectedItem = selectedItem;
		}
		else
		{
			cboProcessID.SelectedItem = "自动识别";
		}
	}

	private void RefreshProcessList()
	{
		cboProcessID.Items.Clear();
		cboProcessID.Items.Add((object)"自动识别");
		foreach (int item in _readProcesses.Read64(true))
		{
			cboProcessID.Items.Add((object)item.ToString(CultureInfo.InvariantCulture));
		}
	}

	/// <summary>
	/// Lists the machine's own addresses so the capture can be pinned to one
	/// network interface.
	/// </summary>
	private void RefreshNetworkList()
	{
		cboFilterIP.Items.Clear();
		cboFilterIP.Items.Add((object)"不过滤[默认]");
		foreach (string item in InterfaceHelper.GetNetworkInterfaceIPs().ToList())
		{
			cboFilterIP.Items.Add((object)item);
		}
	}

	private void cboRefreshNetwork_Click(object sender, EventArgs e)
	{
		object selectedItem = cboFilterIP.SelectedItem;
		RefreshNetworkList();
		if (cboFilterIP.Items.Contains(selectedItem))
		{
			cboFilterIP.SelectedItem = selectedItem;
		}
		else
		{
			cboFilterIP.SelectedItem = "不过滤[默认]";
		}
	}

	private void cboFilterIP_SelectedIndexChanged(object sender, EventArgs e)
	{
		OnDataCollectionSettingsChanged(GetDataCollectionSettings());
	}

	private void chkDisableSocketFilter_CheckedChanged(object sender, EventArgs e)
	{
		OnDataCollectionSettingsChanged(GetDataCollectionSettings());
	}

	private void chkUsePcap_CheckedChanged(object sender, EventArgs e)
	{
		OnDataCollectionSettingsChanged(GetDataCollectionSettings());
	}

	private void chkUseDeucalion_CheckedChanged(object sender, EventArgs e)
	{
		((Control)chkUsePcap).Enabled = !chkUseDeucalion.Checked;
		OnDataCollectionSettingsChanged(GetDataCollectionSettings());
	}

	private void cboProcessID_SelectedIndexChanged(object sender, EventArgs e)
	{
		OnDataCollectionSettingsChanged(GetDataCollectionSettings());
	}

	private void chkLogAllNetwork_CheckedChanged(object sender, EventArgs e)
	{
		OnDataCollectionSettingsChanged(GetDataCollectionSettings());
	}

	private void chkShowDebug_CheckedChanged(object sender, EventArgs e)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		if (chkShowDebug.Checked && m_Initialized && (int)MessageBox.Show("正常使用FFXIV解析插件时不应该开启调试选项。你真的要开启调试选项吗？", "", (MessageBoxButtons)4) != 6)
		{
			chkShowDebug.Checked = false;
			return;
		}
		((Control)grpDebug).Visible = chkShowDebug.Checked;
		if (!chkShowDebug.Checked)
		{
			if (chkShowDebug.Checked)
			{
				chkShowDebug.Checked = false;
			}
			if (chkShowRealDoTs.Checked)
			{
				chkShowRealDoTs.Checked = false;
			}
			if (chkLogAllNetwork.Checked)
			{
				chkLogAllNetwork.Checked = false;
			}
			if (chkSimulateDoTCrits.Checked)
			{
				chkSimulateDoTCrits.Checked = false;
			}
			if (chkGraphPotency.Checked)
			{
				chkGraphPotency.Checked = false;
			}
			if (chkEnableBenchmark.Checked)
			{
				chkEnableBenchmark.Checked = false;
			}
		}
		ACT_UIMods.UpdateACTTables(chkShowDebug.Checked);
		OnParseSettingsChanged(ParseSettings);
	}

	private void chkDisableCombatLog_CheckedChanged(object sender, EventArgs e)
	{
		OnDataCollectionSettingsChanged(GetDataCollectionSettings());
	}

	private void cmdChangeFolder_Click(object sender, EventArgs e)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		folderBrowserDialog1.SelectedPath = ((Control)txtLogFileDirectory).Text;
		if ((int)((CommonDialog)folderBrowserDialog1).ShowDialog() == 1)
		{
			((Control)txtLogFileDirectory).Text = folderBrowserDialog1.SelectedPath;
			OnDataCollectionSettingsChanged(GetDataCollectionSettings());
		}
	}

	private void cmdResetLogPath_Click(object sender, EventArgs e)
	{
		((Control)txtLogFileDirectory).Text = Path.Combine(ACTWrapper.AppDataFolder.FullName, "FFXIVLogs");
		OnDataCollectionSettingsChanged(GetDataCollectionSettings());
	}

	private void cboLanguage_SelectedIndexChanged(object sender, EventArgs e)
	{
		OnParseSettingsChanged(ParseSettings);
	}

	private void cboRegion_SelectedIndexChanged(object sender, EventArgs e)
	{
		OnDataCollectionSettingsChanged(GetDataCollectionSettings());
	}

	private void cboParseFilter_SelectedIndexChanged(object sender, EventArgs e)
	{
		OnParseSettingsChanged(ParseSettings);
	}

	private void chkDisableDamageShield_CheckedChanged(object sender, EventArgs e)
	{
		OnParseSettingsChanged(ParseSettings);
	}

	private void chkDisableCombinePets_CheckedChanged(object sender, EventArgs e)
	{
		OnParseSettingsChanged(ParseSettings);
	}

	private void chkSimulateDoTCrits_CheckedChanged(object sender, EventArgs e)
	{
		OnParseSettingsChanged(ParseSettings);
	}

	private void chkShowRealDoTs_CheckedChanged(object sender, EventArgs e)
	{
		OnParseSettingsChanged(ParseSettings);
	}

	private void chkEnableBenchmark_CheckedChanged(object sender, EventArgs e)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected Obj, but got Unknown
		OnParseSettingsChanged(ParseSettings);
		if (chkEnableBenchmark.Checked)
		{
			_benchmarkTabPage = new TabPage();
			((Control)_benchmarkTabPage).Controls.Add((Control)(object)_benchmarkPropertyPage);
			((Control)_benchmarkTabPage).Text = "插件性能测试";
			((Control)_benchmarkPropertyPage).Dock = (DockStyle)5;
			((Control)this).Parent.Parent.Controls.Add((Control)(object)_benchmarkTabPage);
		}
		else
		{
			if (_benchmarkTabPage != null)
			{
				((Control)this).Parent.Parent.Controls.Remove((Control)(object)_benchmarkTabPage);
			}
			((Component)(object)_benchmarkTabPage)?.Dispose();
			_benchmarkTabPage = null;
		}
	}

	public DataCollectionSettingsEventArgs GetDataCollectionSettings()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected Obj, but got Unknown
		DataCollectionSettingsEventArgs e = new DataCollectionSettingsEventArgs();
		int.TryParse(((string)cboProcessID.SelectedItem) ?? "", out var result);
		e.ProcessID = result;
		e.LogFileFolder = ((Control)txtLogFileDirectory).Text;
		e.LogAllNetworkData = chkLogAllNetwork.Checked;
		e.DisableCombatLog = chkDisableCombatLog.Checked;
		e.RegionID = (Config.Region)(((ListControl)cboRegion).SelectedIndex + 1);
		e.NetworkIP = ((cboFilterIP.SelectedItem == null || cboFilterIP.SelectedItem.ToString() == "不过滤[默认]") ? "" : cboFilterIP.SelectedItem.ToString());
		e.UseWinPCap = chkUsePcap.Checked && !chkUseDeucalion.Checked;
		e.UseSocketFilter = !chkDisableSocketFilter.Checked;
		e.UseDeucalion = chkUseDeucalion.Checked;
		return e;
	}

	public void AddParserMessage(DateTime timestamp, string message)
	{
		ACTWrapper.RunOnACTUIThread(() =>
		{
			if (lstMessages.Items.Count < 100 || chkShowDebug.Checked)
			{
				lstMessages.Items.Add((object)$"[{timestamp:HH:mm:ss.fff}] {message}");
			}
		});
	}

	private void cmdCopyProblematic_Click(object sender, EventArgs e)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (object item in lstMessages.Items)
		{
			stringBuilder.AppendLine((item ?? "").ToString());
		}
		if (stringBuilder.Length > 0)
		{
			Clipboard.SetText(stringBuilder.ToString());
		}
	}

	private void cmdClearMessages_Click(object sender, EventArgs e)
	{
		lstMessages.Items.Clear();
	}

	protected virtual void OnParseSettingsChanged(ParseSettings settings)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (m_Initialized)
		{
			_settingsManager.ParseSettings = settings;
			string text = _logFormat.FormatParseSettings(settings.DisableDamageShield, settings.DisableCombinePets, settings.LanguageID, settings.ParseFilter, settings.SimulateIndividualDoTCrits, settings.ShowRealDoTTicks);
			_logOutput.WriteLine((LogMessageType)249, DateTime.MinValue, text);
		}
	}

	protected virtual void OnDataCollectionSettingsChanged(DataCollectionSettingsEventArgs settings)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (m_Initialized)
		{
			_settingsManager.DataCollectionSettings = settings;
			string text = _logFormat.FormatMemorySettings(settings.ProcessID, settings.LogFileFolder, settings.LogAllNetworkData, settings.DisableCombatLog, settings.RegionID, settings.NetworkIP ?? "", settings.UseWinPCap, settings.UseSocketFilter, settings.UseDeucalion);
			_logOutput.WriteLine((LogMessageType)249, DateTime.MinValue, text);
		}
	}

	private void chkGraphPotency_CheckedChanged(object sender, EventArgs e)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected Obj, but got Unknown
		if (chkGraphPotency.Checked)
		{
			m_oldGenerator = ACTWrapper.GenerateAttackTypeGraph;
			ACTWrapper.GenerateAttackTypeGraph = ACT_UIMods.GenAttackTypeGraph;
		}
		else if (m_oldGenerator != null)
		{
			ACTWrapper.GenerateAttackTypeGraph = m_oldGenerator;
		}
	}

	private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		MessageBox.Show("The ACT FFXIV Plugin is Copyright © 2025 Ravahn.\r\n\r\nThis software makes use of libraries covered under the MIT License: \r\n\r\nCostura/Fody: https://github.com/Fody/Costura\r\n\r\nFody: https://github.com/Fody/Fody\r\n\r\nMicrosoft.MinIoc: https://github.com/microsoft/MinIoC\r\n\r\nUnscrambler (WTFPL): https://github.com/perchbirdd/Unscrambler\r\n\r\nMachina (GPLv3): https://github.com/Lotlab/machina", "版权信息", (MessageBoxButtons)0);
	}

	private void InitializeComponent()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected Obj, but got Unknown
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected Obj, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected Obj, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected Obj, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected Obj, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected Obj, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected Obj, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected Obj, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected Obj, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected Obj, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected Obj, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected Obj, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected Obj, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected Obj, but got Unknown
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected Obj, but got Unknown
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected Obj, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected Obj, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected Obj, but got Unknown
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected Obj, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Expected Obj, but got Unknown
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected Obj, but got Unknown
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Expected Obj, but got Unknown
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected Obj, but got Unknown
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected Obj, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Expected Obj, but got Unknown
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected Obj, but got Unknown
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Expected Obj, but got Unknown
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Expected Obj, but got Unknown
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected Obj, but got Unknown
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Expected Obj, but got Unknown
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Expected Obj, but got Unknown
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Expected Obj, but got Unknown
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Expected Obj, but got Unknown
		//IL_13af: Unknown result type (might be due to invalid IL or missing references)
		//IL_13b9: Expected Obj, but got Unknown
		lblEventRollover = new Label();
		chkDisableDamageShield = new CheckBox();
		cboLanguage = new ComboBox();
		label2 = new Label();
		groupBox1 = new GroupBox();
		chkDisableCombinePets = new CheckBox();
		label6 = new Label();
		cboParseFilter = new ComboBox();
		chkShowRealDoTs = new CheckBox();
		chkSimulateDoTCrits = new CheckBox();
		chkGraphPotency = new CheckBox();
		lstMessages = new ListBox();
		cmdClearMessages = new Button();
		cmdCopyProblematic = new Button();
		label1 = new Label();
		cmdValidateAddress = new Button();
		cboProcessID = new ComboBox();
		label7 = new Label();
		cmdReloadProcess = new Button();
		cmdOpenExportFolder = new Button();
		txtLogFileDirectory = new TextBox();
		cmdChangeFolder = new Button();
		groupBox3 = new GroupBox();
		cboRegion = new ComboBox();
		label3 = new Label();
		label4 = new Label();
		cboFilterIP = new ComboBox();
		cboRefreshNetwork = new Button();
		chkUsePcap = new CheckBox();
		chkUseDeucalion = new CheckBox();
		chkDisableSocketFilter = new CheckBox();
		chkDisableCombatLog = new CheckBox();
		cmdResetLogPath = new Button();
		chkLogAllNetwork = new CheckBox();
		folderBrowserDialog1 = new FolderBrowserDialog();
		chkShowDebug = new CheckBox();
		grpDebug = new GroupBox();
		chkEnableBenchmark = new CheckBox();
		groupBox2 = new GroupBox();
		linkLabel1 = new LinkLabel();
		((Control)groupBox1).SuspendLayout();
		((Control)groupBox3).SuspendLayout();
		((Control)grpDebug).SuspendLayout();
		((Control)groupBox2).SuspendLayout();
		((Control)this).SuspendLayout();
		((Control)lblEventRollover).AutoSize = true;
		((Control)lblEventRollover).Location = new Point(34, -18);
		((Control)lblEventRollover).Name = "lblEventRollover";
		((Control)lblEventRollover).Size = new Size(0, 13);
		((Control)lblEventRollover).TabIndex = 3;
		((Control)chkDisableDamageShield).AutoSize = true;
		((Control)chkDisableDamageShield).Location = new Point(13, 82);
		((Control)chkDisableDamageShield).Name = "chkDisableDamageShield";
		((Control)chkDisableDamageShield).Size = new Size(183, 17);
		((Control)chkDisableDamageShield).TabIndex = 38;
		((Control)chkDisableDamageShield).Text = "不估算伤害吸收类技能 [正常时请勿勾选]";
		((ButtonBase)chkDisableDamageShield).UseVisualStyleBackColor = true;
		chkDisableDamageShield.CheckedChanged += chkDisableDamageShield_CheckedChanged;
		cboLanguage.DropDownStyle = (ComboBoxStyle)2;
		((ListControl)cboLanguage).FormattingEnabled = true;
		cboLanguage.Items.AddRange(new object[7] { "请勿选择此项！", "请勿选择此项！", "请勿选择此项！", "请勿选择此项！", "国服", "请勿选择此项！", "请勿选择此项！" });
		((Control)cboLanguage).Location = new Point(113, 27);
		((Control)cboLanguage).Name = "cboLanguage";
		((Control)cboLanguage).Size = new Size(121, 21);
		((Control)cboLanguage).TabIndex = 41;
		cboLanguage.SelectedIndexChanged += cboLanguage_SelectedIndexChanged;
		((Control)label2).AutoSize = true;
		((Control)label2).Location = new Point(10, 30);
		((Control)label2).Name = "label2";
		((Control)label2).Size = new Size(58, 13);
		((Control)label2).TabIndex = 43;
		((Control)label2).Text = "游戏语言：";
		((Control)groupBox1).Controls.Add((Control)(object)chkDisableCombinePets);
		((Control)groupBox1).Controls.Add((Control)(object)label6);
		((Control)groupBox1).Controls.Add((Control)(object)cboParseFilter);
		((Control)groupBox1).Controls.Add((Control)(object)chkDisableDamageShield);
		((Control)groupBox1).Controls.Add((Control)(object)label2);
		((Control)groupBox1).Controls.Add((Control)(object)cboLanguage);
		((Control)groupBox1).Location = new Point(474, 11);
		((Control)groupBox1).Name = "groupBox1";
		((Control)groupBox1).Size = new Size(240, 129);
		((Control)groupBox1).TabIndex = 44;
		groupBox1.TabStop = false;
		((Control)groupBox1).Text = "解析选项";
		((Control)chkDisableCombinePets).AutoSize = true;
		((Control)chkDisableCombinePets).Location = new Point(13, 105);
		((Control)chkDisableCombinePets).Name = "chkDisableCombinePets";
		((Control)chkDisableCombinePets).Size = new Size(185, 17);
		((Control)chkDisableCombinePets).TabIndex = 47;
		((Control)chkDisableCombinePets).Text = "单独统计召唤物数据";
		((ButtonBase)chkDisableCombinePets).UseVisualStyleBackColor = true;
		chkDisableCombinePets.CheckedChanged += chkDisableCombinePets_CheckedChanged;
		((Control)label6).AutoSize = true;
		((Control)label6).Location = new Point(10, 58);
		((Control)label6).Name = "label6";
		((Control)label6).Size = new Size(62, 13);
		((Control)label6).TabIndex = 46;
		((Control)label6).Text = "游戏解析范围：";
		cboParseFilter.DropDownStyle = (ComboBoxStyle)2;
		((ListControl)cboParseFilter).FormattingEnabled = true;
		cboParseFilter.Items.AddRange(new object[4] { "所有人", "仅自己", "小队成员", "团队成员" });
		((Control)cboParseFilter).Location = new Point(113, 55);
		((Control)cboParseFilter).Name = "cboParseFilter";
		((Control)cboParseFilter).Size = new Size(121, 21);
		((Control)cboParseFilter).TabIndex = 45;
		cboParseFilter.SelectedIndexChanged += cboParseFilter_SelectedIndexChanged;
		((Control)chkShowRealDoTs).AutoSize = true;
		((Control)chkShowRealDoTs).Location = new Point(274, 19);
		((Control)chkShowRealDoTs).Name = "chkShowRealDoTs";
		((Control)chkShowRealDoTs).Size = new Size(205, 17);
		((Control)chkShowRealDoTs).TabIndex = 76;
		((Control)chkShowRealDoTs).Text = "[调试] 显示“实际”持续伤害判定次数";
		((ButtonBase)chkShowRealDoTs).UseVisualStyleBackColor = true;
		chkShowRealDoTs.CheckedChanged += chkShowRealDoTs_CheckedChanged;
		((Control)chkSimulateDoTCrits).AutoSize = true;
		((Control)chkSimulateDoTCrits).Location = new Point(274, 42);
		((Control)chkSimulateDoTCrits).Name = "chkSimulateDoTCrits";
		((Control)chkSimulateDoTCrits).Size = new Size(208, 17);
		((Control)chkSimulateDoTCrits).TabIndex = 75;
		((Control)chkSimulateDoTCrits).Text = "[调试] 独立模拟持续伤害暴击";
		((ButtonBase)chkSimulateDoTCrits).UseVisualStyleBackColor = true;
		chkSimulateDoTCrits.CheckedChanged += chkSimulateDoTCrits_CheckedChanged;
		((Control)chkGraphPotency).AutoSize = true;
		((Control)chkGraphPotency).Location = new Point(521, 19);
		((Control)chkGraphPotency).Name = "chkGraphPotency";
		((Control)chkGraphPotency).Size = new Size(208, 17);
		((Control)chkGraphPotency).TabIndex = 74;
		((Control)chkGraphPotency).Text = "[调试] 统计技能基本威力(而非DPS)";
		((ButtonBase)chkGraphPotency).UseVisualStyleBackColor = true;
		((Control)chkGraphPotency).Visible = false;
		chkGraphPotency.CheckedChanged += chkGraphPotency_CheckedChanged;
		((ListControl)lstMessages).FormattingEnabled = true;
		((Control)lstMessages).Location = new Point(6, 81);
		((Control)lstMessages).Name = "lstMessages";
		lstMessages.ScrollAlwaysVisible = true;
		((Control)lstMessages).Size = new Size(925, 199);
		((Control)lstMessages).TabIndex = 80;
		((Control)cmdClearMessages).Location = new Point(348, 289);
		((Control)cmdClearMessages).Name = "cmdClearMessages";
		((Control)cmdClearMessages).Size = new Size(97, 26);
		((Control)cmdClearMessages).TabIndex = 82;
		((Control)cmdClearMessages).Text = "清空日志";
		((ButtonBase)cmdClearMessages).UseVisualStyleBackColor = true;
		((Control)cmdClearMessages).Click += cmdClearMessages_Click;
		((Control)cmdCopyProblematic).Location = new Point(460, 289);
		((Control)cmdCopyProblematic).Name = "cmdCopyProblematic";
		((Control)cmdCopyProblematic).Size = new Size(109, 26);
		((Control)cmdCopyProblematic).TabIndex = 81;
		((Control)cmdCopyProblematic).Text = "复制到剪贴板";
		((ButtonBase)cmdCopyProblematic).UseVisualStyleBackColor = true;
		((Control)cmdCopyProblematic).Click += cmdCopyProblematic_Click;
		((Control)label1).AutoSize = true;
		((Control)label1).Location = new Point(3, 64);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(90, 13);
		((Control)label1).TabIndex = 83;
		((Control)label1).Text = "调试信息";
		((Control)cmdValidateAddress).Location = new Point(293, 177);
		((Control)cmdValidateAddress).Name = "cmdValidateAddress";
		((Control)cmdValidateAddress).Size = new Size(132, 23);
		((Control)cmdValidateAddress).TabIndex = 61;
		((Control)cmdValidateAddress).Text = "测试游戏连接";
		((ButtonBase)cmdValidateAddress).UseVisualStyleBackColor = true;
		((Control)cmdValidateAddress).Click += cmdValidateAddress_Click;
		cboProcessID.DropDownStyle = (ComboBoxStyle)2;
		((ListControl)cboProcessID).FormattingEnabled = true;
		((Control)cboProcessID).Location = new Point(141, 27);
		((Control)cboProcessID).Name = "cboProcessID";
		((Control)cboProcessID).Size = new Size(146, 21);
		((Control)cboProcessID).TabIndex = 62;
		cboProcessID.SelectedIndexChanged += cboProcessID_SelectedIndexChanged;
		((Control)label7).AutoSize = true;
		((Control)label7).Location = new Point(16, 30);
		((Control)label7).Name = "label7";
		((Control)label7).Size = new Size(94, 13);
		((Control)label7).TabIndex = 63;
		((Control)label7).Text = "FFXIV进程ID：";
		((Control)cmdReloadProcess).Location = new Point(293, 27);
		((Control)cmdReloadProcess).Name = "cmdReloadProcess";
		((Control)cmdReloadProcess).Size = new Size(132, 23);
		((Control)cmdReloadProcess).TabIndex = 64;
		((Control)cmdReloadProcess).Text = "刷新列表";
		((ButtonBase)cmdReloadProcess).UseVisualStyleBackColor = true;
		((Control)cmdReloadProcess).Click += cmdReloadProcess_Click;
		((Control)cmdOpenExportFolder).Location = new Point(18, 53);
		((Control)cmdOpenExportFolder).Name = "cmdOpenExportFolder";
		((Control)cmdOpenExportFolder).Size = new Size(140, 23);
		((Control)cmdOpenExportFolder).TabIndex = 88;
		((Control)cmdOpenExportFolder).Text = "打开日志文件夹";
		((ButtonBase)cmdOpenExportFolder).UseVisualStyleBackColor = true;
		((Control)cmdOpenExportFolder).Click += cmdOpenExportFolder_Click;
		((Control)txtLogFileDirectory).Location = new Point(20, 24);
		((TextBoxBase)txtLogFileDirectory).Multiline = true;
		((Control)txtLogFileDirectory).Name = "txtLogFileDirectory";
		((TextBoxBase)txtLogFileDirectory).ReadOnly = true;
		((Control)txtLogFileDirectory).Size = new Size(674, 26);
		((Control)txtLogFileDirectory).TabIndex = 87;
		((Control)cmdChangeFolder).Location = new Point(562, 53);
		((Control)cmdChangeFolder).Name = "cmdChangeFolder";
		((Control)cmdChangeFolder).Size = new Size(132, 23);
		((Control)cmdChangeFolder).TabIndex = 89;
		((Control)cmdChangeFolder).Text = "改变位置";
		((ButtonBase)cmdChangeFolder).UseVisualStyleBackColor = true;
		((Control)cmdChangeFolder).Click += cmdChangeFolder_Click;
		((Control)groupBox3).Controls.Add((Control)(object)cboRegion);
		((Control)groupBox3).Controls.Add((Control)(object)label3);
		((Control)groupBox3).Controls.Add((Control)(object)chkDisableCombatLog);
		((Control)groupBox3).Controls.Add((Control)(object)cmdReloadProcess);
		((Control)groupBox3).Controls.Add((Control)(object)label7);
		((Control)groupBox3).Controls.Add((Control)(object)cmdValidateAddress);
		((Control)groupBox3).Controls.Add((Control)(object)cboProcessID);
		((Control)groupBox3).Controls.Add((Control)(object)cboFilterIP);
		((Control)groupBox3).Controls.Add((Control)(object)label4);
		((Control)groupBox3).Controls.Add((Control)(object)cboRefreshNetwork);
		((Control)groupBox3).Controls.Add((Control)(object)chkUsePcap);
		((Control)groupBox3).Controls.Add((Control)(object)chkUseDeucalion);
		((Control)groupBox3).Controls.Add((Control)(object)chkDisableSocketFilter);
		((Control)groupBox3).Location = new Point(14, 11);
		((Control)groupBox3).Name = "groupBox3";
		((Control)groupBox3).Size = new Size(445, 200);
		((Control)groupBox3).TabIndex = 60;
		groupBox3.TabStop = false;
		((Control)groupBox3).Text = "FFXIV解析插件";
		cboRegion.DropDownStyle = (ComboBoxStyle)2;
		((ListControl)cboRegion).FormattingEnabled = true;
		cboRegion.Items.AddRange(new object[4] { "请勿选择此项！", "国服", "请勿选择此项！", "请勿选择此项！" });
		((Control)cboRegion).Location = new Point(141, 55);
		((Control)cboRegion).Name = "cboRegion";
		((Control)cboRegion).Size = new Size(146, 21);
		((Control)cboRegion).TabIndex = 48;
		cboRegion.SelectedIndexChanged += cboRegion_SelectedIndexChanged;
		((Control)label3).AutoSize = true;
		((Control)label3).Location = new Point(17, 58);
		((Control)label3).Name = "label3";
		((Control)label3).Size = new Size(107, 13);
		((Control)label3).TabIndex = 93;
		((Control)label3).Text = "FFXIV游戏区域";
		((Control)chkDisableCombatLog).AutoSize = true;
		((Control)chkDisableCombatLog).Location = new Point(16, 177);
		((Control)chkDisableCombatLog).Name = "chkDisableCombatLog";
		((Control)chkDisableCombatLog).Size = new Size(152, 17);
		((Control)chkDisableCombatLog).TabIndex = 92;
		((Control)chkDisableCombatLog).Text = "不解析聊天日志 [仅用于保护隐私]";
		((ButtonBase)chkDisableCombatLog).UseVisualStyleBackColor = true;
		((Control)label4).AutoSize = true;
		((Control)label4).Location = new Point(16, 86);
		((Control)label4).Name = "label4";
		((Control)label4).Size = new Size(89, 13);
		((Control)label4).TabIndex = 95;
		((Control)label4).Text = "本机IP过滤：";
		cboFilterIP.DropDownStyle = (ComboBoxStyle)2;
		((ListControl)cboFilterIP).FormattingEnabled = true;
		((Control)cboFilterIP).Location = new Point(141, 83);
		((Control)cboFilterIP).Name = "cboFilterIP";
		((Control)cboFilterIP).Size = new Size(146, 21);
		((Control)cboFilterIP).TabIndex = 96;
		cboFilterIP.SelectedIndexChanged += cboFilterIP_SelectedIndexChanged;
		((Control)cboRefreshNetwork).Location = new Point(293, 83);
		((Control)cboRefreshNetwork).Name = "cboRefreshNetwork";
		((Control)cboRefreshNetwork).Size = new Size(132, 23);
		((Control)cboRefreshNetwork).TabIndex = 97;
		((Control)cboRefreshNetwork).Text = "刷新网卡列表";
		((ButtonBase)cboRefreshNetwork).UseVisualStyleBackColor = true;
		((Control)cboRefreshNetwork).Click += cboRefreshNetwork_Click;
		((Control)chkUsePcap).AutoSize = true;
		((Control)chkUsePcap).Location = new Point(16, 114);
		((Control)chkUsePcap).Name = "chkUsePcap";
		((Control)chkUsePcap).Size = new Size(257, 17);
		((Control)chkUsePcap).TabIndex = 98;
		((Control)chkUsePcap).Text = "使用Npcap捕获游戏数据 [需要安装驱动]";
		((ButtonBase)chkUsePcap).UseVisualStyleBackColor = true;
		chkUsePcap.CheckedChanged += chkUsePcap_CheckedChanged;
		((Control)chkUseDeucalion).AutoSize = true;
		((Control)chkUseDeucalion).Location = new Point(16, 135);
		((Control)chkUseDeucalion).Name = "chkUseDeucalion";
		((Control)chkUseDeucalion).Size = new Size(224, 17);
		((Control)chkUseDeucalion).TabIndex = 99;
		((Control)chkUseDeucalion).Text = "使用进程注入捕获游戏数据 [更稳定]";
		((ButtonBase)chkUseDeucalion).UseVisualStyleBackColor = true;
		chkUseDeucalion.CheckedChanged += chkUseDeucalion_CheckedChanged;
		((Control)chkDisableSocketFilter).AutoSize = true;
		((Control)chkDisableSocketFilter).Location = new Point(16, 156);
		((Control)chkDisableSocketFilter).Name = "chkDisableSocketFilter";
		((Control)chkDisableSocketFilter).Size = new Size(209, 17);
		((Control)chkDisableSocketFilter).TabIndex = 100;
		((Control)chkDisableSocketFilter).Text = "禁用高性能网络数据过滤器 [正常时请勿勾选]";
		((ButtonBase)chkDisableSocketFilter).UseVisualStyleBackColor = true;
		chkDisableSocketFilter.CheckedChanged += chkDisableSocketFilter_CheckedChanged;
		((Control)cmdResetLogPath).Location = new Point(424, 53);
		((Control)cmdResetLogPath).Name = "cmdResetLogPath";
		((Control)cmdResetLogPath).Size = new Size(132, 23);
		((Control)cmdResetLogPath).TabIndex = 90;
		((Control)cmdResetLogPath).Text = "恢复默认";
		((ButtonBase)cmdResetLogPath).UseVisualStyleBackColor = true;
		((Control)cmdResetLogPath).Click += cmdResetLogPath_Click;
		((Control)chkLogAllNetwork).AutoSize = true;
		((Control)chkLogAllNetwork).Location = new Point(19, 19);
		((Control)chkLogAllNetwork).Name = "chkLogAllNetwork";
		((Control)chkLogAllNetwork).Size = new Size(189, 17);
		((Control)chkLogAllNetwork).TabIndex = 78;
		((Control)chkLogAllNetwork).Text = "[调试] 记录所有网络包";
		((ButtonBase)chkLogAllNetwork).UseVisualStyleBackColor = true;
		chkLogAllNetwork.CheckedChanged += chkLogAllNetwork_CheckedChanged;
		((Control)chkShowDebug).AutoSize = true;
		((Control)chkShowDebug).Location = new Point(34, 305);
		((Control)chkShowDebug).Name = "chkShowDebug";
		((Control)chkShowDebug).Size = new Size(180, 17);
		((Control)chkShowDebug).TabIndex = 79;
		((Control)chkShowDebug).Text = "[调试] 开启调试选项";
		((ButtonBase)chkShowDebug).UseVisualStyleBackColor = true;
		chkShowDebug.CheckedChanged += chkShowDebug_CheckedChanged;
		((Control)grpDebug).Controls.Add((Control)(object)chkEnableBenchmark);
		((Control)grpDebug).Controls.Add((Control)(object)label1);
		((Control)grpDebug).Controls.Add((Control)(object)cmdClearMessages);
		((Control)grpDebug).Controls.Add((Control)(object)cmdCopyProblematic);
		((Control)grpDebug).Controls.Add((Control)(object)lstMessages);
		((Control)grpDebug).Controls.Add((Control)(object)chkShowRealDoTs);
		((Control)grpDebug).Controls.Add((Control)(object)chkSimulateDoTCrits);
		((Control)grpDebug).Controls.Add((Control)(object)chkLogAllNetwork);
		((Control)grpDebug).Controls.Add((Control)(object)chkGraphPotency);
		((Control)grpDebug).Location = new Point(14, 329);
		((Control)grpDebug).Name = "grpDebug";
		((Control)grpDebug).Size = new Size(937, 330);
		((Control)grpDebug).TabIndex = 75;
		grpDebug.TabStop = false;
		((Control)chkEnableBenchmark).AutoSize = true;
		((Control)chkEnableBenchmark).Location = new Point(20, 42);
		((Control)chkEnableBenchmark).Name = "chkEnableBenchmark";
		((Control)chkEnableBenchmark).Size = new Size(185, 17);
		((Control)chkEnableBenchmark).TabIndex = 84;
		((Control)chkEnableBenchmark).Text = "[调试] 开启性能测试选项卡";
		((ButtonBase)chkEnableBenchmark).UseVisualStyleBackColor = true;
		chkEnableBenchmark.CheckedChanged += chkEnableBenchmark_CheckedChanged;
		((Control)groupBox2).Controls.Add((Control)(object)cmdResetLogPath);
		((Control)groupBox2).Controls.Add((Control)(object)cmdChangeFolder);
		((Control)groupBox2).Controls.Add((Control)(object)txtLogFileDirectory);
		((Control)groupBox2).Controls.Add((Control)(object)cmdOpenExportFolder);
		((Control)groupBox2).Location = new Point(14, 217);
		((Control)groupBox2).Name = "groupBox2";
		((Control)groupBox2).Size = new Size(700, 83);
		((Control)groupBox2).TabIndex = 87;
		groupBox2.TabStop = false;
		((Control)groupBox2).Text = "日志文件路径";
		((Control)linkLabel1).AutoSize = true;
		((Control)linkLabel1).Location = new Point(532, 306);
		((Control)linkLabel1).Name = "linkLabel1";
		((Control)linkLabel1).Size = new Size(90, 13);
		((Control)linkLabel1).TabIndex = 88;
		linkLabel1.TabStop = true;
		((Control)linkLabel1).Text = "版权信息";
		linkLabel1.LinkClicked += linkLabel1_LinkClicked;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).Controls.Add((Control)(object)linkLabel1);
		((Control)this).Controls.Add((Control)(object)groupBox2);
		((Control)this).Controls.Add((Control)(object)grpDebug);
		((Control)this).Controls.Add((Control)(object)groupBox3);
		((Control)this).Controls.Add((Control)(object)groupBox1);
		((Control)this).Controls.Add((Control)(object)lblEventRollover);
		((Control)this).Controls.Add((Control)(object)chkShowDebug);
		((Control)this).Name = "SettingsPropertyPage";
		((Control)this).Size = new Size(969, 672);
		((Control)groupBox1).ResumeLayout(false);
		((Control)groupBox1).PerformLayout();
		((Control)groupBox3).ResumeLayout(false);
		((Control)groupBox3).PerformLayout();
		((Control)grpDebug).ResumeLayout(false);
		((Control)grpDebug).PerformLayout();
		((Control)groupBox2).ResumeLayout(false);
		((Control)groupBox2).PerformLayout();
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
