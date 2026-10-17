using System;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public class CriticalHitHeal : IEffectEntryStrategy
{
	private readonly CritDhStatusApplication _critDhStatusApplication;

	public Type[] EffectEntryTypes => new Type[1] { typeof(HealEffectEntry) };

	public int Order => 2;

	public CriticalHitHeal(CritDhStatusApplication critDhStatusApplication)
	{
		_critDhStatusApplication = critDhStatusApplication;
	}

	public void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry
	{
		if (entry is HealEffectEntry healEffectEntry && state.Combatants.ContainsKey(actionEffect.SourceId))
		{
			CombatantState combatantState = state.Combatants[actionEffect.SourceId];
			CombatantState targetState = null;
			if (state.Combatants.ContainsKey(actionEffect.TargetId))
			{
				targetState = state.Combatants[actionEffect.TargetId];
			}
			_critDhStatusApplication.CalculateCrit(healEffectEntry, actionEffect.Timestamp, combatantState, targetState, actionEffect.ActionId);
			if (healEffectEntry.CritBuffAmount == 0.0)
			{
				double num = (healEffectEntry.IsCritical ? 1.0 : 0.0);
				combatantState.Tracking.CriticalDamageRate = (combatantState.Tracking.CriticalDamageRate * (double)combatantState.Tracking.CriticalDamageRateSwings + num) / (double)(combatantState.Tracking.CriticalDamageRateSwings + 1);
				combatantState.Tracking.CriticalDamageRateSwings++;
			}
			if (combatantState.Tracking.CriticalDamageRateSwings > 10)
			{
				healEffectEntry.SourceCritRate = combatantState.Tracking.CriticalDamageRate;
			}
			else
			{
				healEffectEntry.SourceCritRate = 0.15;
			}
			if (healEffectEntry.SourceCritRate < 0.05)
			{
				healEffectEntry.SourceCritRate = 0.05;
			}
		}
	}
}
