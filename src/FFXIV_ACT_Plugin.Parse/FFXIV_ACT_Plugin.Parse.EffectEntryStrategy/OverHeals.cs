using System;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public class OverHeals : IEffectEntryStrategy
{
	public Type[] EffectEntryTypes => new Type[1] { typeof(HealEffectEntry) };

	public int Order => 1;

	public void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry
	{
		HealEffectEntry healEffectEntry = entry as HealEffectEntry;
		uint num = ((actionEffect.TargetCurrentHp <= actionEffect.TargetMaxHp) ? (actionEffect.TargetMaxHp - actionEffect.TargetCurrentHp) : 0u);
		if (healEffectEntry.ProcTargetId != 0)
		{
			if (healEffectEntry.ProcTargetId == actionEffect.SourceId)
			{
				num = ((actionEffect.SourceCurrentHp <= actionEffect.SourceMaxHp) ? (actionEffect.SourceMaxHp - actionEffect.SourceCurrentHp) : 0u);
			}
			else
			{
				_ = healEffectEntry.ProcTargetId;
				_ = actionEffect.TargetId;
			}
		}
		healEffectEntry.OverHealAmount = ((healEffectEntry.Amount >= num) ? (healEffectEntry.Amount - num) : 0u);
	}
}
