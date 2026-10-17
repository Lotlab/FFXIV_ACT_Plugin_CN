using System;
using System.Collections.Generic;
using System.Linq;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Resource;
using FFXIV_ACT_Plugin.Resource.Definitions;

namespace FFXIV_ACT_Plugin.Parse;

public class PotencyStatusApplication : IPotencyStatusApplication
{
	private readonly IActionList _actionList;

	private readonly ISettingsMediator _settingsMediator;

	private readonly uint[] _excludeEffects = new uint[3]
	{
		StatusId.Medicated,
		StatusId.Weakness,
		StatusId.BrinkOfDeath
	};

	public PotencyStatusApplication(IActionList actionList, ISettingsMediator settingsMediator)
	{
		_actionList = actionList;
		_settingsMediator = settingsMediator;
	}

	public void ApplyStatusEffects(DateTime timestamp, CombatantState sourceState, CombatantState targetState, IEffectAppliable entry, uint zoneId, uint actionId, byte echoPercent)
	{
		List<StatusEffect> list = sourceState.StatusEffects.Where((StatusEffect x) => x.Timestamp <= timestamp && (x.Duration == 0.0 || x.Timestamp.AddSeconds(x.Duration + 1.0) >= timestamp)).ToList();
		foreach (StatusEffect item in list.Where((StatusEffect x) =>
		{
			FFXIV_ACT_Plugin.Resource.Definitions.StatusEffect definition = x.Definition;
			return definition != null && definition.PotencyEffects.Any();
		}))
		{
			if (entry.IsHeal)
			{
				ApplyStatusEffectToHealEntry(item, entry, zoneId, isSource: true, actionId, echoPercent);
			}
			else
			{
				ApplyStatusEffectToDamageEntry(item, entry, zoneId, isSource: true, timestamp, actionId, echoPercent);
			}
		}
		foreach (StatusEffect item2 in targetState.StatusEffects.Where((StatusEffect x) => x.Timestamp <= timestamp && (x.Duration == 0.0 || x.Timestamp.AddSeconds(x.Duration) >= timestamp)).ToList().Where((StatusEffect x) =>
		{
			FFXIV_ACT_Plugin.Resource.Definitions.StatusEffect definition = x.Definition;
			return definition != null && definition.PotencyEffects.Any();
		}))
		{
			if (entry.IsHeal)
			{
				ApplyStatusEffectToHealEntry(item2, entry, zoneId, isSource: false, actionId, echoPercent);
			}
			else if ((item2.StatusId != StatusId.DeathsDesign && item2.StatusId != StatusId.TrickAttack) || item2.SourceId == sourceState.Id)
			{
				ApplyStatusEffectToDamageEntry(item2, entry, zoneId, isSource: false, timestamp, actionId, echoPercent);
			}
		}
		IgnoreSomeStatusesExceptAtStart(sourceState, list, entry);
	}

	private void IgnoreSomeStatusesExceptAtStart(CombatantState sourceState, List<StatusEffect> sourceEffects, IEffectAppliable entry)
	{
		if (entry is DamageEffectEntry && sourceEffects.Any((StatusEffect x) => Enumerable.Contains(_excludeEffects, x.StatusId)) && sourceState.Tracking.DamagePotencyHistoryIndex > 10)
		{
			entry.CalculatedPotency = 0.0;
		}
	}

