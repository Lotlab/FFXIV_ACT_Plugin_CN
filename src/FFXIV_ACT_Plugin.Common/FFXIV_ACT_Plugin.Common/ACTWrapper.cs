using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
			return ActGlobals.oFormActMain?.LogFilePath ?? "";
		}
		set
		{
			if (ActGlobals.oFormActMain?.LogFilePath != null)
			{
				ActGlobals.oFormActMain.LogFilePath = value;
			}
		}
	}

	public int GlobalTimeSorter
	{
		get
		{
			return ActGlobals.oFormActMain?.GlobalTimeSorter ?? (-1);
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

	public bool InCombat => ActGlobals.oFormActMain?.InCombat ?? false;

	public string CharName => ActGlobals.charName;

	public int TimeStampLen
	{
		get
		{
			return ActGlobals.oFormActMain?.TimeStampLen ?? (-1);
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
			return ActGlobals.oFormActMain?.LogPathHasCharName ?? false;
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
			return ActGlobals.oFormActMain?.ZoneChangeRegex;
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
			return ActGlobals.oFormActMain?.LogFileFilter ?? "";
		}
		set
		{
			if (ActGlobals.oFormActMain != null)
			{
				ActGlobals.oFormActMain.LogFileFilter = value;
			}
		}
	}

	public long NoDamage => Dnum.NoDamage;

	public long Miss => Dnum.Miss;

	public long Death => Dnum.Death;

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
		bool num = ActGlobals.oFormActMain?.Visible ?? false;
		bool flag = !(ActGlobals.oFormActMain?.ReadThreadLock ?? true);
		return num & flag;
	}

	public void OpenLog(bool GetCurrentZone, bool GetCharNameFromFile)
	{
		ActGlobals.oFormActMain?.OpenLog(GetCurrentZone, GetCharNameFromFile);
	}

	public void AddCombatAction(int SwingType, bool Critical, string Special, long damage, DateTime Time, int TimeSorter, string theAttackType, string Attacker, string theDamageType, string Victim, Dictionary<string, object> Tags)
	{
		_benchmarkRepository.Measure("ACTAddCombatAction", () =>
		{
			MasterSwing masterSwing = new MasterSwing(SwingType, Critical, Special, new Dnum(damage), Time, TimeSorter, theAttackType, Attacker, theDamageType, Victim);
			if (Tags != null && Tags.Count > 0)
			{
				masterSwing.Tags = Tags;
			}
			ActGlobals.oFormActMain?.AddCombatAction(masterSwing);
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
			ActGlobals.oFormActMain?.ChangeZone(zoneName);
		});
	}

	public void ActCommands(string commandText)
	{
		ActGlobals.oFormActMain?.ActCommands(commandText);
	}

	public void WriteErrorLog(Exception ex, string moreInfo)
	{
		ActGlobals.oFormActMain?.WriteExceptionLog(ex, moreInfo);
	}

	public void EndCombat(bool actExport)
	{
		ActGlobals.oFormActMain?.EndCombat(actExport);
	}
}
