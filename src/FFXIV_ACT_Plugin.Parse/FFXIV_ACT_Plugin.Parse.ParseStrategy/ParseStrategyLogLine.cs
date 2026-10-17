using System;
using System.Text.RegularExpressions;

namespace FFXIV_ACT_Plugin.Parse.ParseStrategy;

public class ParseStrategyLogLine : IParseStrategy
{
	private static readonly string _echoBuffEnglish = "Maximum HP, damage dealt, and spell-based HP restoration have been increased by (?<amount>[\\d]+)\\%\\.";

	private static readonly string _echoBuffKorean = "'초월하는 힘'을 발휘하여 공격력, HP 회복 효과, 최대 HP가 (?<amount>[\\d]+)\\% 상승했습니다.";

	private static readonly Regex _regexEchoBuffEnglish = new Regex(_echoBuffEnglish, RegexOptions.IgnoreCase | RegexOptions.Compiled);

	private static readonly Regex _regexEchoBuffKorean = new Regex(_echoBuffKorean, RegexOptions.IgnoreCase | RegexOptions.Compiled);

	private readonly IReportCombatData _actWrapper;

	public ParseStrategyLogLine(IReportCombatData actWrapper)
	{
		_actWrapper = actWrapper;
	}

	public void Parse(EnvironmentState state, DateTime timestamp, string[] data)
	{
		switch (Convert.ToInt32(data[2], 16))
		{
		case 56:
		{
			string command = data[4]?.ToLowerInvariant()?.Trim();
			_actWrapper.EchoCommand(command);
			break;
		}
		case 57:
		{
			Match match = _regexEchoBuffEnglish.Match(data[4]);
			if (!match.Success)
			{
				match = _regexEchoBuffKorean.Match(data[4]);
			}
			if (match.Success && match.Groups["amount"].Success && byte.TryParse(match.Groups["amount"].Value, out var result) && result > 0)
			{
				state.EchoBuff = result;
			}
			break;
		}
		}
	}
}
