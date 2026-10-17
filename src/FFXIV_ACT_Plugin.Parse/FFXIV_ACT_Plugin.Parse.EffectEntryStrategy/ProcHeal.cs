using System;
using System.Linq;
using FFXIV_ACT_Plugin.Resource;
using FFXIV_ACT_Plugin.Resource.Definitions;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public class ProcHeal : IEffectEntryStrategy
{
	private readonly IStatusList _statusList;

	private readonly Lazy<uint[]> _damageDealtHealProcs;

	private readonly Lazy<uint[]> _damageReceivedHealProcs;

	private readonly Lazy<uint[]> _healCastHealProcs;

	public Type[] EffectEntryTypes => new Type[1] { typeof(HealEffectEntry) };

	public int Order => 0;

	public ProcHeal(IStatusList statusList, IDefinitionRepository definitionRepository)
	{
		_statusList = statusList;
		_damageDealtHealProcs = new Lazy<uint[]>(() => definitionRepository.GetStatusIdsByReactiveProcType(ReactiveProc.ReactiveProcType.HealOnDamageDealt));
		_damageReceivedHealProcs = new Lazy<uint[]>(() => definitionRepository.GetStatusIdsByReactiveProcType(ReactiveProc.ReactiveProcType.HealOnDamageReceived));
		_healCastHealProcs = new Lazy<uint[]>(() => definitionRepository.GetStatusIdsByReactiveProcType(ReactiveProc.ReactiveProcType.HealOnHealCast));
	}

	public void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry
	{
		if (!(entry is HealEffectEntry healEffectEntry) || !state.Combatants.ContainsKey(actionEffect.SourceId))
		{
			return;
		}
		CombatantState combatantState = state.Combatants[actionEffect.SourceId];
		if (entry.IsSourceEntry)
		{
			foreach (StatusEffect item in combatantState?.StatusEffects.Where((StatusEffect x) => _damageDealtHealProcs.Value.Contains(x.StatusId)).ToList())
			{
				if (item.Timestamp.AddSeconds(item.Duration + 1.0) > actionEffect.Timestamp)
				{
					healEffectEntry.ProcActionName = _statusList.GetStatusNameByID(item.StatusId);
					healEffectEntry.ProcSourceId = item.SourceId;
					healEffectEntry.ProcTargetId = actionEffect.SourceId;
				}
			}
			return;
		}
		if (Array.IndexOf(actionEffect.Effects.OfType<HealEffectEntry>().ToArray(), healEffectEntry) < ((actionEffect.Definition?.IsHeal ?? false) ? 1 : 0))
		{
			return;
		}
		foreach (StatusEffect item2 in combatantState?.StatusEffects.Where((StatusEffect x) => _damageReceivedHealProcs.Value.Contains(x.StatusId) || _healCastHealProcs.Value.Contains(x.StatusId)).ToList())
		{
			if (item2.Timestamp.AddSeconds(item2.Duration + 1.0) > actionEffect.Timestamp)
			{
				healEffectEntry.ProcActionName = _statusList.GetStatusNameByID(item2.StatusId);
				healEffectEntry.ProcSourceId = item2.SourceId;
				healEffectEntry.ProcTargetId = actionEffect.TargetId;
			}
		}
	}
}
