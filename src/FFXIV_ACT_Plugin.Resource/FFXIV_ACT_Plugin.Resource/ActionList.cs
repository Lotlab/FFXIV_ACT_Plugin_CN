using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FFXIV_ACT_Plugin.Common;
using FFXIV_ACT_Plugin.Config;

namespace FFXIV_ACT_Plugin.Resource;

public class ActionList : IActionList
{
	private readonly ISettingsMediator _settingsManager;

	private readonly Dictionary<Language, SortedDictionary<uint, string>> _actionList = new Dictionary<Language, SortedDictionary<uint, string>>();

	private SortedDictionary<uint, string> _itemActionList;

	private SortedDictionary<uint, ActionCategory> _actionCategoryList;

	public ActionList(ISettingsMediator settingsManager)
	{
		_settingsManager = settingsManager;
	}

	public void LoadResources()
	{
		LoadActionCategories();
		LoadGeneratedDefinitions();
	}

	public void LoadActionCategories()
	{
		SortedDictionary<uint, string> dictionary = new ResourceFile("Generated.ActionCategoryList_English.txt").Dictionary;
		_actionCategoryList = new SortedDictionary<uint, ActionCategory>(dictionary.ToDictionary((KeyValuePair<uint, string> x) => x.Key, (KeyValuePair<uint, string> x) => (ActionCategory)int.Parse(x.Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture)));
	}

	public void LoadGeneratedDefinitions()
	{
		foreach (Language value in Enum.GetValues(typeof(Language)))
		{
			SortedDictionary<uint, string> dictionary = new ResourceFile($"Generated.ActionList_{value}.txt").Dictionary;
			_actionList.Add(value, dictionary);
		}
		_itemActionList = new ResourceFile("Generated.ItemActionList.txt").Dictionary;
	}

	public string GetActionNameByID(uint actionId)
	{
		if (!_actionList.ContainsKey(_settingsManager.ParseSettings.LanguageID))
		{
			return "";
		}
		if (actionId == 0)
		{
			return "unknown";
		}
		if ((actionId & 0x2000000) != 0)
		{
			return "item_" + Convert.ToString(actionId - 33554432, 16);
		}
		if ((actionId & 0xD000000) != 0)
		{
			return "mount_" + Convert.ToString(actionId - 218103808, 16);
		}
		if (_actionList[_settingsManager.ParseSettings.LanguageID].TryGetValue(actionId, out var value))
		{
			return value;
		}
		if (_itemActionList.TryGetValue(actionId, out value))
		{
			return value;
		}
		return "unknown_" + Convert.ToString(actionId, 16);
	}

	public uint[] GetAbilityIdsByEnglishName(string actionName)
	{
		return (from x in _actionList[Language.English]
			where x.Value.Equals(actionName, StringComparison.OrdinalIgnoreCase)
			select x.Key).ToArray();
	}

	public ActionCategory GetActionCategory(uint actionId)
	{
		if (_actionCategoryList.TryGetValue(actionId, out var value))
		{
			return value;
		}
		return ActionCategory.Unknown;
	}
}
