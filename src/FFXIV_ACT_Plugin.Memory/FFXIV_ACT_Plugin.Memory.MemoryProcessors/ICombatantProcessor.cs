using System;
using FFXIV_ACT_Plugin.Common.Models;

namespace FFXIV_ACT_Plugin.Memory.MemoryProcessors;

public interface ICombatantProcessor
{
	void RefreshCombatant(Combatant combatant, IntPtr address, bool refresh);

	void RefreshAndAddCombatant(Combatant combatant, IntPtr address, Combatant[] oldCombatants);
}
