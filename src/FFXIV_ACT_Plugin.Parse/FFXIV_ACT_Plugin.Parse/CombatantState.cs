using System;
using System.Collections.Generic;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Resource.Definitions;

namespace FFXIV_ACT_Plugin.Parse;

public class CombatantState
{
	private readonly ISettingsMediator _settingsMediator;

	private readonly IDoTSimulator _dotSimulator;

	private readonly int STATUS_TIMEOUT = 5000;

	private readonly int ACTION_TIMEOUT = 35000;

	private readonly int CAST_TIMEOUT = 10000;

	public uint Id { get; set; }

	public uint ParentId { get; set; }

	public string Name { get; set; }

	public Job Job { get; set; }

	public byte Level { get; set; }

	public uint MaxHp { get; set; }

	public uint BNpcNameId { get; set; }

	public string WorldName { get; set; }

	public DateTime RemovedTimestamp { get; set; }

	public List<ActionEffect> ActionEffects { get; } = new List<ActionEffect>();

	public List<StatusEffect> StatusEffects { get; } = new List<StatusEffect>();

	public List<SimulatedDamageState> SimulatedDamage { get; } = new List<SimulatedDamageState>();

	public List<DeferredEvent> DeferredEvents { get; } = new List<DeferredEvent>();

	public TrackingState Tracking { get; } = new TrackingState();

	public List<ActionCast> ActionCasts { get; } = new List<ActionCast>();

	public CombatantState(ISettingsMediator settingsMediator, IDoTSimulator dotSimulator)
	{
		_settingsMediator = settingsMediator;
		_dotSimulator = dotSimulator;
	}

	public void Reset()
	{
		ActionEffects.Clear();
		StatusEffects.Clear();
		SimulatedDamage.Clear();
		Tracking.Clear();
		DeferredEvents.Clear();
		ActionCasts.Clear();
	}

	public void Clean(EnvironmentState state, DateTime timestamp)
	{
		for (int num = StatusEffects.Count - 1; num >= 0; num--)
		{
			StatusEffect statusEffect = StatusEffects[num];
			if (statusEffect.Duration > 0.0 && timestamp.Subtract(statusEffect.Timestamp.AddSeconds(statusEffect.Duration)).TotalMilliseconds > (double)STATUS_TIMEOUT)
			{
				StatusEffects.RemoveAt(num);
			}
		}
		for (int num2 = SimulatedDamage.Count - 1; num2 >= 0; num2--)
		{
			SimulatedDamageState simulatedDamageState = SimulatedDamage[num2];
			if (simulatedDamageState.LastTickTimestamp == DateTime.MinValue || simulatedDamageState.TickType == TimeProc.TickType.GroundDamage || simulatedDamageState.TickType == TimeProc.TickType.GroundHeal)
			{
				if (timestamp.Subtract(simulatedDamageState.Timestamp.AddSeconds(3 * simulatedDamageState.MaxTicks)).TotalMilliseconds > (double)STATUS_TIMEOUT)
				{
					_settingsMediator.AddExceptionMessage(timestamp, $"SimulatedDamage Timeout with no ticks/GroundAoE: Removing status {simulatedDamageState.StatusId:X2} {simulatedDamageState.ActionName} from {simulatedDamageState.TargetId:X4}.");
					SimulatedDamage.RemoveAt(num2);
				}
			}
			else if (simulatedDamageState.UsedTicks >= simulatedDamageState.MaxTicks)
			{
				SimulatedDamage.RemoveAt(num2);
			}
			else if (timestamp.Subtract(simulatedDamageState.LastTickTimestamp.AddSeconds(3 * (simulatedDamageState.MaxTicks - simulatedDamageState.UsedTicks))).TotalMilliseconds > (double)STATUS_TIMEOUT)
			{
				SimulatedDamage.RemoveAt(num2);
			}
		}
		for (int num3 = ActionEffects.Count - 1; num3 >= 0; num3--)
		{
			ActionEffect actionEffect = ActionEffects[num3];
			if (timestamp.Subtract(actionEffect.Timestamp).TotalMilliseconds > (double)ACTION_TIMEOUT)
			{
				ActionEffects.RemoveAt(num3);
			}
		}
		for (int num4 = DeferredEvents.Count - 1; num4 >= 0; num4--)
		{
			DeferredEvent deferredEvent = DeferredEvents[num4];
			if (timestamp.Subtract(deferredEvent.Timestamp).TotalMilliseconds > (double)STATUS_TIMEOUT)
			{
				_dotSimulator.ProcessRemovedDeferredTick(state, timestamp, deferredEvent);
				DeferredEvents.RemoveAt(num4);
			}
		}
		for (int num5 = ActionCasts.Count - 1; num5 >= 0; num5--)
		{
			ActionCast actionCast = ActionCasts[num5];
			if (timestamp.Subtract(actionCast.Timestamp).TotalMilliseconds > (double)CAST_TIMEOUT)
			{
				ActionCasts.RemoveAt(num5);
			}
		}
	}
}
