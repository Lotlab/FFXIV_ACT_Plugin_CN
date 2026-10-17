using System;
using FFXIV_ACT_Plugin.Resource;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public class AutoAttackDamage : IEffectEntryStrategy
{
	private readonly IActionList _actionList;

	public Type[] EffectEntryTypes => new Type[1] { typeof(DamageEffectEntry) };

	public int Order => 1;

	public AutoAttackDamage(IActionList actionList)
	{
		_actionList = actionList;
	}

	public void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Invalid comparison between Unknown and I4
		if (entry is DamageEffectEntry damageEffectEntry)
		{
			if (!string.IsNullOrWhiteSpace(damageEffectEntry.ProcActionName))
			{
				damageEffectEntry.IsAutoAttack = false;
			}
			else
			{
				damageEffectEntry.IsAutoAttack = (int)_actionList.GetActionCategory(actionEffect.ActionId) == 1;
			}
		}
	}
}
