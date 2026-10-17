using System;
using System.Globalization;
using System.Linq;

namespace FFXIV_ACT_Plugin.Parse.ParseStrategy;

public class ParseStrategyParty : IParseStrategy
{
	public void Parse(EnvironmentState state, DateTime timestamp, string[] data)
	{
		if (data.Length < 4)
		{
			return;
		}
		uint[] array = new uint[data.Length - 4];
		if (!byte.TryParse(data[2], out var result))
		{
			throw new LineParseException("Cannot parse Party Count as a byte.")
			{
				LineData = data
			};
		}
		for (int i = 3; i < data.Length - 1; i++)
		{
			if (!uint.TryParse(data[i], NumberStyles.AllowHexSpecifier, null, out array[i - 3]))
			{
				throw new LineParseException($"Cannot parse Party Member ID {data[i]:X4} as a number.")
				{
					LineData = data
				};
			}
		}
		state.PartyCount = result;
		state.PartyMembers = array.ToList();
	}
}
