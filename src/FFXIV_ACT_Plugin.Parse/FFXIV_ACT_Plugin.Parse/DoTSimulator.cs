using System;
using System.Collections.Generic;
using System.Linq;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Resource;
using FFXIV_ACT_Plugin.Resource.Definitions;

namespace FFXIV_ACT_Plugin.Parse;

public class DoTSimulator : IDoTSimulator
{
	private readonly IActionList _actionList;

	private readonly IPotencyStatusApplication _statusApplication;

	private readonly ISettingsMediator _settingsManager;

	private readonly CritDhStatusApplication _critStatusApplication;

	private readonly IDefinitionRepository _definitionRepo;

	private readonly IReportCombatData _actWrapper;

	private readonly IStatusList _statusList;

	private readonly Random _random;

	private readonly uint[] _kardionActionIds = new uint[14]
	{
		ActionId.Dosis,
		ActionId.Phlegma,
		ActionId.EukrasianDosis,
		ActionId.Dyskrasia,
		ActionId.Toxikon,
		ActionId.DosisII,
		ActionId.PhlegmaII,
		ActionId.EukrasianDosisII,
		ActionId.DosisIII,
		ActionId.PhlegmaIII,
		ActionId.EukrasianDosisIII,
		ActionId.DyskrasiaII,
		ActionId.ToxikonII,
		ActionId.Pneuma
	};

	public DoTSimulator(IActionList actionList, IPotencyStatusApplication statusApplication, CritDhStatusApplication critStatusApplication, IDefinitionRepository definitionRepo, ISettingsMediator settingsManager, IStatusList statusList, IReportCombatData actWrapper)
	{
		_definitionRepo = definitionRepo;
		_actionList = actionList;
		_statusApplication = statusApplication;
		_critStatusApplication = critStatusApplication;
		_settingsManager = settingsManager;
		_statusList = statusList;
		_actWrapper = actWrapper;
		_random = new Random();
	}

	public void CreateSimulatedDamageState(EnvironmentState state, StatusEffect status, CombatantState target)
	{
		if (status.Definition?.TimeProc == null)
		{
			return;
		}
		if (status.ActionEffect == null && status.StatusId != StatusId.Kardia && status.StatusId != StatusId.Kardion && status.StatusId != StatusId.PrimevalImpulse && status.StatusId != StatusId.EarthsResolve && status.StatusId != StatusId.Seraphism)
		{
			_settingsManager.AddExceptionMessage(status.Timestamp, $"Status Effect {status.StatusId:X2} {status.Definition?.Name} landed on {target.Id:X4} but no ActionEffect found.");
		}
		if (status.StatusId != StatusId.SacredSoilGround || status.ActionEffect == null || !state.Combatants.ContainsKey(status.ActionEffect.SourceId) || state.Combatants[status.ActionEffect.SourceId].Level >= 78)
		{
			_ = status.StatusId;
			uint? num = null;
			if (status.StatusId == StatusId.WheelOfFortune)
			{
				num = ActionId.CollectiveUnconcious;
			}
			else if (status.StatusId == StatusId.UndyingFlame)
			{
				num = ActionId.Rekindle;
			}
			else if (status.StatusId == StatusId.CrestOfTimeReturned)
			{
				num = ActionId.ArcaneCrest;
			}
			SimulatedDamageState simulatedDamageState = target.SimulatedDamage.FirstOrDefault((SimulatedDamageState x) => x.SourceId == status.SourceId && x.StatusId == status.StatusId);
			target.SimulatedDamage.RemoveAll((SimulatedDamageState x) => x.SourceId == status.SourceId && x.StatusId == status.StatusId);
			simulatedDamageState = CreateSimulatedDamageState(state, status.Timestamp, num ?? status.ActionEffect?.ActionId ?? 0, status.SourceId, target.Id, (ushort)status.StatusId, status.EffectByte2 ?? simulatedDamageState?.AmountLowByte, status.EffectByte1 ?? simulatedDamageState?.CritLowByte, status.ActionEffect?.TargetIndex ?? 0);
			if (simulatedDamageState != null)
			{
				target.SimulatedDamage.Add(simulatedDamageState);
			}
		}
	}

