using System;
using FFXIV_ACT_Plugin.Resource;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public class ActionName : IEffectEntryStrategy
{
	private readonly IActionList _actionList;

	public Type[] EffectEntryTypes => new Type[3]
	{
		typeof(HealEffectEntry),
		typeof(DamageEffectEntry),
		typeof(ResourceEffectEntry)
	};

	public int Order => 1;

	public ActionName(IActionList actionList)
	{
		_actionList = actionList;
	}

	public void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry
	{
		string actionNameByID = _actionList.GetActionNameByID(actionEffect.ActionId);
		if (entry is DamageEffectEntry damageEffectEntry)
		{
			damageEffectEntry.ActionName = actionNameByID ?? $"Unknown_{actionEffect.ActionId:X2}";
		}
		else if (entry is HealEffectEntry healEffectEntry)
		{
			healEffectEntry.ActionName = actionNameByID ?? $"Unknown_{actionEffect.ActionId:X2}";
		}
		else if (entry is ResourceEffectEntry resourceEffectEntry)
		{
			resourceEffectEntry.ActionName = actionNameByID ?? $"Unknown_{actionEffect.ActionId:X2}";
		}
	}
}
