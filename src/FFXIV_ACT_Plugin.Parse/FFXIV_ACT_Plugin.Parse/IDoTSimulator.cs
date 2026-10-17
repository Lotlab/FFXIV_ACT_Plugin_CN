using System;

namespace FFXIV_ACT_Plugin.Parse;

public interface IDoTSimulator
{
	void CreateSimulatedDamageState(EnvironmentState state, StatusEffect status, CombatantState target);

	void ProcessTick(EnvironmentState state, DateTime timestamp, bool isHoT, uint targetId, uint statusId, uint amount, uint targetCurrentHp, uint targetMaxHp, uint sourceId);

	void ProcessPneumaTick(EnvironmentState state, DateTime timestamp, uint actionId, uint sourceId);

	void ProcessRemovedDeferredTick(EnvironmentState state, DateTime timestamp, DeferredEvent deferredEvent);
}