	internal SimulatedDamageState CreateSimulatedDamageState(EnvironmentState state, DateTime timestamp, uint actionId, uint sourceId, uint targetId, ushort statusId, byte? lowAmount, byte? lowCrit, int targetIndex)
	{
		if (!state.Combatants.ContainsKey(sourceId))
		{
			return null;
		}
		if (!state.Combatants.ContainsKey(targetId))
		{
			return null;
		}
		CombatantState sourceState = state.Combatants[sourceId];
		CombatantState targetState = state.Combatants[targetId];
		TimeProc timeProc = _definitionRepo.GetStatusEffectById(statusId)?.TimeProc;
		if (timeProc == null)
		{
			_settingsManager?.ProcessException(timestamp, $"DoT landed on target {targetId:X4} for Status Id {statusId:X2} but no DoT definition found.");
			return null;
		}
		byte maxTicks = timeProc.MaxTicks;
		byte usedTicks = 0;
		if (statusId == StatusId.Wildfire)
		{
			usedTicks = maxTicks;
		}
		SimulatedDamageState simulatedDamageState = new SimulatedDamageState
		{
			Timestamp = timestamp,
			ActionName = _actionList.GetActionNameByID(actionId),
			StatusId = statusId,
			SourceId = sourceId,
			TargetId = targetId,
			AmountLowByte = lowAmount,
			CritLowByte = lowCrit,
			TickType = timeProc.Type,
			UsedTicks = usedTicks,
			MaxTicks = maxTicks,
			CalculatedPotency = timeProc.Potency,
			CalculatedPotencyMultiplier = 1.0,
			AppliedStatusEffects = "",
			AppliedCritStatusList = "",
			SimulatedAmount = lowAmount.GetValueOrDefault()
		};
		if (actionId == ActionId.KaeshiHiganbana && statusId == StatusId.Higanbana)
		{
			simulatedDamageState.CalculatedPotency += 15.0;
		}
		if (statusId == StatusId.BladeOfValor && targetIndex > 0)
		{
			simulatedDamageState.CalculatedPotency /= 2.0;
		}
		_statusApplication.ApplyStatusEffects(timestamp, sourceState, targetState, simulatedDamageState, state.ZoneId, actionId, state.EchoBuff);
		CalculateAmount(simulatedDamageState, sourceState);
		CalculateCritDh(simulatedDamageState, timestamp, sourceState, targetState);
		return simulatedDamageState;
	}

	private void CalculateAmount(SimulatedDamageState damageState, CombatantState sourceState)
	{
		damageState.AppliedPotencyMultiplier = (damageState.IsHeal ? sourceState.Tracking.HealPotency : sourceState.Tracking.DamagePotency).GetValueOrDefault();
		if (damageState.AppliedPotencyMultiplier == 0.0)
		{
			damageState.AppliedPotencyMultiplier = (damageState.IsHeal ? sourceState.Tracking.DamagePotency : sourceState.Tracking.HealPotency) ?? 1.0;
		}
		if (damageState.CalculatedPotency > 0.0)
		{
			uint num = (uint)(damageState.CalculatedPotency * damageState.CalculatedPotencyMultiplier * damageState.AppliedPotencyMultiplier);
			if (!damageState.AmountLowByte.HasValue || num < damageState.AmountLowByte.Value)
			{
				damageState.SimulatedAmount = num;
			}
			else
			{
				byte b = (byte)Math.Round(((decimal)num - (((decimal?)damageState.AmountLowByte) ?? 0m)) / 256m);
				damageState.SimulatedAmount = (uint)((b << 8) + damageState.AmountLowByte.Value);
			}
			if (damageState.SimulatedAmount > 4096)
			{
				damageState.SimulatedAmount += 256u;
			}
		}
	}

	private void CalculateCritDh(SimulatedDamageState damageState, DateTime timestamp, CombatantState sourceState, CombatantState targetState)
	{
		double num = _critStatusApplication.CalculateCrit(damageState, timestamp, sourceState, targetState, null);
		if (damageState.CritLowByte.HasValue)
		{
			damageState.CalculatedCritRate = (double)(int)damageState.CritLowByte.Value / 1000.0;
			while (num - damageState.CalculatedCritRate > 0.125)
			{
				damageState.CalculatedCritRate += 0.255;
			}
		}
		else
		{
			damageState.CalculatedCritRate = num;
		}
		if (damageState.CalculatedCritRate > 1.0)
		{
			damageState.CalculatedCritRate = (((double?)(int?)damageState.CritLowByte) ?? 0.05) / 1000.0;
		}
		double calculatedDirectHitRate = _critStatusApplication.CalculateDirectHit(damageState, timestamp, sourceState, null);
		damageState.CalculatedDirectHitRate = calculatedDirectHitRate;
	}

