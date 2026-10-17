using System;

namespace FFXIV_ACT_Plugin.Parse;

public interface IPotencyStatusApplication
{
	void ApplyStatusEffects(DateTime timestamp, CombatantState sourceState, CombatantState targetState, IEffectAppliable entry, uint zoneId, uint actionId, byte echoPercent);
}
