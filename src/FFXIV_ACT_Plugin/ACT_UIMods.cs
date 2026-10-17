using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Advanced_Combat_Tracker;
using FFXIV_ACT_Plugin.Common;
using FFXIV_ACT_Plugin.Parse;

namespace FFXIV_ACT_Plugin;

public class ACT_UIMods
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static EncounterData.ExportStringDataCallback _003C_003E9__14_0;

		public static CombatantData.ExportStringDataCallback _003C_003E9__14_1;

		public static CombatantData.ExportStringDataCallback _003C_003E9__14_2;

		public static CombatantData.ExportStringDataCallback _003C_003E9__14_3;

		public static EncounterData.ExportStringDataCallback _003C_003E9__14_4;

		public static EncounterData.ExportStringDataCallback _003C_003E9__14_5;

		public static EncounterData.ExportStringDataCallback _003C_003E9__14_6;

		public static MasterSwing.StringDataCallback _003C_003E9__14_7;

		public static MasterSwing.StringDataCallback _003C_003E9__14_8;

		public static Comparison<MasterSwing> _003C_003E9__14_9;

		public static CombatantData.StringDataCallback _003C_003E9__14_10;

		public static CombatantData.StringDataCallback _003C_003E9__14_11;

		public static Comparison<CombatantData> _003C_003E9__14_12;

		public static CombatantData.ExportStringDataCallback _003C_003E9__14_13;

		public static CombatantData.StringDataCallback _003C_003E9__14_14;

		public static CombatantData.StringDataCallback _003C_003E9__14_15;

		public static Comparison<CombatantData> _003C_003E9__14_16;

		public static CombatantData.ExportStringDataCallback _003C_003E9__14_17;

		public static CombatantData.StringDataCallback _003C_003E9__14_18;

		public static CombatantData.StringDataCallback _003C_003E9__14_19;

		public static Comparison<CombatantData> _003C_003E9__14_20;

		public static CombatantData.ExportStringDataCallback _003C_003E9__14_21;

		public static CombatantData.StringDataCallback _003C_003E9__14_22;

		public static CombatantData.StringDataCallback _003C_003E9__14_23;

		public static Comparison<CombatantData> _003C_003E9__14_24;

		public static CombatantData.ExportStringDataCallback _003C_003E9__14_25;

		public static DamageTypeData.StringDataCallback _003C_003E9__14_26;

		public static DamageTypeData.StringDataCallback _003C_003E9__14_27;

		public static DamageTypeData.StringDataCallback _003C_003E9__14_28;

		public static DamageTypeData.StringDataCallback _003C_003E9__14_29;

		public static AttackType.StringDataCallback _003C_003E9__14_30;

		public static AttackType.StringDataCallback _003C_003E9__14_31;

		public static Comparison<AttackType> _003C_003E9__14_32;

		public static AttackType.StringDataCallback _003C_003E9__14_33;

		public static AttackType.StringDataCallback _003C_003E9__14_34;

		public static Comparison<AttackType> _003C_003E9__14_35;

		public static AttackType.StringDataCallback _003C_003E9__14_36;

		public static AttackType.StringDataCallback _003C_003E9__14_37;

		public static Comparison<AttackType> _003C_003E9__14_38;

		public static AttackType.StringDataCallback _003C_003E9__14_39;

		public static AttackType.StringDataCallback _003C_003E9__14_40;

		public static Comparison<AttackType> _003C_003E9__14_41;

		public static CombatantData.StringDataCallback _003C_003E9__14_42;

		public static CombatantData.StringDataCallback _003C_003E9__14_43;

		public static Comparison<CombatantData> _003C_003E9__14_44;

		public static CombatantData.ExportStringDataCallback _003C_003E9__14_45;

		public static DamageTypeData.StringDataCallback _003C_003E9__14_46;

		public static DamageTypeData.StringDataCallback _003C_003E9__14_47;

		public static AttackType.StringDataCallback _003C_003E9__14_48;

		public static AttackType.StringDataCallback _003C_003E9__14_49;

		public static Comparison<AttackType> _003C_003E9__14_50;

		public static MasterSwing.StringDataCallback _003C_003E9__14_51;

		public static MasterSwing.StringDataCallback _003C_003E9__14_52;

		public static Comparison<MasterSwing> _003C_003E9__14_53;

		public static MasterSwing.StringDataCallback _003C_003E9__14_54;

		public static MasterSwing.StringDataCallback _003C_003E9__14_55;

		public static Comparison<MasterSwing> _003C_003E9__14_56;

		public static AttackType.StringDataCallback _003C_003E9__14_57;

		public static AttackType.StringDataCallback _003C_003E9__14_58;

		public static Comparison<AttackType> _003C_003E9__14_59;

		public static DamageTypeData.StringDataCallback _003C_003E9__14_60;

		public static DamageTypeData.StringDataCallback _003C_003E9__14_61;

		public static CombatantData.StringDataCallback _003C_003E9__14_62;

		public static CombatantData.StringDataCallback _003C_003E9__14_63;

		public static Comparison<CombatantData> _003C_003E9__14_64;

		public static CombatantData.ExportStringDataCallback _003C_003E9__14_65;

		public static AttackType.StringDataCallback _003C_003E9__14_66;

		public static AttackType.StringDataCallback _003C_003E9__14_67;

		public static Comparison<AttackType> _003C_003E9__14_68;

		public static DamageTypeData.StringDataCallback _003C_003E9__14_69;

		public static DamageTypeData.StringDataCallback _003C_003E9__14_70;

		public static CombatantData.StringDataCallback _003C_003E9__14_71;

		public static CombatantData.StringDataCallback _003C_003E9__14_72;

		public static Comparison<CombatantData> _003C_003E9__14_73;

		public static CombatantData.ExportStringDataCallback _003C_003E9__14_74;

		public static AttackType.StringDataCallback _003C_003E9__14_75;

		public static AttackType.StringDataCallback _003C_003E9__14_76;

		public static Comparison<AttackType> _003C_003E9__14_77;

		public static DamageTypeData.StringDataCallback _003C_003E9__14_78;

		public static DamageTypeData.StringDataCallback _003C_003E9__14_79;

		public static CombatantData.StringDataCallback _003C_003E9__14_80;

		public static CombatantData.StringDataCallback _003C_003E9__14_81;

		public static Comparison<CombatantData> _003C_003E9__14_82;

		public static CombatantData.ExportStringDataCallback _003C_003E9__14_83;

		public static AttackType.StringDataCallback _003C_003E9__14_84;

		public static AttackType.StringDataCallback _003C_003E9__14_85;

		public static Comparison<AttackType> _003C_003E9__14_86;

		public static DamageTypeData.StringDataCallback _003C_003E9__14_87;

		public static DamageTypeData.StringDataCallback _003C_003E9__14_88;

		public static CombatantData.StringDataCallback _003C_003E9__14_89;

		public static CombatantData.StringDataCallback _003C_003E9__14_90;

		public static Comparison<CombatantData> _003C_003E9__14_91;

		public static CombatantData.ExportStringDataCallback _003C_003E9__14_92;

		public static MasterSwing.StringDataCallback _003C_003E9__14_93;

		public static MasterSwing.StringDataCallback _003C_003E9__14_94;

		public static Comparison<MasterSwing> _003C_003E9__14_95;

		public static MasterSwing.StringDataCallback _003C_003E9__14_96;

		public static MasterSwing.StringDataCallback _003C_003E9__14_97;

		public static Comparison<MasterSwing> _003C_003E9__14_98;

		public static MasterSwing.StringDataCallback _003C_003E9__14_99;

		public static MasterSwing.StringDataCallback _003C_003E9__14_100;

		public static Comparison<MasterSwing> _003C_003E9__14_101;

		public static MasterSwing.StringDataCallback _003C_003E9__14_102;

		public static MasterSwing.StringDataCallback _003C_003E9__14_103;

		public static Comparison<MasterSwing> _003C_003E9__14_104;

		public static MasterSwing.StringDataCallback _003C_003E9__14_105;

		public static MasterSwing.StringDataCallback _003C_003E9__14_106;

		public static Comparison<MasterSwing> _003C_003E9__14_107;

		public static MasterSwing.StringDataCallback _003C_003E9__14_108;

		public static MasterSwing.StringDataCallback _003C_003E9__14_109;

		public static Comparison<MasterSwing> _003C_003E9__14_110;

		public static MasterSwing.StringDataCallback _003C_003E9__14_111;

		public static MasterSwing.StringDataCallback _003C_003E9__14_112;

		public static Comparison<MasterSwing> _003C_003E9__14_113;

		public static MasterSwing.StringDataCallback _003C_003E9__14_114;

		public static MasterSwing.StringDataCallback _003C_003E9__14_115;

		public static Comparison<MasterSwing> _003C_003E9__14_116;

		public static MasterSwing.StringDataCallback _003C_003E9__14_117;

		public static MasterSwing.StringDataCallback _003C_003E9__14_118;

		public static Comparison<MasterSwing> _003C_003E9__14_119;

		public static MasterSwing.StringDataCallback _003C_003E9__14_120;

		public static MasterSwing.StringDataCallback _003C_003E9__14_121;

		public static Comparison<MasterSwing> _003C_003E9__14_122;

		public static Func<MasterSwing, bool> _003C_003E9__17_0;

		internal string _003CUpdateACTTables_003Eb__14_0(EncounterData data, List<CombatantData> SelectiveAllies, string extra)
		{
			return data.ZoneName;
		}

		internal string _003CUpdateACTTables_003Eb__14_1(CombatantData Data, string ExtraFormat)
		{
			return Data.LastNDPS(10).ToString("0", CultureInfo.InvariantCulture);
		}

		internal string _003CUpdateACTTables_003Eb__14_2(CombatantData Data, string ExtraFormat)
		{
			return Data.LastNDPS(30).ToString("0", CultureInfo.InvariantCulture);
		}

		internal string _003CUpdateACTTables_003Eb__14_3(CombatantData Data, string ExtraFormat)
		{
			return Data.LastNDPS(60).ToString("0", CultureInfo.InvariantCulture);
		}

		internal string _003CUpdateACTTables_003Eb__14_4(EncounterData Data, List<CombatantData> SelectiveAllies, string Extra)
		{
			return Data.LastNDPS(SelectiveAllies, 10).ToString("0", CultureInfo.InvariantCulture);
		}

		internal string _003CUpdateACTTables_003Eb__14_5(EncounterData Data, List<CombatantData> SelectiveAllies, string Extra)
		{
			return Data.LastNDPS(SelectiveAllies, 30).ToString("0", CultureInfo.InvariantCulture);
		}

		internal string _003CUpdateACTTables_003Eb__14_6(EncounterData Data, List<CombatantData> SelectiveAllies, string Extra)
		{
			return Data.LastNDPS(SelectiveAllies, 60).ToString("0", CultureInfo.InvariantCulture);
		}

		internal string _003CUpdateACTTables_003Eb__14_7(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("StatusDuration"))
			{
				return "";
			}
			return ((double)Data.Tags["StatusDuration"]).ToString("0.#", CultureInfo.InvariantCulture);
		}

		internal string _003CUpdateACTTables_003Eb__14_8(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("StatusDuration"))
			{
				return "";
			}
			return ((double)Data.Tags["StatusDuration"]).ToString("0.#", CultureInfo.InvariantCulture);
		}

		internal int _003CUpdateACTTables_003Eb__14_9(MasterSwing Left, MasterSwing Right)
		{
			return (Left.Tags.ContainsKey("StatusDuration") ? ((double)Left.Tags["StatusDuration"]) : 0.0).CompareTo(Right.Tags.ContainsKey("StatusDuration") ? ((double)Right.Tags["StatusDuration"]) : 0.0);
		}

		internal string _003CUpdateACTTables_003Eb__14_10(CombatantData Data)
		{
			return Data.Job();
		}

		internal string _003CUpdateACTTables_003Eb__14_11(CombatantData Data)
		{
			return Data.Job();
		}

		internal int _003CUpdateACTTables_003Eb__14_12(CombatantData Left, CombatantData Right)
		{
			return string.Compare(Left.Job(), Right.Job(), StringComparison.OrdinalIgnoreCase);
		}

		internal string _003CUpdateACTTables_003Eb__14_13(CombatantData Data, string ExtraFormat)
		{
			return Data.GetColumnByName("Job");
		}

		internal string _003CUpdateACTTables_003Eb__14_14(CombatantData Data)
		{
			return Data.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ParryPct");
		}

		internal string _003CUpdateACTTables_003Eb__14_15(CombatantData Data)
		{
			return Data.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ParryPct");
		}

		internal int _003CUpdateACTTables_003Eb__14_16(CombatantData Left, CombatantData Right)
		{
			return string.Compare(Left.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ParryPct"), Right.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ParryPct"), StringComparison.OrdinalIgnoreCase);
		}

		internal string _003CUpdateACTTables_003Eb__14_17(CombatantData Data, string ExtraFormat)
		{
			return Data.GetColumnByName("ParryPct");
		}

		internal string _003CUpdateACTTables_003Eb__14_18(CombatantData Data)
		{
			return Data.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("BlockPct");
		}

		internal string _003CUpdateACTTables_003Eb__14_19(CombatantData Data)
		{
			return Data.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("BlockPct");
		}

		internal int _003CUpdateACTTables_003Eb__14_20(CombatantData Left, CombatantData Right)
		{
			return string.Compare(Left.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("BlockPct"), Right.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("BlockPct"), StringComparison.OrdinalIgnoreCase);
		}

		internal string _003CUpdateACTTables_003Eb__14_21(CombatantData Data, string ExtraFormat)
		{
			return Data.GetColumnByName("BlockPct");
		}

		internal string _003CUpdateACTTables_003Eb__14_22(CombatantData Data)
		{
			return Data.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ToHit");
		}

		internal string _003CUpdateACTTables_003Eb__14_23(CombatantData Data)
		{
			return Data.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ToHit");
		}

		internal int _003CUpdateACTTables_003Eb__14_24(CombatantData Left, CombatantData Right)
		{
			return string.Compare(Left.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ToHit"), Right.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ToHit"), StringComparison.OrdinalIgnoreCase);
		}

		internal string _003CUpdateACTTables_003Eb__14_25(CombatantData Data, string ExtraFormat)
		{
			return Data.GetColumnByName("IncToHit");
		}

		internal string _003CUpdateACTTables_003Eb__14_26(DamageTypeData Data)
		{
			if (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText))
			{
				return 0.ToString("0'%", CultureInfo.InvariantCulture);
			}
			return Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("ParryPct");
		}

		internal string _003CUpdateACTTables_003Eb__14_27(DamageTypeData Data)
		{
			if (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText))
			{
				return 0.ToString("0'%", CultureInfo.InvariantCulture);
			}
			return Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("ParryPct");
		}

		internal string _003CUpdateACTTables_003Eb__14_28(DamageTypeData Data)
		{
			if (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText))
			{
				return 0.ToString("0'%", CultureInfo.InvariantCulture);
			}
			return Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("BlockPct");
		}

		internal string _003CUpdateACTTables_003Eb__14_29(DamageTypeData Data)
		{
			if (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText))
			{
				return 0.ToString("0'%", CultureInfo.InvariantCulture);
			}
			return Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("BlockPct");
		}

		internal string _003CUpdateACTTables_003Eb__14_30(AttackType Data)
		{
			return Data.Parry().ToString(CultureInfo.InvariantCulture);
		}

		internal string _003CUpdateACTTables_003Eb__14_31(AttackType Data)
		{
			return Data.Parry().ToString(CultureInfo.InvariantCulture);
		}

		internal int _003CUpdateACTTables_003Eb__14_32(AttackType Left, AttackType Right)
		{
			return Left.Parry().CompareTo(Right.Parry());
		}

		internal string _003CUpdateACTTables_003Eb__14_33(AttackType Data)
		{
			return ((double)Data.Parry() * 100.0 / (double)OneOrInt(Data.BlockParryCount())).ToString("0'%", CultureInfo.InvariantCulture);
		}

		internal string _003CUpdateACTTables_003Eb__14_34(AttackType Data)
		{
			return ((double)Data.Parry() * 100.0 / (double)OneOrInt(Data.BlockParryCount())).ToString("0'%", CultureInfo.InvariantCulture);
		}

		internal int _003CUpdateACTTables_003Eb__14_35(AttackType Left, AttackType Right)
		{
			return Left.Parry().CompareTo(Right.Parry());
		}

		internal string _003CUpdateACTTables_003Eb__14_36(AttackType Data)
		{
			return Data.Block().ToString(CultureInfo.InvariantCulture);
		}

		internal string _003CUpdateACTTables_003Eb__14_37(AttackType Data)
		{
			return Data.Block().ToString(CultureInfo.InvariantCulture);
		}

		internal int _003CUpdateACTTables_003Eb__14_38(AttackType Left, AttackType Right)
		{
			return Left.Block().CompareTo(Right.Block());
		}

		internal string _003CUpdateACTTables_003Eb__14_39(AttackType Data)
		{
			return ((double)Data.Block() * 100.0 / (double)OneOrInt(Data.BlockParryCount())).ToString("0'%", CultureInfo.InvariantCulture);
		}

		internal string _003CUpdateACTTables_003Eb__14_40(AttackType Data)
		{
			return ((double)Data.Block() * 100.0 / (double)OneOrInt(Data.BlockParryCount())).ToString("0'%", CultureInfo.InvariantCulture);
		}

		internal int _003CUpdateACTTables_003Eb__14_41(AttackType Left, AttackType Right)
		{
			return Left.Block().CompareTo(Right.Block());
		}

		internal string _003CUpdateACTTables_003Eb__14_42(CombatantData Data)
		{
			return (long.Parse(Data.Items[CombatantData.DamageTypeDataOutgoingHealing].GetColumnByName("OverHeal"), CultureInfo.InvariantCulture) * 100 / OneOrInt((!Data.Items[CombatantData.DamageTypeDataOutgoingHealing].Items.ContainsKey("All")) ? 0 : Data.DirectHeal())).ToString("0'%", CultureInfo.InvariantCulture);
		}

		internal string _003CUpdateACTTables_003Eb__14_43(CombatantData Data)
		{
			return (long.Parse(Data.Items[CombatantData.DamageTypeDataOutgoingHealing].GetColumnByName("OverHeal"), CultureInfo.InvariantCulture) * 100 / OneOrInt((!Data.Items[CombatantData.DamageTypeDataOutgoingHealing].Items.ContainsKey("All")) ? 0 : Data.DirectHeal())).ToString("0'%", CultureInfo.InvariantCulture);
		}

		internal int _003CUpdateACTTables_003Eb__14_44(CombatantData Left, CombatantData Right)
		{
			return long.Parse(Left.GetColumnByName("OverHealPct").Replace('%', ' '), CultureInfo.InvariantCulture).CompareTo(long.Parse(Right.GetColumnByName("OverHealPct").Replace('%', ' '), CultureInfo.InvariantCulture));
		}

		internal string _003CUpdateACTTables_003Eb__14_45(CombatantData Data, string ExtraFormat)
		{
			return Data.GetColumnByName("OverHealPct");
		}

		internal string _003CUpdateACTTables_003Eb__14_46(DamageTypeData Data)
		{
			if (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText))
			{
				return "0";
			}
			return Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("OverHeal");
		}

		internal string _003CUpdateACTTables_003Eb__14_47(DamageTypeData Data)
		{
			if (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText))
			{
				return "0";
			}
			return Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("OverHeal");
		}

		internal string _003CUpdateACTTables_003Eb__14_48(AttackType Data)
		{
			return Data.Overheal().ToString(CultureInfo.InvariantCulture);
		}

		internal string _003CUpdateACTTables_003Eb__14_49(AttackType Data)
		{
			return Data.Overheal().ToString(CultureInfo.InvariantCulture);
		}

		internal int _003CUpdateACTTables_003Eb__14_50(AttackType Left, AttackType Right)
		{
			return Left.Overheal().CompareTo(Right.Overheal());
		}

		internal string _003CUpdateACTTables_003Eb__14_51(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("overheal"))
			{
				return "0";
			}
			return Data.Tags["overheal"].ToString();
		}

		internal string _003CUpdateACTTables_003Eb__14_52(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("overheal"))
			{
				return "0";
			}
			return Data.Tags["overheal"].ToString();
		}

		internal int _003CUpdateACTTables_003Eb__14_53(MasterSwing Left, MasterSwing Right)
		{
			return string.Compare(Left.Tags.ContainsKey("overheal") ? Left.Tags["overheal"].ToString() : "0", Right.Tags.ContainsKey("overheal") ? Right.Tags["overheal"].ToString() : "0", StringComparison.OrdinalIgnoreCase);
		}

		internal string _003CUpdateACTTables_003Eb__14_54(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("DirectHit"))
			{
				return "";
			}
			return Data.Tags["DirectHit"].ToString();
		}

		internal string _003CUpdateACTTables_003Eb__14_55(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("DirectHit"))
			{
				return "";
			}
			return Data.Tags["DirectHit"].ToString();
		}

		internal int _003CUpdateACTTables_003Eb__14_56(MasterSwing Left, MasterSwing Right)
		{
			return string.Compare(Left.Tags.ContainsKey("DirectHit") ? Left.Tags["DirectHit"].ToString() : "", Right.Tags.ContainsKey("DirectHit") ? Right.Tags["DirectHit"].ToString() : "", StringComparison.OrdinalIgnoreCase);
		}

		internal string _003CUpdateACTTables_003Eb__14_57(AttackType Data)
		{
			return ((double)Data.DirectHitCount() * 100.0 / (double)OneOrInt(Data.Items.Count)).ToString("0.0'%", CultureInfo.InvariantCulture);
		}

		internal string _003CUpdateACTTables_003Eb__14_58(AttackType Data)
		{
			return ((double)Data.DirectHitCount() * 100.0 / (double)OneOrInt(Data.Items.Count)).ToString("0.0'%", CultureInfo.InvariantCulture);
		}

		internal int _003CUpdateACTTables_003Eb__14_59(AttackType Left, AttackType Right)
		{
			return Left.DirectHitCount().CompareTo(Right.DirectHitCount());
		}

		internal string _003CUpdateACTTables_003Eb__14_60(DamageTypeData Data)
		{
			if (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText))
			{
				return 0.ToString("0.0'%", CultureInfo.InvariantCulture);
			}
			return Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("DirectHitPct");
		}

		internal string _003CUpdateACTTables_003Eb__14_61(DamageTypeData Data)
		{
			if (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText))
			{
				return 0.ToString("0.0'%", CultureInfo.InvariantCulture);
			}
			return Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("DirectHitPct");
		}

		internal string _003CUpdateACTTables_003Eb__14_62(CombatantData Data)
		{
			return Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitPct");
		}

		internal string _003CUpdateACTTables_003Eb__14_63(CombatantData Data)
		{
			return Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitPct");
		}

		internal int _003CUpdateACTTables_003Eb__14_64(CombatantData Left, CombatantData Right)
		{
			return string.Compare(Left.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitPct"), Right.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitPct"), StringComparison.OrdinalIgnoreCase);
		}

		internal string _003CUpdateACTTables_003Eb__14_65(CombatantData Data, string ExtraFormat)
		{
			return ((Data.GetColumnByName("DirectHitPct") == "") ? 0.0 : Convert.ToDouble(Data.GetColumnByName("DirectHitPct").Replace("%", ""), CultureInfo.InvariantCulture)).ToString("0'%", CultureInfo.InvariantCulture);
		}

		internal string _003CUpdateACTTables_003Eb__14_66(AttackType Data)
		{
			return Data.DirectHitCount().ToString(CultureInfo.InvariantCulture);
		}

		internal string _003CUpdateACTTables_003Eb__14_67(AttackType Data)
		{
			return Data.DirectHitCount().ToString(CultureInfo.InvariantCulture);
		}

		internal int _003CUpdateACTTables_003Eb__14_68(AttackType Left, AttackType Right)
		{
			return Left.DirectHitCount().CompareTo(Right.DirectHitCount());
		}

		internal string _003CUpdateACTTables_003Eb__14_69(DamageTypeData Data)
		{
			if (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText))
			{
				return "0";
			}
			return Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("DirectHitCount");
		}

		internal string _003CUpdateACTTables_003Eb__14_70(DamageTypeData Data)
		{
			if (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText))
			{
				return "0";
			}
			return Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("DirectHitCount");
		}

		internal string _003CUpdateACTTables_003Eb__14_71(CombatantData Data)
		{
			return Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitCount");
		}

		internal string _003CUpdateACTTables_003Eb__14_72(CombatantData Data)
		{
			return Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitCount");
		}

		internal int _003CUpdateACTTables_003Eb__14_73(CombatantData Left, CombatantData Right)
		{
			return string.Compare(Left.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitCount"), Right.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitCount"), StringComparison.OrdinalIgnoreCase);
		}

		internal string _003CUpdateACTTables_003Eb__14_74(CombatantData Data, string ExtraFormat)
		{
			return Data.GetColumnByName("DirectHitCount");
		}

		internal string _003CUpdateACTTables_003Eb__14_75(AttackType Data)
		{
			return Data.CritDirectHitCount().ToString(CultureInfo.InvariantCulture);
		}

		internal string _003CUpdateACTTables_003Eb__14_76(AttackType Data)
		{
			return Data.CritDirectHitCount().ToString(CultureInfo.InvariantCulture);
		}

		internal int _003CUpdateACTTables_003Eb__14_77(AttackType Left, AttackType Right)
		{
			return Left.CritDirectHitCount().CompareTo(Right.CritDirectHitCount());
		}

		internal string _003CUpdateACTTables_003Eb__14_78(DamageTypeData Data)
		{
			if (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText))
			{
				return "0";
			}
			return Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("CritDirectHitCount");
		}

		internal string _003CUpdateACTTables_003Eb__14_79(DamageTypeData Data)
		{
			if (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText))
			{
				return "0";
			}
			return Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("CritDirectHitCount");
		}

		internal string _003CUpdateACTTables_003Eb__14_80(CombatantData Data)
		{
			return Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitCount");
		}

		internal string _003CUpdateACTTables_003Eb__14_81(CombatantData Data)
		{
			return Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitCount");
		}

		internal int _003CUpdateACTTables_003Eb__14_82(CombatantData Left, CombatantData Right)
		{
			return string.Compare(Left.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitCount"), Right.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitCount"), StringComparison.OrdinalIgnoreCase);
		}

		internal string _003CUpdateACTTables_003Eb__14_83(CombatantData Data, string ExtraFormat)
		{
			return Data.GetColumnByName("CritDirectHitCount");
		}

		internal string _003CUpdateACTTables_003Eb__14_84(AttackType Data)
		{
			return ((double)Data.CritDirectHitCount() * 100.0 / (double)OneOrInt(Data.Items.Count)).ToString("0.0'%", CultureInfo.InvariantCulture);
		}

		internal string _003CUpdateACTTables_003Eb__14_85(AttackType Data)
		{
			return ((double)Data.CritDirectHitCount() * 100.0 / (double)OneOrInt(Data.Items.Count)).ToString("0.0'%", CultureInfo.InvariantCulture);
		}

		internal int _003CUpdateACTTables_003Eb__14_86(AttackType Left, AttackType Right)
		{
			return (Left.CritDirectHitCount() * 100 / OneOrInt(Left.Items.Count)).CompareTo(Right.CritDirectHitCount() * 100 / OneOrInt(Right.Items.Count));
		}

		internal string _003CUpdateACTTables_003Eb__14_87(DamageTypeData Data)
		{
			if (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText))
			{
				return 0.ToString("0.0'%", CultureInfo.InvariantCulture);
			}
			return Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("CritDirectHitPct");
		}

		internal string _003CUpdateACTTables_003Eb__14_88(DamageTypeData Data)
		{
			if (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText))
			{
				return 0.ToString("0.0'%", CultureInfo.InvariantCulture);
			}
			return Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("CritDirectHitPct");
		}

		internal string _003CUpdateACTTables_003Eb__14_89(CombatantData Data)
		{
			return Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitPct");
		}

		internal string _003CUpdateACTTables_003Eb__14_90(CombatantData Data)
		{
			return Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitPct");
		}

		internal int _003CUpdateACTTables_003Eb__14_91(CombatantData Left, CombatantData Right)
		{
			return string.Compare(Left.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitPct"), Right.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitPct"), StringComparison.OrdinalIgnoreCase);
		}

		internal string _003CUpdateACTTables_003Eb__14_92(CombatantData Data, string ExtraFormat)
		{
			return ((Data.GetColumnByName("CritDirectHitPct") == "") ? 0.0 : Convert.ToDouble(Data.GetColumnByName("CritDirectHitPct").Replace("%", ""), CultureInfo.InvariantCulture)).ToString("0'%", CultureInfo.InvariantCulture);
		}

		internal string _003CUpdateACTTables_003Eb__14_93(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("potency"))
			{
				return "0";
			}
			return ((double)Data.Tags["potency"]).ToString("0.00", CultureInfo.InvariantCulture);
		}

		internal string _003CUpdateACTTables_003Eb__14_94(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("potency"))
			{
				return "0";
			}
			return ((double)Data.Tags["potency"]).ToString("0.00", CultureInfo.InvariantCulture);
		}

		internal int _003CUpdateACTTables_003Eb__14_95(MasterSwing Left, MasterSwing Right)
		{
			return string.Compare(Left.Tags.ContainsKey("potency") ? Left.Tags["potency"].ToString() : "0", Right.Tags.ContainsKey("potency") ? Right.Tags["potency"].ToString() : "0", StringComparison.OrdinalIgnoreCase);
		}

		internal string _003CUpdateACTTables_003Eb__14_96(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("StatusEffects"))
			{
				return "";
			}
			return Data.Tags["StatusEffects"]?.ToString();
		}

		internal string _003CUpdateACTTables_003Eb__14_97(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("StatusEffects"))
			{
				return "";
			}
			return Data.Tags["StatusEffects"]?.ToString();
		}

		internal int _003CUpdateACTTables_003Eb__14_98(MasterSwing Left, MasterSwing Right)
		{
			return string.Compare(Left.Tags.ContainsKey("StatusEffects") ? Left.Tags["StatusEffects"].ToString() : "", Right.Tags.ContainsKey("StatusEffects") ? Right.Tags["StatusEffects"].ToString() : "", StringComparison.OrdinalIgnoreCase);
		}

		internal string _003CUpdateACTTables_003Eb__14_99(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("dotbase"))
			{
				return "";
			}
			return ((uint)Data.Tags["dotbase"]).ToString("0", CultureInfo.InvariantCulture);
		}

		internal string _003CUpdateACTTables_003Eb__14_100(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("dotbase"))
			{
				return "";
			}
			return ((uint)Data.Tags["dotbase"]).ToString("0", CultureInfo.InvariantCulture);
		}

		internal int _003CUpdateACTTables_003Eb__14_101(MasterSwing Left, MasterSwing Right)
		{
			return string.Compare(Left.Tags.ContainsKey("dotbase") ? Left.Tags["dotbase"].ToString() : "0", Right.Tags.ContainsKey("dotbase") ? Right.Tags["dotbase"].ToString() : "0", StringComparison.OrdinalIgnoreCase);
		}

		internal string _003CUpdateACTTables_003Eb__14_102(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("BuffByte1"))
			{
				return "";
			}
			return Data.Tags["BuffByte1"].ToString();
		}

		internal string _003CUpdateACTTables_003Eb__14_103(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("BuffByte1"))
			{
				return "";
			}
			return Data.Tags["BuffByte1"].ToString();
		}

		internal int _003CUpdateACTTables_003Eb__14_104(MasterSwing Left, MasterSwing Right)
		{
			return string.Compare(Left.Tags.ContainsKey("BuffByte1") ? Left.Tags["BuffByte1"].ToString() : "", Right.Tags.ContainsKey("BuffByte1") ? Right.Tags["BuffByte1"].ToString() : "", StringComparison.OrdinalIgnoreCase);
		}

		internal string _003CUpdateACTTables_003Eb__14_105(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("BuffByte2"))
			{
				return "";
			}
			return Data.Tags["BuffByte2"].ToString();
		}

		internal string _003CUpdateACTTables_003Eb__14_106(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("BuffByte2"))
			{
				return "";
			}
			return Data.Tags["BuffByte2"].ToString();
		}

		internal int _003CUpdateACTTables_003Eb__14_107(MasterSwing Left, MasterSwing Right)
		{
			return string.Compare(Left.Tags.ContainsKey("BuffByte2") ? Left.Tags["BuffByte2"].ToString() : "", Right.Tags.ContainsKey("BuffByte2") ? Right.Tags["BuffByte2"].ToString() : "", StringComparison.OrdinalIgnoreCase);
		}

		internal string _003CUpdateACTTables_003Eb__14_108(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("BuffByte3"))
			{
				return "";
			}
			return Data.Tags["BuffByte3"].ToString();
		}

		internal string _003CUpdateACTTables_003Eb__14_109(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("BuffByte3"))
			{
				return "";
			}
			return Data.Tags["BuffByte3"].ToString();
		}

		internal int _003CUpdateACTTables_003Eb__14_110(MasterSwing Left, MasterSwing Right)
		{
			return string.Compare(Left.Tags.ContainsKey("BuffByte3") ? Left.Tags["BuffByte3"].ToString() : "", Right.Tags.ContainsKey("BuffByte3") ? Right.Tags["BuffByte3"].ToString() : "", StringComparison.OrdinalIgnoreCase);
		}

		internal string _003CUpdateACTTables_003Eb__14_111(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("CritRate"))
			{
				return "";
			}
			return Data.Tags["CritRate"].ToString();
		}

		internal string _003CUpdateACTTables_003Eb__14_112(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("CritRate"))
			{
				return "";
			}
			return Data.Tags["CritRate"].ToString();
		}

		internal int _003CUpdateACTTables_003Eb__14_113(MasterSwing Left, MasterSwing Right)
		{
			return string.Compare(Left.Tags.ContainsKey("CritRate") ? Left.Tags["CritRate"].ToString() : "0", Right.Tags.ContainsKey("CritRate") ? Right.Tags["CritRate"].ToString() : "0", StringComparison.OrdinalIgnoreCase);
		}

		internal string _003CUpdateACTTables_003Eb__14_114(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("CritEffects"))
			{
				return "";
			}
			return Data.Tags["CritEffects"].ToString();
		}

		internal string _003CUpdateACTTables_003Eb__14_115(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("CritEffects"))
			{
				return "";
			}
			return Data.Tags["CritEffects"].ToString();
		}

		internal int _003CUpdateACTTables_003Eb__14_116(MasterSwing Left, MasterSwing Right)
		{
			return string.Compare(Left.Tags.ContainsKey("CritEffects") ? Left.Tags["CritEffects"].ToString() : "", Right.Tags.ContainsKey("CritEffects") ? Right.Tags["CritEffects"].ToString() : "", StringComparison.OrdinalIgnoreCase);
		}

		internal string _003CUpdateACTTables_003Eb__14_117(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("DHRate"))
			{
				return "";
			}
			return Data.Tags["DHRate"].ToString();
		}

		internal string _003CUpdateACTTables_003Eb__14_118(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("DHRate"))
			{
				return "";
			}
			return Data.Tags["DHRate"].ToString();
		}

		internal int _003CUpdateACTTables_003Eb__14_119(MasterSwing Left, MasterSwing Right)
		{
			return string.Compare(Left.Tags.ContainsKey("DHRate") ? Left.Tags["DHRate"].ToString() : "", Right.Tags.ContainsKey("DHRate") ? Right.Tags["DHRate"].ToString() : "", StringComparison.OrdinalIgnoreCase);
		}

		internal string _003CUpdateACTTables_003Eb__14_120(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("DHEffects"))
			{
				return "";
			}
			return Data.Tags["DHEffects"].ToString();
		}

		internal string _003CUpdateACTTables_003Eb__14_121(MasterSwing Data)
		{
			if (!Data.Tags.ContainsKey("DHEffects"))
			{
				return "";
			}
			return Data.Tags["DHEffects"].ToString();
		}

		internal int _003CUpdateACTTables_003Eb__14_122(MasterSwing Left, MasterSwing Right)
		{
			return string.Compare(Left.Tags.ContainsKey("DHEffects") ? Left.Tags["DHEffects"].ToString() : "", Right.Tags.ContainsKey("DHEffects") ? Right.Tags["DHEffects"].ToString() : "", StringComparison.OrdinalIgnoreCase);
		}

		internal bool _003CGenAttackTypeGraph_003Eb__17_0(MasterSwing x)
		{
			if (x.Tags.ContainsKey("potency") && float.TryParse(x.Tags["potency"].ToString(), out var result))
			{
				return result > 0f;
			}
			return false;
		}
	}

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
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected Obj, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected Obj, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected Obj, but got Unknown
		_pluginScreenSpace = pluginScreenSpace;
		cmdClear = ((Control)(object)ActGlobals.oFormActMain).FindControl<Button>("tc1\\tpMain\\pLeftView\\pTvBtns\\tableLayoutPanel11\\btnClear");
		if (cmdClear != null)
		{
			((Control)cmdClear).Click += _parseMediator.OnClear;
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
		_benchmarkRepository.Measure("ParseLogFileChanged", (Action)(() =>
		{
			_parseMediator.LogFileChanged(IsImport);
		}));
	}

	private void OFormActMain_BeforeLogLineRead(bool isImport, LogLineEventArgs logInfo)
	{
		(logInfo.logLine, logInfo.detectedType) = _benchmarkRepository.Measure<(string, int)>("ParseBeforeLogLineRead", (Func<(string, int)>)(() => ((string, int))_parseMediator.BeforeLogLineRead(isImport, logInfo.detectedTime, logInfo.logLine)));
	}

	private void LoadPluginSettingsControl()
	{
		((Control)_settingsPropertyPage).Dock = (DockStyle)5;
		((Control)_pluginScreenSpace).Controls.Add((Control)(object)_settingsPropertyPage);
		((Control)_pluginScreenSpace).Text = "FFXIV解析插件";
	}

	public void LoadACTSettings()
	{
		SettingsSerializer xmlSettings = _settingsPropertyPage.InitializeSettingsSerializer();
		_settingsPropertyPage.LoadSettings(xmlSettings);
	}

	public void UnloadControls()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected Obj, but got Unknown
		ActGlobals.oFormActMain.BeforeLogLineRead -= OFormActMain_BeforeLogLineRead;
		ActGlobals.oFormActMain.GetDateTimeFromLog = _oldDateTimeDelegate;
		_settingsPropertyPage?.SaveSettings();
		_settingsPropertyPage = null;
		if (cmdClear != null)
		{
			((Control)cmdClear).Click -= _parseMediator.OnClear;
			cmdClear = null;
		}
	}

	public static void UpdateACTTables(bool showDebug)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected Obj, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected Obj, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected Obj, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected Obj, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected Obj, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected Obj, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected Obj, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected Obj, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected Obj, but got Unknown
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected Obj, but got Unknown
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected Obj, but got Unknown
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Expected Obj, but got Unknown
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Expected Obj, but got Unknown
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Expected Obj, but got Unknown
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Expected Obj, but got Unknown
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Expected Obj, but got Unknown
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Expected Obj, but got Unknown
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Expected Obj, but got Unknown
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Expected Obj, but got Unknown
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Expected Obj, but got Unknown
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Expected Obj, but got Unknown
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Expected Obj, but got Unknown
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Expected Obj, but got Unknown
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Expected Obj, but got Unknown
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Expected Obj, but got Unknown
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Expected Obj, but got Unknown
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Expected Obj, but got Unknown
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0602: Expected Obj, but got Unknown
		//IL_0655: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Expected Obj, but got Unknown
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0655: Expected Obj, but got Unknown
		//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b2: Expected Obj, but got Unknown
		//IL_069d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Expected Obj, but got Unknown
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Expected Obj, but got Unknown
		//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Expected Obj, but got Unknown
		//IL_074e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0758: Expected Obj, but got Unknown
		//IL_0743: Unknown result type (might be due to invalid IL or missing references)
		//IL_0748: Unknown result type (might be due to invalid IL or missing references)
		//IL_074e: Expected Obj, but got Unknown
		//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ab: Expected Obj, but got Unknown
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_079b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a1: Expected Obj, but got Unknown
		//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fe: Expected Obj, but got Unknown
		//IL_07e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f4: Expected Obj, but got Unknown
		//IL_0840: Unknown result type (might be due to invalid IL or missing references)
		//IL_0845: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Expected Obj, but got Unknown
		//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08da: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e0: Expected Obj, but got Unknown
		//IL_085f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0864: Unknown result type (might be due to invalid IL or missing references)
		//IL_086a: Expected Obj, but got Unknown
		//IL_0971: Unknown result type (might be due to invalid IL or missing references)
		//IL_097b: Expected Obj, but got Unknown
		//IL_0966: Unknown result type (might be due to invalid IL or missing references)
		//IL_096b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0971: Expected Obj, but got Unknown
		//IL_08f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ff: Expected Obj, but got Unknown
		//IL_0889: Unknown result type (might be due to invalid IL or missing references)
		//IL_0893: Expected Obj, but got Unknown
		//IL_09bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c8: Expected Obj, but got Unknown
		//IL_091e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0928: Expected Obj, but got Unknown
		//IL_0a59: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a63: Expected Obj, but got Unknown
		//IL_0a4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a59: Expected Obj, but got Unknown
		//IL_09dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e7: Expected Obj, but got Unknown
		//IL_0aa5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aaa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab0: Expected Obj, but got Unknown
		//IL_0a06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a10: Expected Obj, but got Unknown
		//IL_0b41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4b: Expected Obj, but got Unknown
		//IL_0b36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b41: Expected Obj, but got Unknown
		//IL_0ac4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acf: Expected Obj, but got Unknown
		//IL_0b8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b92: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b98: Expected Obj, but got Unknown
		//IL_0aee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af8: Expected Obj, but got Unknown
		//IL_0c29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c33: Expected Obj, but got Unknown
		//IL_0c1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c29: Expected Obj, but got Unknown
		//IL_0bac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb7: Expected Obj, but got Unknown
		//IL_0c72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c7d: Expected Obj, but got Unknown
		//IL_0bd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be0: Expected Obj, but got Unknown
		//IL_0ce5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf0: Expected Obj, but got Unknown
		//IL_0c9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca6: Expected Obj, but got Unknown
		//IL_0c91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9c: Expected Obj, but got Unknown
		//IL_0d5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d66: Expected Obj, but got Unknown
		//IL_0d0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d19: Expected Obj, but got Unknown
		//IL_0d04: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d0f: Expected Obj, but got Unknown
		//IL_0df0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dfb: Expected Obj, but got Unknown
		//IL_0d7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d85: Expected Obj, but got Unknown
		//IL_0e85: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e90: Expected Obj, but got Unknown
		//IL_0e0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1a: Expected Obj, but got Unknown
		//IL_0da4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dae: Expected Obj, but got Unknown
		//IL_0f1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f25: Expected Obj, but got Unknown
		//IL_0ea4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eaf: Expected Obj, but got Unknown
		//IL_0e39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e43: Expected Obj, but got Unknown
		//IL_0faf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fba: Expected Obj, but got Unknown
		//IL_0f39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f44: Expected Obj, but got Unknown
		//IL_0ece: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed8: Expected Obj, but got Unknown
		//IL_104b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1055: Expected Obj, but got Unknown
		//IL_1040: Unknown result type (might be due to invalid IL or missing references)
		//IL_1045: Unknown result type (might be due to invalid IL or missing references)
		//IL_104b: Expected Obj, but got Unknown
		//IL_0fce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd9: Expected Obj, but got Unknown
		//IL_0f63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6d: Expected Obj, but got Unknown
		//IL_1094: Unknown result type (might be due to invalid IL or missing references)
		//IL_1099: Unknown result type (might be due to invalid IL or missing references)
		//IL_109f: Expected Obj, but got Unknown
		//IL_0ff8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1002: Expected Obj, but got Unknown
		//IL_110a: Unknown result type (might be due to invalid IL or missing references)
		//IL_110f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1115: Expected Obj, but got Unknown
		//IL_10be: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c8: Expected Obj, but got Unknown
		//IL_10b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_10be: Expected Obj, but got Unknown
		//IL_119f: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_11aa: Expected Obj, but got Unknown
		//IL_1129: Unknown result type (might be due to invalid IL or missing references)
		//IL_112e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1134: Expected Obj, but got Unknown
		//IL_1234: Unknown result type (might be due to invalid IL or missing references)
		//IL_1239: Unknown result type (might be due to invalid IL or missing references)
		//IL_123f: Expected Obj, but got Unknown
		//IL_11be: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c9: Expected Obj, but got Unknown
		//IL_1153: Unknown result type (might be due to invalid IL or missing references)
		//IL_115d: Expected Obj, but got Unknown
		//IL_12c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d4: Expected Obj, but got Unknown
		//IL_1253: Unknown result type (might be due to invalid IL or missing references)
		//IL_1258: Unknown result type (might be due to invalid IL or missing references)
		//IL_125e: Expected Obj, but got Unknown
		//IL_11e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f2: Expected Obj, but got Unknown
		//IL_135b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1360: Unknown result type (might be due to invalid IL or missing references)
		//IL_1366: Expected Obj, but got Unknown
		//IL_12e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f3: Expected Obj, but got Unknown
		//IL_127d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1287: Expected Obj, but got Unknown
		//IL_13d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_13d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_13dc: Expected Obj, but got Unknown
		//IL_1385: Unknown result type (might be due to invalid IL or missing references)
		//IL_138f: Expected Obj, but got Unknown
		//IL_137a: Unknown result type (might be due to invalid IL or missing references)
		//IL_137f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1385: Expected Obj, but got Unknown
		//IL_1312: Unknown result type (might be due to invalid IL or missing references)
		//IL_131c: Expected Obj, but got Unknown
		//IL_146d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1477: Expected Obj, but got Unknown
		//IL_1462: Unknown result type (might be due to invalid IL or missing references)
		//IL_1467: Unknown result type (might be due to invalid IL or missing references)
		//IL_146d: Expected Obj, but got Unknown
		//IL_13f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_13f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_13fb: Expected Obj, but got Unknown
		//IL_14b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_14be: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c4: Expected Obj, but got Unknown
		//IL_141a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1424: Expected Obj, but got Unknown
		//IL_154b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1550: Unknown result type (might be due to invalid IL or missing references)
		//IL_1556: Expected Obj, but got Unknown
		//IL_14d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_14dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_14e3: Expected Obj, but got Unknown
		//IL_15c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_15c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_15cc: Expected Obj, but got Unknown
		//IL_1575: Unknown result type (might be due to invalid IL or missing references)
		//IL_157f: Expected Obj, but got Unknown
		//IL_156a: Unknown result type (might be due to invalid IL or missing references)
		//IL_156f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1575: Expected Obj, but got Unknown
		//IL_1502: Unknown result type (might be due to invalid IL or missing references)
		//IL_150c: Expected Obj, but got Unknown
		//IL_165d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1667: Expected Obj, but got Unknown
		//IL_1652: Unknown result type (might be due to invalid IL or missing references)
		//IL_1657: Unknown result type (might be due to invalid IL or missing references)
		//IL_165d: Expected Obj, but got Unknown
		//IL_15e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_15e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_15eb: Expected Obj, but got Unknown
		//IL_16a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_16ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_16b4: Expected Obj, but got Unknown
		//IL_160a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1614: Expected Obj, but got Unknown
		//IL_173b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1740: Unknown result type (might be due to invalid IL or missing references)
		//IL_1746: Expected Obj, but got Unknown
		//IL_16c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_16cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_16d3: Expected Obj, but got Unknown
		//IL_17b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_17b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_17bc: Expected Obj, but got Unknown
		//IL_1765: Unknown result type (might be due to invalid IL or missing references)
		//IL_176f: Expected Obj, but got Unknown
		//IL_175a: Unknown result type (might be due to invalid IL or missing references)
		//IL_175f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1765: Expected Obj, but got Unknown
		//IL_16f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_16fc: Expected Obj, but got Unknown
		//IL_184d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1857: Expected Obj, but got Unknown
		//IL_1842: Unknown result type (might be due to invalid IL or missing references)
		//IL_1847: Unknown result type (might be due to invalid IL or missing references)
		//IL_184d: Expected Obj, but got Unknown
		//IL_17d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_17d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_17db: Expected Obj, but got Unknown
		//IL_1899: Unknown result type (might be due to invalid IL or missing references)
		//IL_189e: Unknown result type (might be due to invalid IL or missing references)
		//IL_18a4: Expected Obj, but got Unknown
		//IL_17fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1804: Expected Obj, but got Unknown
		//IL_192b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1930: Unknown result type (might be due to invalid IL or missing references)
		//IL_1936: Expected Obj, but got Unknown
		//IL_18b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_18bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_18c3: Expected Obj, but got Unknown
		//IL_19a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_19a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_19ac: Expected Obj, but got Unknown
		//IL_1955: Unknown result type (might be due to invalid IL or missing references)
		//IL_195f: Expected Obj, but got Unknown
		//IL_194a: Unknown result type (might be due to invalid IL or missing references)
		//IL_194f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1955: Expected Obj, but got Unknown
		//IL_18e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_18ec: Expected Obj, but got Unknown
		//IL_1a3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a47: Expected Obj, but got Unknown
		//IL_1a32: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a37: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a3d: Expected Obj, but got Unknown
		//IL_19c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_19c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_19cb: Expected Obj, but got Unknown
		//IL_19ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_19f4: Expected Obj, but got Unknown
		//IL_1a8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a94: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a9a: Expected Obj, but got Unknown
		//IL_1b24: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b29: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b2f: Expected Obj, but got Unknown
		//IL_1aae: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab9: Expected Obj, but got Unknown
		//IL_1bb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bbe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bc4: Expected Obj, but got Unknown
		//IL_1b43: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b48: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b4e: Expected Obj, but got Unknown
		//IL_1ad8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ae2: Expected Obj, but got Unknown
		//IL_1c4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c53: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c59: Expected Obj, but got Unknown
		//IL_1bd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bdd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1be3: Expected Obj, but got Unknown
		//IL_1b6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b77: Expected Obj, but got Unknown
		//IL_1ce3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ce8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cee: Expected Obj, but got Unknown
		//IL_1c6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c72: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c78: Expected Obj, but got Unknown
		//IL_1c02: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c0c: Expected Obj, but got Unknown
		//IL_1d78: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d83: Expected Obj, but got Unknown
		//IL_1d02: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d07: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d0d: Expected Obj, but got Unknown
		//IL_1c97: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ca1: Expected Obj, but got Unknown
		//IL_1e0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e12: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e18: Expected Obj, but got Unknown
		//IL_1d97: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1da2: Expected Obj, but got Unknown
		//IL_1d2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d36: Expected Obj, but got Unknown
		//IL_1ea2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ea7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ead: Expected Obj, but got Unknown
		//IL_1e2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e31: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e37: Expected Obj, but got Unknown
		//IL_1dc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dcb: Expected Obj, but got Unknown
		//IL_1f37: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f42: Expected Obj, but got Unknown
		//IL_1ec1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ec6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ecc: Expected Obj, but got Unknown
		//IL_1e56: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e60: Expected Obj, but got Unknown
		//IL_1fcc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fd1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fd7: Expected Obj, but got Unknown
		//IL_1f56: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f61: Expected Obj, but got Unknown
		//IL_1eeb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ef5: Expected Obj, but got Unknown
		//IL_1feb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ff0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ff6: Expected Obj, but got Unknown
		//IL_1f80: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f8a: Expected Obj, but got Unknown
		//IL_2015: Unknown result type (might be due to invalid IL or missing references)
		//IL_201f: Expected Obj, but got Unknown
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
			Dictionary<string, EncounterData.TextExportFormatter> exportVariables = EncounterData.ExportVariables;
			var val = _003C_003Ec._003C_003E9__14_0;
			if (val == null)
			{
				EncounterData.ExportStringDataCallback val2 = (EncounterData data, List<CombatantData> SelectiveAllies, string extra) => data.ZoneName;
				_003C_003Ec._003C_003E9__14_0 = val2;
				val = val2;
			}
			exportVariables.Add("CurrentZoneName", new EncounterData.TextExportFormatter("CurrentZoneName", "CurrentZoneName", "Current Zone Name", val));
		}
		if (!CombatantData.ExportVariables.ContainsKey("Last10DPS"))
		{
			Dictionary<string, CombatantData.TextExportFormatter> exportVariables2 = CombatantData.ExportVariables;
			var val3 = _003C_003Ec._003C_003E9__14_1;
			if (val3 == null)
			{
				CombatantData.ExportStringDataCallback val4 = (CombatantData Data, string ExtraFormat) => Data.LastNDPS(10).ToString("0", CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_1 = val4;
				val3 = val4;
			}
			exportVariables2.Add("Last10DPS", new CombatantData.TextExportFormatter("Last10DPS", "Last 10 Seconds DPS", "Average DPS for last 10 seconds.", val3));
		}
		if (!CombatantData.ExportVariables.ContainsKey("Last30DPS"))
		{
			Dictionary<string, CombatantData.TextExportFormatter> exportVariables3 = CombatantData.ExportVariables;
			var val5 = _003C_003Ec._003C_003E9__14_2;
			if (val5 == null)
			{
				CombatantData.ExportStringDataCallback val6 = (CombatantData Data, string ExtraFormat) => Data.LastNDPS(30).ToString("0", CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_2 = val6;
				val5 = val6;
			}
			exportVariables3.Add("Last30DPS", new CombatantData.TextExportFormatter("Last30DPS", "Last 30 Seconds DPS", "Average DPS for last 30 seconds.", val5));
		}
		if (!CombatantData.ExportVariables.ContainsKey("Last60DPS"))
		{
			Dictionary<string, CombatantData.TextExportFormatter> exportVariables4 = CombatantData.ExportVariables;
			var val7 = _003C_003Ec._003C_003E9__14_3;
			if (val7 == null)
			{
				CombatantData.ExportStringDataCallback val8 = (CombatantData Data, string ExtraFormat) => Data.LastNDPS(60).ToString("0", CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_3 = val8;
				val7 = val8;
			}
			exportVariables4.Add("Last60DPS", new CombatantData.TextExportFormatter("Last60DPS", "Last 60 Seconds DPS", "Average DPS for last 60 seconds.", val7));
		}
		if (!EncounterData.ExportVariables.ContainsKey("Last10DPS"))
		{
			Dictionary<string, EncounterData.TextExportFormatter> exportVariables5 = EncounterData.ExportVariables;
			var val9 = _003C_003Ec._003C_003E9__14_4;
			if (val9 == null)
			{
				EncounterData.ExportStringDataCallback val10 = (EncounterData Data, List<CombatantData> SelectiveAllies, string Extra) => Data.LastNDPS(SelectiveAllies, 10).ToString("0", CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_4 = val10;
				val9 = val10;
			}
			exportVariables5.Add("Last10DPS", new EncounterData.TextExportFormatter("Last10DPS", "Last 10 Seconds DPS", "Average DPS for last 10 seconds", val9));
		}
		if (!EncounterData.ExportVariables.ContainsKey("Last30DPS"))
		{
			Dictionary<string, EncounterData.TextExportFormatter> exportVariables6 = EncounterData.ExportVariables;
			var val11 = _003C_003Ec._003C_003E9__14_5;
			if (val11 == null)
			{
				EncounterData.ExportStringDataCallback val12 = (EncounterData Data, List<CombatantData> SelectiveAllies, string Extra) => Data.LastNDPS(SelectiveAllies, 30).ToString("0", CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_5 = val12;
				val11 = val12;
			}
			exportVariables6.Add("Last30DPS", new EncounterData.TextExportFormatter("Last30DPS", "Last 30 Seconds DPS", "Average DPS for last 30 seconds", val11));
		}
		if (!EncounterData.ExportVariables.ContainsKey("Last60DPS"))
		{
			Dictionary<string, EncounterData.TextExportFormatter> exportVariables7 = EncounterData.ExportVariables;
			var val13 = _003C_003Ec._003C_003E9__14_6;
			if (val13 == null)
			{
				EncounterData.ExportStringDataCallback val14 = (EncounterData Data, List<CombatantData> SelectiveAllies, string Extra) => Data.LastNDPS(SelectiveAllies, 60).ToString("0", CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_6 = val14;
				val13 = val14;
			}
			exportVariables7.Add("Last60DPS", new EncounterData.TextExportFormatter("Last60DPS", "Last 60 Seconds DPS", "Average DPS for last 60 seconds", val13));
		}
		if (!MasterSwing.ColumnDefs.ContainsKey("StatusDuration"))
		{
			Dictionary<string, MasterSwing.ColumnDef> columnDefs = MasterSwing.ColumnDefs;
			var val15 = _003C_003Ec._003C_003E9__14_7;
			if (val15 == null)
			{
				MasterSwing.StringDataCallback val16 = (MasterSwing Data) => (!Data.Tags.ContainsKey("StatusDuration")) ? "" : ((double)Data.Tags["StatusDuration"]).ToString("0.#", CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_7 = val16;
				val15 = val16;
			}
			var val17 = _003C_003Ec._003C_003E9__14_8;
			if (val17 == null)
			{
				MasterSwing.StringDataCallback val18 = (MasterSwing Data) => (!Data.Tags.ContainsKey("StatusDuration")) ? "" : ((double)Data.Tags["StatusDuration"]).ToString("0.#", CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_8 = val18;
				val17 = val18;
			}
			columnDefs.Add("StatusDuration", new MasterSwing.ColumnDef("StatusDuration", true, "VARCHAR(8)", "StatusDuration", val15, val17, (Comparison<MasterSwing>)((MasterSwing Left, MasterSwing Right) => (Left.Tags.ContainsKey("StatusDuration") ? ((double)Left.Tags["StatusDuration"]) : 0.0).CompareTo(Right.Tags.ContainsKey("StatusDuration") ? ((double)Right.Tags["StatusDuration"]) : 0.0))));
		}
		if (!CombatantData.ColumnDefs.ContainsKey("Job"))
		{
			Dictionary<string, CombatantData.ColumnDef> columnDefs2 = CombatantData.ColumnDefs;
			var val19 = _003C_003Ec._003C_003E9__14_10;
			if (val19 == null)
			{
				CombatantData.StringDataCallback val20 = (CombatantData Data) => Data.Job();
				_003C_003Ec._003C_003E9__14_10 = val20;
				val19 = val20;
			}
			var val21 = _003C_003Ec._003C_003E9__14_11;
			if (val21 == null)
			{
				CombatantData.StringDataCallback val22 = (CombatantData Data) => Data.Job();
				_003C_003Ec._003C_003E9__14_11 = val22;
				val21 = val22;
			}
			columnDefs2.Add("Job", new CombatantData.ColumnDef("Job", true, "VARCHAR(8)", "Job", val19, val21, (Comparison<CombatantData>)((CombatantData Left, CombatantData Right) => string.Compare(Left.Job(), Right.Job(), StringComparison.OrdinalIgnoreCase))));
		}
		if (!CombatantData.ExportVariables.ContainsKey("Job"))
		{
			Dictionary<string, CombatantData.TextExportFormatter> exportVariables8 = CombatantData.ExportVariables;
			var val23 = _003C_003Ec._003C_003E9__14_13;
			if (val23 == null)
			{
				CombatantData.ExportStringDataCallback val24 = (CombatantData Data, string ExtraFormat) => Data.GetColumnByName("Job");
				_003C_003Ec._003C_003E9__14_13 = val24;
				val23 = val24;
			}
			exportVariables8.Add("Job", new CombatantData.TextExportFormatter("Job", "Job Name", "Player's Job", val23));
		}
		if (!CombatantData.ColumnDefs.ContainsKey("ParryPct"))
		{
			Dictionary<string, CombatantData.ColumnDef> columnDefs3 = CombatantData.ColumnDefs;
			var val25 = _003C_003Ec._003C_003E9__14_14;
			if (val25 == null)
			{
				CombatantData.StringDataCallback val26 = (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ParryPct");
				_003C_003Ec._003C_003E9__14_14 = val26;
				val25 = val26;
			}
			var val27 = _003C_003Ec._003C_003E9__14_15;
			if (val27 == null)
			{
				CombatantData.StringDataCallback val28 = (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ParryPct");
				_003C_003Ec._003C_003E9__14_15 = val28;
				val27 = val28;
			}
			columnDefs3.Add("ParryPct", new CombatantData.ColumnDef("ParryPct", false, "VARCHAR(8)", "ParryPct", val25, val27, (Comparison<CombatantData>)((CombatantData Left, CombatantData Right) => string.Compare(Left.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ParryPct"), Right.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ParryPct"), StringComparison.OrdinalIgnoreCase))));
		}
		if (!CombatantData.ExportVariables.ContainsKey("ParryPct"))
		{
			Dictionary<string, CombatantData.TextExportFormatter> exportVariables9 = CombatantData.ExportVariables;
			var val29 = _003C_003Ec._003C_003E9__14_17;
			if (val29 == null)
			{
				CombatantData.ExportStringDataCallback val30 = (CombatantData Data, string ExtraFormat) => Data.GetColumnByName("ParryPct");
				_003C_003Ec._003C_003E9__14_17 = val30;
				val29 = val30;
			}
			exportVariables9.Add("ParryPct", new CombatantData.TextExportFormatter("ParryPct", "Parry Percent", "Percent of hits that were parried.", val29));
		}
		if (!CombatantData.ColumnDefs.ContainsKey("BlockPct"))
		{
			Dictionary<string, CombatantData.ColumnDef> columnDefs4 = CombatantData.ColumnDefs;
			var val31 = _003C_003Ec._003C_003E9__14_18;
			if (val31 == null)
			{
				CombatantData.StringDataCallback val32 = (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("BlockPct");
				_003C_003Ec._003C_003E9__14_18 = val32;
				val31 = val32;
			}
			var val33 = _003C_003Ec._003C_003E9__14_19;
			if (val33 == null)
			{
				CombatantData.StringDataCallback val34 = (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("BlockPct");
				_003C_003Ec._003C_003E9__14_19 = val34;
				val33 = val34;
			}
			columnDefs4.Add("BlockPct", new CombatantData.ColumnDef("BlockPct", false, "VARCHAR(8)", "BlockPct", val31, val33, (Comparison<CombatantData>)((CombatantData Left, CombatantData Right) => string.Compare(Left.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("BlockPct"), Right.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("BlockPct"), StringComparison.OrdinalIgnoreCase))));
		}
		if (!CombatantData.ExportVariables.ContainsKey("BlockPct"))
		{
			Dictionary<string, CombatantData.TextExportFormatter> exportVariables10 = CombatantData.ExportVariables;
			var val35 = _003C_003Ec._003C_003E9__14_21;
			if (val35 == null)
			{
				CombatantData.ExportStringDataCallback val36 = (CombatantData Data, string ExtraFormat) => Data.GetColumnByName("BlockPct");
				_003C_003Ec._003C_003E9__14_21 = val36;
				val35 = val36;
			}
			exportVariables10.Add("BlockPct", new CombatantData.TextExportFormatter("BlockPct", "Block Percent", "Percent of hits that were blocked.", val35));
		}
		if (!CombatantData.ColumnDefs.ContainsKey("IncToHit"))
		{
			Dictionary<string, CombatantData.ColumnDef> columnDefs5 = CombatantData.ColumnDefs;
			var val37 = _003C_003Ec._003C_003E9__14_22;
			if (val37 == null)
			{
				CombatantData.StringDataCallback val38 = (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ToHit");
				_003C_003Ec._003C_003E9__14_22 = val38;
				val37 = val38;
			}
			var val39 = _003C_003Ec._003C_003E9__14_23;
			if (val39 == null)
			{
				CombatantData.StringDataCallback val40 = (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ToHit");
				_003C_003Ec._003C_003E9__14_23 = val40;
				val39 = val40;
			}
			columnDefs5.Add("IncToHit", new CombatantData.ColumnDef("IncToHit", false, "VARCHAR(8)", "IncToHit", val37, val39, (Comparison<CombatantData>)((CombatantData Left, CombatantData Right) => string.Compare(Left.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ToHit"), Right.Items[CombatantData.DamageTypeDataIncomingDamage].GetColumnByName("ToHit"), StringComparison.OrdinalIgnoreCase))));
		}
		if (!CombatantData.ExportVariables.ContainsKey("IncToHit"))
		{
			Dictionary<string, CombatantData.TextExportFormatter> exportVariables11 = CombatantData.ExportVariables;
			var val41 = _003C_003Ec._003C_003E9__14_25;
			if (val41 == null)
			{
				CombatantData.ExportStringDataCallback val42 = (CombatantData Data, string ExtraFormat) => Data.GetColumnByName("IncToHit");
				_003C_003Ec._003C_003E9__14_25 = val42;
				val41 = val42;
			}
			exportVariables11.Add("IncToHit", new CombatantData.TextExportFormatter("IncToHit", "Incoming Hit Rate", "Incoming hits to the target.", val41));
		}
		if (!DamageTypeData.ColumnDefs.ContainsKey("ParryPct"))
		{
			Dictionary<string, DamageTypeData.ColumnDef> columnDefs6 = DamageTypeData.ColumnDefs;
			var val43 = _003C_003Ec._003C_003E9__14_26;
			if (val43 == null)
			{
				DamageTypeData.StringDataCallback val44 = (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? 0.ToString("0'%", CultureInfo.InvariantCulture) : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("ParryPct");
				_003C_003Ec._003C_003E9__14_26 = val44;
				val43 = val44;
			}
			var val45 = _003C_003Ec._003C_003E9__14_27;
			if (val45 == null)
			{
				DamageTypeData.StringDataCallback val46 = (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? 0.ToString("0'%", CultureInfo.InvariantCulture) : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("ParryPct");
				_003C_003Ec._003C_003E9__14_27 = val46;
				val45 = val46;
			}
			columnDefs6.Add("ParryPct", new DamageTypeData.ColumnDef("ParryPct", false, "VARCHAR(8)", "ParryPct", val43, val45));
		}
		if (!DamageTypeData.ColumnDefs.ContainsKey("BlockPct"))
		{
			Dictionary<string, DamageTypeData.ColumnDef> columnDefs7 = DamageTypeData.ColumnDefs;
			var val47 = _003C_003Ec._003C_003E9__14_28;
			if (val47 == null)
			{
				DamageTypeData.StringDataCallback val48 = (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? 0.ToString("0'%", CultureInfo.InvariantCulture) : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("BlockPct");
				_003C_003Ec._003C_003E9__14_28 = val48;
				val47 = val48;
			}
			var val49 = _003C_003Ec._003C_003E9__14_29;
			if (val49 == null)
			{
				DamageTypeData.StringDataCallback val50 = (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? 0.ToString("0'%", CultureInfo.InvariantCulture) : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("BlockPct");
				_003C_003Ec._003C_003E9__14_29 = val50;
				val49 = val50;
			}
			columnDefs7.Add("BlockPct", new DamageTypeData.ColumnDef("BlockPct", false, "VARCHAR(8)", "BlockPct", val47, val49));
		}
		if (!AttackType.ColumnDefs.ContainsKey("Parry"))
		{
			Dictionary<string, AttackType.ColumnDef> columnDefs8 = AttackType.ColumnDefs;
			var val51 = _003C_003Ec._003C_003E9__14_30;
			if (val51 == null)
			{
				AttackType.StringDataCallback val52 = (AttackType Data) => Data.Parry().ToString(CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_30 = val52;
				val51 = val52;
			}
			var val53 = _003C_003Ec._003C_003E9__14_31;
			if (val53 == null)
			{
				AttackType.StringDataCallback val54 = (AttackType Data) => Data.Parry().ToString(CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_31 = val54;
				val53 = val54;
			}
			columnDefs8.Add("Parry", new AttackType.ColumnDef("Parry", false, "INT", "Parry", val51, val53, (Comparison<AttackType>)((AttackType Left, AttackType Right) => Left.Parry().CompareTo(Right.Parry()))));
		}
		if (!AttackType.ColumnDefs.ContainsKey("ParryPct"))
		{
			Dictionary<string, AttackType.ColumnDef> columnDefs9 = AttackType.ColumnDefs;
			var val55 = _003C_003Ec._003C_003E9__14_33;
			if (val55 == null)
			{
				AttackType.StringDataCallback val56 = (AttackType Data) => ((double)Data.Parry() * 100.0 / (double)OneOrInt(Data.BlockParryCount())).ToString("0'%", CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_33 = val56;
				val55 = val56;
			}
			var val57 = _003C_003Ec._003C_003E9__14_34;
			if (val57 == null)
			{
				AttackType.StringDataCallback val58 = (AttackType Data) => ((double)Data.Parry() * 100.0 / (double)OneOrInt(Data.BlockParryCount())).ToString("0'%", CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_34 = val58;
				val57 = val58;
			}
			columnDefs9.Add("ParryPct", new AttackType.ColumnDef("ParryPct", false, "VARCHAR(8)", "ParryPct", val55, val57, (Comparison<AttackType>)((AttackType Left, AttackType Right) => Left.Parry().CompareTo(Right.Parry()))));
		}
		if (!AttackType.ColumnDefs.ContainsKey("Block"))
		{
			Dictionary<string, AttackType.ColumnDef> columnDefs10 = AttackType.ColumnDefs;
			var val59 = _003C_003Ec._003C_003E9__14_36;
			if (val59 == null)
			{
				AttackType.StringDataCallback val60 = (AttackType Data) => Data.Block().ToString(CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_36 = val60;
				val59 = val60;
			}
			var val61 = _003C_003Ec._003C_003E9__14_37;
			if (val61 == null)
			{
				AttackType.StringDataCallback val62 = (AttackType Data) => Data.Block().ToString(CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_37 = val62;
				val61 = val62;
			}
			columnDefs10.Add("Block", new AttackType.ColumnDef("Block", false, "INT", "Block", val59, val61, (Comparison<AttackType>)((AttackType Left, AttackType Right) => Left.Block().CompareTo(Right.Block()))));
		}
		if (!AttackType.ColumnDefs.ContainsKey("BlockPct"))
		{
			Dictionary<string, AttackType.ColumnDef> columnDefs11 = AttackType.ColumnDefs;
			var val63 = _003C_003Ec._003C_003E9__14_39;
			if (val63 == null)
			{
				AttackType.StringDataCallback val64 = (AttackType Data) => ((double)Data.Block() * 100.0 / (double)OneOrInt(Data.BlockParryCount())).ToString("0'%", CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_39 = val64;
				val63 = val64;
			}
			var val65 = _003C_003Ec._003C_003E9__14_40;
			if (val65 == null)
			{
				AttackType.StringDataCallback val66 = (AttackType Data) => ((double)Data.Block() * 100.0 / (double)OneOrInt(Data.BlockParryCount())).ToString("0'%", CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_40 = val66;
				val65 = val66;
			}
			columnDefs11.Add("BlockPct", new AttackType.ColumnDef("BlockPct", false, "VARCHAR(8)", "BlockPct", val63, val65, (Comparison<AttackType>)((AttackType Left, AttackType Right) => Left.Block().CompareTo(Right.Block()))));
		}
		if (!CombatantData.ColumnDefs.ContainsKey("OverHealPct"))
		{
			Dictionary<string, CombatantData.ColumnDef> columnDefs12 = CombatantData.ColumnDefs;
			var val67 = _003C_003Ec._003C_003E9__14_42;
			if (val67 == null)
			{
				CombatantData.StringDataCallback val68 = (CombatantData Data) => (long.Parse(Data.Items[CombatantData.DamageTypeDataOutgoingHealing].GetColumnByName("OverHeal"), CultureInfo.InvariantCulture) * 100 / OneOrInt((!Data.Items[CombatantData.DamageTypeDataOutgoingHealing].Items.ContainsKey("All")) ? 0 : Data.DirectHeal())).ToString("0'%", CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_42 = val68;
				val67 = val68;
			}
			var val69 = _003C_003Ec._003C_003E9__14_43;
			if (val69 == null)
			{
				CombatantData.StringDataCallback val70 = (CombatantData Data) => (long.Parse(Data.Items[CombatantData.DamageTypeDataOutgoingHealing].GetColumnByName("OverHeal"), CultureInfo.InvariantCulture) * 100 / OneOrInt((!Data.Items[CombatantData.DamageTypeDataOutgoingHealing].Items.ContainsKey("All")) ? 0 : Data.DirectHeal())).ToString("0'%", CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_43 = val70;
				val69 = val70;
			}
			columnDefs12.Add("OverHealPct", new CombatantData.ColumnDef("OverHealPct", true, "VARCHAR(8)", "OverHealPct", val67, val69, (Comparison<CombatantData>)((CombatantData Left, CombatantData Right) => long.Parse(Left.GetColumnByName("OverHealPct").Replace('%', ' '), CultureInfo.InvariantCulture).CompareTo(long.Parse(Right.GetColumnByName("OverHealPct").Replace('%', ' '), CultureInfo.InvariantCulture)))));
		}
		if (!CombatantData.ExportVariables.ContainsKey("OverHealPct"))
		{
			Dictionary<string, CombatantData.TextExportFormatter> exportVariables12 = CombatantData.ExportVariables;
			var val71 = _003C_003Ec._003C_003E9__14_45;
			if (val71 == null)
			{
				CombatantData.ExportStringDataCallback val72 = (CombatantData Data, string ExtraFormat) => Data.GetColumnByName("OverHealPct");
				_003C_003Ec._003C_003E9__14_45 = val72;
				val71 = val72;
			}
			exportVariables12.Add("OverHealPct", new CombatantData.TextExportFormatter("OverHealPct", "Over-Heal Percent", "Percent of heals above target's Max HP", val71));
		}
		if (!DamageTypeData.ColumnDefs.ContainsKey("OverHeal"))
		{
			Dictionary<string, DamageTypeData.ColumnDef> columnDefs13 = DamageTypeData.ColumnDefs;
			var val73 = _003C_003Ec._003C_003E9__14_46;
			if (val73 == null)
			{
				DamageTypeData.StringDataCallback val74 = (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? "0" : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("OverHeal");
				_003C_003Ec._003C_003E9__14_46 = val74;
				val73 = val74;
			}
			var val75 = _003C_003Ec._003C_003E9__14_47;
			if (val75 == null)
			{
				DamageTypeData.StringDataCallback val76 = (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? "0" : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("OverHeal");
				_003C_003Ec._003C_003E9__14_47 = val76;
				val75 = val76;
			}
			columnDefs13.Add("OverHeal", new DamageTypeData.ColumnDef("OverHeal", false, "INT", "OverHeal", val73, val75));
		}
		if (!AttackType.ColumnDefs.ContainsKey("OverHeal"))
		{
			Dictionary<string, AttackType.ColumnDef> columnDefs14 = AttackType.ColumnDefs;
			var val77 = _003C_003Ec._003C_003E9__14_48;
			if (val77 == null)
			{
				AttackType.StringDataCallback val78 = (AttackType Data) => Data.Overheal().ToString(CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_48 = val78;
				val77 = val78;
			}
			var val79 = _003C_003Ec._003C_003E9__14_49;
			if (val79 == null)
			{
				AttackType.StringDataCallback val80 = (AttackType Data) => Data.Overheal().ToString(CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_49 = val80;
				val79 = val80;
			}
			columnDefs14.Add("OverHeal", new AttackType.ColumnDef("OverHeal", false, "INT", "OverHeal", val77, val79, (Comparison<AttackType>)((AttackType Left, AttackType Right) => Left.Overheal().CompareTo(Right.Overheal()))));
		}
		if (!MasterSwing.ColumnDefs.ContainsKey("OverHeal"))
		{
			Dictionary<string, MasterSwing.ColumnDef> columnDefs15 = MasterSwing.ColumnDefs;
			var val81 = _003C_003Ec._003C_003E9__14_51;
			if (val81 == null)
			{
				MasterSwing.StringDataCallback val82 = (MasterSwing Data) => (!Data.Tags.ContainsKey("overheal")) ? "0" : Data.Tags["overheal"].ToString();
				_003C_003Ec._003C_003E9__14_51 = val82;
				val81 = val82;
			}
			var val83 = _003C_003Ec._003C_003E9__14_52;
			if (val83 == null)
			{
				MasterSwing.StringDataCallback val84 = (MasterSwing Data) => (!Data.Tags.ContainsKey("overheal")) ? "0" : Data.Tags["overheal"].ToString();
				_003C_003Ec._003C_003E9__14_52 = val84;
				val83 = val84;
			}
			columnDefs15.Add("OverHeal", new MasterSwing.ColumnDef("OverHeal", false, "INT", "OverHeal", val81, val83, (Comparison<MasterSwing>)((MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("overheal") ? Left.Tags["overheal"].ToString() : "0", Right.Tags.ContainsKey("overheal") ? Right.Tags["overheal"].ToString() : "0", StringComparison.OrdinalIgnoreCase))));
		}
		if (!MasterSwing.ColumnDefs.ContainsKey("DirectHit"))
		{
			Dictionary<string, MasterSwing.ColumnDef> columnDefs16 = MasterSwing.ColumnDefs;
			var val85 = _003C_003Ec._003C_003E9__14_54;
			if (val85 == null)
			{
				MasterSwing.StringDataCallback val86 = (MasterSwing Data) => (!Data.Tags.ContainsKey("DirectHit")) ? "" : Data.Tags["DirectHit"].ToString();
				_003C_003Ec._003C_003E9__14_54 = val86;
				val85 = val86;
			}
			var val87 = _003C_003Ec._003C_003E9__14_55;
			if (val87 == null)
			{
				MasterSwing.StringDataCallback val88 = (MasterSwing Data) => (!Data.Tags.ContainsKey("DirectHit")) ? "" : Data.Tags["DirectHit"].ToString();
				_003C_003Ec._003C_003E9__14_55 = val88;
				val87 = val88;
			}
			columnDefs16.Add("DirectHit", new MasterSwing.ColumnDef("DirectHit", true, "BOOL", "DirectHit", val85, val87, (Comparison<MasterSwing>)((MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("DirectHit") ? Left.Tags["DirectHit"].ToString() : "", Right.Tags.ContainsKey("DirectHit") ? Right.Tags["DirectHit"].ToString() : "", StringComparison.OrdinalIgnoreCase))));
		}
		if (!AttackType.ColumnDefs.ContainsKey("DirectHitPct"))
		{
			Dictionary<string, AttackType.ColumnDef> columnDefs17 = AttackType.ColumnDefs;
			var val89 = _003C_003Ec._003C_003E9__14_57;
			if (val89 == null)
			{
				AttackType.StringDataCallback val90 = (AttackType Data) => ((double)Data.DirectHitCount() * 100.0 / (double)OneOrInt(Data.Items.Count)).ToString("0.0'%", CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_57 = val90;
				val89 = val90;
			}
			var val91 = _003C_003Ec._003C_003E9__14_58;
			if (val91 == null)
			{
				AttackType.StringDataCallback val92 = (AttackType Data) => ((double)Data.DirectHitCount() * 100.0 / (double)OneOrInt(Data.Items.Count)).ToString("0.0'%", CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_58 = val92;
				val91 = val92;
			}
			columnDefs17.Add("DirectHitPct", new AttackType.ColumnDef("DirectHitPct", true, "VARCHAR(8)", "DirectHitPct", val89, val91, (Comparison<AttackType>)((AttackType Left, AttackType Right) => Left.DirectHitCount().CompareTo(Right.DirectHitCount()))));
		}
		if (!DamageTypeData.ColumnDefs.ContainsKey("DirectHitPct"))
		{
			Dictionary<string, DamageTypeData.ColumnDef> columnDefs18 = DamageTypeData.ColumnDefs;
			var val93 = _003C_003Ec._003C_003E9__14_60;
			if (val93 == null)
			{
				DamageTypeData.StringDataCallback val94 = (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? 0.ToString("0.0'%", CultureInfo.InvariantCulture) : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("DirectHitPct");
				_003C_003Ec._003C_003E9__14_60 = val94;
				val93 = val94;
			}
			var val95 = _003C_003Ec._003C_003E9__14_61;
			if (val95 == null)
			{
				DamageTypeData.StringDataCallback val96 = (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? 0.ToString("0.0'%", CultureInfo.InvariantCulture) : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("DirectHitPct");
				_003C_003Ec._003C_003E9__14_61 = val96;
				val95 = val96;
			}
			columnDefs18.Add("DirectHitPct", new DamageTypeData.ColumnDef("DirectHitPct", true, "VARCHAR(8)", "DirectHitPct", val93, val95));
		}
		if (!CombatantData.ColumnDefs.ContainsKey("DirectHitPct"))
		{
			Dictionary<string, CombatantData.ColumnDef> columnDefs19 = CombatantData.ColumnDefs;
			var val97 = _003C_003Ec._003C_003E9__14_62;
			if (val97 == null)
			{
				CombatantData.StringDataCallback val98 = (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitPct");
				_003C_003Ec._003C_003E9__14_62 = val98;
				val97 = val98;
			}
			var val99 = _003C_003Ec._003C_003E9__14_63;
			if (val99 == null)
			{
				CombatantData.StringDataCallback val100 = (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitPct");
				_003C_003Ec._003C_003E9__14_63 = val100;
				val99 = val100;
			}
			columnDefs19.Add("DirectHitPct", new CombatantData.ColumnDef("DirectHitPct", true, "VARCHAR(8)", "DirectHitPct", val97, val99, (Comparison<CombatantData>)((CombatantData Left, CombatantData Right) => string.Compare(Left.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitPct"), Right.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitPct"), StringComparison.OrdinalIgnoreCase))));
		}
		if (!CombatantData.ExportVariables.ContainsKey("DirectHitPct"))
		{
			Dictionary<string, CombatantData.TextExportFormatter> exportVariables13 = CombatantData.ExportVariables;
			var val101 = _003C_003Ec._003C_003E9__14_65;
			if (val101 == null)
			{
				CombatantData.ExportStringDataCallback val102 = (CombatantData Data, string ExtraFormat) => ((Data.GetColumnByName("DirectHitPct") == "") ? 0.0 : Convert.ToDouble(Data.GetColumnByName("DirectHitPct").Replace("%", ""), CultureInfo.InvariantCulture)).ToString("0'%", CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_65 = val102;
				val101 = val102;
			}
			exportVariables13.Add("DirectHitPct", new CombatantData.TextExportFormatter("DirectHitPct", "Direct Hit Percent", "Percent of hits that were Direct Hits.", val101));
		}
		if (!AttackType.ColumnDefs.ContainsKey("DirectHitCount"))
		{
			Dictionary<string, AttackType.ColumnDef> columnDefs20 = AttackType.ColumnDefs;
			var val103 = _003C_003Ec._003C_003E9__14_66;
			if (val103 == null)
			{
				AttackType.StringDataCallback val104 = (AttackType Data) => Data.DirectHitCount().ToString(CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_66 = val104;
				val103 = val104;
			}
			var val105 = _003C_003Ec._003C_003E9__14_67;
			if (val105 == null)
			{
				AttackType.StringDataCallback val106 = (AttackType Data) => Data.DirectHitCount().ToString(CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_67 = val106;
				val105 = val106;
			}
			columnDefs20.Add("DirectHitCount", new AttackType.ColumnDef("DirectHitCount", false, "INT", "DirectHitCount", val103, val105, (Comparison<AttackType>)((AttackType Left, AttackType Right) => Left.DirectHitCount().CompareTo(Right.DirectHitCount()))));
		}
		if (!DamageTypeData.ColumnDefs.ContainsKey("DirectHitCount"))
		{
			Dictionary<string, DamageTypeData.ColumnDef> columnDefs21 = DamageTypeData.ColumnDefs;
			var val107 = _003C_003Ec._003C_003E9__14_69;
			if (val107 == null)
			{
				DamageTypeData.StringDataCallback val108 = (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? "0" : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("DirectHitCount");
				_003C_003Ec._003C_003E9__14_69 = val108;
				val107 = val108;
			}
			var val109 = _003C_003Ec._003C_003E9__14_70;
			if (val109 == null)
			{
				DamageTypeData.StringDataCallback val110 = (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? "0" : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("DirectHitCount");
				_003C_003Ec._003C_003E9__14_70 = val110;
				val109 = val110;
			}
			columnDefs21.Add("DirectHitCount", new DamageTypeData.ColumnDef("DirectHitCount", false, "INT", "DirectHitCount", val107, val109));
		}
		if (!CombatantData.ColumnDefs.ContainsKey("DirectHitCount"))
		{
			Dictionary<string, CombatantData.ColumnDef> columnDefs22 = CombatantData.ColumnDefs;
			var val111 = _003C_003Ec._003C_003E9__14_71;
			if (val111 == null)
			{
				CombatantData.StringDataCallback val112 = (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitCount");
				_003C_003Ec._003C_003E9__14_71 = val112;
				val111 = val112;
			}
			var val113 = _003C_003Ec._003C_003E9__14_72;
			if (val113 == null)
			{
				CombatantData.StringDataCallback val114 = (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitCount");
				_003C_003Ec._003C_003E9__14_72 = val114;
				val113 = val114;
			}
			columnDefs22.Add("DirectHitCount", new CombatantData.ColumnDef("DirectHitCount", false, "INT", "DirectHitCount", val111, val113, (Comparison<CombatantData>)((CombatantData Left, CombatantData Right) => string.Compare(Left.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitCount"), Right.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("DirectHitCount"), StringComparison.OrdinalIgnoreCase))));
		}
		if (!CombatantData.ExportVariables.ContainsKey("DirectHitCount"))
		{
			Dictionary<string, CombatantData.TextExportFormatter> exportVariables14 = CombatantData.ExportVariables;
			var val115 = _003C_003Ec._003C_003E9__14_74;
			if (val115 == null)
			{
				CombatantData.ExportStringDataCallback val116 = (CombatantData Data, string ExtraFormat) => Data.GetColumnByName("DirectHitCount");
				_003C_003Ec._003C_003E9__14_74 = val116;
				val115 = val116;
			}
			exportVariables14.Add("DirectHitCount", new CombatantData.TextExportFormatter("DirectHitCount", "Direct Hit Count", "Number of hits that were direct hit.", val115));
		}
		if (!AttackType.ColumnDefs.ContainsKey("CritDirectHitCount"))
		{
			Dictionary<string, AttackType.ColumnDef> columnDefs23 = AttackType.ColumnDefs;
			var val117 = _003C_003Ec._003C_003E9__14_75;
			if (val117 == null)
			{
				AttackType.StringDataCallback val118 = (AttackType Data) => Data.CritDirectHitCount().ToString(CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_75 = val118;
				val117 = val118;
			}
			var val119 = _003C_003Ec._003C_003E9__14_76;
			if (val119 == null)
			{
				AttackType.StringDataCallback val120 = (AttackType Data) => Data.CritDirectHitCount().ToString(CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_76 = val120;
				val119 = val120;
			}
			columnDefs23.Add("CritDirectHitCount", new AttackType.ColumnDef("CritDirectHitCount", false, "INT", "CritDirectHitCount", val117, val119, (Comparison<AttackType>)((AttackType Left, AttackType Right) => Left.CritDirectHitCount().CompareTo(Right.CritDirectHitCount()))));
		}
		if (!DamageTypeData.ColumnDefs.ContainsKey("CritDirectHitCount"))
		{
			Dictionary<string, DamageTypeData.ColumnDef> columnDefs24 = DamageTypeData.ColumnDefs;
			var val121 = _003C_003Ec._003C_003E9__14_78;
			if (val121 == null)
			{
				DamageTypeData.StringDataCallback val122 = (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? "0" : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("CritDirectHitCount");
				_003C_003Ec._003C_003E9__14_78 = val122;
				val121 = val122;
			}
			var val123 = _003C_003Ec._003C_003E9__14_79;
			if (val123 == null)
			{
				DamageTypeData.StringDataCallback val124 = (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? "0" : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("CritDirectHitCount");
				_003C_003Ec._003C_003E9__14_79 = val124;
				val123 = val124;
			}
			columnDefs24.Add("CritDirectHitCount", new DamageTypeData.ColumnDef("CritDirectHitCount", false, "INT", "CritDirectHitCount", val121, val123));
		}
		if (!CombatantData.ColumnDefs.ContainsKey("CritDirectHitCount"))
		{
			Dictionary<string, CombatantData.ColumnDef> columnDefs25 = CombatantData.ColumnDefs;
			var val125 = _003C_003Ec._003C_003E9__14_80;
			if (val125 == null)
			{
				CombatantData.StringDataCallback val126 = (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitCount");
				_003C_003Ec._003C_003E9__14_80 = val126;
				val125 = val126;
			}
			var val127 = _003C_003Ec._003C_003E9__14_81;
			if (val127 == null)
			{
				CombatantData.StringDataCallback val128 = (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitCount");
				_003C_003Ec._003C_003E9__14_81 = val128;
				val127 = val128;
			}
			columnDefs25.Add("CritDirectHitCount", new CombatantData.ColumnDef("CritDirectHitCount", false, "INT", "CritDirectHitCount", val125, val127, (Comparison<CombatantData>)((CombatantData Left, CombatantData Right) => string.Compare(Left.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitCount"), Right.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitCount"), StringComparison.OrdinalIgnoreCase))));
		}
		if (!CombatantData.ExportVariables.ContainsKey("CritDirectHitCount"))
		{
			Dictionary<string, CombatantData.TextExportFormatter> exportVariables15 = CombatantData.ExportVariables;
			var val129 = _003C_003Ec._003C_003E9__14_83;
			if (val129 == null)
			{
				CombatantData.ExportStringDataCallback val130 = (CombatantData Data, string ExtraFormat) => Data.GetColumnByName("CritDirectHitCount");
				_003C_003Ec._003C_003E9__14_83 = val130;
				val129 = val130;
			}
			exportVariables15.Add("CritDirectHitCount", new CombatantData.TextExportFormatter("CritDirectHitCount", "Crit Direct Hit Count", "Number of hits that were critical as well as direct hit.", val129));
		}
		if (!AttackType.ColumnDefs.ContainsKey("CritDirectHitPct"))
		{
			Dictionary<string, AttackType.ColumnDef> columnDefs26 = AttackType.ColumnDefs;
			var val131 = _003C_003Ec._003C_003E9__14_84;
			if (val131 == null)
			{
				AttackType.StringDataCallback val132 = (AttackType Data) => ((double)Data.CritDirectHitCount() * 100.0 / (double)OneOrInt(Data.Items.Count)).ToString("0.0'%", CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_84 = val132;
				val131 = val132;
			}
			var val133 = _003C_003Ec._003C_003E9__14_85;
			if (val133 == null)
			{
				AttackType.StringDataCallback val134 = (AttackType Data) => ((double)Data.CritDirectHitCount() * 100.0 / (double)OneOrInt(Data.Items.Count)).ToString("0.0'%", CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_85 = val134;
				val133 = val134;
			}
			columnDefs26.Add("CritDirectHitPct", new AttackType.ColumnDef("CritDirectHitPct", true, "VARCHAR(8)", "CritDirectHitPct", val131, val133, (Comparison<AttackType>)((AttackType Left, AttackType Right) => (Left.CritDirectHitCount() * 100 / OneOrInt(Left.Items.Count)).CompareTo(Right.CritDirectHitCount() * 100 / OneOrInt(Right.Items.Count)))));
		}
		if (!DamageTypeData.ColumnDefs.ContainsKey("CritDirectHitPct"))
		{
			Dictionary<string, DamageTypeData.ColumnDef> columnDefs27 = DamageTypeData.ColumnDefs;
			var val135 = _003C_003Ec._003C_003E9__14_87;
			if (val135 == null)
			{
				DamageTypeData.StringDataCallback val136 = (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? 0.ToString("0.0'%", CultureInfo.InvariantCulture) : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("CritDirectHitPct");
				_003C_003Ec._003C_003E9__14_87 = val136;
				val135 = val136;
			}
			var val137 = _003C_003Ec._003C_003E9__14_88;
			if (val137 == null)
			{
				DamageTypeData.StringDataCallback val138 = (DamageTypeData Data) => (!Data.Items.ContainsKey(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText)) ? 0.ToString("0.0'%", CultureInfo.InvariantCulture) : Data.Items[ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText].GetColumnByName("CritDirectHitPct");
				_003C_003Ec._003C_003E9__14_88 = val138;
				val137 = val138;
			}
			columnDefs27.Add("CritDirectHitPct", new DamageTypeData.ColumnDef("CritDirectHitPct", true, "VARCHAR(8)", "CritDirectHitPct", val135, val137));
		}
		if (!CombatantData.ColumnDefs.ContainsKey("CritDirectHitPct"))
		{
			Dictionary<string, CombatantData.ColumnDef> columnDefs28 = CombatantData.ColumnDefs;
			var val139 = _003C_003Ec._003C_003E9__14_89;
			if (val139 == null)
			{
				CombatantData.StringDataCallback val140 = (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitPct");
				_003C_003Ec._003C_003E9__14_89 = val140;
				val139 = val140;
			}
			var val141 = _003C_003Ec._003C_003E9__14_90;
			if (val141 == null)
			{
				CombatantData.StringDataCallback val142 = (CombatantData Data) => Data.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitPct");
				_003C_003Ec._003C_003E9__14_90 = val142;
				val141 = val142;
			}
			columnDefs28.Add("CritDirectHitPct", new CombatantData.ColumnDef("CritDirectHitPct", true, "VARCHAR(8)", "CritDirectHitPct", val139, val141, (Comparison<CombatantData>)((CombatantData Left, CombatantData Right) => string.Compare(Left.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitPct"), Right.Items[CombatantData.DamageTypeDataOutgoingDamage].GetColumnByName("CritDirectHitPct"), StringComparison.OrdinalIgnoreCase))));
		}
		if (!CombatantData.ExportVariables.ContainsKey("CritDirectHitPct"))
		{
			Dictionary<string, CombatantData.TextExportFormatter> exportVariables16 = CombatantData.ExportVariables;
			var val143 = _003C_003Ec._003C_003E9__14_92;
			if (val143 == null)
			{
				CombatantData.ExportStringDataCallback val144 = (CombatantData Data, string ExtraFormat) => ((Data.GetColumnByName("CritDirectHitPct") == "") ? 0.0 : Convert.ToDouble(Data.GetColumnByName("CritDirectHitPct").Replace("%", ""), CultureInfo.InvariantCulture)).ToString("0'%", CultureInfo.InvariantCulture);
				_003C_003Ec._003C_003E9__14_92 = val144;
				val143 = val144;
			}
			exportVariables16.Add("CritDirectHitPct", new CombatantData.TextExportFormatter("CritDirectHitPct", "Crit Direct Hit Percent", "Percent of hits that were Direct Hits as well as Critical Hits.", val143));
		}
		if (showDebug)
		{
			if (!MasterSwing.ColumnDefs.ContainsKey("Potency"))
			{
				Dictionary<string, MasterSwing.ColumnDef> columnDefs29 = MasterSwing.ColumnDefs;
				var val145 = _003C_003Ec._003C_003E9__14_93;
				if (val145 == null)
				{
					MasterSwing.StringDataCallback val146 = (MasterSwing Data) => (!Data.Tags.ContainsKey("potency")) ? "0" : ((double)Data.Tags["potency"]).ToString("0.00", CultureInfo.InvariantCulture);
					_003C_003Ec._003C_003E9__14_93 = val146;
					val145 = val146;
				}
				var val147 = _003C_003Ec._003C_003E9__14_94;
				if (val147 == null)
				{
					MasterSwing.StringDataCallback val148 = (MasterSwing Data) => (!Data.Tags.ContainsKey("potency")) ? "0" : ((double)Data.Tags["potency"]).ToString("0.00", CultureInfo.InvariantCulture);
					_003C_003Ec._003C_003E9__14_94 = val148;
					val147 = val148;
				}
				columnDefs29.Add("Potency", new MasterSwing.ColumnDef("Potency", true, "FLOAT", "Potency", val145, val147, (Comparison<MasterSwing>)((MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("potency") ? Left.Tags["potency"].ToString() : "0", Right.Tags.ContainsKey("potency") ? Right.Tags["potency"].ToString() : "0", StringComparison.OrdinalIgnoreCase))));
			}
			if (!MasterSwing.ColumnDefs.ContainsKey("StatusEffects"))
			{
				Dictionary<string, MasterSwing.ColumnDef> columnDefs30 = MasterSwing.ColumnDefs;
				var val149 = _003C_003Ec._003C_003E9__14_96;
				if (val149 == null)
				{
					MasterSwing.StringDataCallback val150 = (MasterSwing Data) => (!Data.Tags.ContainsKey("StatusEffects")) ? "" : Data.Tags["StatusEffects"]?.ToString();
					_003C_003Ec._003C_003E9__14_96 = val150;
					val149 = val150;
				}
				var val151 = _003C_003Ec._003C_003E9__14_97;
				if (val151 == null)
				{
					MasterSwing.StringDataCallback val152 = (MasterSwing Data) => (!Data.Tags.ContainsKey("StatusEffects")) ? "" : Data.Tags["StatusEffects"]?.ToString();
					_003C_003Ec._003C_003E9__14_97 = val152;
					val151 = val152;
				}
				columnDefs30.Add("StatusEffects", new MasterSwing.ColumnDef("StatusEffects", true, "VARCHAR(50)", "StatusEffects", val149, val151, (Comparison<MasterSwing>)((MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("StatusEffects") ? Left.Tags["StatusEffects"].ToString() : "", Right.Tags.ContainsKey("StatusEffects") ? Right.Tags["StatusEffects"].ToString() : "", StringComparison.OrdinalIgnoreCase))));
			}
			if (!MasterSwing.ColumnDefs.ContainsKey("DoTBase"))
			{
				Dictionary<string, MasterSwing.ColumnDef> columnDefs31 = MasterSwing.ColumnDefs;
				var val153 = _003C_003Ec._003C_003E9__14_99;
				if (val153 == null)
				{
					MasterSwing.StringDataCallback val154 = (MasterSwing Data) => (!Data.Tags.ContainsKey("dotbase")) ? "" : ((uint)Data.Tags["dotbase"]).ToString("0", CultureInfo.InvariantCulture);
					_003C_003Ec._003C_003E9__14_99 = val154;
					val153 = val154;
				}
				var val155 = _003C_003Ec._003C_003E9__14_100;
				if (val155 == null)
				{
					MasterSwing.StringDataCallback val156 = (MasterSwing Data) => (!Data.Tags.ContainsKey("dotbase")) ? "" : ((uint)Data.Tags["dotbase"]).ToString("0", CultureInfo.InvariantCulture);
					_003C_003Ec._003C_003E9__14_100 = val156;
					val155 = val156;
				}
				columnDefs31.Add("DoTBase", new MasterSwing.ColumnDef("DoTBase", true, "INT", "DoTBase", val153, val155, (Comparison<MasterSwing>)((MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("dotbase") ? Left.Tags["dotbase"].ToString() : "0", Right.Tags.ContainsKey("dotbase") ? Right.Tags["dotbase"].ToString() : "0", StringComparison.OrdinalIgnoreCase))));
			}
			if (!MasterSwing.ColumnDefs.ContainsKey("BuffByte1"))
			{
				Dictionary<string, MasterSwing.ColumnDef> columnDefs32 = MasterSwing.ColumnDefs;
				var val157 = _003C_003Ec._003C_003E9__14_102;
				if (val157 == null)
				{
					MasterSwing.StringDataCallback val158 = (MasterSwing Data) => (!Data.Tags.ContainsKey("BuffByte1")) ? "" : Data.Tags["BuffByte1"].ToString();
					_003C_003Ec._003C_003E9__14_102 = val158;
					val157 = val158;
				}
				var val159 = _003C_003Ec._003C_003E9__14_103;
				if (val159 == null)
				{
					MasterSwing.StringDataCallback val160 = (MasterSwing Data) => (!Data.Tags.ContainsKey("BuffByte1")) ? "" : Data.Tags["BuffByte1"].ToString();
					_003C_003Ec._003C_003E9__14_103 = val160;
					val159 = val160;
				}
				columnDefs32.Add("BuffByte1", new MasterSwing.ColumnDef("BuffByte1", false, "int", "BuffByte1", val157, val159, (Comparison<MasterSwing>)((MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("BuffByte1") ? Left.Tags["BuffByte1"].ToString() : "", Right.Tags.ContainsKey("BuffByte1") ? Right.Tags["BuffByte1"].ToString() : "", StringComparison.OrdinalIgnoreCase))));
			}
			if (!MasterSwing.ColumnDefs.ContainsKey("BuffByte2"))
			{
				Dictionary<string, MasterSwing.ColumnDef> columnDefs33 = MasterSwing.ColumnDefs;
				var val161 = _003C_003Ec._003C_003E9__14_105;
				if (val161 == null)
				{
					MasterSwing.StringDataCallback val162 = (MasterSwing Data) => (!Data.Tags.ContainsKey("BuffByte2")) ? "" : Data.Tags["BuffByte2"].ToString();
					_003C_003Ec._003C_003E9__14_105 = val162;
					val161 = val162;
				}
				var val163 = _003C_003Ec._003C_003E9__14_106;
				if (val163 == null)
				{
					MasterSwing.StringDataCallback val164 = (MasterSwing Data) => (!Data.Tags.ContainsKey("BuffByte2")) ? "" : Data.Tags["BuffByte2"].ToString();
					_003C_003Ec._003C_003E9__14_106 = val164;
					val163 = val164;
				}
				columnDefs33.Add("BuffByte2", new MasterSwing.ColumnDef("BuffByte2", false, "int", "BuffByte2", val161, val163, (Comparison<MasterSwing>)((MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("BuffByte2") ? Left.Tags["BuffByte2"].ToString() : "", Right.Tags.ContainsKey("BuffByte2") ? Right.Tags["BuffByte2"].ToString() : "", StringComparison.OrdinalIgnoreCase))));
			}
			if (!MasterSwing.ColumnDefs.ContainsKey("BuffByte3"))
			{
				Dictionary<string, MasterSwing.ColumnDef> columnDefs34 = MasterSwing.ColumnDefs;
				var val165 = _003C_003Ec._003C_003E9__14_108;
				if (val165 == null)
				{
					MasterSwing.StringDataCallback val166 = (MasterSwing Data) => (!Data.Tags.ContainsKey("BuffByte3")) ? "" : Data.Tags["BuffByte3"].ToString();
					_003C_003Ec._003C_003E9__14_108 = val166;
					val165 = val166;
				}
				var val167 = _003C_003Ec._003C_003E9__14_109;
				if (val167 == null)
				{
					MasterSwing.StringDataCallback val168 = (MasterSwing Data) => (!Data.Tags.ContainsKey("BuffByte3")) ? "" : Data.Tags["BuffByte3"].ToString();
					_003C_003Ec._003C_003E9__14_109 = val168;
					val167 = val168;
				}
				columnDefs34.Add("BuffByte3", new MasterSwing.ColumnDef("BuffByte3", false, "int", "BuffByte3", val165, val167, (Comparison<MasterSwing>)((MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("BuffByte3") ? Left.Tags["BuffByte3"].ToString() : "", Right.Tags.ContainsKey("BuffByte3") ? Right.Tags["BuffByte3"].ToString() : "", StringComparison.OrdinalIgnoreCase))));
			}
			if (!MasterSwing.ColumnDefs.ContainsKey("CritRate"))
			{
				Dictionary<string, MasterSwing.ColumnDef> columnDefs35 = MasterSwing.ColumnDefs;
				var val169 = _003C_003Ec._003C_003E9__14_111;
				if (val169 == null)
				{
					MasterSwing.StringDataCallback val170 = (MasterSwing Data) => (!Data.Tags.ContainsKey("CritRate")) ? "" : Data.Tags["CritRate"].ToString();
					_003C_003Ec._003C_003E9__14_111 = val170;
					val169 = val170;
				}
				var val171 = _003C_003Ec._003C_003E9__14_112;
				if (val171 == null)
				{
					MasterSwing.StringDataCallback val172 = (MasterSwing Data) => (!Data.Tags.ContainsKey("CritRate")) ? "" : Data.Tags["CritRate"].ToString();
					_003C_003Ec._003C_003E9__14_112 = val172;
					val171 = val172;
				}
				columnDefs35.Add("CritRate", new MasterSwing.ColumnDef("CritRate", false, "VARCHAR(8)", "CritRate", val169, val171, (Comparison<MasterSwing>)((MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("CritRate") ? Left.Tags["CritRate"].ToString() : "0", Right.Tags.ContainsKey("CritRate") ? Right.Tags["CritRate"].ToString() : "0", StringComparison.OrdinalIgnoreCase))));
			}
			if (!MasterSwing.ColumnDefs.ContainsKey("CritEffects"))
			{
				Dictionary<string, MasterSwing.ColumnDef> columnDefs36 = MasterSwing.ColumnDefs;
				var val173 = _003C_003Ec._003C_003E9__14_114;
				if (val173 == null)
				{
					MasterSwing.StringDataCallback val174 = (MasterSwing Data) => (!Data.Tags.ContainsKey("CritEffects")) ? "" : Data.Tags["CritEffects"].ToString();
					_003C_003Ec._003C_003E9__14_114 = val174;
					val173 = val174;
				}
				var val175 = _003C_003Ec._003C_003E9__14_115;
				if (val175 == null)
				{
					MasterSwing.StringDataCallback val176 = (MasterSwing Data) => (!Data.Tags.ContainsKey("CritEffects")) ? "" : Data.Tags["CritEffects"].ToString();
					_003C_003Ec._003C_003E9__14_115 = val176;
					val175 = val176;
				}
				columnDefs36.Add("CritEffects", new MasterSwing.ColumnDef("CritEffects", false, "VARCHAR(8)", "CritEffects", val173, val175, (Comparison<MasterSwing>)((MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("CritEffects") ? Left.Tags["CritEffects"].ToString() : "", Right.Tags.ContainsKey("CritEffects") ? Right.Tags["CritEffects"].ToString() : "", StringComparison.OrdinalIgnoreCase))));
			}
			if (!MasterSwing.ColumnDefs.ContainsKey("DHRate"))
			{
				Dictionary<string, MasterSwing.ColumnDef> columnDefs37 = MasterSwing.ColumnDefs;
				var val177 = _003C_003Ec._003C_003E9__14_117;
				if (val177 == null)
				{
					MasterSwing.StringDataCallback val178 = (MasterSwing Data) => (!Data.Tags.ContainsKey("DHRate")) ? "" : Data.Tags["DHRate"].ToString();
					_003C_003Ec._003C_003E9__14_117 = val178;
					val177 = val178;
				}
				var val179 = _003C_003Ec._003C_003E9__14_118;
				if (val179 == null)
				{
					MasterSwing.StringDataCallback val180 = (MasterSwing Data) => (!Data.Tags.ContainsKey("DHRate")) ? "" : Data.Tags["DHRate"].ToString();
					_003C_003Ec._003C_003E9__14_118 = val180;
					val179 = val180;
				}
				columnDefs37.Add("DHRate", new MasterSwing.ColumnDef("DHRate", false, "VARCHAR(8)", "DHRate", val177, val179, (Comparison<MasterSwing>)((MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("DHRate") ? Left.Tags["DHRate"].ToString() : "", Right.Tags.ContainsKey("DHRate") ? Right.Tags["DHRate"].ToString() : "", StringComparison.OrdinalIgnoreCase))));
			}
			if (!MasterSwing.ColumnDefs.ContainsKey("DHEffects"))
			{
				Dictionary<string, MasterSwing.ColumnDef> columnDefs38 = MasterSwing.ColumnDefs;
				var val181 = _003C_003Ec._003C_003E9__14_120;
				if (val181 == null)
				{
					MasterSwing.StringDataCallback val182 = (MasterSwing Data) => (!Data.Tags.ContainsKey("DHEffects")) ? "" : Data.Tags["DHEffects"].ToString();
					_003C_003Ec._003C_003E9__14_120 = val182;
					val181 = val182;
				}
				var val183 = _003C_003Ec._003C_003E9__14_121;
				if (val183 == null)
				{
					MasterSwing.StringDataCallback val184 = (MasterSwing Data) => (!Data.Tags.ContainsKey("DHEffects")) ? "" : Data.Tags["DHEffects"].ToString();
					_003C_003Ec._003C_003E9__14_121 = val184;
					val183 = val184;
				}
				columnDefs38.Add("DHEffects", new MasterSwing.ColumnDef("DHEffects", false, "VARCHAR(8)", "DHEffects", val181, val183, (Comparison<MasterSwing>)((MasterSwing Left, MasterSwing Right) => string.Compare(Left.Tags.ContainsKey("DHEffects") ? Left.Tags["DHEffects"].ToString() : "", Right.Tags.ContainsKey("DHEffects") ? Right.Tags["DHEffects"].ToString() : "", StringComparison.OrdinalIgnoreCase))));
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
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected Obj, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected Obj, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected Obj, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected Obj, but got Unknown
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected Obj, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected Obj, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected Obj, but got Unknown
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected Obj, but got Unknown
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Expected Obj, but got Unknown
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Expected Obj, but got Unknown
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Expected Obj, but got Unknown
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Expected Obj, but got Unknown
		DateTime now = DateTime.Now;
		if (SizeX < 16 || SizeY < 16)
		{
			return new Bitmap(16, 16);
		}
		Bitmap val = new Bitmap(SizeX, SizeY);
		try
		{
			List<MasterSwing> list = AttackTypeSource.Items.Where((MasterSwing x) => x.Tags.ContainsKey("potency") && float.TryParse(x.Tags["potency"].ToString(), out var result) && result > 0f).ToList();
			try
			{
				list.Sort((IComparer<MasterSwing>?)new MasterSwing.DualComparison(Sorting, Sorting));
			}
			catch (Exception ex)
			{
				ActGlobals.oFormActMain.WriteExceptionLog(ex, string.Empty);
				val = GraphDrawMessage(ex.ToString(), 12f, val);
			}
			SolidBrush val2 = new SolidBrush(Color.LightGray);
			try
			{
				SolidBrush val3 = new SolidBrush(Color.Black);
				try
				{
					Pen val4 = new Pen(Color.Black);
					try
					{
						Pen val5 = new Pen(Color.DarkGray);
						try
						{
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
							Graphics val6 = Graphics.FromImage((Image)(object)val);
							val6.SmoothingMode = (SmoothingMode)4;
							val6.Clear(val2.Color);
							int num = 16;
							Rectangle rectangle = new Rectangle(4, 4, ((Image)val).Width - 1 - num * 4, ((Image)val).Height - 1 - num * 2);
							val6.DrawRectangle(val4, rectangle);
							float num2 = rectangle.Bottom;
							float num3;
							try
							{
								num3 = (float)rectangle.Width / (float)list.Count;
							}
							catch (Exception ex2)
							{
								return GraphDrawMessage(ex2.ToString(), 12f, val);
							}
							float num4 = rectangle.Left;
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
							float num8 = (float)rectangle.Height / (float)num7;
							_ = 1f / num8;
							Font val7 = new Font("Arial", 8f);
							val6.DrawString("0", val7, (Brush)(object)val3, (float)(rectangle.Right + 5), (float)rectangle.Bottom);
							val6.DrawString(Convert.ToInt32(num7).ToString(), val7, (Brush)(object)val3, (float)(rectangle.Right + 5), (float)rectangle.Top);
							try
							{
								int num9 = Convert.ToInt32(num2 - (float)Convert.ToInt32((float)(num7 / 4) * num8));
								val6.DrawLine(val5, rectangle.Left, num9, rectangle.Right, num9);
								val6.DrawString((num7 / 4).ToString(), val7, (Brush)(object)val3, (float)(rectangle.Right + 5), (float)num9);
								num9 = Convert.ToInt32(num2 - (float)Convert.ToInt32((float)(num7 / 4 * 2) * num8));
								val6.DrawLine(val5, rectangle.Left, num9, rectangle.Right, num9);
								val6.DrawString((num7 / 4 * 2).ToString(), val7, (Brush)(object)val3, (float)(rectangle.Right + 5), (float)num9);
								num9 = Convert.ToInt32(num2 - (float)Convert.ToInt32((float)(num7 / 4 * 3) * num8));
								val6.DrawLine(val5, rectangle.Left, num9, rectangle.Right, num9);
								val6.DrawString((num7 / 4 * 3).ToString(), val7, (Brush)(object)val3, (float)(rectangle.Right + 5), (float)num9);
							}
							catch
							{
							}
							for (int num10 = 0; num10 < list.Count; num10++)
							{
								MasterSwing val8 = list[num10];
								float num11 = num4;
								float num12 = num2 - (float)Math.Round(float.Parse(val8.Tags["potency"].ToString(), CultureInfo.InvariantCulture), 2) * num8;
								float num13 = num3;
								float num14 = (float)Math.Round(float.Parse(val8.Tags["potency"].ToString(), CultureInfo.InvariantCulture), 2) * num8;
								if (num10 > 0 && val8.Time != list[num10 - 1].Time)
								{
									val6.DrawLine(val5, num4, (float)rectangle.Top, num4, (float)rectangle.Bottom);
								}
								if (val8.Critical)
								{
									val6.FillRectangle((Brush)(object)dictionary2[val8.SwingType], num11, num12, num13, num14);
								}
								else
								{
									val6.FillRectangle((Brush)(object)dictionary[val8.SwingType], num11, num12, num13, num14);
								}
								val6.DrawRectangle(val4, num11, num12, num13, num14);
								if (rectangle.Width / list.Count > 15)
								{
									if (rectangle.Width / list.Count > 25)
									{
										val6.DrawString(Math.Round(float.Parse(val8.Tags["potency"].ToString(), CultureInfo.InvariantCulture), 2).ToString(CultureInfo.InvariantCulture), val7, (Brush)(object)val3, num4 - 8f + num3 / 2f, num2 + 2f);
									}
									else if (num10 % 2 == 0)
									{
										val6.DrawString(Math.Round(float.Parse(val8.Tags["potency"].ToString(), CultureInfo.InvariantCulture), 2).ToString(CultureInfo.InvariantCulture), val7, (Brush)(object)val3, num4 - 8f + num3 / 2f, num2 + 12f);
									}
									else
									{
										val6.DrawString(Math.Round(float.Parse(val8.Tags["potency"].ToString(), CultureInfo.InvariantCulture), 2).ToString(CultureInfo.InvariantCulture), val7, (Brush)(object)val3, num4 - 8f + num3 / 2f, num2 + 2f);
									}
								}
								num4 += num3;
							}
							val6.DrawLine(val5, num4, (float)rectangle.Top, num4, (float)rectangle.Bottom);
						}
						finally
						{
							((IDisposable)val5)?.Dispose();
						}
					}
					finally
					{
						((IDisposable)val4)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		catch (Exception ex3)
		{
			ActGlobals.oFormActMain.WriteExceptionLog(ex3, string.Empty);
			val = GraphDrawMessage(ex3.ToString(), 12f, val);
		}
		TimeSpan timeSpan = DateTime.Now - now;
		ActGlobals.oFormActMain.WriteDebugLog("GraphHitBars: " + timeSpan.TotalMilliseconds.ToString("F"));
		return val;
	}

	internal static Bitmap GraphDrawMessage(string Message, float FontSize, Bitmap BlankImage)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected Obj, but got Unknown
		//IL_003e: Expected Obj, but got Unknown
		Graphics val = Graphics.FromImage((Image)(object)BlankImage);
		val.SmoothingMode = (SmoothingMode)4;
		val.Clear(Color.White);
		val.DrawString(Message, new Font("Arial Black", FontSize, (FontStyle)0), (Brush)new SolidBrush(Color.Black), 12f, 12f);
		return BlankImage;
	}
}
