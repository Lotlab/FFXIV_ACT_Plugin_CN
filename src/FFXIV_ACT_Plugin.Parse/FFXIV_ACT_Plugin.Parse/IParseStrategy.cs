using System;

namespace FFXIV_ACT_Plugin.Parse;

public interface IParseStrategy
{
	void Parse(EnvironmentState state, DateTime timestamp, string[] data);
}
