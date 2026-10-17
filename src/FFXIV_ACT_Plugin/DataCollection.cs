using System;
using System.Diagnostics;
using System.Threading;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory;
using FFXIV_ACT_Plugin.Network;

namespace FFXIV_ACT_Plugin;

public class DataCollection : IDisposable
{
	private readonly ILogOutput _logOutput;

	private readonly ILogFormat _logFormat;

	private readonly IScanMemory _scanMemory;

	private readonly IScanPackets _scanPackets;

	private readonly LoggingTraceListener _logTraceListener;

	private readonly MonitorNetwork _monitor;

	private readonly DataEventProcessor _dataEventProcessor;

	private readonly ISettingsMediator _settingsMediator;

	private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

	private bool disposedValue;

	public DataCollection(ILogOutput logOutput, ILogFormat logFormat, IScanMemory scanMemory, IScanPackets scanPackets, DataEventProcessor dataEventProcessor, MonitorNetwork monitor, ISettingsMediator settingsMediator, LoggingTraceListener traceListener)
	{
		_logOutput = logOutput;
		_logFormat = logFormat;
		_scanMemory = scanMemory;
		_scanPackets = scanPackets;
		_dataEventProcessor = dataEventProcessor;
		_monitor = monitor;
		_settingsMediator = settingsMediator;
		_logTraceListener = traceListener;
		if (!Trace.Listeners.Contains((TraceListener?)(object)_logTraceListener))
		{
			Trace.Listeners.Add((TraceListener)(object)_logTraceListener);
		}
		_logOutput.WriteLine((LogMessageType)253, DateTime.MinValue, _logFormat.FormatVersion());
		_settingsMediator.RestartDataCollection = () =>
		{
			_scanPackets.StopScan();
			_scanPackets.StartScan();
		};
	}

	public void StartMemory()
	{
		_logOutput.ScanThread.Start(_tokenSource.Token);
		_scanMemory.ScanThread.Start(_tokenSource.Token);
		_monitor.ScanThread.Start(_tokenSource.Token);
	}

	public void StopMemory()
	{
		_settingsMediator.RestartDataCollection = null;
		_tokenSource.Cancel(throwOnFirstException: true);
		_scanPackets.StopScan();
		for (int i = 0; i < 25; i++)
		{
			if (!_logOutput.ScanThread.IsAlive && !_scanMemory.ScanThread.IsAlive)
			{
				break;
			}
			Thread.Sleep(10);
		}
		if (_logOutput.ScanThread.IsAlive)
		{
			_logOutput.ScanThread.Abort();
		}
		if (_scanMemory.ScanThread.IsAlive)
		{
			_scanMemory.ScanThread.Abort();
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposedValue)
		{
			return;
		}
		if (disposing)
		{
			if (_logTraceListener != null)
			{
				((TraceListener)(object)_logTraceListener).Flush();
				if (Trace.Listeners.Contains((TraceListener?)(object)_logTraceListener))
				{
					Trace.Listeners.Remove((TraceListener?)(object)_logTraceListener);
				}
			}
			((TraceListener)(object)_logTraceListener)?.Dispose();
			_tokenSource?.Dispose();
		}
		disposedValue = true;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
