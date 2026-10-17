using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Advanced_Combat_Tracker;
using FFXIV_ACT_Plugin.Common;
using FFXIV_ACT_Plugin.Parse;

namespace FFXIV_ACT_Plugin;

public class ACT_UIMods
{
	private Button cmdClear;

	private TabPage _pluginScreenSpace;

	private SettingsPropertyPage _settingsPropertyPage;

	private readonly ParseMediator _parseMediator;

	private readonly IActWrapper _actWrapper;

	private readonly IBenchmarkRepository _benchmarkRepository;

	private FormActMain.DateTimeLogParser _oldDateTimeDelegate;

	public ACT_UIMods(ParseMediator parseMediator, SettingsPropertyPage settingsPropertyPage, IActWrapper actWrapper, IBenchmarkRepository benchmarkRepository)
	{
		_actWrapper = actWrapper;
		_parseMediator = parseMediator;
		_settingsPropertyPage = settingsPropertyPage;
		_benchmarkRepository = benchmarkRepository;
	}

	public void ConfigureUI(TabPage pluginScreenSpace)
	{
		_pluginScreenSpace = pluginScreenSpace;
		cmdClear = ActGlobals.oFormActMain.FindControl<Button>("tc1\\tpMain\\pLeftView\\pTvBtns\\tableLayoutPanel11\\btnClear");
		if (cmdClear != null)
		{
			cmdClear.Click += _parseMediator.OnClear;
		}
		_actWrapper.TimeStampLen = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture).Length + 3;
		_actWrapper.LogPathHasCharName = false;
		_actWrapper.ZoneChangeRegex = new Regex("^01[|][^|]*[|][^|]*[|](?<zone>.+)[|].*$", RegexOptions.Compiled);
		_oldDateTimeDelegate = ActGlobals.oFormActMain.GetDateTimeFromLog;
		ActGlobals.oFormActMain.GetDateTimeFromLog = _parseMediator.ParseLogDateTime;
		ActGlobals.oFormActMain.BeforeLogLineRead += OFormActMain_BeforeLogLineRead;
		ActGlobals.oFormActMain.LogFileChanged += OFormActMain_LogFileChanged;
		LoadPluginSettingsControl();
	}

	private void OFormActMain_LogFileChanged(bool IsImport, string NewLogFileName)
	{
		_benchmarkRepository.Measure("ParseLogFileChanged", () =>
		{
			_parseMediator.LogFileChanged(IsImport);
		});
	}

	private void OFormActMain_BeforeLogLineRead(bool isImport, LogLineEventArgs logInfo)
	{
		(logInfo.logLine, logInfo.detectedType) = _benchmarkRepository.Measure("ParseBeforeLogLineRead", () => _parseMediator.BeforeLogLineRead(isImport, logInfo.detectedTime, logInfo.logLine));
	}

	private void LoadPluginSettingsControl()
	{
		_settingsPropertyPage.Dock = DockStyle.Fill;
		_pluginScreenSpace.Controls.Add(_settingsPropertyPage);
		_pluginScreenSpace.Text = "FFXIV解析插件";
	}

	public void LoadACTSettings()
	{
		SettingsSerializer xmlSettings = _settingsPropertyPage.InitializeSettingsSerializer();
		_settingsPropertyPage.LoadSettings(xmlSettings);
	}

	public void UnloadControls()
	{
		ActGlobals.oFormActMain.BeforeLogLineRead -= OFormActMain_BeforeLogLineRead;
		ActGlobals.oFormActMain.GetDateTimeFromLog = _oldDateTimeDelegate;
		_settingsPropertyPage?.SaveSettings();
		_settingsPropertyPage = null;
		if (cmdClear != null)
		{
			cmdClear.Click -= _parseMediator.OnClear;
			cmdClear = null;
		}
	}

	public static void UpdateACTTables(bool showDebug)
	{
		CombatantData.IncomingDamageTypeDataObjects = new Dictionary<string, CombatantData.DamageTypeDef>
		{
			{
				"模拟的持续伤害 (承受)",
				new CombatantData.DamageTypeDef("模拟的持续伤害 (承受)", -1, Color.Red)
			},
			{
				"承受伤害",
				new CombatantData.DamageTypeDef("承受伤害", -1, Color.Red)
			},
			{
				"护盾 (承受)",
				new CombatantData.DamageTypeDef("护盾 (承受)", 1, Color.YellowGreen)
			},
			{
				"模拟的持续治疗 (承受)",
				new CombatantData.DamageTypeDef("模拟的持续治疗 (承受)", 1, Color.GreenYellow)
			},
			{
				"治疗 (承受)",
				new CombatantData.DamageTypeDef("治疗 (承受)", 1, Color.LimeGreen)
			},
			{
				"其他 (承受)",
				new CombatantData.DamageTypeDef("其他 (承受)", 0, Color.Lime)
			},
			{
				"状态 (承受)",
				new CombatantData.DamageTypeDef("状态 (承受)", 0, Color.Wheat)
			},
			{
				"MP吸收 (承受)",
				new CombatantData.DamageTypeDef("MP吸收 (承受)", -1, Color.Magenta)
			},
			{
				"MP回复 (承受)",
				new CombatantData.DamageTypeDef("MP回复 (承受)", 1, Color.MediumPurple)
			},
			{
				"康复/驱散 (承受)",
				new CombatantData.DamageTypeDef("康复/驱散 (承受)", 0, Color.Wheat)
			},
			{
				"仇恨 (承受)",
				new CombatantData.DamageTypeDef("仇恨 (承受)", -1, Color.Yellow)
			},
			{
				"总承伤 (自己)",
				new CombatantData.DamageTypeDef("总承伤 (自己)", 0, Color.Black)
			}
		};
		CombatantData.OutgoingDamageTypeDataObjects = new Dictionary<string, CombatantData.DamageTypeDef>
		{
			{
				"自动攻击 (输出)",
				new CombatantData.DamageTypeDef("自动攻击 (输出)", -1, Color.DarkGoldenrod)
			},
			{
				"技能/能力 (输出)",
				new CombatantData.DamageTypeDef("技能/能力 (输出)", -1, Color.DarkOrange)
			},
			{
				"模拟的持续伤害 (输出)",
				new CombatantData.DamageTypeDef("模拟的持续伤害 (输出)", -1, Color.OrangeRed)
			},
			{
				"输出伤害",
				new CombatantData.DamageTypeDef("输出伤害", 0, Color.Orange)
			},
			{
				"护盾 (输出)",
				new CombatantData.DamageTypeDef("护盾 (输出)", 1, Color.LightSkyBlue)
			},
			{
				"模拟的持续治疗 (输出)",
				new CombatantData.DamageTypeDef("模拟的持续治疗 (输出)", 1, Color.LightBlue)
			},
			{
				"治疗 (输出)",
				new CombatantData.DamageTypeDef("治疗 (输出)", 1, Color.Blue)
			},
			{
				"其他 (输出)",
				new CombatantData.DamageTypeDef("其他 (输出)", 0, Color.Lime)
			},
			{
				"状态 (输出)",
				new CombatantData.DamageTypeDef("状态 (输出)", 0, Color.Wheat)
			},
			{
				"MP吸收 (输出)",
				new CombatantData.DamageTypeDef("MP吸收 (输出)", -1, Color.Purple)
			},
			{
				"MP回复 (输出)",
				new CombatantData.DamageTypeDef("MP回复 (输出)", 1, Color.Violet)
			},
			{
				"康复/驱散 (输出)",
				new CombatantData.DamageTypeDef("康复/驱散 (输出)", 0, Color.Wheat)
			},
			{
				"仇恨 (输出)",
				new CombatantData.DamageTypeDef("仇恨 (输出)", -1, Color.Yellow)
			},
			{
				"所有技能 (自己)",
				new CombatantData.DamageTypeDef("所有技能 (自己)", 0, Color.Black)
			}
		};
		CombatantData.SwingTypeToDamageTypeDataLinksOutgoing = new SortedDictionary<int, List<string>>
		{
			{
				0,
				new List<string> { "自动攻击 (输出)", "输出伤害" }
			},
			{
				1,
				new List<string> { "其他 (输出)" }
			},
			{
				2,
				new List<string> { "技能/能力 (输出)", "输出伤害" }
			},
			{
				3,
				new List<string> { "模拟的持续伤害 (输出)", "输出伤害" }
			},
			{
				4,
				new List<string> { "治疗 (输出)" }
			},
			{
				5,
				new List<string> { "模拟的持续治疗 (输出)", "治疗 (输出)" }
			},
			{
				6,
				new List<string> { "MP吸收 (输出)" }
			},
			{
				7,
				new List<string> { "MP回复 (输出)" }
			},
			{
				8,
				new List<string> { "状态 (输出)" }
			},
			{
				9,
				new List<string> { "康复/驱散 (输出)" }
			},
			{
				10,
				new List<string> { "仇恨 (输出)" }
			},
			{
				11,
				new List<string> { "护盾 (输出)", "治疗 (输出)" }
			}
		};
		CombatantData.SwingTypeToDamageTypeDataLinksIncoming = new SortedDictionary<int, List<string>>
		{
			{
				0,
				new List<string> { "承受伤害" }
			},
			{
				1,
				new List<string> { "其他 (承受)" }
			},
			{
				2,
				new List<string> { "承受伤害" }
			},
			{
				3,
				new List<string> { "模拟的持续伤害 (承受)", "承受伤害" }
			},
			{
				4,
				new List<string> { "治疗 (承受)" }
			},
			{
				5,
				new List<string> { "模拟的持续治疗 (承受)", "治疗 (承受)" }
			},
			{
				6,
				new List<string> { "MP吸收 (承受)" }
			},
			{
				7,
				new List<string> { "MP回复 (承受)" }
			},
			{
				8,
				new List<string> { "状态 (承受)" }
			},
			{
				9,
				new List<string> { "康复/驱散 (承受)" }
			},
			{
				10,
				new List<string> { "仇恨 (承受)" }
			},
			{
				11,
				new List<string> { "护盾 (承受)", "治疗 (承受)" }
			}
		};
		CombatantData.DamageSwingTypes = new List<int> { 0, 2, 3 };
		CombatantData.HealingSwingTypes = new List<int> { 4, 5, 8, 9, 1 };
		if (!EncounterData.ExportVariables.ContainsKey("CurrentZoneName"))
		{
			EncounterData.ExportVariables.Add("CurrentZoneName", new EncounterData.TextExportFormatter("CurrentZoneName", "CurrentZoneName", "Current Zone Name", (EncounterData data, List<CombatantData> SelectiveAllies, string extra) => data.ZoneName));
		}
		if (!CombatantData.ExportVariables.ContainsKey("Last10DPS"))
		{
			CombatantData.ExportVariables.Add("Last10DPS", new CombatantData.TextExportFormatter("Last10DPS", "Last 10 Seconds DPS", "Average DPS for last 10 seconds.", (CombatantData Data, string ExtraFormat) => Data.LastNDPS(10).ToString("0", CultureInfo.InvariantCulture)));
		}
		if (!CombatantData.ExportVariables.ContainsKey("Last30DPS"))
		{
			CombatantData.ExportVariables.Add("Last30DPS", new CombatantData.TextExportFormatter("Last30DPS", "Last 30 Seconds DPS", "Average DPS for last 30 seconds.", (CombatantData Data, string ExtraFormat) => Data.LastNDPS(30).ToString("0", CultureInfo.InvariantCulture)));
		}
		if (!CombatantData.ExportVariables.ContainsKey("Last60DPS"))
		{
			CombatantData.ExportVariables.Add("Last60DPS", new CombatantData.TextExportFormatter("Last60DPS", "Last 60 Seconds DPS", "Average DPS for last 60 seconds.", (CombatantData Data, string ExtraFormat) => Data.LastNDPS(60).ToString("0", CultureInfo.InvariantCulture)));
		}
		if (!EncounterData.ExportVariables.ContainsKey("Last10DPS"))
		{
			EncounterData.ExportVariables.Add("Last10DPS", new EncounterData.TextExportFormatter("Last10DPS", "Last 10 Seconds DPS", "Average DPS for last 10 seconds", (EncounterData Data, List<CombatantData> SelectiveAllies, string Extra) => Data.LastNDPS(SelectiveAllies, 10).ToString("0", CultureInfo.InvariantCulture)));
		}
		if (!EncounterData.ExportVariables.ContainsKey("Last30DPS"))
		{
			EncounterData.ExportVariables.Add("Last30DPS", new EncounterData.TextExportFormatter("Last30DPS", "Last 30 Seconds DPS", "Average DPS for last 30 seconds", (EncounterData Data, List<CombatantData> SelectiveAllies, string Extra) => Data.LastNDPS(SelectiveAllies, 30).ToString("0", CultureInfo.InvariantCulture)));
		}
		if (!EncounterData.ExportVariables.ContainsKey("Last60DPS"))
		{
			EncounterData.ExportVariables.Add("Last60DPS", new EncounterData.TextExportFormatter("Last60DPS", "Last 60 Seconds DPS", "Average DPS for last 60 seconds", (EncounterData Data, List<CombatantData> SelectiveAllies, string Extra) => Data.LastNDPS(SelectiveAllies, 60).ToString("0", CultureInfo.InvariantCulture)));
		}
		if (!MasterSwing.ColumnDefs.ContainsKey("StatusDuration"))
		{
			MasterSwing.ColumnDefs.Add("StatusDuration", new MasterSwing.ColumnDef("StatusDuration", DefaultVisible: true, "VARCHAR(8)", "StatusDuration", (MasterSwing Data) => (!Data.Tags.ContainsKey("StatusDuration")) ? "" : ((double)Data.Tags["StatusDuration"]).ToString("0.#", CultureInfo.InvariantCulture), (MasterSwing Data) => (!Data.Tags.ContainsKey("StatusDuration")) ? "" : ((double)Data.Tags["StatusDuration"]).ToString("0.#", CultureInfo.InvariantCulture), (MasterSwing Left, MasterSwing Right) => (Left.Tags.ContainsKey("StatusDuration") ? ((double)Left.Tags["StatusDuration"]) : 0.0).CompareTo(Right.Tags.ContainsKey("StatusDuration") ? ((double)Right.Tags["StatusDuration"]) : 0.0)));
		}
		if (!CombatantData.ColumnDefs.ContainsKey("Job"))
		{
			CombatantData.ColumnDefs.Add("Job", new CombatantData.ColumnDef("Job", DefaultVisible: true, "VARCHAR(8)", "Job", (CombatantData Data) => Data.Job(), (CombatantData Data) => Data.Job(), (CombatantData Left, CombatantData Right) => string.Compare(Left.Job(), Right.Job(), StringComparison.OrdinalIgnoreCase)));
		}
		if (!CombatantData.ExportVariables.ContainsKey("Job"))
		{
			CombatantData.ExportVariables.Add("Job", new CombatantData.TextExportFormatter("Job", "Job Name", "Player's Job", (CombatantData Data, string ExtraFormat) => Data.GetColumnByName("Job")));
		}
		if (!CombatantData.ColumnDefs.ContainsKey("ParryPct"))
		{
			CombatantData.ColumnDefs.Add("ParryPct", new CombatantData.ColumnDef("ParryPct", DefaultVisible: false, "VARCHAR(8)", "ParryPct", (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ParryPct"), (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ParryPct"), (CombatantData Left, CombatantData Right) => string.Compare(Left.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ParryPct"), Right.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ParryPct"), StringComparison.OrdinalIgnoreCase)));
		}
		if (!CombatantData.ExportVariables.ContainsKey("ParryPct"))
		{
			CombatantData.ExportVariables.Add("ParryPct", new CombatantData.TextExportFormatter("ParryPct", "Parry Percent", "Percent of hits that were parried.", (CombatantData Data, string ExtraFormat) => Data.GetColumnByName("ParryPct")));
		}
		if (!CombatantData.ColumnDefs.ContainsKey("BlockPct"))
		{
			CombatantData.ColumnDefs.Add("BlockPct", new CombatantData.ColumnDef("BlockPct", DefaultVisible: false, "VARCHAR(8)", "BlockPct", (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("BlockPct"), (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("BlockPct"), (CombatantData Left, CombatantData Right) => string.Compare(Left.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("BlockPct"), Right.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("BlockPct"), StringComparison.OrdinalIgnoreCase)));
		}
		if (!CombatantData.ExportVariables.ContainsKey("BlockPct"))
		{
			CombatantData.ExportVariables.Add("BlockPct", new CombatantData.TextExportFormatter("BlockPct", "Block Percent", "Percent of hits that were blocked.", (CombatantData Data, string ExtraFormat) => Data.GetColumnByName("BlockPct")));
		}
		if (!CombatantData.ColumnDefs.ContainsKey("IncToHit"))
		{
			CombatantData.ColumnDefs.Add("IncToHit", new CombatantData.ColumnDef("IncToHit", DefaultVisible: false, "VARCHAR(8)", "IncToHit", (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ToHit"), (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ToHit"), (CombatantData Left, CombatantData Right) => string.Compare(Left.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ToHit"), Right.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ToHit"), StringComparison.OrdinalIgnoreCase)));
		}
		if (!CombatantData.ExportVariables.ContainsKey("IncToHit"))
		{
			CombatantData.ExportVariables.Add("IncToHit", new CombatantData.TextExportFormatter("IncToHit", "Incoming Hit Rate", "Incoming hits to the target.", (CombatantData Data, string ExtraFormat) => Data.GetColumnByName("IncToHit")));
		}
		if (!DamageTypeData.ColumnDefs.ContainsKey("ParryPct"))
		{
			DamageTypeData.ColumnDefs.Add("ParryPct", new DamageTypeData.ColumnDef("ParryPct", DefaultVisible: false, "VARCHAR(8)", "ParryPct", (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? 0.ToString("0'%", CultureInfo.InvariantCulture) : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("ParryPct"), (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? 0.ToString("0'%", CultureInfo.InvariantCulture) : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("ParryPct")));
		}
		if (!DamageTypeData.ColumnDefs.ContainsKey("BlockPct"))
		{
			DamageTypeData.ColumnDefs.Add("BlockPct", new DamageTypeData.ColumnDef("BlockPct", DefaultVisible: false, "VARCHAR(8)", "BlockPct", (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? 0.ToString("0'%", CultureInfo.InvariantCulture) : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("BlockPct"), (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? 0.ToString("0'%", CultureInfo.InvariantCulture) : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("BlockPct")));
		}
		if (!AttackType.ColumnDefs.ContainsKey("Parry"))
		{
			AttackType.ColumnDefs.Add("Parry", new AttackType.ColumnDef("Parry", DefaultVisible: false, "INT", "Parry", (AttackType Data) => Data.Parry().ToString(CultureInfo.InvariantCulture), (AttackType Data) => Data.Parry().ToString(CultureInfo.InvariantCulture), (AttackType Left, AttackType Right) => Left.Parry().CompareTo(Right.Parry())));
		}
		if (!AttackType.ColumnDefs.ContainsKey("ParryPct"))
		{
			AttackType.ColumnDefs.Add("ParryPct", new AttackType.ColumnDef("ParryPct", DefaultVisible: false, "VARCHAR(8)", "ParryPct", (AttackType Data) => ((double)Data.Parry() * 100.0 / (double)OneOrInt(Data.BlockParryCount())).ToString("0'%", CultureInfo.InvariantCulture), (AttackType Data) => ((double)Data.Parry() * 100.0 / (double)OneOrInt(Data.BlockParryCount())).ToString("0'%", CultureInfo.InvariantCulture), (AttackType Left, AttackType Right) => Left.Parry().CompareTo(Right.Parry())));
		}
		if (!AttackType.ColumnDefs.ContainsKey("Block"))
		{
			AttackType.ColumnDefs.Add("Block", new AttackType.ColumnDef("Block", DefaultVisible: false, "INT", "Block", (AttackType Data) => Data.Block().ToString(CultureInfo.InvariantCulture), (AttackType Data) => Data.Block().ToString(CultureInfo.InvariantCulture), (AttackType Left, AttackType Right) => Left.Block().CompareTo(Right.Block())));
		}
		if (!AttackType.ColumnDefs.ContainsKey("BlockPct"))
		{
			AttackType.ColumnDefs.Add("BlockPct", new AttackType.ColumnDef("BlockPct", DefaultVisible: false, "VARCHAR(8)", "BlockPct", (AttackType Data) => ((double)Data.Block() * 100.0 / (double)OneOrInt(Data.BlockParryCount())).ToString("0'%", CultureInfo.InvariantCulture), (AttackType Data) => ((double)Data.Block() * 100.0 / (double)OneOrInt(Data.BlockParryCount())).ToString("0'%", CultureInfo.InvariantCulture), (AttackType Left, AttackType Right) => Left.Block().CompareTo(Right.Block())));
		}
		if (!CombatantData.ColumnDefs.ContainsKey("OverHealPct"))
		{
			CombatantData.ColumnDefs.Add("OverHealPct", new CombatantData.ColumnDef("OverHealPct", DefaultVisible: true, "VARCHAR(8)", "OverHealPct", (CombatantData Data) => (long.Parse(Data.Items[CombatantData.DamageTypeDataOutgoingHealing].GetColumnByName("OverHeal"), CultureInfo.InvariantCulture) * 100 / OneOrInt((!Data.Items[CombatantData.DamageTypeDataOutgoingHealing].Items.ContainsKey("All")) ? 0 : Data.DirectHeal())).ToString("0'%", CultureInfo.InvariantCulture), (CombatantData Data) => (long.Parse(Data.Items[CombatantData.DamageTypeDataOutgoingHealing].GetColumnByName("OverHeal"), CultureInfo.InvariantCulture) * 100 / OneOrInt((!Data.Items[CombatantData.DamageTypeDataOutgoingHealing].Items.ContainsKey("All")) ? 0 : Data.DirectHeal())).ToString("0'%", CultureInfo.InvariantCulture), (CombatantData Left, CombatantData Right) => long.Parse(Left.GetColumnByName("OverHealPct").Replace('%', ' '), CultureInfo.InvariantCulture).CompareTo(long.Parse(Right.GetColumnByName("OverHealPct").Replace('%', ' '), CultureInfo.InvariantCulture))));
		}
		if (!CombatantData.ExportVariables.ContainsKey("OverHealPct"))
		{
			CombatantData.ExportVariables.Add("OverHealPct", new CombatantData.TextExportFormatter("OverHealPct", "Over-Heal Percent", "Percent of heals above target's Max HP", (CombatantData Data, string ExtraFormat) => Data.GetColumnByName("OverHealPct")));
		}
		if (!DamageTypeData.ColumnDefs.ContainsKey("OverHeal"))
		{
			DamageTypeData.ColumnDefs.Add("OverHeal", new DamageTypeData.ColumnDef("OverHeal", DefaultVisible: false, "INT", "OverHeal", (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? "0" : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("OverHeal"), (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? "0" : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("OverHeal")));
		}
		if (!AttackType.ColumnDefs.ContainsKey("OverHeal"))
		{
			AttackType.ColumnDefs.Add("OverHeal", new AttackType.ColumnDef("OverHeal", DefaultVisible: false, "INT", "OverHeal", (AttackType Data) => Data.Overheal().ToString(CultureInfo.InvariantCulture), (AttackType Data) => Data.Overheal().ToString(CultureInfo.InvariantCulture), (AttackType Left, AttackType Right) => Left.Overheal().CompareTo(Right.Overheal())));
		}
		if (!MasterSwing.ColumnDefs.ContainsKey("OverHeal"))
		{
			MasterSwing.ColumnDefs.Add("OverHeal", new MasterSwing.ColumnDef("OverHeal", DefaultVisible: false, "INT", "OverHeal", (MasterSwing Data) => (!Data.Tags.ContainsKey("overheal")) ? "0" : Data.Tags["overheal"].ToString(), (MasterSwing Data) => (!Data.Tags.ContainsKey("overheal")) ? "0" : Data.Tags["overheal"].ToString(), (MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("overheal") ? Left.Tags["overheal"].ToString() : "0", Right.Tags.ContainsKey("overheal") ? Right.Tags["overheal"].ToString() : "0", StringComparison.OrdinalIgnoreCase)));
		}
		if (!MasterSwing.ColumnDefs.ContainsKey("DirectHit"))
		{
			MasterSwing.ColumnDefs.Add("DirectHit", new MasterSwing.ColumnDef("DirectHit", DefaultVisible: true, "BOOL", "DirectHit", (MasterSwing Data) => (!Data.Tags.ContainsKey("DirectHit")) ? "" : Data.Tags["DirectHit"].ToString(), (MasterSwing Data) => (!Data.Tags.ContainsKey("DirectHit")) ? "" : Data.Tags["DirectHit"].ToString(), (MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("DirectHit") ? Left.Tags["DirectHit"].ToString() : "", Right.Tags.ContainsKey("DirectHit") ? Right.Tags["DirectHit"].ToString() : "", StringComparison.OrdinalIgnoreCase)));
		}
		if (!AttackType.ColumnDefs.ContainsKey("DirectHitPct"))
		{
			AttackType.ColumnDefs.Add("DirectHitPct", new AttackType.ColumnDef("DirectHitPct", DefaultVisible: true, "VARCHAR(8)", "DirectHitPct", (AttackType Data) => ((double)Data.DirectHitCount() * 100.0 / (double)OneOrInt(Data.Items.Count)).ToString("0.0'%", CultureInfo.InvariantCulture), (AttackType Data) => ((double)Data.DirectHitCount() * 100.0 / (double)OneOrInt(Data.Items.Count)).ToString("0.0'%", CultureInfo.InvariantCulture), (AttackType Left, AttackType Right) => Left.DirectHitCount().CompareTo(Right.DirectHitCount())));
		}
		if (!DamageTypeData.ColumnDefs.ContainsKey("DirectHitPct"))
		{
			DamageTypeData.ColumnDefs.Add("DirectHitPct", new DamageTypeData.ColumnDef("DirectHitPct", DefaultVisible: true, "VARCHAR(8)", "DirectHitPct", (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? 0.ToString("0.0'%", CultureInfo.InvariantCulture) : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("DirectHitPct"), (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? 0.ToString("0.0'%", CultureInfo.InvariantCulture) : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("DirectHitPct")));
		}
		if (!CombatantData.ColumnDefs.ContainsKey("DirectHitPct"))
		{
			CombatantData.ColumnDefs.Add("DirectHitPct", new CombatantData.ColumnDef("DirectHitPct", DefaultVisible: true, "VARCHAR(8)", "DirectHitPct", (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitPct"), (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitPct"), (CombatantData Left, CombatantData Right) => string.Compare(Left.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitPct"), Right.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitPct"), StringComparison.OrdinalIgnoreCase)));
		}
		if (!CombatantData.ExportVariables.ContainsKey("DirectHitPct"))
		{
			CombatantData.ExportVariables.Add("DirectHitPct", new CombatantData.TextExportFormatter("DirectHitPct", "Direct Hit Percent", "Percent of hits that were Direct Hits.", (CombatantData Data, string ExtraFormat) => ((Data.GetColumnByName("DirectHitPct") == "") ? 0.0 : Convert.ToDouble(Data.GetColumnByName("DirectHitPct").Replace("%", ""), CultureInfo.InvariantCulture)).ToString("0'%", CultureInfo.InvariantCulture)));
		}
		if (!AttackType.ColumnDefs.ContainsKey("DirectHitCount"))
		{
			AttackType.ColumnDefs.Add("DirectHitCount", new AttackType.ColumnDef("DirectHitCount", DefaultVisible: false, "INT", "DirectHitCount", (AttackType Data) => Data.DirectHitCount().ToString(CultureInfo.InvariantCulture), (AttackType Data) => Data.DirectHitCount().ToString(CultureInfo.InvariantCulture), (AttackType Left, AttackType Right) => Left.DirectHitCount().CompareTo(Right.DirectHitCount())));
		}
		if (!DamageTypeData.ColumnDefs.ContainsKey("DirectHitCount"))
		{
			DamageTypeData.ColumnDefs.Add("DirectHitCount", new DamageTypeData.ColumnDef("DirectHitCount", DefaultVisible: false, "INT", "DirectHitCount", (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? "0" : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("DirectHitCount"), (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? "0" : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("DirectHitCount")));
		}
		if (!CombatantData.ColumnDefs.ContainsKey("DirectHitCount"))
		{
			CombatantData.ColumnDefs.Add("DirectHitCount", new CombatantData.ColumnDef("DirectHitCount", DefaultVisible: false, "INT", "DirectHitCount", (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitCount"), (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitCount"), (CombatantData Left, CombatantData Right) => string.Compare(Left.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitCount"), Right.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitCount"), StringComparison.OrdinalIgnoreCase)));
		}
		if (!CombatantData.ExportVariables.ContainsKey("DirectHitCount"))
		{
			CombatantData.ExportVariables.Add("DirectHitCount", new CombatantData.TextExportFormatter("DirectHitCount", "Direct Hit Count", "Number of hits that were direct hit.", (CombatantData Data, string ExtraFormat) => Data.GetColumnByName("DirectHitCount")));
		}
		if (!AttackType.ColumnDefs.ContainsKey("CritDirectHitCount"))
		{
			AttackType.ColumnDefs.Add("CritDirectHitCount", new AttackType.ColumnDef("CritDirectHitCount", DefaultVisible: false, "INT", "CritDirectHitCount", (AttackType Data) => Data.CritDirectHitCount().ToString(CultureInfo.InvariantCulture), (AttackType Data) => Data.CritDirectHitCount().ToString(CultureInfo.InvariantCulture), (AttackType Left, AttackType Right) => Left.CritDirectHitCount().CompareTo(Right.CritDirectHitCount())));
		}
		if (!DamageTypeData.ColumnDefs.ContainsKey("CritDirectHitCount"))
		{
			DamageTypeData.ColumnDefs.Add("CritDirectHitCount", new DamageTypeData.ColumnDef("CritDirectHitCount", DefaultVisible: false, "INT", "CritDirectHitCount", (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? "0" : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("CritDirectHitCount"), (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? "0" : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("CritDirectHitCount")));
		}
		if (!CombatantData.ColumnDefs.ContainsKey("CritDirectHitCount"))
		{
			CombatantData.ColumnDefs.Add("CritDirectHitCount", new CombatantData.ColumnDef("CritDirectHitCount", DefaultVisible: false, "INT", "CritDirectHitCount", (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitCount"), (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitCount"), (CombatantData Left, CombatantData Right) => string.Compare(Left.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitCount"), Right.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitCount"), StringComparison.OrdinalIgnoreCase)));
		}
		if (!CombatantData.ExportVariables.ContainsKey("CritDirectHitCount"))
		{
			CombatantData.ExportVariables.Add("CritDirectHitCount", new CombatantData.TextExportFormatter("CritDirectHitCount", "Crit Direct Hit Count", "Number of hits that were critical as well as direct hit.", (CombatantData Data, string ExtraFormat) => Data.GetColumnByName("CritDirectHitCount")));
		}
		if (!AttackType.ColumnDefs.ContainsKey("CritDirectHitPct"))
		{
			AttackType.ColumnDefs.Add("CritDirectHitPct", new AttackType.ColumnDef("CritDirectHitPct", DefaultVisible: true, "VARCHAR(8)", "CritDirectHitPct", (AttackType Data) => ((double)Data.CritDirectHitCount() * 100.0 / (double)OneOrInt(Data.Items.Count)).ToString("0.0'%", CultureInfo.InvariantCulture), (AttackType Data) => ((double)Data.CritDirectHitCount() * 100.0 / (double)OneOrInt(Data.Items.Count)).ToString("0.0'%", CultureInfo.InvariantCulture), (AttackType Left, AttackType Right) => (Left.CritDirectHitCount() * 100 / OneOrInt(Left.Items.Count)).CompareTo(Right.CritDirectHitCount() * 100 / OneOrInt(Right.Items.Count))));
		}
		if (!DamageTypeData.ColumnDefs.ContainsKey("CritDirectHitPct"))
		{
			DamageTypeData.ColumnDefs.Add("CritDirectHitPct", new DamageTypeData.ColumnDef("CritDirectHitPct", DefaultVisible: true, "VARCHAR(8)", "CritDirectHitPct", (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? 0.ToString("0.0'%", CultureInfo.InvariantCulture) : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("CritDirectHitPct"), (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? 0.ToString("0.0'%", CultureInfo.InvariantCulture) : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("CritDirectHitPct")));
		}
		if (!CombatantData.ColumnDefs.ContainsKey("CritDirectHitPct"))
		{
			CombatantData.ColumnDefs.Add("CritDirectHitPct", new CombatantData.ColumnDef("CritDirectHitPct", DefaultVisible: true, "VARCHAR(8)", "CritDirectHitPct", (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitPct"), (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitPct"), (CombatantData Left, CombatantData Right) => string.Compare(Left.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitPct"), Right.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitPct"), StringComparison.OrdinalIgnoreCase)));
		}
		if (!CombatantData.ExportVariables.ContainsKey("CritDirectHitPct"))
		{
			CombatantData.ExportVariables.Add("CritDirectHitPct", new CombatantData.TextExportFormatter("CritDirectHitPct", "Crit Direct Hit Percent", "Percent of hits that were Direct Hits as well as Critical Hits.", (CombatantData Data, string ExtraFormat) => ((Data.GetColumnByName("CritDirectHitPct") == "") ? 0.0 : Convert.ToDouble(Data.GetColumnByName("CritDirectHitPct").Replace("%", ""), CultureInfo.InvariantCulture)).ToString("0'%", CultureInfo.InvariantCulture)));
		}
		if (showDebug)
		{
			if (!MasterSwing.ColumnDefs.ContainsKey("Potency"))
			{
				MasterSwing.ColumnDefs.Add("Potency", new MasterSwing.ColumnDef("Potency", DefaultVisible: true, "FLOAT", "Potency", (MasterSwing Data) => (!Data.Tags.ContainsKey("potency")) ? "0" : ((double)Data.Tags["potency"]).ToString("0.00", CultureInfo.InvariantCulture), (MasterSwing Data) => (!Data.Tags.ContainsKey("potency")) ? "0" : ((double)Data.Tags["potency"]).ToString("0.00", CultureInfo.InvariantCulture), (MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("potency") ? Left.Tags["potency"].ToString() : "0", Right.Tags.ContainsKey("potency") ? Right.Tags["potency"].ToString() : "0", StringComparison.OrdinalIgnoreCase)));
			}
			if (!MasterSwing.ColumnDefs.ContainsKey("StatusEffects"))
			{
				MasterSwing.ColumnDefs.Add("StatusEffects", new MasterSwing.ColumnDef("StatusEffects", DefaultVisible: true, "VARCHAR(50)", "StatusEffects", (MasterSwing Data) => (!Data.Tags.ContainsKey("StatusEffects")) ? "" : Data.Tags["StatusEffects"]?.ToString(), (MasterSwing Data) => (!Data.Tags.ContainsKey("StatusEffects")) ? "" : Data.Tags["StatusEffects"]?.ToString(), (MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("StatusEffects") ? Left.Tags["StatusEffects"].ToString() : "", Right.Tags.ContainsKey("StatusEffects") ? Right.Tags["StatusEffects"].ToString() : "", StringComparison.OrdinalIgnoreCase)));
			}
			if (!MasterSwing.ColumnDefs.ContainsKey("DoTBase"))
			{
				MasterSwing.ColumnDefs.Add("DoTBase", new MasterSwing.ColumnDef("DoTBase", DefaultVisible: true, "INT", "DoTBase", (MasterSwing Data) => (!Data.Tags.ContainsKey("dotbase")) ? "" : ((uint)Data.Tags["dotbase"]).ToString("0", CultureInfo.InvariantCulture), (MasterSwing Data) => (!Data.Tags.ContainsKey("dotbase")) ? "" : ((uint)Data.Tags["dotbase"]).ToString("0", CultureInfo.InvariantCulture), (MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("dotbase") ? Left.Tags["dotbase"].ToString() : "0", Right.Tags.ContainsKey("dotbase") ? Right.Tags["dotbase"].ToString() : "0", StringComparison.OrdinalIgnoreCase)));
			}
			if (!MasterSwing.ColumnDefs.ContainsKey("BuffByte1"))
			{
				MasterSwing.ColumnDefs.Add("BuffByte1", new MasterSwing.ColumnDef("BuffByte1", DefaultVisible: false, "int", "BuffByte1", (MasterSwing Data) => (!Data.Tags.ContainsKey("BuffByte1")) ? "" : Data.Tags["BuffByte1"].ToString(), (MasterSwing Data) => (!Data.Tags.ContainsKey("BuffByte1")) ? "" : Data.Tags["BuffByte1"].ToString(), (MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("BuffByte1") ? Left.Tags["BuffByte1"].ToString() : "", Right.Tags.ContainsKey("BuffByte1") ? Right.Tags["BuffByte1"].ToString() : "", StringComparison.OrdinalIgnoreCase)));
			}
			if (!MasterSwing.ColumnDefs.ContainsKey("BuffByte2"))
			{
				MasterSwing.ColumnDefs.Add("BuffByte2", new MasterSwing.ColumnDef("BuffByte2", DefaultVisible: false, "int", "BuffByte2", (MasterSwing Data) => (!Data.Tags.ContainsKey("BuffByte2")) ? "" : Data.Tags["BuffByte2"].ToString(), (MasterSwing Data) => (!Data.Tags.ContainsKey("BuffByte2")) ? "" : Data.Tags["BuffByte2"].ToString(), (MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("BuffByte2") ? Left.Tags["BuffByte2"].ToString() : "", Right.Tags.ContainsKey("BuffByte2") ? Right.Tags["BuffByte2"].ToString() : "", StringComparison.OrdinalIgnoreCase)));
			}
			if (!MasterSwing.ColumnDefs.ContainsKey("BuffByte3"))
			{
				MasterSwing.ColumnDefs.Add("BuffByte3", new MasterSwing.ColumnDef("BuffByte3", DefaultVisible: false, "int", "BuffByte3", (MasterSwing Data) => (!Data.Tags.ContainsKey("BuffByte3")) ? "" : Data.Tags["BuffByte3"].ToString(), (MasterSwing Data) => (!Data.Tags.ContainsKey("BuffByte3")) ? "" : Data.Tags["BuffByte3"].ToString(), (MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("BuffByte3") ? Left.Tags["BuffByte3"].ToString() : "", Right.Tags.ContainsKey("BuffByte3") ? Right.Tags["BuffByte3"].ToString() : "", StringComparison.OrdinalIgnoreCase)));
			}
			if (!MasterSwing.ColumnDefs.ContainsKey("CritRate"))
			{
				MasterSwing.ColumnDefs.Add("CritRate", new MasterSwing.ColumnDef("CritRate", DefaultVisible: false, "VARCHAR(8)", "CritRate", (MasterSwing Data) => (!Data.Tags.ContainsKey("CritRate")) ? "" : Data.Tags["CritRate"].ToString(), (MasterSwing Data) => (!Data.Tags.ContainsKey("CritRate")) ? "" : Data.Tags["CritRate"].ToString(), (MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("CritRate") ? Left.Tags["CritRate"].ToString() : "0", Right.Tags.ContainsKey("CritRate") ? Right.Tags["CritRate"].ToString() : "0", StringComparison.OrdinalIgnoreCase)));
			}
			if (!MasterSwing.ColumnDefs.ContainsKey("CritEffects"))
			{
				MasterSwing.ColumnDefs.Add("CritEffects", new MasterSwing.ColumnDef("CritEffects", DefaultVisible: false, "VARCHAR(8)", "CritEffects", (MasterSwing Data) => (!Data.Tags.ContainsKey("CritEffects")) ? "" : Data.Tags["CritEffects"].ToString(), (MasterSwing Data) => (!Data.Tags.ContainsKey("CritEffects")) ? "" : Data.Tags["CritEffects"].ToString(), (MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("CritEffects") ? Left.Tags["CritEffects"].ToString() : "", Right.Tags.ContainsKey("CritEffects") ? Right.Tags["CritEffects"].ToString() : "", StringComparison.OrdinalIgnoreCase)));
			}
			if (!MasterSwing.ColumnDefs.ContainsKey("DHRate"))
			{
				MasterSwing.ColumnDefs.Add("DHRate", new MasterSwing.ColumnDef("DHRate", DefaultVisible: false, "VARCHAR(8)", "DHRate", (MasterSwing Data) => (!Data.Tags.ContainsKey("DHRate")) ? "" : Data.Tags["DHRate"].ToString(), (MasterSwing Data) => (!Data.Tags.ContainsKey("DHRate")) ? "" : Data.Tags["DHRate"].ToString(), (MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("DHRate") ? Left.Tags["DHRate"].ToString() : "", Right.Tags.ContainsKey("DHRate") ? Right.Tags["DHRate"].ToString() : "", StringComparison.OrdinalIgnoreCase)));
			}
			if (!MasterSwing.ColumnDefs.ContainsKey("DHEffects"))
			{
				MasterSwing.ColumnDefs.Add("DHEffects", new MasterSwing.ColumnDef("DHEffects", DefaultVisible: false, "VARCHAR(8)", "DHEffects", (MasterSwing Data) => (!Data.Tags.ContainsKey("DHEffects")) ? "" : Data.Tags["DHEffects"].ToString(), (MasterSwing Data) => (!Data.Tags.ContainsKey("DHEffects")) ? "" : Data.Tags["DHEffects"].ToString(), (MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("DHEffects") ? Left.Tags["DHEffects"].ToString() : "", Right.Tags.ContainsKey("DHEffects") ? Right.Tags["DHEffects"].ToString() : "", StringComparison.OrdinalIgnoreCase)));
			}
		}
		else
		{
			if (MasterSwing.ColumnDefs.ContainsKey("Potency"))
			{
				MasterSwing.ColumnDefs.Remove("Potency");
			}
			if (MasterSwing.ColumnDefs.ContainsKey("StatusEffects"))
			{
				MasterSwing.ColumnDefs.Remove("StatusEffects");
			}
			if (MasterSwing.ColumnDefs.ContainsKey("DoTBase"))
			{
				MasterSwing.ColumnDefs.Remove("DoTBase");
			}
			if (MasterSwing.ColumnDefs.ContainsKey("BuffByte1"))
			{
				MasterSwing.ColumnDefs.Remove("BuffByte1");
			}
			if (MasterSwing.ColumnDefs.ContainsKey("BuffByte2"))
			{
				MasterSwing.ColumnDefs.Remove("BuffByte2");
			}
			if (MasterSwing.ColumnDefs.ContainsKey("BuffByte3"))
			{
				MasterSwing.ColumnDefs.Remove("BuffByte3");
			}
			if (MasterSwing.ColumnDefs.ContainsKey("CritRate"))
			{
				MasterSwing.ColumnDefs.Remove("CritRate");
			}
			if (MasterSwing.ColumnDefs.ContainsKey("CritEffects"))
			{
				MasterSwing.ColumnDefs.Remove("CritEffects");
			}
			if (MasterSwing.ColumnDefs.ContainsKey("DHRate"))
			{
				MasterSwing.ColumnDefs.Remove("DHRate");
			}
			if (MasterSwing.ColumnDefs.ContainsKey("DHEffects"))
			{
				MasterSwing.ColumnDefs.Remove("DHEffects");
			}
		}
		ActGlobals.oFormActMain.ValidateLists();
		ActGlobals.oFormActMain.ValidateTableSetup();
	}

	private static int OneOrInt(int data)
	{
		if (data == 0)
		{
			return 1;
		}
		return data;
	}

	private static long OneOrInt(long data)
	{
		if (data == 0L)
		{
			return 1L;
		}
		return data;
	}

	public static Bitmap GenAttackTypeGraph(AttackType AttackTypeSource, int SizeX, int SizeY, string Sorting)
	{
		DateTime now = DateTime.Now;
		if (SizeX < 16 || SizeY < 16)
		{
			return new Bitmap(16, 16);
		}
		Bitmap bitmap = new Bitmap(SizeX, SizeY);
		try
		{
			List<MasterSwing> list = AttackTypeSource.Items.Where((MasterSwing masterSwing2) => masterSwing2.Tags.ContainsKey("potency") && float.TryParse(masterSwing2.Tags["potency"].ToString(), out var result) && result > 0f).ToList();
			try
			{
				list.Sort(new MasterSwing.DualComparison(Sorting, Sorting));
			}
			catch (Exception ex)
			{
				ActGlobals.oFormActMain.WriteExceptionLog(ex, string.Empty);
				bitmap = GraphDrawMessage(ex.ToString(), 12f, bitmap);
			}
			using SolidBrush solidBrush = new SolidBrush(Color.LightGray);
			using SolidBrush brush = new SolidBrush(Color.Black);
			using Pen pen = new Pen(Color.Black);
			using Pen pen2 = new Pen(Color.DarkGray);
			Dictionary<int, SolidBrush> dictionary = new Dictionary<int, SolidBrush>();
			foreach (KeyValuePair<int, List<string>> item in CombatantData.SwingTypeToDamageTypeDataLinksOutgoing)
			{
				if (!dictionary.ContainsKey(item.Key))
				{
					dictionary.Add(item.Key, new SolidBrush(Color.FromArgb(180, CombatantData.OutgoingDamageTypeDataObjects[item.Value[0]].TypeColor)));
				}
			}
			foreach (KeyValuePair<int, List<string>> item2 in CombatantData.SwingTypeToDamageTypeDataLinksIncoming)
			{
				if (!dictionary.ContainsKey(item2.Key))
				{
					dictionary.Add(item2.Key, new SolidBrush(Color.FromArgb(180, CombatantData.IncomingDamageTypeDataObjects[item2.Value[0]].TypeColor)));
				}
			}
			Dictionary<int, SolidBrush> dictionary2 = new Dictionary<int, SolidBrush>();
			foreach (KeyValuePair<int, List<string>> item3 in CombatantData.SwingTypeToDamageTypeDataLinksOutgoing)
			{
				if (!dictionary2.ContainsKey(item3.Key))
				{
					dictionary2.Add(item3.Key, new SolidBrush(Color.FromArgb(255, CombatantData.OutgoingDamageTypeDataObjects[item3.Value[0]].TypeColor)));
				}
			}
			foreach (KeyValuePair<int, List<string>> item4 in CombatantData.SwingTypeToDamageTypeDataLinksIncoming)
			{
				if (!dictionary2.ContainsKey(item4.Key))
				{
					dictionary2.Add(item4.Key, new SolidBrush(Color.FromArgb(255, CombatantData.IncomingDamageTypeDataObjects[item4.Value[0]].TypeColor)));
				}
			}
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			graphics.Clear(solidBrush.Color);
			int num = 16;
			Rectangle rect = new Rectangle(4, 4, bitmap.Width - 1 - num * 4, bitmap.Height - 1 - num * 2);
			graphics.DrawRectangle(pen, rect);
			float num2 = rect.Bottom;
			float num3;
			try
			{
				num3 = (float)rect.Width / (float)list.Count;
			}
			catch (Exception ex2)
			{
				return GraphDrawMessage(ex2.ToString(), 12f, bitmap);
			}
			float num4 = rect.Left;
			float num5 = 0f;
			foreach (MasterSwing item5 in list)
			{
				if ((float)Math.Round(float.Parse(item5.Tags["potency"].ToString(), CultureInfo.InvariantCulture), 2) > num5)
				{
					num5 = (float)Math.Round(float.Parse(item5.Tags["potency"].ToString(), CultureInfo.InvariantCulture), 2);
				}
			}
			int num6 = num5.ToString().ToCharArray().Length;
			int num7 = (int)Math.Pow(10.0, num6);
			while ((float)(num7 / 2) > num5)
			{
				num7 /= 2;
			}
			if ((float)num7 / 1.25f > num5)
			{
				num7 = Convert.ToInt32((float)num7 / 1.25f);
			}
			float num8 = (float)rect.Height / (float)num7;
			_ = 1f / num8;
			Font font = new Font("Arial", 8f);
			graphics.DrawString("0", font, brush, rect.Right + 5, rect.Bottom);
			graphics.DrawString(Convert.ToInt32(num7).ToString(), font, brush, rect.Right + 5, rect.Top);
			try
			{
				int num9 = Convert.ToInt32(num2 - (float)Convert.ToInt32((float)(num7 / 4) * num8));
				graphics.DrawLine(pen2, rect.Left, num9, rect.Right, num9);
				graphics.DrawString((num7 / 4).ToString(), font, brush, rect.Right + 5, num9);
				num9 = Convert.ToInt32(num2 - (float)Convert.ToInt32((float)(num7 / 4 * 2) * num8));
				graphics.DrawLine(pen2, rect.Left, num9, rect.Right, num9);
				graphics.DrawString((num7 / 4 * 2).ToString(), font, brush, rect.Right + 5, num9);
				num9 = Convert.ToInt32(num2 - (float)Convert.ToInt32((float)(num7 / 4 * 3) * num8));
				graphics.DrawLine(pen2, rect.Left, num9, rect.Right, num9);
				graphics.DrawString((num7 / 4 * 3).ToString(), font, brush, rect.Right + 5, num9);
			}
			catch
			{
			}
			for (int num10 = 0; num10 < list.Count; num10++)
			{
				MasterSwing masterSwing = list[num10];
				float x = num4;
				float y = num2 - (float)Math.Round(float.Parse(masterSwing.Tags["potency"].ToString(), CultureInfo.InvariantCulture), 2) * num8;
				float width = num3;
				float height = (float)Math.Round(float.Parse(masterSwing.Tags["potency"].ToString(), CultureInfo.InvariantCulture), 2) * num8;
				if (num10 > 0 && masterSwing.Time != list[num10 - 1].Time)
				{
					graphics.DrawLine(pen2, num4, rect.Top, num4, rect.Bottom);
				}
				if (masterSwing.Critical)
				{
					graphics.FillRectangle(dictionary2[masterSwing.SwingType], x, y, width, height);
				}
				else
				{
					graphics.FillRectangle(dictionary[masterSwing.SwingType], x, y, width, height);
				}
				graphics.DrawRectangle(pen, x, y, width, height);
				if (rect.Width / list.Count > 15)
				{
					if (rect.Width / list.Count > 25)
					{
						graphics.DrawString(Math.Round(float.Parse(masterSwing.Tags["potency"].ToString(), CultureInfo.InvariantCulture), 2).ToString(CultureInfo.InvariantCulture), font, brush, num4 - 8f + num3 / 2f, num2 + 2f);
					}
					else if (num10 % 2 == 0)
					{
						graphics.DrawString(Math.Round(float.Parse(masterSwing.Tags["potency"].ToString(), CultureInfo.InvariantCulture), 2).ToString(CultureInfo.InvariantCulture), font, brush, num4 - 8f + num3 / 2f, num2 + 12f);
					}
					else
					{
						graphics.DrawString(Math.Round(float.Parse(masterSwing.Tags["potency"].ToString(), CultureInfo.InvariantCulture), 2).ToString(CultureInfo.InvariantCulture), font, brush, num4 - 8f + num3 / 2f, num2 + 2f);
					}
				}
				num4 += num3;
			}
			graphics.DrawLine(pen2, num4, rect.Top, num4, rect.Bottom);
		}
		catch (Exception ex3)
		{
			ActGlobals.oFormActMain.WriteExceptionLog(ex3, string.Empty);
			bitmap = GraphDrawMessage(ex3.ToString(), 12f, bitmap);
		}
		TimeSpan timeSpan = DateTime.Now - now;
		ActGlobals.oFormActMain.WriteDebugLog("GraphHitBars: " + timeSpan.TotalMilliseconds.ToString("F"));
		return bitmap;
	}

	internal static Bitmap GraphDrawMessage(string Message, float FontSize, Bitmap BlankImage)
	{
		Graphics graphics = Graphics.FromImage(BlankImage);
		graphics.SmoothingMode = SmoothingMode.AntiAlias;
		graphics.Clear(Color.White);
		graphics.DrawString(Message, new Font("Arial Black", FontSize, FontStyle.Regular), new SolidBrush(Color.Black), 12f, 12f);
		return BlankImage;
	}
}
