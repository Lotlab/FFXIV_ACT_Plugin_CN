using System;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public class PotencyMultiplierHeal : IEffectEntryStrategy
{
	public Type[] EffectEntryTypes => new Type[1] { typeof(HealEffectEntry) };

	public int Order => 3;

	public void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry
	{
		if (!(entry is HealEffectEntry healEffectEntry))
		{
			return;
		}
		healEffectEntry.AppliedPotencyMultiplier = 0.0;
		if (healEffectEntry.CalculatedPotency != 0.0 && healEffectEntry.CalculatedPotencyMultiplier != 0.0)
		{
			healEffectEntry.AppliedPotencyMultiplier += healEffectEntry.Amount;
			if (healEffectEntry.IsCritical)
			{
				double num = 1.35 + healEffectEntry.SourceCritRate;
				healEffectEntry.AppliedPotencyMultiplier /= num;
			}
			healEffectEntry.AppliedPotencyMultiplier /= healEffectEntry.CalculatedPotency;
			healEffectEntry.AppliedPotencyMultiplier /= healEffectEntry.CalculatedPotencyMultiplier;
		}
	}
}
