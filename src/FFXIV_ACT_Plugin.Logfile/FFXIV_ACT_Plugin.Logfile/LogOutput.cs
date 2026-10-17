#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using FFXIV_ACT_Plugin.Common;
using FFXIV_ACT_Plugin.Config;

namespace FFXIV_ACT_Plugin.Logfile;

public class LogOutput : ILogOutput
{
	private readonly Queue<string> _LogQueue = new Queue<string>(1000);

	private readonly object _LogQueueLock = new object();

	private int _logIndex;

	private bool _disableHash;

	private string _logFileName;

	private DateTime _lastLogTime = DateTime.MinValue;

	private readonly object _lastLogTimeLock = new object();

	private readonly ISettingsMediator _settingsManager;

	private readonly IActWrapper _actWrapper;

	private readonly IBenchmarkRepository _benchmarkRepository;

	private static readonly uint[] _lookup32 = u_49151();

	public Thread ScanThread { get; private set; }

	public LogOutput(ISettingsMediator settingsManager, IActWrapper actWrapper, IBenchmarkRepository benchmarkRepository)
	{
		_settingsManager = settingsManager;
		_actWrapper = actWrapper;
		_benchmarkRepository = benchmarkRepository;
		ScanThread = new Thread(Run)
		{
			IsBackground = true,
			Name = GetType().FullName
		};
	}

	private string ConfigureLogFile()
	{
		try
		{
			using SHA256 sHA = SHA256.Create();
			sHA.ComputeHash(Encoding.UTF8.GetBytes("1234567890"));
		}
		catch (Exception)
		{
			Trace.WriteLine("ERROR: Windows feature is missing, disabling support for FFLogs Uploads.", "FFXIV_ACT_Plugin");
			_disableHash = true;
		}
		_actWrapper.LogFileFilter = "Network_*.log";
		Version version = typeof(ILogOutput).Assembly.GetName().Version;
		_logFileName = Path.Combine(_settingsManager.DataCollectionSettings.LogFileFolder, $"Network_{version.Major}{version.Minor}{version.Build}0{version.Revision}_{DateTime.Today:yyyyMMdd}.log");
		if (!Directory.Exists(_settingsManager.DataCollectionSettings.LogFileFolder))
		{
			Directory.CreateDirectory(_settingsManager.DataCollectionSettings.LogFileFolder);
		}
		bool flag = File.Exists(_logFileName);
		if (!flag)
		{
			File.AppendAllText(_logFileName, "");
		}
		if (_actWrapper.LogFilePath != _logFileName || !flag)
		{
			_actWrapper.LogFilePath = _logFileName;
			_actWrapper.OpenLog(GetCurrentZone: true, GetCharNameFromFile: false);
		}
		return _logFileName;
	}

	public void Run(object parameter)
	{
		CancellationToken cancellationToken = (CancellationToken)parameter;
		while (_settingsManager.DataCollectionSettings == null || !_actWrapper.IsMainFormVisible())
		{
			if (cancellationToken.WaitHandle.WaitOne(100))
			{
				return;
			}
		}
		bool flag = false;
		string logFileFolder = _settingsManager.DataCollectionSettings.LogFileFolder;
		while (!cancellationToken.WaitHandle.WaitOne(50))
		{
			try
			{
				using FileStream stream = new FileStream(ConfigureLogFile(), FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
				StreamWriter outputWriter = new StreamWriter(stream);
				try
				{
					while (_settingsManager.DataCollectionSettings.LogFileFolder == logFileFolder)
					{
						while (!cancellationToken.IsCancellationRequested)
						{
							string logLine = null;
							_benchmarkRepository.Measure("LogLineDequeue", () =>
							{
								lock (_LogQueueLock)
								{
									if (_LogQueue.Count > 0)
									{
										logLine = _LogQueue.Dequeue();
									}
								}
							});
							if (logLine != null)
							{
								_benchmarkRepository.Measure("LogLineWrite", () =>
								{
									outputWriter.WriteLine(logLine);
								});
								_benchmarkRepository.Measure("LogLineFlush", () =>
								{
									outputWriter.Flush();
								});
							}
							else if (cancellationToken.WaitHandle.WaitOne(50))
							{
								return;
							}
						}
					}
				}
				finally
				{
					if (outputWriter != null)
					{
						((IDisposable)outputWriter).Dispose();
					}
				}
			}
			catch (ThreadAbortException)
			{
				break;
			}
			catch (IOException ex2)
			{
				_settingsManager.AddExceptionMessage(DateTime.Now, "FFXIV_ACT_Plugin.LogOutput Error writing to log file. " + ex2.ToString());
			}
			catch (Exception ex3)
			{
				if (!flag)
				{
					_actWrapper.WriteErrorLog(ex3, "FFXIV_ACT_Plugin.LogOutput exception occurred.");
				}
				flag = true;
				if (cancellationToken.WaitHandle.WaitOne(100))
				{
					break;
				}
			}
		}
	}

	public void WriteLine(LogMessageType messageType, DateTime ServerDate, string line)
	{
		lock (_lastLogTimeLock)
		{
			if (ServerDate == DateTime.MinValue)
			{
				if (_lastLogTime == DateTime.MinValue)
				{
					_lastLogTime = DateTime.Now;
				}
				ServerDate = _lastLogTime;
			}
			else
			{
				_lastLogTime = ServerDate;
			}
		}
		string[] array = new string[5];
		int num = (int)messageType;
		array[0] = num.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');
		array[1] = "|";
		array[2] = ServerDate.ToString("O");
		array[3] = "|";
		array[4] = line.Replace('\0', ' ');
		string text = string.Concat(array);
		lock (_LogQueueLock)
		{
			if (messageType == LogMessageType.Version || messageType == LogMessageType.Territory)
			{
				Interlocked.Exchange(ref _logIndex, 0);
			}
			text = text + "|" + u_65535(text + "|" + Interlocked.Increment(ref _logIndex).ToString(CultureInfo.InvariantCulture));
			_LogQueue.Enqueue(text);
		}
	}

	private string u_65535(string text)
	{
		if (_disableHash)
		{
			return "0";
		}
		using SHA256 sHA = SHA256.Create();
		return u_49152(sHA.ComputeHash(Encoding.UTF8.GetBytes(text)));
	}

	private static uint[] u_49151()
	{
		uint[] array = new uint[256];
		for (int i = 0; i < 256; i++)
		{
			string text = $"{i:x2}";
			array[i] = text[0] + ((uint)text[1] << 16);
		}
		return array;
	}

	private static string u_49152(byte[] bytes)
	{
		uint[] lookup = _lookup32;
		char[] array = new char[16];
		for (int i = 0; i < array.Length / 2; i++)
		{
			uint num = lookup[bytes[i]];
			array[2 * i] = (char)num;
			array[2 * i + 1] = (char)(num >> 16);
		}
		return new string(array);
	}
}
