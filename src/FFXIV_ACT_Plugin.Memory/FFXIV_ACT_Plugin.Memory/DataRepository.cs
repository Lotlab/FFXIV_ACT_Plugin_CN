using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using FFXIV_ACT_Plugin.Common;
using FFXIV_ACT_Plugin.Common.Models;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Memory.MemoryProcessors;
using FFXIV_ACT_Plugin.Memory.MemoryReader;
using FFXIV_ACT_Plugin.Resource;

namespace FFXIV_ACT_Plugin.Memory;

public class DataRepository : IDataRepository
{
	private readonly ICombatantManager _combatantManager;

	private readonly IProcessManager _processManager;

	private readonly IZoneMapProcessor _zoneProcessor;

	private readonly IServerTimeProcessor _serverTime;

	private readonly ISettingsMediator _settingsManager;

	private readonly IPlayerProcessor _playerProcessor;

	private readonly IBenchmarkRepository _benchmarkRepository;

	private readonly IReadAntiVirus _readAntiVirus;

	private readonly Dictionary<ResourceType, IDictionary<uint, string>> _resourceDictionaryCache = new Dictionary<ResourceType, IDictionary<uint, string>>();

	public DataRepository(ICombatantManager combatantManager, IProcessManager processManager, ISettingsMediator settingsManager, IZoneMapProcessor zoneProcessor, IServerTimeProcessor serverTime, IPlayerProcessor playerProcessor, IBenchmarkRepository benchmarkRepository, IReadAntiVirus readAntiVirus)
	{
		_combatantManager = combatantManager;
		_processManager = processManager;
		_zoneProcessor = zoneProcessor;
		_settingsManager = settingsManager;
		_playerProcessor = playerProcessor;
		_serverTime = serverTime;
		_benchmarkRepository = benchmarkRepository;
		_readAntiVirus = readAntiVirus;
	}

	public ReadOnlyCollection<Combatant> GetCombatantList()
	{
		return _benchmarkRepository.Measure("DataGetCombatantList", () => _combatantManager.Combatants);
	}

	public uint GetCurrentPlayerID()
	{
		return _benchmarkRepository.Measure("DataGetCurrentPlayerID", () =>
		{
			uint result = 0u;
			if (_combatantManager.Combatants.Any())
			{
				result = _combatantManager.Combatants[0].ID;
			}
			return result;
		});
	}

	public Process GetCurrentFFXIVProcess()
	{
		return _benchmarkRepository.Measure("DataGetCurrentFFXIVProcess", () => _processManager.Current.Process);
	}

	public string GetGameVersion()
	{
		return _benchmarkRepository.Measure("DataGetGameVersion", () => _processManager.Current.GameVersion);
	}

	public uint GetCurrentTerritoryID()
	{
		return _benchmarkRepository.Measure("DataGetCurrentTerritoryID", () => _zoneProcessor.ZoneID);
	}

	public Player GetPlayer()
	{
		return _benchmarkRepository.Measure("DataGetPlayer", () => _playerProcessor.Player);
	}

	public IDictionary<uint, string> GetResourceDictionary(ResourceType resourceType)
	{
		return _benchmarkRepository.Measure("DataGetResourceDictionary", () =>
		{
			IDictionary<uint, string> dictionary = null;
			lock (_resourceDictionaryCache)
			{
				if (_resourceDictionaryCache.TryGetValue(resourceType, out var value))
				{
					return value;
				}
				string text = Enum.GetName(typeof(ResourceType), resourceType) ?? "";
				if (text.StartsWith("BuffList_", StringComparison.OrdinalIgnoreCase))
				{
					text = text.Replace("BuffList_", "StatusList_");
				}
				else if (text.StartsWith("SkillList_", StringComparison.OrdinalIgnoreCase))
				{
					text = text.Replace("SkillList_", "ActionList_");
				}
				else if (text == "ZoneList_EN")
				{
					text = "ContentFinderList_English";
				}
				if (text.EndsWith("_EN", StringComparison.OrdinalIgnoreCase))
				{
					text = text.Substring(0, text.Length - 3) + "_English";
				}
				if (text.EndsWith("_FR", StringComparison.OrdinalIgnoreCase))
				{
					text = text.Substring(0, text.Length - 3) + "_French";
				}
				if (text.EndsWith("_DE", StringComparison.OrdinalIgnoreCase))
				{
					text = text.Substring(0, text.Length - 3) + "_German";
				}
				if (text.EndsWith("_JP", StringComparison.OrdinalIgnoreCase))
				{
					text = text.Substring(0, text.Length - 3) + "_Japanese";
				}
				if (text.EndsWith("_KR", StringComparison.OrdinalIgnoreCase))
				{
					text = text.Substring(0, text.Length - 3) + "_Korean";
				}
				text = "Generated." + text + ".txt";
				bool isIdHex = resourceType != ResourceType.TerritoryList_EN && resourceType != ResourceType.ZoneList_EN && resourceType != ResourceType.WorldList_EN;
				dictionary = new ResourceFile(text, isIdHex).Dictionary;
				_resourceDictionaryCache.Add(resourceType, dictionary);
				return dictionary;
			}
		});
	}

	public Language GetSelectedLanguageID()
	{
		return _benchmarkRepository.Measure("DataGetSelectedLanguageID", () => _settingsManager.ParseSettings.LanguageID);
	}

	public bool IsChatLogAvailable()
	{
		return _benchmarkRepository.Measure("DataIsChatLogAvailable", () => !_settingsManager.DataCollectionSettings.DisableCombatLog);
	}

	public DateTime GetServerTimestamp()
	{
		return _benchmarkRepository.Measure("DataGetServerTimestamp", () => _serverTime.ServerTime);
	}

	public string[] GetAntiVirusNames()
	{
		return _benchmarkRepository.Measure("DataGetAntiVirusNames", () => _readAntiVirus.Read());
	}

	public byte GetGameRegion()
	{
		return _benchmarkRepository.Measure("DataGetGameRegion", () => (byte)_settingsManager.DataCollectionSettings.RegionID);
	}
}
