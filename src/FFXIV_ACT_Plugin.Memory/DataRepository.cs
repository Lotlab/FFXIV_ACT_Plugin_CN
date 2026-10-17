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
		return _benchmarkRepository.Measure<ReadOnlyCollection<Combatant>>("DataGetCombatantList", (Func<ReadOnlyCollection<Combatant>>)(() => _combatantManager.Combatants));
	}

	public uint GetCurrentPlayerID()
	{
		return _benchmarkRepository.Measure<uint>("DataGetCurrentPlayerID", (Func<uint>)(() =>
		{
			uint result = 0u;
			if (_combatantManager.Combatants.Any())
			{
				result = _combatantManager.Combatants[0].ID;
			}
			return result;
		}));
	}

	public Process GetCurrentFFXIVProcess()
	{
		return _benchmarkRepository.Measure<Process>("DataGetCurrentFFXIVProcess", (Func<Process>)(() => _processManager.Current.Process));
	}

	public string GetGameVersion()
	{
		return _benchmarkRepository.Measure<string>("DataGetGameVersion", (Func<string>)(() => _processManager.Current.GameVersion));
	}

	public uint GetCurrentTerritoryID()
	{
		return _benchmarkRepository.Measure<uint>("DataGetCurrentTerritoryID", (Func<uint>)(() => _zoneProcessor.ZoneID));
	}

	public Player GetPlayer()
	{
		return _benchmarkRepository.Measure<Player>("DataGetPlayer", (Func<Player>)(() => _playerProcessor.Player));
	}

	public IDictionary<uint, string> GetResourceDictionary(ResourceType resourceType)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return _benchmarkRepository.Measure<IDictionary<uint, string>>("DataGetResourceDictionary", (Func<IDictionary<uint, string>>)(() =>
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Invalid comparison between Unknown and I4
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Invalid comparison between Unknown and I4
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Invalid comparison between Unknown and I4
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
				bool flag = (int)resourceType != 10 && (int)resourceType != 9 && (int)resourceType != 8;
				dictionary = new ResourceFile(text, flag).Dictionary;
				_resourceDictionaryCache.Add(resourceType, dictionary);
				return dictionary;
			}
		}));
	}

	public Language GetSelectedLanguageID()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		return _benchmarkRepository.Measure<Language>("DataGetSelectedLanguageID", (Func<Language>)(() =>
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return _settingsManager.ParseSettings.LanguageID;
		}));
	}

	public bool IsChatLogAvailable()
	{
		return _benchmarkRepository.Measure<bool>("DataIsChatLogAvailable", (Func<bool>)(() => !_settingsManager.DataCollectionSettings.DisableCombatLog));
	}

	public DateTime GetServerTimestamp()
	{
		return _benchmarkRepository.Measure<DateTime>("DataGetServerTimestamp", (Func<DateTime>)(() => _serverTime.ServerTime));
	}

	public string[] GetAntiVirusNames()
	{
		return _benchmarkRepository.Measure<string[]>("DataGetAntiVirusNames", (Func<string[]>)(() => _readAntiVirus.Read()));
	}

	public byte GetGameRegion()
	{
		return _benchmarkRepository.Measure<byte>("DataGetGameRegion", (Func<byte>)(() =>
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (byte)_settingsManager.DataCollectionSettings.RegionID;
		}));
	}
}
