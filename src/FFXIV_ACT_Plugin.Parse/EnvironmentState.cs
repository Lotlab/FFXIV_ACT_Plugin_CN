using System;
using System.Collections.Generic;

namespace FFXIV_ACT_Plugin.Parse;

public class EnvironmentState
{
	private readonly int CLEAN_INTERVAL = 5000;

	private readonly int REMOVE_COMBATANT_INTERVAL = 60000;

	private DateTime _lastCleanTime = DateTime.MinValue;

	public uint PlayerId { get; set; }

	public uint ZoneId { get; set; }

	public byte PartyCount { get; set; }

	public List<uint> PartyMembers { get; set; }

	public byte EchoBuff { get; set; }

	public Dictionary<uint, CombatantState> Combatants { get; } = new Dictionary<uint, CombatantState>();

	public DateTime LastLineTimestamp { get; set; }

	public void Reset(bool erase)
	{
		if (erase)
		{
			Combatants.Clear();
			PlayerId = 0u;
			ZoneId = 0u;
			EchoBuff = 0;
			LastLineTimestamp = DateTime.MinValue;
		}
		else
		{
			Clean(DateTime.MaxValue);
		}
	}

	public void Clean(DateTime timestamp)
	{
		if (timestamp.Subtract(_lastCleanTime).TotalMilliseconds < (double)CLEAN_INTERVAL)
		{
			return;
		}
		_lastCleanTime = timestamp;
		List<uint> list = new List<uint>();
		foreach (uint key in Combatants.Keys)
		{
			if (Combatants[key].RemovedTimestamp.Subtract(timestamp).TotalMilliseconds > (double)REMOVE_COMBATANT_INTERVAL)
			{
				list.Add(key);
			}
		}
		foreach (uint item in list)
		{
			Combatants.Remove(item);
		}
		foreach (CombatantState value in Combatants.Values)
		{
			value.Clean(this, timestamp);
		}
	}
}
