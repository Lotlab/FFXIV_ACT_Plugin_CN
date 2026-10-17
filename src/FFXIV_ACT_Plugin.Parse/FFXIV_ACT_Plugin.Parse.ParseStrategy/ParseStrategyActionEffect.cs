#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;
using FFXIV_ACT_Plugin.Resource;

namespace FFXIV_ACT_Plugin.Parse.ParseStrategy;

public class ParseStrategyActionEffect : IParseStrategy
{
	private readonly IReportCombatData _actWrapper;

	private readonly IEffectEntryStrategyMediator _mediator;

	private readonly IDefinitionRepository _definitionRepo;

	private readonly IActionList _actionList;

	private readonly IDoTSimulator _dotSimulator;

	public ParseStrategyActionEffect(IReportCombatData actWrapper, IEffectEntryStrategyMediator mediator, IDoTSimulator dotSimulator, IDefinitionRepository definitionRepo, IActionList actionList)
	{
		_actWrapper = actWrapper;
		_actionList = actionList;
		_mediator = mediator;
		_definitionRepo = definitionRepo;
		_dotSimulator = dotSimulator;
	}

	public void Parse(EnvironmentState state, DateTime timestamp, string[] data)
	{
		if (data.Length < 46)
		{
			throw new LineFormatException("Line too short.")
			{
				LineData = data,
				ExpectedSegmentCount = 46
			};
		}
		if (!uint.TryParse(data[2], NumberStyles.AllowHexSpecifier, null, out var result))
		{
			throw new LineParseException("Cannot parse Source Id.")
			{
				LineData = data
			};
		}
		if (!uint.TryParse(data[4], NumberStyles.AllowHexSpecifier, null, out var result2))
		{
			throw new LineParseException("Cannot parse Action Id.")
			{
				LineData = data
			};
		}
		if (!uint.TryParse(data[6], NumberStyles.AllowHexSpecifier, null, out var result3))
		{
			throw new LineParseException("Cannot parse Target Id.")
			{
				LineData = data
			};
		}
		if (!uint.TryParse(data[44], NumberStyles.AllowHexSpecifier, null, out var result4))
		{
			throw new LineParseException("Cannot parse Sequence Id.")
			{
				LineData = data
			};
		}
		uint.TryParse(data[24], out var result5);
		uint.TryParse(data[25], out var result6);
		uint.TryParse(data[34], out var result7);
		uint.TryParse(data[35], out var result8);
		if (!int.TryParse(data[45], out var result9))
		{
			throw new LineParseException("Cannot parse index.")
			{
				LineData = data
			};
		}
		List<EffectEntry> list = new List<EffectEntry>();
		for (byte b = 8; b < 24; b += 2)
		{
			EffectEntry effectEntry = ParseEffectEntry(data[b].PadLeft(8, '0') + data[b + 1].PadLeft(8, '0'), (byte)((b - 8) / 2));
			if (effectEntry != null)
			{
				list.Add(effectEntry);
			}
		}
		ActionEffect actionEffect = new ActionEffect
		{
			Timestamp = timestamp,
			SequenceId = result4,
			SourceId = result,
			TargetId = result3,
			ActionId = result2,
			TargetIndex = result9,
			Effects = list.ToArray(),
			SourceCurrentHp = result7,
			SourceMaxHp = result8,
			TargetCurrentHp = result5,
			TargetMaxHp = result6,
			Definition = _definitionRepo.GetActionById(result2)
		};
		EffectEntry[] effects = actionEffect.Effects;
		foreach (EffectEntry effectEntry2 in effects)
		{
			if (effectEntry2 is DamageEffectEntry entry)
			{
				_mediator.ApplyStrategies(state, actionEffect, entry);
			}
			else if (effectEntry2 is HealEffectEntry entry2)
			{
				_mediator.ApplyStrategies(state, actionEffect, entry2);
			}
			else if (effectEntry2 is ResourceEffectEntry entry3)
			{
				_mediator.ApplyStrategies(state, actionEffect, entry3);
			}
		}
		if (state.Combatants.ContainsKey(result))
		{
			state.Combatants[result].ActionEffects.Add(actionEffect);
		}
		if (state.Combatants.ContainsKey(result3))
		{
			state.Combatants[result3].ActionEffects.Add(actionEffect);
		}
		ReportActionEffect(timestamp, actionEffect.SourceId, result3, actionEffect, state);
		if (actionEffect.ActionId == ActionId.Pneuma)
		{
			_dotSimulator.ProcessPneumaTick(state, timestamp, actionEffect.ActionId, actionEffect.SourceId);
		}
	}

