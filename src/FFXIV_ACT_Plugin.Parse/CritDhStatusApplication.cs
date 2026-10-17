using System;
using System.Linq;
using FFXIV_ACT_Plugin.Resource;
using FFXIV_ACT_Plugin.Resource.Definitions;

namespace FFXIV_ACT_Plugin.Parse;

public class CritDhStatusApplication
{
	private readonly IStatusList _statusList;

	public CritDhStatusApplication(IStatusList statusList)
	{
		_statusList = statusList;
	}

	public double CalculateCrit(IEffectAppliable damageState, DateTime timestamp, CombatantState sourceState, CombatantState targetState, uint? actionId)
	{
		foreach (StatusEffect item in sourceState.StatusEffects.Where((StatusEffect x) =>
		{
			FFXIV_ACT_Plugin.Resource.Definitions.StatusEffect definition = x.Definition;
			return definition != null && definition.Multipliers.Any((Multiplier y) =>
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Invalid comparison between Unknown and I4
				return (int)y.Type == 1 && (!y.LimitToActionIds.Any() || Enumerable.Contains(y.LimitToActionIds, actionId.GetValueOrDefault()));
			}) && x.Timestamp.AddSeconds(x.Duration) >= timestamp.AddSeconds(1.0);
		}).ToList())
		{
			double num = item.Definition.Multipliers.First((Multiplier x) =>
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Invalid comparison between Unknown and I4
				return (int)x.Type == 1;
			}).Amount;
			damageState.AppliedCritStatusList += $"{_statusList.GetStatusNameByID(item.StatusId)} ({num}%) ";
			damageState.CritBuffAmount += num;
		}
		if (targetState != null && targetState.StatusEffects.Any((StatusEffect x) => x.StatusId == StatusId.ChainStrategem))
		{
			damageState.AppliedCritStatusList = damageState.AppliedCritStatusList + _statusList.GetStatusNameByID(StatusId.ChainStrategem) + " (10%)";
			damageState.CritBuffAmount += 10.0;
		}
		if (actionId == ActionId.InnerChaos || actionId == ActionId.ChaoticCyclone || actionId == ActionId.PrimalRend || actionId == ActionId.Bootshine || actionId == ActionId.ShadowOfTheDestroyer || actionId == ActionId.Assassinate || actionId == ActionId.KaeshiSetsugekka || actionId == ActionId.OgiNamikiri || actionId == ActionId.KaeshiNamikiri || actionId == ActionId.FullMetalfield)
		{
			damageState.AppliedCritStatusList = damageState.AppliedCritStatusList + damageState.ActionName + " (100%) ";
			damageState.CritBuffAmount = 100.0;
		}
		if (actionId != ActionId.Attack && sourceState.StatusEffects.Any((StatusEffect x) => x.StatusId == StatusId.LifeSurge))
		{
			damageState.AppliedCritStatusList = damageState.AppliedCritStatusList + _statusList.GetStatusNameByID(StatusId.LifeSurge) + " (100%) ";
			damageState.CritBuffAmount = 100.0;
		}
		if (actionId != ActionId.Shot && sourceState.StatusEffects.Any((StatusEffect x) => x.StatusId == StatusId.Reassemble))
		{
			damageState.AppliedCritStatusList = damageState.AppliedCritStatusList + _statusList.GetStatusNameByID(StatusId.Reassemble) + " (100%) ";
			damageState.CritBuffAmount = 100.0;
		}
		double num2 = 0.0;
		if (sourceState.Tracking.CriticalDamageRateSwings > 10)
		{
			num2 = sourceState.Tracking.CriticalDamageRate + damageState.CritBuffAmount / 100.0;
		}
		if (num2 > 1.0)
		{
			num2 = 1.0;
		}
		return num2;
	}

	public double CalculateDirectHit(IEffectAppliable damageState, DateTime timestamp, CombatantState sourceState, uint? actionId)
	{
		if (actionId == ActionId.FullMetalfield)
		{
			damageState.AppliedDirectHitStatusList = damageState.AppliedDirectHitStatusList + damageState.ActionName + " (100%) ";
			damageState.DirectHitBuffs = 100.0;
		}
		foreach (StatusEffect item in sourceState.StatusEffects.Where((StatusEffect x) =>
		{
			FFXIV_ACT_Plugin.Resource.Definitions.StatusEffect definition = x.Definition;
			return definition != null && definition.Multipliers.Any((Multiplier y) =>
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Invalid comparison between Unknown and I4
				return (int)y.Type == 2 && (!y.LimitToActionIds.Any() || Enumerable.Contains(y.LimitToActionIds, actionId.GetValueOrDefault()));
			}) && x.Timestamp.AddSeconds(x.Duration) >= timestamp.AddSeconds(1.0);
		}).ToList())
		{
			double num = item.Definition.Multipliers.First((Multiplier x) =>
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Invalid comparison between Unknown and I4
				return (int)x.Type == 2;
			}).Amount;
			damageState.AppliedDirectHitStatusList += $"{_statusList.GetStatusNameByID(item.StatusId)} ({num}%) ";
			damageState.DirectHitBuffs += num;
		}
		double num2 = sourceState.Level / 2;
		num2 = ((sourceState.Tracking.DirectHitRateSwings <= 10) ? 0.05 : (sourceState.Tracking.DirectHitRate + damageState.DirectHitBuffs / 100.0));
		if (num2 > 1.0)
		{
			num2 = 1.0;
		}
		return num2;
	}
}
