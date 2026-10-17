using System;
using System.Globalization;
using System.Linq;
using FFXIV_ACT_Plugin.Resource;
using FFXIV_ACT_Plugin.Resource.Definitions;

namespace FFXIV_ACT_Plugin.Parse.ParseStrategy;

public class ParseStrategyCancelAbility : IParseStrategy
{
	private readonly IActionList _actionlist;

	private readonly IReportCombatData _actWrapper;

	private readonly IDefinitionRepository _definitionRepo;

	public ParseStrategyCancelAbility(IActionList actionlist, IReportCombatData actWrapper, IDefinitionRepository definitionRepo)
	{
		_actionlist = actionlist;
		_actWrapper = actWrapper;
		_definitionRepo = definitionRepo;
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
			throw new LineParseException("Cannot parse Source Id.")
			{
				LineData = data
			};
		}
		if (!uint.TryParse(data[4], NumberStyles.AllowHexSpecifier, null, out var actionId))
		{
			throw new LineParseException("Cannot parse Action Id.")
			{
				LineData = data
			};
		}
		string actionNameByID = _actionlist.GetActionNameByID(actionId);
		FFXIV_ACT_Plugin.Resource.Definitions.Action actionById = _definitionRepo.GetActionById(actionId);
		uint targetId = 0u;
		if (state.Combatants.ContainsKey(result))
		{
			CombatantState combatantState = state.Combatants[result];
			if (combatantState.ActionCasts.Any((ActionCast x) => x.ActionId == actionId))
			{
				targetId = (from x in combatantState.ActionCasts
					where x.ActionId == actionId
					orderby x.Timestamp descending
					select x).First().TargetId;
			}
		}
		_actWrapper.AddCancelEntry(state, timestamp, result, targetId, actionNameByID, data[6], actionById?.IsHeal ?? false, actionById?.IsDamage ?? false);
	}
}