	public void ProcessTick(EnvironmentState state, DateTime timestamp, bool isHoT, uint targetId, uint statusId, uint amount, uint targetCurrentHp, uint targetMaxHp, uint sourceId)
	{
		if (_settingsManager.ParseSettings.ShowRealDoTTicks)
		{
			_actWrapper.AddDoTEntry(state, timestamp, 0u, targetId, new DoTEffectEntry
			{
				ActionName = (isHoT ? "真实持续治疗" : "真实持续伤害"),
				Value = (ushort)amount,
				EntryType = (isHoT ? EffectEntryType.Heal : EffectEntryType.Damage),
				Flags2 = (byte)((amount > 65535) ? 64u : 0u),
				Flags1 = (byte)(amount / 65536)
			}, isHoT);
		}
		if (statusId != 0)
		{
			AddGroundAoeEntry(state, timestamp, isHoT, targetId, statusId, amount, targetCurrentHp, targetMaxHp, sourceId);
		}
		else
		{
			SimulateTicks(state, timestamp, isHoT, targetId, statusId, amount, targetCurrentHp, targetMaxHp);
		}
	}

	private void AddGroundAoeEntry(EnvironmentState state, DateTime timestamp, bool isHoT, uint targetId, uint statusId, uint amount, uint targetCurrentHp, uint targetMaxHp, uint sourceId)
	{
		string statusNameByID = _statusList.GetStatusNameByID(statusId);
		if (sourceId != 0)
		{
			_actWrapper.AddDoTEntry(state, timestamp, sourceId, targetId, new DoTEffectEntry
			{
				AppliedStatusEffects = "",
				SourceCritRate = 0.0,
				ActionName = (statusNameByID ?? ""),
				Value = (ushort)(amount & 0xFFFF),
				Flags2 = (byte)((amount > 65535) ? 64u : 0u),
				Flags1 = (byte)(amount / 65536),
				EntryType = (isHoT ? EffectEntryType.Damage : EffectEntryType.Heal),
				AmountByte = 0,
				CritByte = 0,
				OverHealAmount = (isHoT ? CalculateOverHealAmount(amount, targetCurrentHp, targetMaxHp) : 0u)
			}, isHoT);
			return;
		}
		state.Combatants.SelectMany((KeyValuePair<uint, CombatantState> c) => c.Value.SimulatedDamage).ToList();
		List<SimulatedDamageState> source = state.Combatants.SelectMany((KeyValuePair<uint, CombatantState> c) => c.Value.SimulatedDamage.Where((SimulatedDamageState x) => (x.TickType == TimeProc.TickType.GroundDamage || x.TickType == TimeProc.TickType.GroundHeal) && x.StatusId == statusId && c.Value.StatusEffects.Any((StatusEffect y) => y.StatusId == x.StatusId))).ToList();
		if (statusId == StatusId.Kardia && state.Combatants.ContainsKey(targetId))
		{
			source = ChooseKardionEffect(state, timestamp, statusId, targetId);
		}
		if (source.Any())
		{
			SimulatedDamageState simulatedDamageState = source.OrderByDescending((SimulatedDamageState x) => timestamp.Subtract((x.LastTickTimestamp == DateTime.MinValue) ? x.Timestamp.AddSeconds(-3.0) : x.LastTickTimestamp).TotalMilliseconds).First();
			_actWrapper.AddDoTEntry(state, timestamp, simulatedDamageState.SourceId, targetId, new DoTEffectEntry
			{
				AppliedStatusEffects = "",
				SourceCritRate = 0.0,
				ActionName = (statusNameByID ?? ""),
				Value = (ushort)(amount & 0xFFFF),
				Flags2 = (byte)((amount > 65535) ? 64u : 0u),
				Flags1 = (byte)(amount / 65536),
				EntryType = (isHoT ? EffectEntryType.Damage : EffectEntryType.Heal),
				AmountByte = simulatedDamageState.AmountLowByte.GetValueOrDefault(),
				CritByte = simulatedDamageState.CritLowByte.GetValueOrDefault(),
				OverHealAmount = (isHoT ? CalculateOverHealAmount(amount, targetCurrentHp, targetMaxHp) : 0u)
			}, isHoT);
			simulatedDamageState.UsedTicks++;
			simulatedDamageState.LastTickTimestamp = timestamp;
		}
		else
		{
			_actWrapper.AddDoTEntry(state, timestamp, 0u, targetId, new DoTEffectEntry
			{
				ActionName = (statusNameByID ?? ""),
				Value = (ushort)(amount & 0xFFFF),
				Flags2 = (byte)((amount > 65535) ? 64u : 0u),
				Flags1 = (byte)(amount / 65536),
				EntryType = (isHoT ? EffectEntryType.Damage : EffectEntryType.Heal),
				OverHealAmount = (isHoT ? CalculateOverHealAmount(amount, targetCurrentHp, targetMaxHp) : 0u)
			}, isHoT);
		}
	}