	private EffectEntry ParseEffectEntry(string entryText, byte index)
	{
		byte b = Convert.ToByte(entryText.Substring(6, 2), 16);
		if (!Enum.IsDefined(typeof(EffectEntryType), b))
		{
			Trace.WriteLine("ParseEffectEntry: Unknown Effect EntryType for [" + entryText + "].", "FFXIV_ACT_Plugin");
			return null;
		}
		EffectEntry effectEntry;
		switch (b)
		{
		case 0:
			return null;
		case 1:
		case 3:
		case 5:
		case 6:
			effectEntry = new DamageEffectEntry();
			break;
		case 4:
			effectEntry = new HealEffectEntry();
			break;
		case 26:
			effectEntry = new ResourceEffectEntry
			{
				ResourceType = ResourceType.Threat
			};
			break;
		case 25:
			effectEntry = new ResourceEffectEntry
			{
				ResourceType = ResourceType.Threat,
				Increase = true
			};
			break;
		case 24:
			effectEntry = new ResourceEffectEntry
			{
				ResourceType = ResourceType.Threat
			};
			break;
		case 11:
			effectEntry = new ResourceEffectEntry
			{
				ResourceType = ResourceType.Power,
				Increase = true
			};
			break;
		case 10:
			effectEntry = new ResourceEffectEntry
			{
				ResourceType = ResourceType.Power
			};
			break;
		default:
			effectEntry = new EffectEntry();
			break;
		}
		effectEntry.EntryType = (EffectEntryType)b;
		effectEntry.Param0 = Convert.ToByte(entryText.Substring(4, 2), 16);
		effectEntry.Param1 = Convert.ToByte(entryText.Substring(2, 2), 16);
		effectEntry.Param2 = Convert.ToByte(entryText.Substring(0, 2), 16);
		effectEntry.Flags1 = Convert.ToByte(entryText.Substring(14, 2), 16);
		effectEntry.Flags2 = Convert.ToByte(entryText.Substring(12, 2), 16);
		effectEntry.Value = Convert.ToUInt16(entryText.Substring(8, 4), 16);
		effectEntry.EffectIndex = index;
		return effectEntry;
	}

	private void ReportActionEffect(DateTime timestamp, uint sourceId, uint targetId, ActionEffect effect, EnvironmentState state)
	{
		EffectEntry[] effects = effect.Effects;
		foreach (EffectEntry effectEntry in effects)
		{
			if (effectEntry is DamageEffectEntry damageEffectEntry)
			{
				IReportCombatData actWrapper = _actWrapper;
				EnvironmentState state2 = state;
				DateTime timestamp2 = timestamp;
				uint sourceId2 = ((damageEffectEntry.ProcSourceId != 0) ? damageEffectEntry.ProcSourceId : effect.SourceId);
				uint targetId2;
				if (damageEffectEntry.ProcTargetId != 0)
				{
					targetId2 = damageEffectEntry.ProcTargetId;
				}
				else
				{
					targetId2 = (effectEntry.IsSourceEntry ? effect.SourceId : effect.TargetId);
				}
				actWrapper.AddDamageEntry(state2, timestamp2, sourceId2, targetId2, damageEffectEntry);
			}
			else if (effectEntry is HealEffectEntry healEffectEntry)
			{
				IReportCombatData actWrapper2 = _actWrapper;
				EnvironmentState state3 = state;
				DateTime timestamp3 = timestamp;
				uint sourceId3 = ((healEffectEntry.ProcSourceId != 0) ? healEffectEntry.ProcSourceId : effect.SourceId);
				uint targetId3;
				if (healEffectEntry.ProcTargetId != 0)
				{
					targetId3 = healEffectEntry.ProcTargetId;
				}
				else
				{
					targetId3 = (effectEntry.IsSourceEntry ? effect.SourceId : effect.TargetId);
				}
				actWrapper2.AddHealEntry(state3, timestamp3, sourceId3, targetId3, healEffectEntry);
			}
			else if (effectEntry is ResourceEffectEntry resourceEffectEntry)
			{
				_actWrapper.AddResourceEntry(state, timestamp, sourceId, resourceEffectEntry.IsSourceEntry ? sourceId : targetId, resourceEffectEntry);
			}
		}
		if (!effect.Effects.Any((EffectEntry x) => x is DamageEffectEntry || x is HealEffectEntry || x is ResourceEffectEntry || x.EntryType == EffectEntryType.ApplyStatusEffectSource || x.EntryType == EffectEntryType.ApplyStatusEffectTarget))
		{
			string actionNameByID = _actionList.GetActionNameByID(effect.ActionId);
			_actWrapper.AddActionEntry(state, timestamp, effect.SourceId, effect.TargetId, actionNameByID);
		}
	}
}
