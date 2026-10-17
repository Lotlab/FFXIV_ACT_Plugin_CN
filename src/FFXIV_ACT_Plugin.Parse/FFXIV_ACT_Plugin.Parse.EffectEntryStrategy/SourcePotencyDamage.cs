using System;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public class SourcePotencyDamage : IEffectEntryStrategy
{
	public Type[] EffectEntryTypes => new Type[1] { typeof(DamageEffectEntry) };

	public int Order => 4;

	public void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry
	{
		if (entry is DamageEffectEntry damageEffectEntry && string.IsNullOrWhiteSpace(damageEffectEntry.ProcActionName) && state.Combatants.ContainsKey(actionEffect.SourceId))
		{
			CombatantState combatantState = state.Combatants[actionEffect.SourceId];
			if (combatantState.Tracking.DamagePotencyHistoryIndex >= combatantState.Tracking.DamagePotencyHistory.Length - 1)
			{
				combatantState.Tracking.DamagePotencyHistoryIndex = 0;
			}
			else
			{
				combatantState.Tracking.DamagePotencyHistoryIndex++;
			}
			combatantState.Tracking.DamagePotencyHistory[combatantState.Tracking.DamagePotencyHistoryIndex] = damageEffectEntry.AppliedPotencyMultiplier;
		}
	}
}
