using System;
using System.Collections.Generic;
using System.Linq;
using FFXIV_ACT_Plugin.Resource;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy._1;

public class SmnFesterPotency : IEffectEntryStrategy
{
	public Type[] EffectEntryTypes => new Type[1] { typeof(DamageEffectEntry) };

	public int Order => 1;

	public void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry
	{
		if (entry is DamageEffectEntry damageEffectEntry && actionEffect.ActionId == ActionId.Fester && state.Combatants.ContainsKey(actionEffect.TargetId))
		{
			List<StatusEffect> source = state.Combatants[actionEffect.TargetId].StatusEffects.Where((StatusEffect x) => x.Timestamp <= actionEffect.Timestamp && (x.Duration == 0.0 || x.Timestamp.AddSeconds(x.Duration) >= actionEffect.Timestamp) && x.SourceId == actionEffect.SourceId && new uint[5]
			{
				StatusId.Bio,
				StatusId.Miasma,
				StatusId.Bio2,
				StatusId.Miasma3,
				StatusId.Bio3
			}.Contains(x.StatusId)).ToList();
			bool flag = source.Any((StatusEffect x) => x.StatusId == StatusId.Bio || x.StatusId == StatusId.Bio2 || x.StatusId == StatusId.Bio3);
			bool flag2 = source.Any((StatusEffect x) => x.StatusId == StatusId.Miasma || x.StatusId == StatusId.Miasma3);
			uint num = 100u;
			if (flag)
			{
				damageEffectEntry.CalculatedPotency += num;
				damageEffectEntry.AppliedStatusEffects += $"fester(bio +{num}),";
			}
			if (flag2)
			{
				damageEffectEntry.CalculatedPotency += num;
				damageEffectEntry.AppliedStatusEffects += $"fester(miasma +{num}),";
			}
		}
	}
}
