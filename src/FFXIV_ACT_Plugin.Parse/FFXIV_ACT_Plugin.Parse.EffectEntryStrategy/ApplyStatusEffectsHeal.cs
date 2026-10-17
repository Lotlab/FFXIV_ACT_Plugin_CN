using System;
using FFXIV_ACT_Plugin.Resource.Definitions;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public class ApplyStatusEffectsHeal : IEffectEntryStrategy
{
	private readonly IPotencyStatusApplication _statusApplication;

	public Type[] EffectEntryTypes => new Type[1] { typeof(HealEffectEntry) };

	public int Order => 2;

	public ApplyStatusEffectsHeal(IPotencyStatusApplication statusApplication)
	{
		_statusApplication = statusApplication;
	}

	public void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry
	{
		if (!(entry is HealEffectEntry healEffectEntry) || !string.IsNullOrWhiteSpace(healEffectEntry.ProcActionName))
		{
			return;
		}
		FFXIV_ACT_Plugin.Resource.Definitions.Action definition = actionEffect.Definition;
		uint? num = ((definition != null) ? new uint?(definition.GetHealPotency(actionEffect.TargetIndex)) : ((uint?)null));
		if (!num.HasValue)
		{
			return;
		}
		healEffectEntry.CalculatedPotency += num.Value;
		if (healEffectEntry.CalculatedPotency != 0.0)
		{
			CombatantState combatantState = (state.Combatants.ContainsKey(actionEffect.SourceId) ? state.Combatants[actionEffect.SourceId] : null);
			CombatantState combatantState2 = (state.Combatants.ContainsKey(actionEffect.TargetId) ? state.Combatants[actionEffect.TargetId] : null);
			if (combatantState != null && combatantState2 != null)
			{
				_statusApplication.ApplyStatusEffects(actionEffect.Timestamp, combatantState, combatantState2, healEffectEntry, state.ZoneId, actionEffect.ActionId, state.EchoBuff);
			}
		}
	}
}
