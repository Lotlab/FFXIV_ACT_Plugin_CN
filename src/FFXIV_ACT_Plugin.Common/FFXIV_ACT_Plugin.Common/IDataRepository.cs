using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using FFXIV_ACT_Plugin.Common.Models;

namespace FFXIV_ACT_Plugin.Common;

public interface IDataRepository
{
	Language GetSelectedLanguageID();

	Process GetCurrentFFXIVProcess();

	IDictionary<uint, string> GetResourceDictionary(ResourceType resourceType);

	uint GetCurrentTerritoryID();

	uint GetCurrentPlayerID();

	ReadOnlyCollection<Combatant> GetCombatantList();

	Player GetPlayer();

	DateTime GetServerTimestamp();

	string GetGameVersion();

	bool IsChatLogAvailable();

	string[] GetAntiVirusNames();

	byte GetGameRegion();
}
