using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FFXIV_ACT_Plugin.Common;

public interface IActWrapper
{
	string CharName { get; }

	int TimeStampLen { get; set; }

	bool LogPathHasCharName { get; set; }

	Regex ZoneChangeRegex { get; set; }

	string LogFilePath { get; set; }

	string LogFileFilter { get; set; }

	int GlobalTimeSorter { get; set; }

	bool InCombat { get; }

	long NoDamage { get; }

	long Miss { get; }

	long Death { get; }

	bool IsMainFormVisible();

	void OpenLog(bool GetCurrentZone, bool GetCharNameFromFile);

	bool SetEncounter(DateTime Time, string Attacker, string Victim);

	void AddCombatAction(int SwingType, bool Critical, string Special, long damage, DateTime Time, int TimeSorter, string theAttackType, string Attacker, string theDamageType, string Victim, Dictionary<string, object> Tags);

	void ChangeZone(string zoneName);

	void ActCommands(string commandText);

	void WriteErrorLog(Exception ex, string moreInfo);

	void EndCombat(bool actExport);
}
