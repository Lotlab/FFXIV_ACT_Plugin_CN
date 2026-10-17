#define TRACE
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FFXIV_ACT_Plugin.Common;
using FFXIV_ACT_Plugin.Config;

namespace FFXIV_ACT_Plugin.Resource;

public class TerritoryList : ITerritoryList
{
	private readonly Dictionary<Language, SortedDictionary<uint, string>> _territories = new Dictionary<Language, SortedDictionary<uint, string>>();

	private readonly ISettingsMediator _settingsManager;

	public TerritoryList(ISettingsMediator settingsManager)
	{
		_settingsManager = settingsManager;
	}

	public void LoadResources()
	{
		Language[] array = new Language[2]
		{
			Language.English,
			Language.Korean
		};
		foreach (Language language in array)
		{
			string fileName = "Generated.ContentFinderList_English.txt";
			if (language == Language.Korean)
			{
				fileName = "Generated.ContentFinderList_Korean.txt";
			}
			_territories[language] = new ResourceFile(fileName, isIdHex: false).Dictionary;
			fileName = "Generated.TerritoryList_English.txt";
			if (language == Language.Korean)
			{
				fileName = "Generated.TerritoryList_Korean.txt";
			}
			KeyValuePair<uint, string>[] array2 = new ResourceFile(fileName, isIdHex: false).Dictionary.Where((KeyValuePair<uint, string> x) => !_territories[language].ContainsKey(x.Key)).ToArray();
			for (int num = 0; num < array2.Length; num++)
			{
				KeyValuePair<uint, string> keyValuePair = array2[num];
				_territories[language].Add(keyValuePair.Key, keyValuePair.Value);
			}
			foreach (uint item in _territories[language].Keys.ToList())
			{
				_territories[language][item] = FixZoneNameNumbers(_territories[language][item]);
			}
		}
	}

	private string FixZoneNameNumbers(string originalName)
	{
		string text = originalName;
		if (string.IsNullOrWhiteSpace(text))
		{
			return text;
		}
		if (char.IsDigit(originalName.Last()))
		{
			text = ((!originalName.Contains(' ')) ? ("(" + originalName + ")") : (originalName.Substring(0, originalName.LastIndexOf(' ')) + " (" + originalName.Substring(originalName.LastIndexOf(' ') + 1) + ")"));
		}
		return text;
	}

	public string GetTerritoryNameById(uint territoryId)
	{
		Language key = ((_settingsManager.ParseSettings.LanguageID != Language.Korean) ? Language.English : Language.Korean);
		if (_territories == null || !_territories.ContainsKey(key))
		{
			return $"Unknown Territory ({territoryId:X2})";
		}
		if (!_territories[key].ContainsKey(territoryId) || string.IsNullOrWhiteSpace(_territories[key][territoryId]))
		{
			Trace.WriteLine($"UNKNOWN TERRITORY ID [{territoryId:X2}]", "FFXIV_ACT_Plugin");
			return $"Unknown Territory ({territoryId:X2})";
		}
		return _territories[key][territoryId];
	}
}
