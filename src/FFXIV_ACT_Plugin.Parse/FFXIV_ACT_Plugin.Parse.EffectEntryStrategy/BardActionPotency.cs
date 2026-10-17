using System;
using FFXIV_ACT_Plugin.Resource;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public class BardActionPotency : IEffectEntryStrategy
{
	public Type[] EffectEntryTypes => Array.Empty<Type>();

	public int Order => 1;

	public void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry
	{
		if (!(entry is DamageEffectEntry damageEffectEntry) || !state.Combatants.ContainsKey(actionEffect.SourceId))
		{
			return;
		}
		CombatantState combatantState = state.Combatants[actionEffect.SourceId];
		if (combatantState.Job == Job.Brd && damageEffectEntry.DamageType != DamageType.Magic)
		{
			if (combatantState.Level >= 20)
			{
				damageEffectEntry.CalculatedPotencyMultiplier += 0.1;
			}
			if (combatantState.Level >= 40)
			{
				damageEffectEntry.CalculatedPotencyMultiplier += 0.1;
			}
			damageEffectEntry.AppliedStatusEffects = $"ActionDamage ({(damageEffectEntry.CalculatedPotencyMultiplier - 1.0) * 100.0:00}) ";
		}
	}
}
