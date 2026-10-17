using System;
using System.Linq;
using FFXIV_ACT_Plugin.Config;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public class UnknownProcHeal : IEffectEntryStrategy
{
	private readonly ISettingsMediator _settingsManager;

	public Type[] EffectEntryTypes => new Type[1] { typeof(HealEffectEntry) };

	public int Order => 4;

	public UnknownProcHeal(ISettingsMediator settingsManager)
	{
		_settingsManager = settingsManager;
	}

	public void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry
	{
		if (entry is HealEffectEntry healEffectEntry && actionEffect.Definition != null && (healEffectEntry.ProcSourceId == 0 || healEffectEntry.ProcTargetId == 0) && Array.IndexOf((from x in actionEffect.Effects.OfType<HealEffectEntry>()
			where x.ProcSourceId == 0 && x.ProcTargetId == 0
			select x).ToArray(), healEffectEntry) >= (actionEffect.Definition.IsHeal ? 1 : 0))
		{
			healEffectEntry.ProcActionName = "Unknown";
			healEffectEntry.SourceNameOverride = "Unknown";
			healEffectEntry.ProcSourceId = actionEffect.SourceId;
			healEffectEntry.ProcTargetId = (healEffectEntry.IsSourceEntry ? actionEffect.SourceId : actionEffect.TargetId);
			_settingsManager.AddExceptionMessage(actionEffect.Timestamp, "Unknown Heal Proc: " + actionEffect.Definition.Name);
		}
	}
}
