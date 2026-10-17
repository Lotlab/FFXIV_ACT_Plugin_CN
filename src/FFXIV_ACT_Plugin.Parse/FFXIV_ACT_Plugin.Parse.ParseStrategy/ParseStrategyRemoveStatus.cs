using System;
using System.Globalization;
using System.Linq;
using FFXIV_ACT_Plugin.Resource;

namespace FFXIV_ACT_Plugin.Parse.ParseStrategy;

public class ParseStrategyRemoveStatus : IParseStrategy
{
	private readonly IDeferredEventProcessor _deferredProcessor;

	private readonly IDefinitionRepository _definitionRepository;

	public ParseStrategyRemoveStatus(IDeferredEventProcessor deferredProcessor, IDefinitionRepository definitionRepository)
	{
		_deferredProcessor = deferredProcessor;
		_definitionRepository = definitionRepository;
	}

	public void Parse(EnvironmentState state, DateTime timestamp, string[] data)
	{
		if (data.Length < 8)
		{
			throw new LineFormatException("Line too short.")
			{
				LineData = data,
				ExpectedSegmentCount = 8
			};
		}
		if (!uint.TryParse(data[2], NumberStyles.AllowHexSpecifier, null, out var statusId))
		{
			throw new LineParseException("Cannot parse Status Id.")
			{
				LineData = data
			};
		}
		if (!uint.TryParse(data[5], NumberStyles.AllowHexSpecifier, null, out var result))
		{
			throw new LineParseException("Cannot parse Source Id.")
			{
				LineData = data
			};
		}
		if (!uint.TryParse(data[7], NumberStyles.AllowHexSpecifier, null, out var result2))
		{
			throw new LineParseException("Cannot parse TargetId Id.")
			{
				LineData = data
			};
		}
		if (!state.Combatants.ContainsKey(result2))
		{
			throw new LookupException($"Cannot find Target Id {result2:X4} in current state.");
		}
		CombatantState combatantState = state.Combatants[result2];
		StatusEffect statusEffect = null;
		int num = -1;
		for (int num2 = combatantState.StatusEffects.Count - 1; num2 >= 0; num2--)
		{
			if (combatantState.StatusEffects[num2].StatusId == statusId && combatantState.StatusEffects[num2].SourceId == result)
			{
				statusEffect = combatantState.StatusEffects[num2];
				num = num2;
				break;
			}
		}
		if (_definitionRepository.GetStatusEffectById(statusId)?.TimeProc != null)
		{
			foreach (CombatantState item in state.Combatants.Values.Where((CombatantState x) => x.DeferredEvents.Any((DeferredEvent y) => y.StatusId == statusId)))
			{
				foreach (DeferredEvent item2 in item.DeferredEvents.Where((DeferredEvent x) => x.StatusId == statusId).ToList())
				{
					_deferredProcessor.ProcessEvent(state, item, item2);
				}
			}
		}
		if (num >= 0)
		{
			combatantState.StatusEffects.RemoveAt(num);
		}
		for (int num3 = 0; num3 < combatantState.SimulatedDamage.Count; num3++)
		{
			if (combatantState.SimulatedDamage[num3].StatusId == statusId && combatantState.SimulatedDamage[num3].SourceId == result)
			{
				if (statusEffect != null)
				{
					combatantState.SimulatedDamage.RemoveAt(num3);
				}
				break;
			}
		}
	}
}
