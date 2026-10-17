using System;
using System.Linq;
using FFXIV_ACT_Plugin.Resource;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public class PaladinClemencyHeal : IEffectEntryStrategy
{
	public Type[] EffectEntryTypes => new Type[1] { typeof(HealEffectEntry) };

	public int Order => 2;

	public void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry
	{
		HealEffectEntry healEffectEntry = entry as HealEffectEntry;
		if (actionEffect.ActionId == ActionId.Clemency && Array.IndexOf(actionEffect.Effects.OfType<HealEffectEntry>().ToArray(), healEffectEntry) == 1)
		{
			healEffectEntry.ProcActionName = healEffectEntry.ActionName;
			healEffectEntry.ProcSourceId = actionEffect.SourceId;
			healEffectEntry.ProcTargetId = actionEffect.SourceId;
		}
	}
}
