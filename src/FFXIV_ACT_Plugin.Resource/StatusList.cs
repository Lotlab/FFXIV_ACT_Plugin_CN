using System;
using System.Collections.Generic;
using System.Linq;
using FFXIV_ACT_Plugin.Common;
using FFXIV_ACT_Plugin.Config;

namespace FFXIV_ACT_Plugin.Resource;

public class StatusList : IStatusList
{
	private readonly Dictionary<Language, SortedDictionary<uint, string>> _statusList = new Dictionary<Language, SortedDictionary<uint, string>>();

	private readonly uint[] _ignoreStatusList = new uint[18]
	{
		353u, 354u, 355u, 356u, 357u, 360u, 361u, 362u, 363u, 364u,
		365u, 366u, 367u, 368u, 413u, 414u, 902u, 2932u
	};

	private readonly ISettingsMediator _settingsManager;

	public StatusList(ISettingsMediator settingsManager)
	{
		_settingsManager = settingsManager;
	}

	public void LoadResources()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		foreach (Language value in Enum.GetValues(typeof(Language)))
		{
			_statusList[value] = new ResourceFile($"Generated.StatusList_{(object)value}.txt").Dictionary;
		}
	}

	public string GetStatusNameByID(uint statusId)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (_statusList == null || !_statusList.ContainsKey(_settingsManager.ParseSettings.LanguageID))
		{
			return "";
		}
		if (statusId == 0)
		{
			return "Unknown";
		}
		if (_statusList[_settingsManager.ParseSettings.LanguageID].TryGetValue(statusId, out var value))
		{
			return value;
		}
		return $"Unknown_{statusId:X2}";
	}

	public bool IsIgnoreStatus(uint statusId)
	{
		return Enumerable.Contains(_ignoreStatusList, statusId);
	}
}
