using System;
using FFXIV_ACT_Plugin.Resource;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public class RedirectBigActions : IEffectEntryStrategy
{
	private readonly IActionList _actionList;

	public Type[] EffectEntryTypes => new Type[1] { typeof(HealEffectEntry) };

	public int Order => 1;

	public RedirectBigActions(IActionList actionList)
	{
		_actionList = actionList;
	}

	public void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry
	{
		if (entry is HealEffectEntry healEffectEntry)
		{
			string actionNameByID = _actionList.GetActionNameByID(actionEffect.ActionId);
			healEffectEntry.ActionName = actionNameByID ?? $"Unknown_{actionEffect.ActionId}";
		}
	}
}
