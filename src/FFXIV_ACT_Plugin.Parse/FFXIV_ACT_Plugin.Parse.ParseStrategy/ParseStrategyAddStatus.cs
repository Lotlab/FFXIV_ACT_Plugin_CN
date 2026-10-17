using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Resource;
using FFXIV_ACT_Plugin.Resource.Definitions;

namespace FFXIV_ACT_Plugin.Parse.ParseStrategy;

public class ParseStrategyAddStatus : IParseStrategy
{
	private readonly IReportCombatData _actWrapper;

	private readonly IStatusList _statusList;

	private readonly IDefinitionRepository _definitionRepo;

	private readonly DamageShieldSimulator _damageShieldSimulator;

	private readonly IDoTSimulator _dotSimulator;

	private readonly ISettingsMediator _settingsMediator;

	public ParseStrategyAddStatus(IStatusList statusList, IReportCombatData actWrapper, ISettingsMediator settingsMediator, DamageShieldSimulator damageShieldSimulator, IDefinitionRepository definitionRepo, IDoTSimulator dotSimulator)
	{
		_statusList = statusList;
		_actWrapper = actWrapper;
		_damageShieldSimulator = damageShieldSimulator;
		_definitionRepo = definitionRepo;
		_dotSimulator = dotSimulator;
		_settingsMediator = settingsMediator;
	}

	public void Parse(EnvironmentState state, DateTime timestamp, string[] data)
	{
		if (data.Length < 10)
		{
			throw new LineFormatException("Line too short.")
			{
				LineData = data,
				ExpectedSegmentCount = 10
			};
		}
		if (!uint.TryParse(data[2], NumberStyles.AllowHexSpecifier, null, out var result))
		{
			throw new LineParseException("Cannot parse Status Id.")
			{
				LineData = data
			};
		}
		if (!double.TryParse(data[4], NumberStyles.Any, CultureInfo.InvariantCulture, out var result2))
		{
			throw new LineParseException("Cannot parse Duration.")
			{
				LineData = data
			};
		}
		if (!uint.TryParse(data[5], NumberStyles.AllowHexSpecifier, null, out var result3))
		{
			throw new LineParseException("Cannot parse Source Id.")
			{
				LineData = data
			};
		}
		if (!uint.TryParse(data[7], NumberStyles.AllowHexSpecifier, null, out var result4))
		{
			throw new LineParseException("Cannot parse TargetId Id.")
			{
				LineData = data
			};
		}
		if (!ushort.TryParse(data[9], NumberStyles.AllowHexSpecifier, null, out var result5))
		{
			throw new LineParseException("Cannot parse Status Param.")
			{
				LineData = data
			};
		}
		if (!state.Combatants.ContainsKey(result4))
		{
			throw new LookupException($"Cannot find target id {result4:X4}");
		}
		CombatantState target = state.Combatants[result4];
		CombatantState source = (state.Combatants.TryGetValue(result3, out var value) ? value : null);
		string statusNameByID = _statusList.GetStatusNameByID(result);
		ActionEffect actionEffect = GetActionEffect(state, source, target, result, result4);
		StatusEffect statusEffect = AddOrUpdateStatus(timestamp, target, result3, result, result2, result5, actionEffect);
		_actWrapper.AddStatusEffect(state, timestamp, statusNameByID, result3, result4, statusEffect.Duration, statusEffect.Param1, statusEffect.EffectByte0, statusEffect.EffectByte1, statusEffect.EffectByte2);
		_damageShieldSimulator.ProcessDamageShield(state, statusEffect, source, target);
		_dotSimulator.CreateSimulatedDamageState(state, statusEffect, target);
		ProcessDeferredEvents(state, timestamp, statusEffect, result3, result4);
	}

	private ActionEffect GetActionEffect(EnvironmentState state, CombatantState source, CombatantState target, uint statusId, uint targetId)
	{
		ActionEffect actionEffect = null;
		if (statusId == StatusId.StandardFinishPartner)
		{
			targetId = source.Id;
		}
		uint[] overrideStatusId = GetOverrideStatusIds(statusId);
		if (overrideStatusId != null)
		{
			return (from x in state.Combatants.SelectMany((KeyValuePair<uint, CombatantState> x) => x.Value.ActionEffects.Where((ActionEffect y) => y.Effects.Any((EffectEntry z) => overrideStatusId.Contains(z.Value) && ((z.EntryType == EffectEntryType.ApplyStatusEffectSource && y.TargetId == source.Id) || (z.EntryType == EffectEntryType.ApplyStatusEffectTarget && y.TargetId == targetId)))))
				orderby x.Timestamp descending
				select x).FirstOrDefault();
		}
		actionEffect = source?.ActionEffects.Where((ActionEffect x) => x.Effects.Any((EffectEntry y) => y.Value == statusId && ((y.EntryType == EffectEntryType.ApplyStatusEffectSource && x.SourceId == targetId) || (y.EntryType == EffectEntryType.ApplyStatusEffectTarget && x.TargetId == targetId)))).OrderByDescending((ActionEffect x) => x.Timestamp).FirstOrDefault();
		if (actionEffect == null && target.ParentId != 0)
		{
			actionEffect = state.Combatants.FirstOrDefault((KeyValuePair<uint, CombatantState> x) => x.Key == target.ParentId).Value?.ActionEffects.Where((ActionEffect x) => x.TargetId == target.ParentId && x.Effects.Any((EffectEntry y) => y.Value == statusId && y.EntryType == EffectEntryType.ApplyStatusEffectTarget)).OrderByDescending((ActionEffect x) => x.Timestamp).FirstOrDefault();
		}
		return actionEffect;
	}

