using System;
using System.Collections.ObjectModel;
using FFXIV_ACT_Plugin.Common.Models;

namespace FFXIV_ACT_Plugin.Memory;

public interface ICombatantManager
{
	ReadOnlyCollection<Combatant> Combatants { get; }

	object CombatantLock { get; }

	Combatant GetCombatantById(uint CombatantID);

	Combatant GetCombatantByPointer(IntPtr combatantPtr);

	void Refresh();

	void Rescan();
}
