using System;

namespace FFXIV_ACT_Plugin.Parse;

public interface IDeferredEventProcessor
{
	void Process(EnvironmentState state, DateTime timestamp);

	void ProcessEvent(EnvironmentState state, CombatantState combatant, DeferredEvent evt);
}
