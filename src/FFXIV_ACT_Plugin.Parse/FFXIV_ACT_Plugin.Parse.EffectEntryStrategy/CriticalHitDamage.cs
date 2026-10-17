using System;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public class CriticalHitDamage : IEffectEntryStrategy
{
	private readonly CritDhStatusApplication _critDhStatusApplication;

	public Type[] EffectEntryTypes => new Type[1] { typeof(DamageEffectEntry) };

	public int Order => 2;

	public CriticalHitDamage(CritDhStatusApplication critDhStatusApplication)
	{
		_critDhStatusApplication = critDhStatusApplication;
	}

	public void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry
	{
		if (entry is DamageEffectEntry damageEffectEntry && state.Combatants.ContainsKey(actionEffect.SourceId))
		{
			CombatantState combatantState = state.Combatants[actionEffect.SourceId];
			CombatantState targetState = null;
			if (state.Combatants.ContainsKey(actionEffect.TargetId))
			{
				targetState = state.Combatants[actionEffect.TargetId];
			}
			_critDhStatusApplication.CalculateCrit(damageEffectEntry, actionEffect.Timestamp, combatantState, targetState, actionEffect.ActionId);
			if (damageEffectEntry.CritBuffAmount == 0.0)
			{
				double num = (damageEffectEntry.IsCritical ? 1.0 : 0.0);
				combatantState.Tracking.CriticalDamageRate = (combatantState.Tracking.CriticalDamageRate * (double)combatantState.Tracking.CriticalDamageRateSwings + num) / (double)(combatantState.Tracking.CriticalDamageRateSwings + 1);
				combatantState.Tracking.CriticalDamageRateSwings++;
			}
			if (combatantState.Tracking.CriticalDamageRateSwings > 10)
			{
				damageEffectEntry.SourceCritRate = combatantState.Tracking.CriticalDamageRate;
			}
			else
			{
				damageEffectEntry.SourceCritRate = 0.15;
			}
			if (damageEffectEntry.SourceCritRate < 0.05)
			{
				damageEffectEntry.SourceCritRate = 0.05;
			}
		}
	}
}