	private void ApplyStatusEffectToDamageEntry(StatusEffect effect, IEffectAppliable entry, uint zoneId, bool isSource, DateTime timestamp, uint actionId, byte echoPercent)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Invalid comparison between Unknown and I4
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Invalid comparison between Unknown and I4
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Invalid comparison between Unknown and I4
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Invalid comparison between Unknown and I4
		if ((effect.StatusId == StatusId.OffGuard && !isSource) || (effect.AppliedTimestamp != DateTime.MinValue && effect.AppliedTimestamp < timestamp))
		{
			return;
		}
		ActionCategory actionCategory = _actionList.GetActionCategory(actionId);
		foreach (Potency item in effect.Definition.PotencyEffects.Where((Potency x) =>
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Invalid comparison between Unknown and I4
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Invalid comparison between Unknown and I4
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Invalid comparison between Unknown and I4
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Invalid comparison between Unknown and I4
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Invalid comparison between Unknown and I4
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Invalid comparison between Unknown and I4
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Invalid comparison between Unknown and I4
			return ((int)x.Type == 1 || (int)x.Type == 2 || (int)x.Type == 4 || (int)x.Type == 3) && (!x.LimitToActionIds.Any() || Enumerable.Contains(x.LimitToActionIds, actionId)) && (!x.LimitToZoneId.HasValue || x.LimitToZoneId == zoneId) && ((int)x.LimitToActionCategory == 0 || x.LimitToActionCategory == actionCategory) && ((int)x.LimitToDamageType == 0 || x.LimitToDamageType == entry.DamageType || ((int)x.LimitToDamageType == 7 && ((int)entry.DamageType == 3 || (int)entry.DamageType == 2 || (int)entry.DamageType == 1)));
		}))
		{
			if ((int)item.Type == 1)
			{
				if (!(entry is SimulatedDamageState))
				{
					entry.CalculatedPotency += item.Amount;
					entry.AppliedStatusEffects += $"{effect.Definition.Name}(+{item.Amount}) ";
					if (effect.StatusId == StatusId.Tingling)
					{
						effect.AppliedTimestamp = timestamp;
					}
				}
			}
			else if (((int)item.Type == 3) & isSource)
			{
				double num = item.Amount;
				entry.CalculatedPotency *= 1.0 + num / 100.0;
				entry.AppliedStatusEffects += $"{effect.Definition.Name}(P {num}%) ";
			}
			else if (((int)item.Type == 2) & isSource)
			{
				double num2;
				if (effect.StatusId == StatusId.Echo)
				{
					num2 = (int)echoPercent;
				}
				else
				{
					num2 = ((!item.AmountByte.HasValue) ? ((double)(item.Amount * ((!item.IsStacked) ? 1 : effect.Param1))) : ((double)(((item.AmountByte == 0) ? ((sbyte)effect.EffectByte0.GetValueOrDefault()) : ((item.AmountByte == 1) ? ((sbyte)effect.EffectByte1.GetValueOrDefault()) : ((sbyte)effect.EffectByte2.GetValueOrDefault()))) * ((!item.IsStacked) ? 1 : effect.Param1))));
				}
				entry.CalculatedPotencyMultiplier += num2 / 100.0;
				entry.AppliedStatusEffects += $"{effect.Definition.Name}({num2}%) ";
				if (effect.StatusId == StatusId.Kaiten || effect.StatusId == StatusId.Boost || effect.StatusId == StatusId.Kassatsu || effect.StatusId == StatusId.LogosBoost || effect.StatusId == StatusId.Harmonized)
				{
					effect.AppliedTimestamp = timestamp;
				}
			}
			else if ((int)item.Type == 4 && !isSource)
			{
				double num3 = ((!item.AmountByte.HasValue) ? ((double)(item.Amount * ((!item.IsStacked) ? 1 : effect.Param1))) : ((double)(((!item.IsStacked) ? 1 : effect.Param1) * ((item.AmountByte == 0) ? effect.EffectByte0.GetValueOrDefault() : ((item.AmountByte == 1) ? effect.EffectByte1.GetValueOrDefault() : effect.EffectByte2.GetValueOrDefault())))));
				entry.CalculatedPotencyMultiplier += num3 / 100.0;
				entry.AppliedStatusEffects += $"{effect.Definition.Name}({num3}%) ";
			}
		}
		if (entry.CalculatedPotencyMultiplier < 0.0)
		{
			_settingsMediator.AddExceptionMessage(timestamp, $"Invalid potency multiplier {entry.CalculatedPotencyMultiplier:F2} for status {effect.StatusId:X2}.");
			entry.CalculatedPotencyMultiplier = 1.0;
		}
	}

	private void ApplyStatusEffectToHealEntry(StatusEffect effect, IEffectAppliable entry, uint zoneId, bool isSource, uint actionId, byte echoPercent)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Invalid comparison between Unknown and I4
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Invalid comparison between Unknown and I4
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Invalid comparison between Unknown and I4
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Invalid comparison between Unknown and I4
		ActionCategory actionCategory = _actionList.GetActionCategory(actionId);
		if ((int)actionCategory != 2 && (int)actionCategory != 4 && effect.StatusId != StatusId.Medicated && effect.StatusId != StatusId.Weakness && effect.StatusId != StatusId.BrinkOfDeath)
		{
			return;
		}
		object obj;
		if (effect == null)
		{
			obj = null;
		}
		else
		{
			FFXIV_ACT_Plugin.Resource.Definitions.StatusEffect definition = effect.Definition;
			obj = ((definition != null) ? definition.PotencyEffects.Where((Potency x) =>
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Invalid comparison between Unknown and I4
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Invalid comparison between Unknown and I4
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				return ((int)x.Type == 5 || (int)x.Type == 6) && ((int)x.LimitToActionCategory == 0 || x.LimitToActionCategory == actionCategory) && (!x.LimitToZoneId.HasValue || x.LimitToZoneId == zoneId) && ((int)x.LimitToDamageType == 0 || x.LimitToDamageType == entry.DamageType);
			}) : null);
		}
		foreach (Potency item in (IEnumerable<Potency>)obj)
		{
			if (((int)item.Type == 5) & isSource)
			{
				double num = 0.0;
				num = ((effect.StatusId != StatusId.Echo) ? ((double)(item.Amount * ((!item.IsStacked) ? 1 : effect.Param1))) : ((double)(int)echoPercent));
				entry.CalculatedPotencyMultiplier += num / 100.0;
				entry.AppliedStatusEffects += $"{effect.Definition.Name}({num}%),";
			}
			else if ((int)item.Type == 6 && !isSource)
			{
				double num2 = item.Amount * ((!item.IsStacked) ? 1 : effect.Param1);
				entry.CalculatedPotencyMultiplier += num2 / 100.0;
				entry.AppliedStatusEffects += $"{effect.Definition.Name}({num2}%),";
			}
		}
	}
}
