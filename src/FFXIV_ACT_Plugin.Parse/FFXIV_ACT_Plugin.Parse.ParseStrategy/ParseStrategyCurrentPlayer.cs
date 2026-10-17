using System;
using System.Globalization;

namespace FFXIV_ACT_Plugin.Parse.ParseStrategy;

public class ParseStrategyCurrentPlayer : IParseStrategy
{
	public void Parse(EnvironmentState state, DateTime timestamp, string[] data)
	{
		if (data.Length < 4)
		{
			throw new LineFormatException("Line too short.")
			{
				LineData = data,
				ExpectedSegmentCount = 4
			};
		}
		if (!uint.TryParse(data[2], NumberStyles.AllowHexSpecifier, null, out var result))
		{
			throw new LineParseException("Cannot parse Player Id.")
			{
				LineData = data
			};
		}
		state.PlayerId = result;
	}
}