	private List<SimulatedDamageState> ChooseKardionEffect(EnvironmentState state, DateTime timestamp, uint statusId, uint targetId)
	{
		List<SimulatedDamageState> list = new List<SimulatedDamageState>();
		uint[] array = (from x in state.Combatants[targetId].StatusEffects
			where x.StatusId == StatusId.Kardion
			select x.SourceId).ToArray();
		List<ActionEffect> list2 = new List<ActionEffect>();
		for (int num = 0; num < array.Length; num++)
		{
			if (state.Combatants.TryGetValue(array[num], out var value))
			{
				ActionEffect actionEffect = (from x in value.ActionEffects
					where !x.IsKardionApplied && timestamp.Subtract(x.Timestamp).TotalMilliseconds < 3000.0 && _kardionActionIds.Contains(x.ActionId)
					orderby x.Timestamp
					select x).FirstOrDefault();
				if (actionEffect != null)
				{
					list2.Add(actionEffect);
				}
			}
		}
		if (list2.Any())
		{
			ActionEffect actionEffect2 = list2.OrderBy((ActionEffect x) => x.Timestamp).First();
			actionEffect2.IsKardionApplied = true;
			list.Add(new SimulatedDamageState
			{
				SourceId = actionEffect2.SourceId,
				StatusId = statusId,
				Timestamp = timestamp
			});
		}
		return list;
	}

	private uint CalculateOverHealAmount(uint amount, uint currentHp, uint maxHp)
	{
		uint num = ((currentHp < maxHp) ? (maxHp - currentHp) : 0u);
		if (amount >= num)
		{
			return amount - num;
		}
		return 0u;
	}

	private void SimulateTicks(EnvironmentState state, DateTime timestamp, bool isHoT, uint targetId, uint statusId, uint amount, uint targetCurrentHp, uint targetMaxHp)
	{
		if (!state.Combatants.ContainsKey(targetId))
		{
			throw new LookupException(string.Format("Cannot find Target Id {0:X4} to simulate incoming {1} Status Id {2:X2}", targetId, isHoT ? "HoT" : "DoT", statusId));
		}
		CombatantState target = state.Combatants[targetId];
		List<SimulatedDamageState> list = target.SimulatedDamage.Where((SimulatedDamageState x) => ((isHoT && x.TickType == TimeProc.TickType.HoT) || (!isHoT && x.TickType == TimeProc.TickType.DoT)) && timestamp.Subtract(x.LastTickTimestamp).TotalMilliseconds > 2700.0 && target.StatusEffects.Any((StatusEffect y) => y.SourceId == x.SourceId && y.StatusId == x.StatusId)).ToList();
		if (list.Count > 1)
		{
			list = list.Where((SimulatedDamageState x) => x.MaxTicks > x.UsedTicks).ToList();
		}
		if (!list.Any())
		{
			if ((!isHoT || targetId <= 1073741824) && (isHoT || targetId >= 1073741824) && _settingsManager.ParseSettings.ShowRealDoTTicks)
			{
				_settingsManager?.ProcessException(timestamp, string.Format("Combined {0} landed on target [{1:X4}] but no effect found.", isHoT ? "HoT" : "DoT", targetId));
			}
			return;
		}
		foreach (SimulatedDamageState item in list)
		{
			string statusNameByID = _statusList.GetStatusNameByID(item.StatusId);
			double num = item.SimulatedAmount;
			bool flag = (double)_random.Next(1000) < item.CalculatedCritRate * 1000.0;
			bool flag2 = !isHoT && (double)_random.Next(1000) < item.CalculatedDirectHitRate * 1000.0;
			double num2 = item.CalculatedCritRate - item.CritBuffAmount / 100.0 + 1.35;
			if (num2 < 1.4)
			{
				num2 = 1.4;
			}
			if (_settingsManager.ParseSettings.SimulateIndividualDoTCrits)
			{
				if (flag & flag2)
				{
					num = (int)(num * num2 * 1.25);
				}
				else if (flag)
				{
					num = (int)(num * num2);
				}
				else if (flag2)
				{
					num = (int)(num * 1.25);
				}
			}
			else
			{
				double num3 = num * (1.0 + (num2 - 1.0) * item.CalculatedCritRate) * (1.0 + 0.25 * item.CalculatedDirectHitRate);
				num = (int)Math.Floor(num3) + ((num3 % 1.0 > _random.NextDouble()) ? 1 : 0);
			}
			if (amount == 0)
			{
				num = 0.0;
			}
			_actWrapper.AddDoTEntry(state, timestamp, item.SourceId, targetId, new DoTEffectEntry
			{
				AppliedPotencyMultiplier = item.AppliedPotencyMultiplier,
				DoTBaseDamage = item.SimulatedAmount,
				AppliedStatusEffects = item.AppliedStatusEffects,
				AppliedCritStatusList = item.AppliedCritStatusList,
				AppliedDhStatusList = item.AppliedDirectHitStatusList,
				SourceCritRate = item.CalculatedCritRate,
				SourceDirectHitRate = item.CalculatedDirectHitRate,
				ActionName = statusNameByID + " (*)",
				Value = (ushort)((int)num & 0xFFFF),
				Flags2 = (byte)((num > 65535.0) ? 64u : 0u),
				Flags1 = (byte)(num / 65536.0),
				EntryType = EffectEntryType.Damage,
				Param0 = (byte)((flag ? 32 : 0) + (flag2 ? 64 : 0)),
				AmountByte = item.AmountLowByte.GetValueOrDefault(),
				CritByte = item.CritLowByte.GetValueOrDefault(),
				OverHealAmount = (isHoT ? CalculateOverHealAmount((uint)num, targetCurrentHp, targetMaxHp) : 0u)
			}, isHoT);
			item.UsedTicks++;
			item.LastTickTimestamp = timestamp;
		}
	}

