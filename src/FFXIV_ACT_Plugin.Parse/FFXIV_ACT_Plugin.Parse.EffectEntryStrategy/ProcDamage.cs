using System;
using System.Linq;
using FFXIV_ACT_Plugin.Resource;
using FFXIV_ACT_Plugin.Resource.Definitions;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public class ProcDamage : IEffectEntryStrategy
{
	private readonly IStatusList _statusList;

	private readonly Lazy<uint[]> _receivedProcs;

	public Type[] EffectEntryTypes => new Type[1] { typeof(DamageEffectEntry) };

	public int Order => 0;

	public ProcDamage(IStatusList statusList, IDefinitionRepository definitionRepository)
	{
		_statusList = statusList;
		_receivedProcs = new Lazy<uint[]>(() => definitionRepository.GetStatusIdsByReactiveProcType((ReactiveProc.ReactiveProcType)1));
	}

	public void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry
	{
		if (!(entry is DamageEffectEntry damageEffectEntry) || !state.Combatants.ContainsKey(actionEffect.TargetId))
		{
			return;
		}
		CombatantState combatantState = state.Combatants[actionEffect.TargetId];
		if (!entry.IsSourceEntry)
		{
			return;
		}
		foreach (StatusEffect item in combatantState?.StatusEffects.Where((StatusEffect x) => Enumerable.Contains(_receivedProcs.Value, x.StatusId)).ToList())
		{
			if (item.Timestamp.AddSeconds(item.Duration + 1.0) > actionEffect.Timestamp)
			{
				damageEffectEntry.ProcActionName = _statusList.GetStatusNameByID(item.StatusId);
				damageEffectEntry.ProcSourceId = item.SourceId;
				damageEffectEntry.ProcTargetId = actionEffect.SourceId;
			}
		}
	}
}
