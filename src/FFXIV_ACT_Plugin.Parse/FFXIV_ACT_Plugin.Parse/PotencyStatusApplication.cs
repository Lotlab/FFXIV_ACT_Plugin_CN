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
		foreach (StatusEffect item in list.Where((StatusEffect x) => x.Definition?.PotencyEffects.Any() ?? false))
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
		foreach (StatusEffect item2 in from x in targetState.StatusEffects.Where((StatusEffect x) => x.Timestamp <= timestamp && (x.Duration == 0.0 || x.Timestamp.AddSeconds(x.Duration) >= timestamp)).ToList()
			where x.Definition?.PotencyEffects.Any() ?? false
			select x)
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
		if (entry is DamageEffectEntry && sourceEffects.Any((StatusEffect x) => _excludeEffects.Contains(x.StatusId)) && sourceState.Tracking.DamagePotencyHistoryIndex > 10)
		{
			entry.CalculatedPotency = 0.0;
		}
	}

	private void ApplyStatusEffectToDamageEntry(StatusEffect effect, IEffectAppliable entry, uint zoneId, bool isSource, DateTime timestamp, uint actionId, byte echoPercent)
	{
		if ((effect.StatusId == StatusId.OffGuard && !isSource) || (effect.AppliedTimestamp != DateTime.MinValue && effect.AppliedTimestamp < timestamp))
		{
			return;
		}
		ActionCategory actionCategory = _actionList.GetActionCategory(actionId);
		foreach (Potency item in effect.Definition.PotencyEffects.Where((Potency x) => (x.Type == Potency.EffectType.DamageAddPotency || x.Type == Potency.EffectType.DamageDoneMultiplier || x.Type == Potency.EffectType.DamageReceivedMultiplier || x.Type == Potency.EffectType.PotencyMultiplier) && (!x.LimitToActionIds.Any() || x.LimitToActionIds.Contains(actionId)) && (!x.LimitToZoneId.HasValue || x.LimitToZoneId == zoneId) && (x.LimitToActionCategory == ActionCategory.Unknown || x.LimitToActionCategory == actionCategory) && (x.LimitToDamageType == DamageType.Unknown || x.LimitToDamageType == entry.DamageType || (x.LimitToDamageType == DamageType.Physical && (entry.DamageType == DamageType.Blunt || entry.DamageType == DamageType.Piercing || entry.DamageType == DamageType.Slashing)))))
		{
			if (item.Type == Potency.EffectType.DamageAddPotency)
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
			else if ((item.Type == Potency.EffectType.PotencyMultiplier) & isSource)
			{
				double num = item.Amount;
				entry.CalculatedPotency *= 1.0 + num / 100.0;
				entry.AppliedStatusEffects += $"{effect.Definition.Name}(P {num}%) ";
			}
			else if ((item.Type == Potency.EffectType.DamageDoneMultiplier) & isSource)
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
			else if (item.Type == Potency.EffectType.DamageReceivedMultiplier && !isSource)
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
		ActionCategory actionCategory = _actionList.GetActionCategory(actionId);
		if (actionCategory != ActionCategory.Spell && actionCategory != ActionCategory.Ability && effect.StatusId != StatusId.Medicated && effect.StatusId != StatusId.Weakness && effect.StatusId != StatusId.BrinkOfDeath)
		{
			return;
		}
		foreach (Potency item in effect?.Definition?.PotencyEffects.Where((Potency x) => (x.Type == Potency.EffectType.HealDoneMultiplier || x.Type == Potency.EffectType.HealReceiveMultiplier) && (x.LimitToActionCategory == ActionCategory.Unknown || x.LimitToActionCategory == actionCategory) && (!x.LimitToZoneId.HasValue || x.LimitToZoneId == zoneId) && (x.LimitToDamageType == DamageType.Unknown || x.LimitToDamageType == entry.DamageType)))
		{
			if ((item.Type == Potency.EffectType.HealDoneMultiplier) & isSource)
			{
				double num = 0.0;
				num = ((effect.StatusId != StatusId.Echo) ? ((double)(item.Amount * ((!item.IsStacked) ? 1 : effect.Param1))) : ((double)(int)echoPercent));
				entry.CalculatedPotencyMultiplier += num / 100.0;
				entry.AppliedStatusEffects += $"{effect.Definition.Name}({num}%),";
			}
			else if (item.Type == Potency.EffectType.HealReceiveMultiplier && !isSource)
			{
				double num2 = item.Amount * ((!item.IsStacked) ? 1 : effect.Param1);
				entry.CalculatedPotencyMultiplier += num2 / 100.0;
				entry.AppliedStatusEffects += $"{effect.Definition.Name}({num2}%),";
			}
		}
	}
}
