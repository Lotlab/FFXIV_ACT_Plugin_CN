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

	private readonly Network.FirewallManager _firewallManager;

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

	private Button cmdAddFirewallRule;

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

	public ParseSettings ParseSettings => new ParseSettings
	{
		DisableCombinePets = chkDisableCombinePets.Checked,
		DisableDamageShield = chkDisableDamageShield.Checked,
		LanguageID = (Language)(cboLanguage.SelectedIndex + 1),
		ParseFilter = ((cboParseFilter.SelectedIndex == 1) ? ParseFilterMode.Self : ((cboParseFilter.SelectedIndex == 2) ? ParseFilterMode.Party : ((cboParseFilter.SelectedIndex == 3) ? ParseFilterMode.Alliance : ParseFilterMode.None))),
		SimulateIndividualDoTCrits = chkSimulateDoTCrits.Checked,
		ShowRealDoTTicks = chkShowRealDoTs.Checked,
		ShowDebug = chkShowDebug.Checked,
		EnableBenchmarks = chkEnableBenchmark.Checked
	};

	public SettingsPropertyPage(ISettingsMediator settingsManager, IReadProcesses readProcesses, ProblemDiagnosisHelper problemDiagnosisHelper, ILogOutput logOutput, ILogFormat logFormat, BenchmarkPropertyPage benchmarkPropertyPage, Network.FirewallManager firewallManager)
	{
		InitializeComponent();
		_settingsManager = settingsManager;
		_readProcesses = readProcesses;
		_logOutput = logOutput;
		_logFormat = logFormat;
		_problemDiagnosisHelper = problemDiagnosisHelper;
		_benchmarkPropertyPage = benchmarkPropertyPage;
		_firewallManager = firewallManager;
		_settingsManager.ProcessException = (DateTime timestamp, string message) =>
		{
			AddParserMessage(timestamp, message);
		};
	}

	public SettingsSerializer InitializeSettingsSerializer()
	{
		xmlSettings = new SettingsSerializer(this);
		xmlSettings.AddControlSetting(chkDisableDamageShield.Name, chkDisableDamageShield);
		xmlSettings.AddControlSetting(cboLanguage.Name, cboLanguage);
		xmlSettings.AddControlSetting(cboParseFilter.Name, cboParseFilter);
		xmlSettings.AddControlSetting(chkDisableCombinePets.Name, chkDisableCombinePets);
		xmlSettings.AddControlSetting(chkDisableCombatLog.Name, chkDisableCombatLog);
		xmlSettings.AddControlSetting(txtLogFileDirectory.Name, txtLogFileDirectory);
		xmlSettings.AddControlSetting(chkSimulateDoTCrits.Name, chkSimulateDoTCrits);
		xmlSettings.AddControlSetting(chkShowRealDoTs.Name, chkShowRealDoTs);
		xmlSettings.AddControlSetting(chkShowDebug.Name, chkShowDebug);
		xmlSettings.AddControlSetting(chkEnableBenchmark.Name, chkEnableBenchmark);
		xmlSettings.AddControlSetting(cboRegion.Name, cboRegion);
		xmlSettings.AddControlSetting(cboFilterIP.Name, cboFilterIP);
		xmlSettings.AddControlSetting(chkUsePcap.Name, chkUsePcap);
		xmlSettings.AddControlSetting(chkDisableSocketFilter.Name, chkDisableSocketFilter);
		xmlSettings.AddControlSetting(chkUseDeucalion.Name, chkUseDeucalion);
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
			cboRegion.SelectedIndex = 0;
		}
		if (cboLanguage.SelectedItem == null)
		{
			cboLanguage.SelectedIndex = 0;
		}
		if (cboParseFilter.SelectedItem == null)
		{
			cboParseFilter.SelectedIndex = 0;
		}
		if (cboFilterIP.SelectedItem == null || cboFilterIP.SelectedItem.ToString() == "")
		{
			cboFilterIP.SelectedItem = "不过滤[默认]";
		}
		if (string.IsNullOrWhiteSpace(txtLogFileDirectory.Text))
		{
			txtLogFileDirectory.Text = Path.Combine(ACTWrapper.AppDataFolder.FullName, "FFXIVLogs");
		}
		chkShowDebug_CheckedChanged(null, null);
		chkUseDeucalion_CheckedChanged(null, null);
		chkGraphPotency.Visible = true;
		cboLanguage.SelectedIndex = 4;
		cboRegion.SelectedIndex = 1;
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

	private void cmdAddFirewallRule_Click(object sender, EventArgs e)
	{
		_firewallManager.FixFirewallRules();
		MessageBox.Show("ACT防火墙规则已经删除并重新添加", "防火墙规则已更新", MessageBoxButtons.OK);
	}

	private void cmdOpenExportFolder_Click(object sender, EventArgs e)
	{
		if (!string.IsNullOrWhiteSpace(txtLogFileDirectory.Text))
		{
			Process.Start(txtLogFileDirectory.Text);
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
		cboProcessID.Items.Add("自动识别");
		foreach (int item in _readProcesses.Read64(verifyMainModule: true))
		{
			cboProcessID.Items.Add(item.ToString(CultureInfo.InvariantCulture));
		}
	}

	/// <summary>
	/// Fills the local-address filter with the machine's IPv4 addresses so the
	/// capture can be pinned to a single interface.
	/// </summary>
	private void RefreshNetworkList()
	{
		cboFilterIP.Items.Clear();
		cboFilterIP.Items.Add("不过滤[默认]");
		foreach (string address in InterfaceHelper.GetNetworkInterfaceIPs().ToList())
		{
			cboFilterIP.Items.Add(address);
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

	private void chkUsePcap_CheckedChanged(object sender, EventArgs e)
	{
		OnDataCollectionSettingsChanged(GetDataCollectionSettings());
	}

	private void chkUseDeucalion_CheckedChanged(object sender, EventArgs e)
	{
		// Npcap only applies to the plain capture path.
		chkUsePcap.Enabled = !chkUseDeucalion.Checked;
		OnDataCollectionSettingsChanged(GetDataCollectionSettings());
	}

	private void chkDisableSocketFilter_CheckedChanged(object sender, EventArgs e)
	{
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
		if (chkShowDebug.Checked && m_Initialized && MessageBox.Show("正常使用FFXIV解析插件时不应该开启调试选项。你真的要开启调试选项吗？", "", MessageBoxButtons.YesNo) != DialogResult.Yes)
		{
			chkShowDebug.Checked = false;
			return;
		}
		grpDebug.Visible = chkShowDebug.Checked;
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
		folderBrowserDialog1.SelectedPath = txtLogFileDirectory.Text;
		if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
		{
			txtLogFileDirectory.Text = folderBrowserDialog1.SelectedPath;
			OnDataCollectionSettingsChanged(GetDataCollectionSettings());
		}
	}

	private void cmdResetLogPath_Click(object sender, EventArgs e)
	{
		txtLogFileDirectory.Text = Path.Combine(ACTWrapper.AppDataFolder.FullName, "FFXIVLogs");
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
		OnParseSettingsChanged(ParseSettings);
		if (chkEnableBenchmark.Checked)
		{
			_benchmarkTabPage = new TabPage();
			_benchmarkTabPage.Controls.Add(_benchmarkPropertyPage);
			_benchmarkTabPage.Text = "插件性能测试";
			_benchmarkPropertyPage.Dock = DockStyle.Fill;
			Parent.Parent.Controls.Add(_benchmarkTabPage);
		}
		else
		{
			if (_benchmarkTabPage != null)
			{
				Parent.Parent.Controls.Remove(_benchmarkTabPage);
			}
			_benchmarkTabPage?.Dispose();
			_benchmarkTabPage = null;
		}
	}

	public DataCollectionSettingsEventArgs GetDataCollectionSettings()
	{
		DataCollectionSettingsEventArgs e = new DataCollectionSettingsEventArgs();
		int.TryParse(((string)cboProcessID.SelectedItem) ?? "", out var result);
		e.ProcessID = result;
		e.LogFileFolder = txtLogFileDirectory.Text;
		e.LogAllNetworkData = chkLogAllNetwork.Checked;
		e.DisableCombatLog = chkDisableCombatLog.Checked;
		e.RegionID = (global::FFXIV_ACT_Plugin.Config.Region)(cboRegion.SelectedIndex + 1);
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
				lstMessages.Items.Add($"[{timestamp:HH:mm:ss.fff}] {message}");
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
		if (m_Initialized)
		{
			_settingsManager.ParseSettings = settings;
			string line = _logFormat.FormatParseSettings(settings.DisableDamageShield, settings.DisableCombinePets, settings.LanguageID, settings.ParseFilter, settings.SimulateIndividualDoTCrits, settings.ShowRealDoTTicks);
			_logOutput.WriteLine(LogMessageType.Settings, DateTime.MinValue, line);
		}
	}

	protected virtual void OnDataCollectionSettingsChanged(DataCollectionSettingsEventArgs settings)
	{
		if (m_Initialized)
		{
			_settingsManager.DataCollectionSettings = settings;
			string line = _logFormat.FormatMemorySettings(settings.ProcessID, settings.LogFileFolder, settings.LogAllNetworkData, settings.DisableCombatLog, settings.RegionID);
			_logOutput.WriteLine(LogMessageType.Settings, DateTime.MinValue, line);
		}
	}

	private void chkGraphPotency_CheckedChanged(object sender, EventArgs e)
	{
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
		MessageBox.Show("The ACT FFXIV Plugin is Copyright © 2025 Ravahn.\r\n\r\nThis software makes use of libraries covered under the MIT License: \r\n\r\nCostura/Fody: https://github.com/Fody/Costura\r\n\r\nFody: https://github.com/Fody/Fody\r\n\r\nWindowsFirewallHelper: https://github.com/falahati/WindowsFirewallHelper\r\n\r\nMicrosoft.MinIoc: https://github.com/microsoft/MinIoC", "版权信息", MessageBoxButtons.OK);
	}

	private void InitializeComponent()
	{
		this.lblEventRollover = new System.Windows.Forms.Label();
		this.chkDisableDamageShield = new System.Windows.Forms.CheckBox();
		this.cboLanguage = new System.Windows.Forms.ComboBox();
		this.label2 = new System.Windows.Forms.Label();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.chkDisableCombinePets = new System.Windows.Forms.CheckBox();
		this.label6 = new System.Windows.Forms.Label();
		this.cboParseFilter = new System.Windows.Forms.ComboBox();
		this.chkShowRealDoTs = new System.Windows.Forms.CheckBox();
		this.chkSimulateDoTCrits = new System.Windows.Forms.CheckBox();
		this.chkGraphPotency = new System.Windows.Forms.CheckBox();
		this.lstMessages = new System.Windows.Forms.ListBox();
		this.cmdClearMessages = new System.Windows.Forms.Button();
		this.cmdCopyProblematic = new System.Windows.Forms.Button();
		this.label1 = new System.Windows.Forms.Label();
		this.cmdValidateAddress = new System.Windows.Forms.Button();
		this.cboProcessID = new System.Windows.Forms.ComboBox();
		this.label7 = new System.Windows.Forms.Label();
		this.cmdReloadProcess = new System.Windows.Forms.Button();
		this.cmdAddFirewallRule = new System.Windows.Forms.Button();
		this.cmdOpenExportFolder = new System.Windows.Forms.Button();
		this.txtLogFileDirectory = new System.Windows.Forms.TextBox();
		this.cmdChangeFolder = new System.Windows.Forms.Button();
		this.groupBox3 = new System.Windows.Forms.GroupBox();
		this.cboRegion = new System.Windows.Forms.ComboBox();
		this.label3 = new System.Windows.Forms.Label();
		this.label4 = new System.Windows.Forms.Label();
		this.cboFilterIP = new System.Windows.Forms.ComboBox();
		this.cboRefreshNetwork = new System.Windows.Forms.Button();
		this.chkUsePcap = new System.Windows.Forms.CheckBox();
		this.chkUseDeucalion = new System.Windows.Forms.CheckBox();
		this.chkDisableSocketFilter = new System.Windows.Forms.CheckBox();
		this.chkDisableCombatLog = new System.Windows.Forms.CheckBox();
		this.cmdResetLogPath = new System.Windows.Forms.Button();
		this.chkLogAllNetwork = new System.Windows.Forms.CheckBox();
		this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
		this.chkShowDebug = new System.Windows.Forms.CheckBox();
		this.grpDebug = new System.Windows.Forms.GroupBox();
		this.chkEnableBenchmark = new System.Windows.Forms.CheckBox();
		this.groupBox2 = new System.Windows.Forms.GroupBox();
		this.linkLabel1 = new System.Windows.Forms.LinkLabel();
		this.groupBox1.SuspendLayout();
		this.groupBox3.SuspendLayout();
		this.grpDebug.SuspendLayout();
		this.groupBox2.SuspendLayout();
		base.SuspendLayout();
		this.lblEventRollover.AutoSize = true;
		this.lblEventRollover.Location = new System.Drawing.Point(34, -18);
		this.lblEventRollover.Name = "lblEventRollover";
		this.lblEventRollover.Size = new System.Drawing.Size(0, 13);
		this.lblEventRollover.TabIndex = 3;
		this.chkDisableDamageShield.AutoSize = true;
		this.chkDisableDamageShield.Location = new System.Drawing.Point(13, 82);
		this.chkDisableDamageShield.Name = "chkDisableDamageShield";
		this.chkDisableDamageShield.Size = new System.Drawing.Size(183, 17);
		this.chkDisableDamageShield.TabIndex = 38;
		this.chkDisableDamageShield.Text = "不估算伤害吸收类技能 [正常时请勿勾选]";
		this.chkDisableDamageShield.UseVisualStyleBackColor = true;
		this.chkDisableDamageShield.CheckedChanged += new System.EventHandler(this.chkDisableDamageShield_CheckedChanged);
		this.cboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cboLanguage.FormattingEnabled = true;
		this.cboLanguage.Items.AddRange(new object[7] { "请勿选择此项！", "请勿选择此项！", "请勿选择此项！", "请勿选择此项！", "国服", "请勿选择此项！", "请勿选择此项！" });
		this.cboLanguage.Location = new System.Drawing.Point(113, 27);
		this.cboLanguage.Name = "cboLanguage";
		this.cboLanguage.Size = new System.Drawing.Size(121, 21);
		this.cboLanguage.TabIndex = 41;
		this.cboLanguage.SelectedIndexChanged += new System.EventHandler(this.cboLanguage_SelectedIndexChanged);
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(10, 30);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(58, 13);
		this.label2.TabIndex = 43;
		this.label2.Text = "游戏语言：";
		this.groupBox1.Controls.Add(this.chkDisableCombinePets);
		this.groupBox1.Controls.Add(this.label6);
		this.groupBox1.Controls.Add(this.cboParseFilter);
		this.groupBox1.Controls.Add(this.chkDisableDamageShield);
		this.groupBox1.Controls.Add(this.label2);
		this.groupBox1.Controls.Add(this.cboLanguage);
		this.groupBox1.Location = new System.Drawing.Point(474, 11);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(240, 129);
		this.groupBox1.TabIndex = 44;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "解析选项";
		this.chkDisableCombinePets.AutoSize = true;
		this.chkDisableCombinePets.Location = new System.Drawing.Point(13, 105);
		this.chkDisableCombinePets.Name = "chkDisableCombinePets";
		this.chkDisableCombinePets.Size = new System.Drawing.Size(185, 17);
		this.chkDisableCombinePets.TabIndex = 47;
		this.chkDisableCombinePets.Text = "单独统计召唤物数据";
		this.chkDisableCombinePets.UseVisualStyleBackColor = true;
		this.chkDisableCombinePets.CheckedChanged += new System.EventHandler(this.chkDisableCombinePets_CheckedChanged);
		this.label6.AutoSize = true;
		this.label6.Location = new System.Drawing.Point(10, 58);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(62, 13);
		this.label6.TabIndex = 46;
		this.label6.Text = "游戏解析范围：";
		this.cboParseFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cboParseFilter.FormattingEnabled = true;
		this.cboParseFilter.Items.AddRange(new object[4] { "所有人", "仅自己", "小队成员", "团队成员" });
		this.cboParseFilter.Location = new System.Drawing.Point(113, 55);
		this.cboParseFilter.Name = "cboParseFilter";
		this.cboParseFilter.Size = new System.Drawing.Size(121, 21);
		this.cboParseFilter.TabIndex = 45;
		this.cboParseFilter.SelectedIndexChanged += new System.EventHandler(this.cboParseFilter_SelectedIndexChanged);
		this.chkShowRealDoTs.AutoSize = true;
		this.chkShowRealDoTs.Location = new System.Drawing.Point(274, 19);
		this.chkShowRealDoTs.Name = "chkShowRealDoTs";
		this.chkShowRealDoTs.Size = new System.Drawing.Size(205, 17);
		this.chkShowRealDoTs.TabIndex = 76;
		this.chkShowRealDoTs.Text = "[调试] 显示“实际”持续伤害判定次数";
		this.chkShowRealDoTs.UseVisualStyleBackColor = true;
		this.chkShowRealDoTs.CheckedChanged += new System.EventHandler(this.chkShowRealDoTs_CheckedChanged);
		this.chkSimulateDoTCrits.AutoSize = true;
		this.chkSimulateDoTCrits.Location = new System.Drawing.Point(274, 42);
		this.chkSimulateDoTCrits.Name = "chkSimulateDoTCrits";
		this.chkSimulateDoTCrits.Size = new System.Drawing.Size(208, 17);
		this.chkSimulateDoTCrits.TabIndex = 75;
		this.chkSimulateDoTCrits.Text = "[调试] 独立模拟持续伤害暴击";
		this.chkSimulateDoTCrits.UseVisualStyleBackColor = true;
		this.chkSimulateDoTCrits.CheckedChanged += new System.EventHandler(this.chkSimulateDoTCrits_CheckedChanged);
		this.chkGraphPotency.AutoSize = true;
		this.chkGraphPotency.Location = new System.Drawing.Point(521, 19);
		this.chkGraphPotency.Name = "chkGraphPotency";
		this.chkGraphPotency.Size = new System.Drawing.Size(208, 17);
		this.chkGraphPotency.TabIndex = 74;
		this.chkGraphPotency.Text = "[调试] 统计技能基本威力(而非DPS)";
		this.chkGraphPotency.UseVisualStyleBackColor = true;
		this.chkGraphPotency.Visible = false;
		this.chkGraphPotency.CheckedChanged += new System.EventHandler(this.chkGraphPotency_CheckedChanged);
		this.lstMessages.FormattingEnabled = true;
		this.lstMessages.Location = new System.Drawing.Point(6, 81);
		this.lstMessages.Name = "lstMessages";
		this.lstMessages.ScrollAlwaysVisible = true;
		this.lstMessages.Size = new System.Drawing.Size(925, 199);
		this.lstMessages.TabIndex = 80;
		this.cmdClearMessages.Location = new System.Drawing.Point(348, 289);
		this.cmdClearMessages.Name = "cmdClearMessages";
		this.cmdClearMessages.Size = new System.Drawing.Size(97, 26);
		this.cmdClearMessages.TabIndex = 82;
		this.cmdClearMessages.Text = "清空日志";
		this.cmdClearMessages.UseVisualStyleBackColor = true;
		this.cmdClearMessages.Click += new System.EventHandler(this.cmdClearMessages_Click);
		this.cmdCopyProblematic.Location = new System.Drawing.Point(460, 289);
		this.cmdCopyProblematic.Name = "cmdCopyProblematic";
		this.cmdCopyProblematic.Size = new System.Drawing.Size(109, 26);
		this.cmdCopyProblematic.TabIndex = 81;
		this.cmdCopyProblematic.Text = "复制到剪贴板";
		this.cmdCopyProblematic.UseVisualStyleBackColor = true;
		this.cmdCopyProblematic.Click += new System.EventHandler(this.cmdCopyProblematic_Click);
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(3, 64);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(90, 13);
		this.label1.TabIndex = 83;
		this.label1.Text = "调试信息";
		this.cmdValidateAddress.Location = new System.Drawing.Point(293, 177);
		this.cmdValidateAddress.Name = "cmdValidateAddress";
		this.cmdValidateAddress.Size = new System.Drawing.Size(132, 23);
		this.cmdValidateAddress.TabIndex = 61;
		this.cmdValidateAddress.Text = "测试游戏连接";
		this.cmdValidateAddress.UseVisualStyleBackColor = true;
		this.cmdValidateAddress.Click += new System.EventHandler(this.cmdValidateAddress_Click);
		this.cboProcessID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cboProcessID.FormattingEnabled = true;
		this.cboProcessID.Location = new System.Drawing.Point(141, 27);
		this.cboProcessID.Name = "cboProcessID";
		this.cboProcessID.Size = new System.Drawing.Size(146, 21);
		this.cboProcessID.TabIndex = 62;
		this.cboProcessID.SelectedIndexChanged += new System.EventHandler(this.cboProcessID_SelectedIndexChanged);
		this.label7.AutoSize = true;
		this.label7.Location = new System.Drawing.Point(16, 30);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(94, 13);
		this.label7.TabIndex = 63;
		this.label7.Text = "FFXIV进程ID：";
		this.cmdReloadProcess.Location = new System.Drawing.Point(293, 27);
		this.cmdReloadProcess.Name = "cmdReloadProcess";
		this.cmdReloadProcess.Size = new System.Drawing.Size(132, 23);
		this.cmdReloadProcess.TabIndex = 64;
		this.cmdReloadProcess.Text = "刷新列表";
		this.cmdReloadProcess.UseVisualStyleBackColor = true;
		this.cmdReloadProcess.Click += new System.EventHandler(this.cmdReloadProcess_Click);
		this.cmdAddFirewallRule.Location = new System.Drawing.Point(293, 135);
		this.cmdAddFirewallRule.Name = "cmdAddFirewallRule";
		this.cmdAddFirewallRule.Size = new System.Drawing.Size(132, 23);
		this.cmdAddFirewallRule.TabIndex = 93;
		this.cmdAddFirewallRule.Text = "修复防火墙规则";
		this.cmdAddFirewallRule.UseVisualStyleBackColor = true;
		this.cmdAddFirewallRule.Click += new System.EventHandler(this.cmdAddFirewallRule_Click);
		this.cmdOpenExportFolder.Location = new System.Drawing.Point(18, 53);
		this.cmdOpenExportFolder.Name = "cmdOpenExportFolder";
		this.cmdOpenExportFolder.Size = new System.Drawing.Size(140, 23);
		this.cmdOpenExportFolder.TabIndex = 88;
		this.cmdOpenExportFolder.Text = "打开日志文件夹";
		this.cmdOpenExportFolder.UseVisualStyleBackColor = true;
		this.cmdOpenExportFolder.Click += new System.EventHandler(this.cmdOpenExportFolder_Click);
		this.txtLogFileDirectory.Location = new System.Drawing.Point(20, 24);
		this.txtLogFileDirectory.Multiline = true;
		this.txtLogFileDirectory.Name = "txtLogFileDirectory";
		this.txtLogFileDirectory.ReadOnly = true;
		this.txtLogFileDirectory.Size = new System.Drawing.Size(674, 26);
		this.txtLogFileDirectory.TabIndex = 87;
		this.cmdChangeFolder.Location = new System.Drawing.Point(562, 53);
		this.cmdChangeFolder.Name = "cmdChangeFolder";
		this.cmdChangeFolder.Size = new System.Drawing.Size(132, 23);
		this.cmdChangeFolder.TabIndex = 89;
		this.cmdChangeFolder.Text = "改变位置";
		this.cmdChangeFolder.UseVisualStyleBackColor = true;
		this.cmdChangeFolder.Click += new System.EventHandler(this.cmdChangeFolder_Click);
		this.groupBox3.Controls.Add(this.cboRegion);
		this.groupBox3.Controls.Add(this.label3);
		this.groupBox3.Controls.Add(this.label4);
		this.groupBox3.Controls.Add(this.cboFilterIP);
		this.groupBox3.Controls.Add(this.cboRefreshNetwork);
		this.groupBox3.Controls.Add(this.chkUsePcap);
		this.groupBox3.Controls.Add(this.chkUseDeucalion);
		this.groupBox3.Controls.Add(this.chkDisableSocketFilter);
		this.groupBox3.Controls.Add(this.chkDisableCombatLog);
		this.groupBox3.Controls.Add(this.cmdAddFirewallRule);
		this.groupBox3.Controls.Add(this.cmdReloadProcess);
		this.groupBox3.Controls.Add(this.label7);
		this.groupBox3.Controls.Add(this.cmdValidateAddress);
		this.groupBox3.Controls.Add(this.cboProcessID);
		this.groupBox3.Location = new System.Drawing.Point(14, 11);
		this.groupBox3.Name = "groupBox3";
		this.groupBox3.Size = new System.Drawing.Size(445, 200);
		this.groupBox3.TabIndex = 60;
		this.groupBox3.TabStop = false;
		this.groupBox3.Text = "FFXIV解析插件";
		this.cboRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cboRegion.FormattingEnabled = true;
		this.cboRegion.Items.AddRange(new object[4] { "请勿选择此项！", "国服", "请勿选择此项！", "请勿选择此项！" });
		this.cboRegion.Location = new System.Drawing.Point(141, 55);
		this.cboRegion.Name = "cboRegion";
		this.cboRegion.Size = new System.Drawing.Size(146, 21);
		this.cboRegion.TabIndex = 48;
		this.cboRegion.SelectedIndexChanged += new System.EventHandler(this.cboRegion_SelectedIndexChanged);
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(17, 58);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(107, 13);
		this.label3.TabIndex = 93;
		this.label3.Text = "FFXIV游戏区域";
		this.label4.AutoSize = true;
		this.label4.Location = new System.Drawing.Point(16, 86);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(89, 13);
		this.label4.TabIndex = 95;
		this.label4.Text = "本机IP过滤：";
		this.cboFilterIP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cboFilterIP.FormattingEnabled = true;
		this.cboFilterIP.Location = new System.Drawing.Point(141, 83);
		this.cboFilterIP.Name = "cboFilterIP";
		this.cboFilterIP.Size = new System.Drawing.Size(146, 21);
		this.cboFilterIP.TabIndex = 96;
		this.cboFilterIP.SelectedIndexChanged += new System.EventHandler(this.cboFilterIP_SelectedIndexChanged);
		this.cboRefreshNetwork.Location = new System.Drawing.Point(293, 83);
		this.cboRefreshNetwork.Name = "cboRefreshNetwork";
		this.cboRefreshNetwork.Size = new System.Drawing.Size(132, 23);
		this.cboRefreshNetwork.TabIndex = 97;
		this.cboRefreshNetwork.Text = "刷新网卡列表";
		this.cboRefreshNetwork.UseVisualStyleBackColor = true;
		this.cboRefreshNetwork.Click += new System.EventHandler(this.cboRefreshNetwork_Click);
		this.chkUsePcap.AutoSize = true;
		this.chkUsePcap.Location = new System.Drawing.Point(16, 114);
		this.chkUsePcap.Name = "chkUsePcap";
		this.chkUsePcap.Size = new System.Drawing.Size(257, 17);
		this.chkUsePcap.TabIndex = 98;
		this.chkUsePcap.Text = "使用Npcap捕获游戏数据 [需要安装驱动]";
		this.chkUsePcap.UseVisualStyleBackColor = true;
		this.chkUsePcap.CheckedChanged += new System.EventHandler(this.chkUsePcap_CheckedChanged);
		this.chkUseDeucalion.AutoSize = true;
		this.chkUseDeucalion.Location = new System.Drawing.Point(16, 135);
		this.chkUseDeucalion.Name = "chkUseDeucalion";
		this.chkUseDeucalion.Size = new System.Drawing.Size(224, 17);
		this.chkUseDeucalion.TabIndex = 99;
		this.chkUseDeucalion.Text = "使用进程注入捕获游戏数据 [更稳定]";
		this.chkUseDeucalion.UseVisualStyleBackColor = true;
		this.chkUseDeucalion.CheckedChanged += new System.EventHandler(this.chkUseDeucalion_CheckedChanged);
		this.chkDisableSocketFilter.AutoSize = true;
		this.chkDisableSocketFilter.Location = new System.Drawing.Point(16, 156);
		this.chkDisableSocketFilter.Name = "chkDisableSocketFilter";
		this.chkDisableSocketFilter.Size = new System.Drawing.Size(209, 17);
		this.chkDisableSocketFilter.TabIndex = 100;
		this.chkDisableSocketFilter.Text = "禁用高性能网络数据过滤器 [正常时请勿勾选]";
		this.chkDisableSocketFilter.UseVisualStyleBackColor = true;
		this.chkDisableSocketFilter.CheckedChanged += new System.EventHandler(this.chkDisableSocketFilter_CheckedChanged);
		this.chkDisableCombatLog.AutoSize = true;
		this.chkDisableCombatLog.Location = new System.Drawing.Point(16, 177);
		this.chkDisableCombatLog.Name = "chkDisableCombatLog";
		this.chkDisableCombatLog.Size = new System.Drawing.Size(152, 17);
		this.chkDisableCombatLog.TabIndex = 92;
		this.chkDisableCombatLog.Text = "不解析聊天日志 [仅用于保护隐私]";
		this.chkDisableCombatLog.UseVisualStyleBackColor = true;
		this.cmdResetLogPath.Location = new System.Drawing.Point(424, 53);
		this.cmdResetLogPath.Name = "cmdResetLogPath";
		this.cmdResetLogPath.Size = new System.Drawing.Size(132, 23);
		this.cmdResetLogPath.TabIndex = 90;
		this.cmdResetLogPath.Text = "恢复默认";
		this.cmdResetLogPath.UseVisualStyleBackColor = true;
		this.cmdResetLogPath.Click += new System.EventHandler(this.cmdResetLogPath_Click);
		this.chkLogAllNetwork.AutoSize = true;
		this.chkLogAllNetwork.Location = new System.Drawing.Point(19, 19);
		this.chkLogAllNetwork.Name = "chkLogAllNetwork";
		this.chkLogAllNetwork.Size = new System.Drawing.Size(189, 17);
		this.chkLogAllNetwork.TabIndex = 78;
		this.chkLogAllNetwork.Text = "[调试] 记录所有网络包";
		this.chkLogAllNetwork.UseVisualStyleBackColor = true;
		this.chkLogAllNetwork.CheckedChanged += new System.EventHandler(this.chkLogAllNetwork_CheckedChanged);
		this.chkShowDebug.AutoSize = true;
		this.chkShowDebug.Location = new System.Drawing.Point(34, 305);
		this.chkShowDebug.Name = "chkShowDebug";
		this.chkShowDebug.Size = new System.Drawing.Size(180, 17);
		this.chkShowDebug.TabIndex = 79;
		this.chkShowDebug.Text = "[调试] 开启调试选项";
		this.chkShowDebug.UseVisualStyleBackColor = true;
		this.chkShowDebug.CheckedChanged += new System.EventHandler(this.chkShowDebug_CheckedChanged);
		this.grpDebug.Controls.Add(this.chkEnableBenchmark);
		this.grpDebug.Controls.Add(this.label1);
		this.grpDebug.Controls.Add(this.cmdClearMessages);
		this.grpDebug.Controls.Add(this.cmdCopyProblematic);
		this.grpDebug.Controls.Add(this.lstMessages);
		this.grpDebug.Controls.Add(this.chkShowRealDoTs);
		this.grpDebug.Controls.Add(this.chkSimulateDoTCrits);
		this.grpDebug.Controls.Add(this.chkLogAllNetwork);
		this.grpDebug.Controls.Add(this.chkGraphPotency);
		this.grpDebug.Location = new System.Drawing.Point(14, 329);
		this.grpDebug.Name = "grpDebug";
		this.grpDebug.Size = new System.Drawing.Size(937, 330);
		this.grpDebug.TabIndex = 75;
		this.grpDebug.TabStop = false;
		this.chkEnableBenchmark.AutoSize = true;
		this.chkEnableBenchmark.Location = new System.Drawing.Point(20, 42);
		this.chkEnableBenchmark.Name = "chkEnableBenchmark";
		this.chkEnableBenchmark.Size = new System.Drawing.Size(185, 17);
		this.chkEnableBenchmark.TabIndex = 84;
		this.chkEnableBenchmark.Text = "[调试] 开启性能测试选项卡";
		this.chkEnableBenchmark.UseVisualStyleBackColor = true;
		this.chkEnableBenchmark.CheckedChanged += new System.EventHandler(this.chkEnableBenchmark_CheckedChanged);
		this.groupBox2.Controls.Add(this.cmdResetLogPath);
		this.groupBox2.Controls.Add(this.cmdChangeFolder);
		this.groupBox2.Controls.Add(this.txtLogFileDirectory);
		this.groupBox2.Controls.Add(this.cmdOpenExportFolder);
		this.groupBox2.Location = new System.Drawing.Point(14, 217);
		this.groupBox2.Name = "groupBox2";
		this.groupBox2.Size = new System.Drawing.Size(700, 83);
		this.groupBox2.TabIndex = 87;
		this.groupBox2.TabStop = false;
		this.groupBox2.Text = "日志文件路径";
		this.linkLabel1.AutoSize = true;
		this.linkLabel1.Location = new System.Drawing.Point(532, 306);
		this.linkLabel1.Name = "linkLabel1";
		this.linkLabel1.Size = new System.Drawing.Size(90, 13);
		this.linkLabel1.TabIndex = 88;
		this.linkLabel1.TabStop = true;
		this.linkLabel1.Text = "版权信息";
		this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.linkLabel1);
		base.Controls.Add(this.groupBox2);
		base.Controls.Add(this.grpDebug);
		base.Controls.Add(this.groupBox3);
		base.Controls.Add(this.groupBox1);
		base.Controls.Add(this.lblEventRollover);
		base.Controls.Add(this.chkShowDebug);
		base.Name = "SettingsPropertyPage";
		base.Size = new System.Drawing.Size(969, 672);
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		this.groupBox3.ResumeLayout(false);
		this.groupBox3.PerformLayout();
		this.grpDebug.ResumeLayout(false);
		this.grpDebug.PerformLayout();
		this.groupBox2.ResumeLayout(false);
		this.groupBox2.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
