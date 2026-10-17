#define TRACE
using System;
using System.Diagnostics;
using System.Threading;
using FFXIV_ACT_Plugin.Common;
using FFXIV_ACT_Plugin.Memory;
using FFXIV_ACT_Plugin.Network;

namespace FFXIV_ACT_Plugin;

public class MonitorNetwork
{
	private readonly IDataRepository _dataRepository;

	private readonly IScanPackets _scanPackets;

	private readonly ProblemDiagnosisHelper _diagnosisHelper;

	private readonly IProcessManager _processRepository;

	private readonly IActWrapper _actWrapper;

	private int _failureCount;

	private int Timeout { get; } = 60;

	public Thread ScanThread { get; }

	public MonitorNetwork(IDataRepository dataRepository, IScanPackets scanPackets, IProcessManager processRepository, ProblemDiagnosisHelper diagnosisHelper, IActWrapper actWrapper)
	{
		_dataRepository = dataRepository;
		_scanPackets = scanPackets;
		_diagnosisHelper = diagnosisHelper;
		_processRepository = processRepository;
		_actWrapper = actWrapper;
		_processRepository.ProcessChanged += OnProcessChanged;
		ScanThread = new Thread(Run)
		{
			IsBackground = true,
			Name = GetType().FullName
		};
	}

	private void OnProcessChanged(object sender, EventArgs e)
	{
		if (_processRepository.Verify())
		{
			Interlocked.Exchange(ref _failureCount, 0);
		}
		else
		{
			Interlocked.Exchange(ref _failureCount, 100);
		}
	}

	public void Run(object parameter)
	{
		CancellationToken cancellationToken = (CancellationToken)parameter;
		while (!_actWrapper.IsMainFormVisible())
		{
			if (cancellationToken.WaitHandle.WaitOne(1000))
			{
				return;
			}
		}
		while (!cancellationToken.WaitHandle.WaitOne(1000))
		{
			try
			{
				uint currentTerritoryID = _dataRepository.GetCurrentTerritoryID();
				if (_dataRepository.GetCurrentPlayerID() == 0 || currentTerritoryID == 0)
				{
					continue;
				}
				if (_scanPackets.IsNetworkDataValid() == 0)
				{
					Interlocked.Exchange(ref _failureCount, 0);
				}
				else
				{
					Interlocked.Increment(ref _failureCount);
				}
				if (_failureCount == Timeout)
				{
					ACTWrapper.RunOnACTUIThread(() =>
					{
						_diagnosisHelper.Diagnose(isAutomatedScan: true);
					});
				}
				if (_failureCount > 2147473647)
				{
					Interlocked.Exchange(ref _failureCount, 100);
				}
			}
			catch (ThreadAbortException)
			{
				break;
			}
			catch (Exception ex2)
			{
				Trace.WriteLine("网络解析错误：" + ex2.ToString().Replace(Environment.NewLine, " "), "FFXIV_ACT_Plugin");
			}
		}
	}
}
