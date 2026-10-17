using System;
using FFXIV_ACT_Plugin.Resource;
using FFXIV_ACT_Plugin.Resource.Definitions;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public class ApplyStatusEffectsDamage : IEffectEntryStrategy
{
	private readonly IPotencyStatusApplication _statusApplication;

	public Type[] EffectEntryTypes => new Type[1] { typeof(DamageEffectEntry) };

	public int Order => 2;

	public ApplyStatusEffectsDamage(IPotencyStatusApplication statusApplication)
	{
		_statusApplication = statusApplication;
	}

	public void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry
	{
		if (!(entry is DamageEffectEntry damageEffectEntry) || !string.IsNullOrWhiteSpace(damageEffectEntry.ProcActionName))
		{
			return;
		}
		FFXIV_ACT_Plugin.Resource.Definitions.Action definition = actionEffect.Definition;
		uint? num = ((definition != null) ? new uint?(definition.GetDamagePotency(actionEffect.TargetIndex, damageEffectEntry.ComboAmount != 0)) : ((uint?)null));
		if (!num.HasValue)
		{
			return;
		}
		damageEffectEntry.CalculatedPotency += num.Value;
		if (damageEffectEntry.CalculatedPotency == 0.0)
		{
			return;
		}
		CombatantState combatantState = (state.Combatants.ContainsKey(actionEffect.SourceId) ? state.Combatants[actionEffect.SourceId] : null);
		CombatantState combatantState2 = (state.Combatants.ContainsKey(actionEffect.TargetId) ? state.Combatants[actionEffect.TargetId] : null);
		if (combatantState == null || combatantState2 == null)
		{
			return;
		}
		if (actionEffect.ActionId == ActionId.Broil)
		{
			if (combatantState.Level >= 72)
			{
				damageEffectEntry.CalculatedPotency += 50.0;
			}
			else if (combatantState.Level >= 64)
			{
				damageEffectEntry.CalculatedPotency += 20.0;
			}
		}
		if (combatantState.Job == Job.Sch && actionEffect.ActionId == ActionId.RuinScholar)
		{
			if (combatantState.Level >= 82)
			{
				damageEffectEntry.CalculatedPotency = 220.0;
			}
			else if (combatantState.Level >= 72)
			{
				damageEffectEntry.CalculatedPotency = 200.0;
			}
			else if (combatantState.Level >= 64)
			{
				damageEffectEntry.CalculatedPotency = 180.0;
			}
			else if (combatantState.Level >= 54)
			{
				damageEffectEntry.CalculatedPotency = 160.0;
			}
		}
		if (actionEffect.ActionId == ActionId.Outburst && combatantState.Level < 76)
		{
			damageEffectEntry.CalculatedPotency = 70.0;
		}
		if (combatantState.Job == Job.Drg && combatantState.Level < 76)
		{
			if (actionEffect.ActionId == ActionId.TrueThrust)
			{
				damageEffectEntry.CalculatedPotency -= 80.0;
			}
			else if (actionEffect.ActionId == ActionId.VorpalThrust)
			{
				damageEffectEntry.CalculatedPotency -= 35.0;
			}
			if (actionEffect.ActionId == ActionId.Disembowel)
			{
				damageEffectEntry.CalculatedPotency -= 45.0;
			}
		}
		_statusApplication.ApplyStatusEffects(actionEffect.Timestamp, combatantState, combatantState2, damageEffectEntry, state.ZoneId, actionEffect.ActionId, state.EchoBuff);
	}
}
