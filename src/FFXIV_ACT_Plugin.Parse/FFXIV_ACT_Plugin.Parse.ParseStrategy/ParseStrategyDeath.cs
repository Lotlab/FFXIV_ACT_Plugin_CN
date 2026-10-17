using System;
using System.Globalization;

namespace FFXIV_ACT_Plugin.Parse.ParseStrategy;

public class ParseStrategyDeath : IParseStrategy
{
	private readonly IReportCombatData _actWrapper;

	public ParseStrategyDeath(IReportCombatData actWrapper)
	{
		_actWrapper = actWrapper;
	}

	public void Parse(EnvironmentState state, DateTime timestamp, string[] data)
	{
		if (data.Length < 6)
		{
			throw new LineFormatException("Line too short.")
			{
				LineData = data,
				ExpectedSegmentCount = 6
			};
		}
		if (!uint.TryParse(data[2], NumberStyles.AllowHexSpecifier, null, out var result))
		{
			throw new LineParseException("Cannot parse Target Id.")
			{
				LineData = data
			};
		}
		if (!uint.TryParse(data[4], NumberStyles.AllowHexSpecifier, null, out var result2))
		{
			throw new LineParseException("Cannot parse Source Id.")
			{
				LineData = data
			};
		}
		_actWrapper.AddDeathEntry(state, timestamp, result2, result);
	}
}
