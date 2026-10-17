using System;
using FFXIV_ACT_Plugin.Resource;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public class RedirectLimitBreak : IEffectEntryStrategy
{
	private readonly IActionList _actionList;

	public Type[] EffectEntryTypes => new Type[2]
	{
		typeof(DamageEffectEntry),
		typeof(HealEffectEntry)
	};

	public int Order => 1;

	public RedirectLimitBreak(IActionList actionList)
	{
		_actionList = actionList;
	}

	public void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		if ((int)_actionList.GetActionCategory(actionEffect.ActionId) == 9)
		{
			if (entry is DamageEffectEntry damageEffectEntry)
			{
				damageEffectEntry.SourceNameOverride = "Limit Break";
			}
			else if (entry is HealEffectEntry healEffectEntry)
			{
				healEffectEntry.SourceNameOverride = "Limit Break";
			}
		}
	}
}
