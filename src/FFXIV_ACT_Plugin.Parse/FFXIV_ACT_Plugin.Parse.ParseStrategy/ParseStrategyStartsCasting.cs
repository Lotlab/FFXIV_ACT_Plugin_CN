using System;
using System.Globalization;

namespace FFXIV_ACT_Plugin.Parse.ParseStrategy;

public class ParseStrategyStartsCasting : IParseStrategy
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
			throw new LineParseException("Cannot parse Source Id.")
			{
				LineData = data
			};
		}
		if (!uint.TryParse(data[4], NumberStyles.AllowHexSpecifier, null, out var result2))
		{
			throw new LineParseException("Cannot parse Action Id.")
			{
				LineData = data
			};
		}
		if (!uint.TryParse(data[6], NumberStyles.AllowHexSpecifier, null, out var result3))
		{
			throw new LineParseException("Cannot parse TargetId Id.")
			{
				LineData = data
			};
		}
		if (state.Combatants.ContainsKey(result))
		{
			state.Combatants[result].ActionCasts.Add(new ActionCast
			{
				ActionId = result2,
				TargetId = result3,
				SourceId = result,
				Timestamp = timestamp
			});
		}
	}
}
