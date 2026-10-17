using System;
using System.Globalization;
using FFXIV_ACT_Plugin.Resource;

namespace FFXIV_ACT_Plugin.Parse.ParseStrategy;

public class ParseStrategyZoneId : IParseStrategy
{
	private readonly ITerritoryList _territoryList;

	private readonly IReportCombatData _reportData;

	public ParseStrategyZoneId(IReportCombatData actWrapper, ITerritoryList territoryList)
	{
		_reportData = actWrapper;
		_territoryList = territoryList;
	}

	public void Parse(EnvironmentState state, DateTime timestamp, string[] data)
	{
		if (data.Length < 3)
		{
			throw new LineFormatException("Line too short.")
			{
				LineData = data,
				ExpectedSegmentCount = 3
			};
		}
		if (!uint.TryParse(data[2], NumberStyles.AllowHexSpecifier, null, out var result))
		{
			throw new LineParseException("Cannot parse Zone Id.")
			{
				LineData = data
			};
		}
		if (state.ZoneId != result)
		{
			state.Reset(erase: true);
			state.ZoneId = result;
			string territoryNameById = _territoryList.GetTerritoryNameById(result);
			_reportData.StopCombat(actExport: false);
			_reportData.ChangeTerritory(territoryNameById);
		}
	}
}
