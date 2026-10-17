using System;

namespace FFXIV_ACT_Plugin.Parse;

public class DeferredEventProcessor : IDeferredEventProcessor
{
	private readonly IDoTSimulator _dotSimulator;

	public DeferredEventProcessor(IDoTSimulator dotSimulator)
	{
		_dotSimulator = dotSimulator;
	}

	public void Process(EnvironmentState state, DateTime timestamp)
	{
		foreach (CombatantState value in state.Combatants.Values)
		{
			for (int num = value.DeferredEvents.Count - 1; num >= 0; num--)
			{
				DeferredEvent deferredEvent = value.DeferredEvents[num];
				if (timestamp.Subtract(deferredEvent.Timestamp).TotalMilliseconds > 10.0 && (deferredEvent.Type == DeferredEvent.EventType.HoT || deferredEvent.Type == DeferredEvent.EventType.DoT))
				{
					_dotSimulator.ProcessTick(state, deferredEvent.Timestamp, deferredEvent.Type == DeferredEvent.EventType.HoT, deferredEvent.TargetId, deferredEvent.StatusId, deferredEvent.Amount, deferredEvent.TargetCurrentHp, deferredEvent.TargetMaxHp, deferredEvent.SourceId);
					value.DeferredEvents.RemoveAt(num);
				}
			}
		}
	}

	public void ProcessEvent(EnvironmentState state, CombatantState combatant, DeferredEvent evt)
	{
		if (evt.Type == DeferredEvent.EventType.HoT || evt.Type == DeferredEvent.EventType.DoT)
		{
			_dotSimulator.ProcessTick(state, evt.Timestamp, evt.Type == DeferredEvent.EventType.HoT, evt.TargetId, evt.StatusId, evt.Amount, evt.TargetCurrentHp, evt.TargetMaxHp, evt.SourceId);
			combatant.DeferredEvents.RemoveAll((DeferredEvent x) => x.StatusId == evt.StatusId && x.Type == evt.Type && x.Amount == evt.Amount);
		}
	}
}
