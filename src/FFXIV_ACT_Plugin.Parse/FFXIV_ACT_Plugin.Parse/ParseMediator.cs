using System;
using System.Linq;
using FFXIV_ACT_Plugin.Common;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Parse.ParseStrategy;

namespace FFXIV_ACT_Plugin.Parse;

public class ParseMediator
{
	private readonly EnvironmentState _liveEnvironmentState = new EnvironmentState();

	private readonly EnvironmentState _importEnvironmentState = new EnvironmentState();

	private readonly ISettingsMediator _settingsManager;

	private readonly IBenchmarkRepository _benchmarkRepository;

	private readonly ParseStrategyFactory _parseStrategyFactory;

	private readonly IDeferredEventProcessor _deferredEventProcessor;

	public ParseMediator(ParseStrategyFactory parseStrategyFactory, ISettingsMediator settingsManager, IBenchmarkRepository benchmarkRepository, IDeferredEventProcessor deferredEventProcessor)
	{
		_parseStrategyFactory = parseStrategyFactory;
		_settingsManager = settingsManager;
		_benchmarkRepository = benchmarkRepository;
		_deferredEventProcessor = deferredEventProcessor;
	}

	public void OnClear(object sender, EventArgs e)
	{
		_liveEnvironmentState.Reset(erase: false);
		_importEnvironmentState.Reset(erase: true);
	}

	public void LogFileChanged(bool isImport)
	{
		if (isImport)
		{
			_importEnvironmentState.Reset(erase: true);
		}
	}

	public (string, int) BeforeLogLineRead(bool isImport, DateTime timestamp, string logLine)
	{
		if (!logLine.Contains('|'))
		{
			_settingsManager.AddExceptionMessage(DateTime.Now, "missing field delimieter. Line=" + logLine);
			return (logLine, 0);
		}
		string[] data = logLine.Split('|');
		if (!Enum.TryParse<LogMessageType>(data[0], out var result))
		{
			_settingsManager.AddExceptionMessage(DateTime.Now, $"Bad LogMessageType: {data}");
			return (logLine, 0);
		}
		if (timestamp == DateTime.MinValue)
		{
			_settingsManager.AddExceptionMessage(DateTime.Now, $"Bad Timestamp: {data}");
			return (logLine, (int)result);
		}
		EnvironmentState state = (isImport ? _importEnvironmentState : _liveEnvironmentState);
		_benchmarkRepository.Measure("ParseEnvironmentStateClean", () =>
		{
			state.Clean(timestamp);
		});
		if (state.LastLineTimestamp != timestamp)
		{
			_benchmarkRepository.Measure("ProcessDeferredEvents", () =>
			{
				_deferredEventProcessor.Process(state, timestamp);
			});
			state.LastLineTimestamp = timestamp;
		}
		IParseStrategy strategy = _parseStrategyFactory.GetParseStrategy(result);
		try
		{
			_benchmarkRepository.Measure($"Parse{result}", () =>
			{
				strategy.Parse(state, timestamp, data);
			});
		}
		catch (LineParseException ex)
		{
			_settingsManager.AddExceptionMessage(timestamp, "LineParse error in " + ex.TargetSite?.Name + "(): " + ex.Message);
		}
		catch (LineFormatException ex2)
		{
			_settingsManager.AddExceptionMessage(timestamp, "LineFormat error in " + ex2.TargetSite?.Name + "(): " + ex2.Message);
		}
		catch (LookupException ex3)
		{
			_settingsManager.AddExceptionMessage(timestamp, "Lookup error in " + ex3.TargetSite?.Name + "(): " + ex3.Message);
		}
		catch (Exception arg)
		{
			_settingsManager.AddExceptionMessage(timestamp, $"Exception: {arg}");
		}
		switch (result)
		{
		case LogMessageType.PacketDump:
			logLine = "";
			break;
		case LogMessageType.ChatLog:
			data[4] = CleanChatlog(data[4]);
			logLine = string.Format("[{0:HH:mm:ss.fff}] {1} {2:X2}:{3}", timestamp, result, (int)result, string.Join(":", data, 2, data.Length - 3));
			break;
		default:
			logLine = string.Format("[{0:HH:mm:ss.fff}] {1} {2:X2}:{3}", timestamp, result, (int)result, string.Join(":", data, 2, data.Length - 3));
			break;
		}
		return (logLine, (int)result);
	}

	private string CleanChatlog(string data)
	{
		string text = data.Replace("\ue06f", "⇒");
		while (text.IndexOf('\u0002') < text.IndexOf('\u0003') && text.IndexOf('\u0002') >= 0)
		{
			text = ((text.IndexOf('\u0002') > 0) ? text.Substring(0, text.IndexOf('\u0002')) : "") + text.Substring(text.IndexOf('\u0003') + 1);
		}
		if (text.Length != 0 && text.Last() == '\u0002')
		{
			text = text.Substring(0, text.Length - 1);
		}
		text = text.Replace("\u0001\u0001\ufffd\ue0bb\ue05b", "");
		text = text.Replace("\ue0bb\ue05b", "");
		text = text.Replace("\ue0bb\ue05c", "");
		text = text.Trim();
		return text.Replace('\0', ' ');
	}

	public DateTime ParseLogDateTime(string logLine)
	{
		DateTime result = DateTime.MinValue;
		if (logLine == null)
		{
			return result;
		}
		int num = logLine.IndexOf('|');
		int num2 = logLine.IndexOf('|', num + 1);
		if (num < num2 && !DateTime.TryParse(logLine.Substring(num + 1, num2 - num - 1), out result))
		{
			return DateTime.MinValue;
		}
		if (result.Year > 2100)
		{
			return DateTime.MinValue;
		}
		return result;
	}
}
