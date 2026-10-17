using System;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Resource.Definitions;

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
			ISettingsMediator settingsManager = _settingsManager;
			if (settingsManager != null)
			{
				Action<DateTime, string> processException = settingsManager.ProcessException;
				DateTime timestamp = actionEffect.Timestamp;
				FFXIV_ACT_Plugin.Resource.Definitions.Action definition = actionEffect.Definition;
				processException(timestamp, "Unknown Damage Proc: " + ((definition != null) ? definition.Name : null));
			}
		}
	}
}