	public void ProcessRemovedDeferredTick(EnvironmentState state, DateTime timestamp, DeferredEvent deferredEvent)
	{
		string statusNameByID = _statusList.GetStatusNameByID(deferredEvent.StatusId);
		_actWrapper.AddDoTEntry(state, timestamp, 0u, deferredEvent.TargetId, new DoTEffectEntry
		{
			AppliedStatusEffects = "",
			SourceCritRate = 0.0,
			ActionName = (statusNameByID ?? ""),
			Value = (ushort)(deferredEvent.Amount & 0xFFFF),
			Flags2 = (byte)((deferredEvent.Amount > 65535) ? 64u : 0u),
			Flags1 = (byte)(deferredEvent.Amount / 65536),
			EntryType = EffectEntryType.Damage,
			AmountByte = 0,
			CritByte = 0,
			OverHealAmount = ((deferredEvent.Type == DeferredEvent.EventType.HoT) ? CalculateOverHealAmount(deferredEvent.Amount, deferredEvent.TargetCurrentHp, deferredEvent.TargetMaxHp) : 0u)
		}, isHoT: true);
	}

	public void ProcessPneumaTick(EnvironmentState state, DateTime timestamp, uint actionId, uint sourceId)
	{
		if (actionId != ActionId.Pneuma)
		{
			return;
		}
		uint statusId = StatusId.Pneuma;
		List<CombatantState> list = state.Combatants.Values.Where((CombatantState x) => x.DeferredEvents.Any((DeferredEvent y) => y.StatusId == statusId && timestamp.Subtract(y.Timestamp).TotalMilliseconds < 500.0)).ToList();
		string actionNameByID = _actionList.GetActionNameByID(actionId);
		for (int num = 0; num < list.Count; num++)
		{
			CombatantState combatantState = list[num];
			for (int num2 = combatantState.DeferredEvents.Count - 1; num2 >= 0; num2--)
			{
				DeferredEvent deferredEvent = combatantState.DeferredEvents[num2];
				if (deferredEvent.StatusId == statusId)
				{
					_actWrapper.AddDoTEntry(state, timestamp, sourceId, combatantState.Id, new DoTEffectEntry
					{
						AppliedStatusEffects = "",
						SourceCritRate = 0.0,
						ActionName = (actionNameByID ?? ""),
						Value = (ushort)(deferredEvent.Amount & 0xFFFF),
						Flags2 = (byte)((deferredEvent.Amount > 65535) ? 64u : 0u),
						Flags1 = (byte)(deferredEvent.Amount / 65536),
						EntryType = EffectEntryType.Damage,
						AmountByte = 0,
						CritByte = 0,
						OverHealAmount = ((deferredEvent.Type == DeferredEvent.EventType.HoT) ? CalculateOverHealAmount(deferredEvent.Amount, deferredEvent.TargetCurrentHp, deferredEvent.TargetMaxHp) : 0u)
					}, isHoT: true);
					combatantState.DeferredEvents.RemoveAt(num2);
				}
			}
		}
	}
}
