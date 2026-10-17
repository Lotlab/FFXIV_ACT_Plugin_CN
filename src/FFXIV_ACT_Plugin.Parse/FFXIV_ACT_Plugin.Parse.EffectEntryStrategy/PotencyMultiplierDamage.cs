using System;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public class PotencyMultiplierDamage : IEffectEntryStrategy
{
	public Type[] EffectEntryTypes => new Type[1] { typeof(DamageEffectEntry) };

	public int Order => 3;

	public void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry
	{
		if (!(entry is DamageEffectEntry damageEffectEntry))
		{
			return;
		}
		damageEffectEntry.AppliedPotencyMultiplier = 0.0;
		if (damageEffectEntry.CalculatedPotency != 0.0 && damageEffectEntry.CalculatedPotencyMultiplier != 0.0)
		{
			damageEffectEntry.AppliedPotencyMultiplier += damageEffectEntry.Amount;
			if (damageEffectEntry.IsParried)
			{
				damageEffectEntry.AppliedPotencyMultiplier /= 1 + damageEffectEntry.ComboAmount / 100;
			}
			if (damageEffectEntry.IsDh)
			{
				damageEffectEntry.AppliedPotencyMultiplier /= 1.25;
			}
			if (damageEffectEntry.IsCritical)
			{
				double num = 1.35 + damageEffectEntry.SourceCritRate;
				damageEffectEntry.AppliedPotencyMultiplier /= num;
			}
			damageEffectEntry.AppliedPotencyMultiplier /= damageEffectEntry.CalculatedPotency;
			damageEffectEntry.AppliedPotencyMultiplier /= damageEffectEntry.CalculatedPotencyMultiplier;
		}
	}
}
