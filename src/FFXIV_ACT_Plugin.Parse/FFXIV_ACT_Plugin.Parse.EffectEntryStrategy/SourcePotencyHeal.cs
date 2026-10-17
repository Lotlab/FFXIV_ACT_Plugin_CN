using System;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public class SourcePotencyHeal : IEffectEntryStrategy
{
	public Type[] EffectEntryTypes => new Type[1] { typeof(HealEffectEntry) };

	public int Order => 4;

	public void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry
	{
		if (entry is HealEffectEntry healEffectEntry && string.IsNullOrWhiteSpace(healEffectEntry.ProcActionName) && state.Combatants.ContainsKey(actionEffect.SourceId))
		{
			CombatantState combatantState = state.Combatants[actionEffect.SourceId];
			if (combatantState.Tracking.HealPotencyHistoryIndex >= combatantState.Tracking.HealPotencyHistory.Length - 1)
			{
				combatantState.Tracking.HealPotencyHistoryIndex = 0;
			}
			else
			{
				combatantState.Tracking.HealPotencyHistoryIndex++;
			}
			combatantState.Tracking.HealPotencyHistory[combatantState.Tracking.HealPotencyHistoryIndex] = healEffectEntry.AppliedPotencyMultiplier;
		}
	}
}
