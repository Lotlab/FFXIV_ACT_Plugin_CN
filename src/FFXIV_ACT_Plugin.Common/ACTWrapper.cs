using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Advanced_Combat_Tracker;

namespace FFXIV_ACT_Plugin.Common;

public class ACTWrapper : IActWrapper
{
	private readonly IBenchmarkRepository _benchmarkRepository;

	public bool? OverrideMainFormVisible { get; set; }

	public string LogFilePath
	{
		get
		{
			FormActMain oFormActMain = ActGlobals.oFormActMain;
			return ((oFormActMain != null) ? oFormActMain.LogFilePath : null) ?? "";
		}
		set
		{
			FormActMain oFormActMain = ActGlobals.oFormActMain;
			if (((oFormActMain != null) ? oFormActMain.LogFilePath : null) != null)
			{
				ActGlobals.oFormActMain.LogFilePath = value;
			}
		}
	}

	public int GlobalTimeSorter
	{
		get
		{
			FormActMain oFormActMain = ActGlobals.oFormActMain;
			if (oFormActMain == null)
			{
				return -1;
			}
			return oFormActMain.GlobalTimeSorter;
		}
		set
		{
			FormActMain oFormActMain = ActGlobals.oFormActMain;
			if (oFormActMain != null)
			{
				_ = oFormActMain.GlobalTimeSorter;
				if (true)
				{
					ActGlobals.oFormActMain.GlobalTimeSorter = value;
				}
			}
		}
	}

	public bool InCombat
	{
		get
		{
			FormActMain oFormActMain = ActGlobals.oFormActMain;
			if (oFormActMain == null)
			{
				return false;
			}
			return oFormActMain.InCombat;
		}
	}

	public string CharName => ActGlobals.charName;

	public int TimeStampLen
	{
		get
		{
			FormActMain oFormActMain = ActGlobals.oFormActMain;
			if (oFormActMain == null)
			{
				return -1;
			}
			return oFormActMain.TimeStampLen;
		}
		set
		{
			if (ActGlobals.oFormActMain != null)
			{
				ActGlobals.oFormActMain.TimeStampLen = value;
			}
		}
	}

	public bool LogPathHasCharName
	{
		get
		{
			FormActMain oFormActMain = ActGlobals.oFormActMain;
			if (oFormActMain == null)
			{
				return false;
			}
			return oFormActMain.LogPathHasCharName;
		}
		set
		{
			if (ActGlobals.oFormActMain != null)
			{
				ActGlobals.oFormActMain.LogPathHasCharName = value;
			}
		}
	}

	public Regex ZoneChangeRegex
	{
		get
		{
			FormActMain oFormActMain = ActGlobals.oFormActMain;
			if (oFormActMain == null)
			{
				return null;
			}
			return oFormActMain.ZoneChangeRegex;
		}
		set
		{
			if (ActGlobals.oFormActMain != null)
			{
				ActGlobals.oFormActMain.ZoneChangeRegex = value;
			}
		}
	}

	public string LogFileFilter
	{
		get
		{
			FormActMain oFormActMain = ActGlobals.oFormActMain;
			return ((oFormActMain != null) ? oFormActMain.LogFileFilter : null) ?? "";
		}
		set
		{
			if (ActGlobals.oFormActMain != null)
			{
				ActGlobals.oFormActMain.LogFileFilter = value;
			}
		}
	}

	public long NoDamage => (long)(Dnum.NoDamage);

	public long Miss => (long)(Dnum.Miss);

	public long Death => (long)(Dnum.Death);

	public ACTWrapper(IBenchmarkRepository benchmarkRepository)
	{
		_benchmarkRepository = benchmarkRepository;
	}

	public bool IsMainFormVisible()
	{
		if (OverrideMainFormVisible.HasValue)
		{
			return OverrideMainFormVisible.Value;
		}
		FormActMain oFormActMain = ActGlobals.oFormActMain;
		bool num = oFormActMain != null && ((Control)oFormActMain).Visible;
		FormActMain oFormActMain2 = ActGlobals.oFormActMain;
		bool flag = oFormActMain2 != null && !oFormActMain2.ReadThreadLock;
		return num & flag;
	}

	public void OpenLog(bool GetCurrentZone, bool GetCharNameFromFile)
	{
		FormActMain oFormActMain = ActGlobals.oFormActMain;
		if (oFormActMain != null)
		{
			oFormActMain.OpenLog(GetCurrentZone, GetCharNameFromFile);
		}
	}

	public void AddCombatAction(int SwingType, bool Critical, string Special, long damage, DateTime Time, int TimeSorter, string theAttackType, string Attacker, string theDamageType, string Victim, Dictionary<string, object> Tags)
	{
		_benchmarkRepository.Measure("ACTAddCombatAction", () =>
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Expected Obj, but got Unknown
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected Obj, but got Unknown
			MasterSwing val = new MasterSwing(SwingType, Critical, Special, new Dnum(damage), Time, TimeSorter, theAttackType, Attacker, theDamageType, Victim);
			if (Tags != null && Tags.Count > 0)
			{
				val.Tags = Tags;
			}
			FormActMain oFormActMain = ActGlobals.oFormActMain;
			if (oFormActMain != null)
			{
				oFormActMain.AddCombatAction(val);
			}
		});
	}

	public bool SetEncounter(DateTime Time, string Attacker, string Victim)
	{
		return _benchmarkRepository.Measure("ACTSetEncounter", () => ActGlobals.oFormActMain.SetEncounter(Time, Attacker, Victim));
	}

	public void ChangeZone(string zoneName)
	{
		_benchmarkRepository.Measure("ACTChangeZone", () =>
		{
			FormActMain oFormActMain = ActGlobals.oFormActMain;
			if (oFormActMain != null)
			{
				oFormActMain.ChangeZone(zoneName);
			}
		});
	}

	public void ActCommands(string commandText)
	{
		FormActMain oFormActMain = ActGlobals.oFormActMain;
		if (oFormActMain != null)
		{
			oFormActMain.ActCommands(commandText);
		}
	}

	public void WriteErrorLog(Exception ex, string moreInfo)
	{
		FormActMain oFormActMain = ActGlobals.oFormActMain;
		if (oFormActMain != null)
		{
			oFormActMain.WriteExceptionLog(ex, moreInfo);
		}
	}

	public void EndCombat(bool actExport)
	{
		FormActMain oFormActMain = ActGlobals.oFormActMain;
		if (oFormActMain != null)
		{
			oFormActMain.EndCombat(actExport);
		}
	}
}
