using System;
using System.Globalization;
using FFXIV_ACT_Plugin.Config;

namespace FFXIV_ACT_Plugin.Parse.ParseStrategy;

public class ParseStrategyDoT : IParseStrategy
{
	private readonly ISettingsMediator _settingsMediator;

	public ParseStrategyDoT(ISettingsMediator settingsMediator)
	{
		_settingsMediator = settingsMediator;
	}

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
			throw new LineParseException("Cannot parse Target Id.")
			{
				LineData = data
			};
		}
		if (!uint.TryParse(data[5], NumberStyles.AllowHexSpecifier, null, out var result2))
		{
			throw new LineParseException("Cannot parse Status Id.")
			{
				LineData = data
			};
		}
		if (!uint.TryParse(data[6], NumberStyles.AllowHexSpecifier, null, out var result3))
		{
			throw new LineParseException("Cannot parse Amount.")
			{
				LineData = data
			};
		}
		uint.TryParse(data[7], out var result4);
		uint.TryParse(data[8], out var result5);
		bool flag = data[4].ToLower(CultureInfo.InvariantCulture) == "hot";
		uint result6 = 0u;
		if (data.Length > 17)
		{
			uint.TryParse(data[17], NumberStyles.AllowHexSpecifier, null, out result6);
		}
		DeferredEvent item = new DeferredEvent
		{
			Amount = result3,
			StatusId = result2,
			TargetId = result,
			Timestamp = timestamp,
			Type = (flag ? DeferredEvent.EventType.HoT : DeferredEvent.EventType.DoT),
			TargetCurrentHp = result4,
			TargetMaxHp = result5,
			SourceId = result6
		};
		if (state.Combatants.ContainsKey(result))
		{
			state.Combatants[result].DeferredEvents.Add(item);
		}
		else
		{
			_settingsMediator.AddExceptionMessage(timestamp, $"Status Id {result2:X2} tick on target {result:X4} for {result3} but target not loaded.");
		}
	}
}