	private uint[] GetOverrideStatusIds(uint statusId)
	{
		uint[] result = null;
		if (statusId == StatusId.SacredSoilGround)
		{
			result = new uint[2]
			{
				StatusId.SacredSoilSelf,
				StatusId.SacredSoilSelfNoRegen
			};
		}
		else if (statusId == StatusId.CrestOfTimeReturned)
		{
			result = new uint[1] { StatusId.CrestOfTimeBorrowed };
		}
		else if (statusId == StatusId.WheelOfFortune)
		{
			result = new uint[1] { StatusId.CollectiveUnconsciousSelf };
		}
		else if (statusId == StatusId.Improvisation)
		{
			result = new uint[1] { StatusId.ImprovisationSelf };
		}
		else if (statusId == StatusId.UndyingFlame)
		{
			result = new uint[1] { StatusId.Rekindle };
		}
		else if (statusId == StatusId.StandardFinishPartner)
		{
			result = new uint[1] { StatusId.StandardFinish };
		}
		return result;
	}

	private StatusEffect AddOrUpdateStatus(DateTime timestamp, CombatantState target, uint sourceId, uint statusId, double duration, ushort statusParam, ActionEffect actionEffect)
	{
		if (target == null)
		{
			return null;
		}
		StatusEffect statusEffect = target.StatusEffects.FirstOrDefault((StatusEffect x) => x.SourceId == sourceId && x.StatusId == statusId);
		if (statusEffect == null)
		{
			statusEffect = new StatusEffect
			{
				SourceId = sourceId,
				Duration = duration,
				Param1 = statusParam,
				StatusId = statusId,
				Timestamp = timestamp,
				Definition = _definitionRepo.GetStatusEffectById(statusId),
				ActionEffect = actionEffect
			};
			target.StatusEffects.Add(statusEffect);
		}
		else
		{
			statusEffect.Timestamp.AddSeconds(statusEffect.Duration);
			statusEffect.Duration = duration;
			statusEffect.Timestamp = timestamp;
			if (statusEffect.Definition?.TimeProc == null)
			{
				statusEffect.Param1 = statusParam;
			}
			statusEffect.ActionEffect = actionEffect ?? statusEffect.ActionEffect;
		}
		if (statusEffect.ActionEffect != null)
		{
			uint[] overrideStatusId = GetOverrideStatusIds(statusId) ?? new uint[1] { statusId };
			EffectEntry effectEntry = statusEffect.ActionEffect.Effects.Where((EffectEntry y) => (y.EntryType == EffectEntryType.ApplyStatusEffectSource || y.EntryType == EffectEntryType.ApplyStatusEffectTarget) && y.Value == overrideStatusId.First()).FirstOrDefault();
			if (effectEntry != null)
			{
				statusEffect.EffectByte0 = effectEntry.Param2;
				statusEffect.EffectByte1 = effectEntry.Param1;
				statusEffect.EffectByte2 = effectEntry.Param0;
			}
		}
		if (statusEffect.Definition?.PotencyEffects?.Any((Potency x) => x.AmountByte.HasValue) == true && !statusEffect.EffectByte0.HasValue)
		{
			_settingsMediator.AddExceptionMessage(timestamp, $"Status {statusId:X2} {statusEffect.Definition.Name} added to target {target.Id:X4} but cannot find ActionEffect.");
		}
		return statusEffect;
	}

	private void ProcessDeferredEvents(EnvironmentState state, DateTime timestamp, StatusEffect status, uint sourceId, uint targetId)
	{
		FFXIV_ACT_Plugin.Resource.Definitions.StatusEffect definition = status.Definition;
		if (definition == null || definition.TimeProc?.Type != TimeProc.TickType.GroundHeal)
		{
			FFXIV_ACT_Plugin.Resource.Definitions.StatusEffect definition2 = status.Definition;
			if (definition2 == null || definition2.TimeProc?.Type != TimeProc.TickType.GroundDamage)
			{
				return;
			}
		}
		foreach (CombatantState item in state.Combatants.Values.Where((CombatantState x) => x.DeferredEvents.Any()))
		{
			for (int num = item.DeferredEvents.Count - 1; num >= 0; num--)
			{
				DeferredEvent deferredEvent = item.DeferredEvents[num];
				if (deferredEvent.StatusId == status.StatusId)
				{
					_dotSimulator.ProcessTick(state, deferredEvent.Timestamp, deferredEvent.Type == DeferredEvent.EventType.HoT, deferredEvent.TargetId, deferredEvent.StatusId, deferredEvent.Amount, deferredEvent.TargetCurrentHp, deferredEvent.TargetMaxHp, deferredEvent.SourceId);
					item.DeferredEvents.RemoveAt(num);
				}
			}
		}
	}
}
