using System;
using FFXIV_ACT_Plugin.Config;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public class UnknownDamage : IEffectEntryStrategy
{
	private readonly ISettingsMediator _settingsManager;

	public Type[] EffectEntryTypes => new Type[1] { typeof(DamageEffectEntry) };

	public int Order => 4;

	public UnknownDamage(ISettingsMediator settingsManager)
	{
		_settingsManager = settingsManager;
	}

	public void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry
	{
		if (entry is DamageEffectEntry damageEffectEntry && actionEffect.Definition != null && (damageEffectEntry.ProcSourceId == 0 || damageEffectEntry.ProcTargetId == 0) && (int)entry.EffectIndex > (actionEffect.Definition.IsDamage ? 1 : 0))
		{
			damageEffectEntry.ProcActionName = "Unknown";
			damageEffectEntry.SourceNameOverride = "Unknown";
			damageEffectEntry.ProcSourceId = actionEffect.SourceId;
			damageEffectEntry.ProcTargetId = (damageEffectEntry.IsSourceEntry ? actionEffect.SourceId : actionEffect.TargetId);
			_settingsManager?.ProcessException(actionEffect.Timestamp, "Unknown Damage Proc: " + actionEffect.Definition?.Name);
		}
	}
}
