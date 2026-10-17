using System;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public class BardActionPotency : IEffectEntryStrategy
{
	public Type[] EffectEntryTypes => Array.Empty<Type>();

	public int Order => 1;

	public void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Invalid comparison between Unknown and I4
		if (!(entry is DamageEffectEntry damageEffectEntry) || !state.Combatants.ContainsKey(actionEffect.SourceId))
		{
			return;
		}
		CombatantState combatantState = state.Combatants[actionEffect.SourceId];
		if (combatantState.Job == Job.Brd && (int)damageEffectEntry.DamageType != 5)
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
