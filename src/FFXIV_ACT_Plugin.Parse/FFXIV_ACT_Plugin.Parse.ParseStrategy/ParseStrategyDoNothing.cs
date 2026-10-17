using System;

namespace FFXIV_ACT_Plugin.Parse.ParseStrategy;

public class ParseStrategyDoNothing : IParseStrategy
{
	public void Parse(EnvironmentState state, DateTime timestamp, string[] data)
	{
	}
}
