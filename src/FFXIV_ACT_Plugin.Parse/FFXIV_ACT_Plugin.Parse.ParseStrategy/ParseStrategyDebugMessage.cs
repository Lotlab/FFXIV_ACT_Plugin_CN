using System;
using System.Linq;
using FFXIV_ACT_Plugin.Config;

namespace FFXIV_ACT_Plugin.Parse.ParseStrategy;

public class ParseStrategyDebugMessage : IParseStrategy
{
	private readonly ISettingsMediator _settingsManager;

	public ParseStrategyDebugMessage(ISettingsMediator settingsManager)
	{
		_settingsManager = settingsManager;
	}

	public void Parse(EnvironmentState state, DateTime timestamp, string[] data)
	{
		if (data.Length >= 4)
		{
			string text = string.Join(":", data.Skip(2).Take(data.Length - 3));
			if (!text.Contains("TCPDecoder: packet data already processed"))
			{
				_settingsManager?.ProcessException(timestamp, text);
			}
		}
	}
}
