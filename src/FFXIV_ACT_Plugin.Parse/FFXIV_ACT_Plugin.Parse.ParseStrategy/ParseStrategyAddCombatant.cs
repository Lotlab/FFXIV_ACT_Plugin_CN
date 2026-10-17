using System;
using System.Globalization;
using FFXIV_ACT_Plugin.Resource;

namespace FFXIV_ACT_Plugin.Parse.ParseStrategy;

public class ParseStrategyAddCombatant : IParseStrategy
{
	private readonly IServiceProvider _serviceProvider;

	private readonly IWorldList _worldList;

	public ParseStrategyAddCombatant(IServiceProvider serviceProvider, IWorldList worldList)
	{
		_serviceProvider = serviceProvider;
		_worldList = worldList;
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
			throw new LineParseException("Cannot parse Combatant Id.")
			{
				LineData = data
			};
		}
		if (result != 0 && result != 3758096384u)
		{
			CombatantState combatantState;
			if (state.Combatants.ContainsKey(result))
			{
				combatantState = state.Combatants[result];
			}
			else
			{
				combatantState = (CombatantState)_serviceProvider.GetService(typeof(CombatantState));
				combatantState.Id = result;
				state.Combatants.Add(result, combatantState);
			}
			if (!byte.TryParse(data[4], NumberStyles.AllowHexSpecifier, null, out var result2))
			{
				throw new LineParseException("Cannot parse Job as a byte.")
				{
					LineData = data
				};
			}
			if (!Enum.IsDefined(typeof(Job), result2))
			{
				throw new LineParseException($"Job [{result2}] is not a valid Job enumeration.")
				{
					LineData = data
				};
			}
			if (!byte.TryParse(data[5], NumberStyles.AllowHexSpecifier, null, out var result3))
			{
				throw new LineParseException("Cannot parse Level.")
				{
					LineData = data
				};
			}
			if (!uint.TryParse(data[6], NumberStyles.AllowHexSpecifier, null, out var result4))
			{
				throw new LineParseException("Cannot parse Parent Id.")
				{
					LineData = data
				};
			}
			if (!uint.TryParse(data[7], NumberStyles.AllowHexSpecifier, null, out var result5))
			{
				throw new LineParseException("Cannot parse World Id.")
				{
					LineData = data
				};
			}
			if (!uint.TryParse(data[9], out var result6))
			{
				throw new LineParseException("Cannot parse Npc Name Id.")
				{
					LineData = data
				};
			}
			if (!uint.TryParse(data[12], out var result7))
			{
				throw new LineParseException("Cannot parse Parent Id.")
				{
					LineData = data
				};
			}
			if (((uint)result2 != (uint)combatantState.Job) | (result3 != combatantState.Level) | (result4 != combatantState.ParentId) | (data[3] != combatantState.Name))
			{
				combatantState.Reset();
			}
			combatantState.RemovedTimestamp = DateTime.MinValue;
			combatantState.Name = data[3];
			combatantState.BNpcNameId = result6;
			combatantState.Job = (Job)result2;
			combatantState.Level = result3;
			combatantState.ParentId = result4;
			combatantState.MaxHp = result7;
			combatantState.WorldName = _worldList.GetWorldNameById(result5);
		}
	}
}
