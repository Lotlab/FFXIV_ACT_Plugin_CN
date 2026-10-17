using System;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public class SourceDirectHit : IEffectEntryStrategy
{
	private readonly CritDhStatusApplication _critDhStatusApplication;

	public Type[] EffectEntryTypes => new Type[1] { typeof(DamageEffectEntry) };

	public int Order => 2;

	public SourceDirectHit(CritDhStatusApplication critDhStatusApplication)
	{
		_critDhStatusApplication = critDhStatusApplication;
	}

	public void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry
	{
		if (entry is DamageEffectEntry damageEffectEntry && state.Combatants.ContainsKey(actionEffect.SourceId))
		{
			CombatantState combatantState = state.Combatants[actionEffect.SourceId];
			_critDhStatusApplication.CalculateDirectHit(damageEffectEntry, actionEffect.Timestamp, combatantState, actionEffect.ActionId);
			if (damageEffectEntry.DirectHitBuffs < 1.0)
			{
				double num = (damageEffectEntry.IsDh ? 1.25 : 0.0) - damageEffectEntry.DirectHitBuffs;
				combatantState.Tracking.DirectHitRate = (combatantState.Tracking.DirectHitRate * (double)combatantState.Tracking.DirectHitRateSwings + num) / (double)(combatantState.Tracking.DirectHitRateSwings + 1);
				combatantState.Tracking.DirectHitRateSwings++;
			}
			if (combatantState.Tracking.DirectHitRateSwings > 10)
			{
				damageEffectEntry.SourceDirectHitRate = combatantState.Tracking.DirectHitRate - ((damageEffectEntry.DirectHitBuffs >= 1.0) ? 0.0 : damageEffectEntry.DirectHitBuffs);
			}
			else
			{
				damageEffectEntry.SourceDirectHitRate = (double)(int)combatantState.Level / 200.0;
			}
			if (damageEffectEntry.SourceDirectHitRate < 0.05)
			{
				damageEffectEntry.SourceDirectHitRate = 0.05;
			}
		}
	}
}
