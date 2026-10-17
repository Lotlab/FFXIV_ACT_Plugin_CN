using System;
using System.Globalization;

namespace FFXIV_ACT_Plugin.Parse.ParseStrategy;

public class ParseStrategyRemoveCombatant : IParseStrategy
{
	public void Parse(EnvironmentState state, DateTime timestamp, string[] data)
	{
		if (data.Length < 7)
		{
			throw new LineFormatException("Line too short.")
			{
				LineData = data,
				ExpectedSegmentCount = 7
			};
		}
		if (!uint.TryParse(data[2], NumberStyles.AllowHexSpecifier, null, out var result))
		{
			throw new LineParseException("Cannot parse Combatant Id.")
			{
				LineData = data
			};
		}
		if (state.Combatants.ContainsKey(result))
		{
			state.Combatants[result].Reset();
			if ((result & 0x10000000) == 0)
			{
				state.Combatants[result].RemovedTimestamp = timestamp;
			}
		}
	}
}
