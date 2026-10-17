#define TRACE
using System;
using System.Diagnostics;
using System.Threading;
using FFXIV_ACT_Plugin.Common;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Memory.MemoryProcessors;

namespace FFXIV_ACT_Plugin.Memory;

public class ScanMemory : IScanMemory
{
	private readonly IProcessManager _processManager;

	private readonly IServerTimeProcessor _serverTimeProcessor;

	private readonly IZoneMapProcessor _zoneProcessor;

	private readonly IMobArrayProcessor _mobArrayProcessor;

	private readonly ICombatantManager _combatantManager;

	private readonly IPlayerProcessor _playerProcessor;

	private readonly IPartyProcessor _partyProcessor;

	private readonly ILogProcessor _logProcessor;

	private readonly IActWrapper _actWrapper;

	private readonly ISettingsMediator _settingsManager;

	private readonly IBenchmarkRepository _benchmarkRepository;

	public Thread ScanThread { get; private set; }

	public ScanMemory(ISettingsMediator settingsManager, IProcessManager processManager, IServerTimeProcessor serverTime, IZoneMapProcessor zoneProcessor, IMobArrayProcessor mobArrayProcessor, ICombatantManager combatantManager, IPlayerProcessor playerProcessor, IPartyProcessor partyProcessor, ILogProcessor logProcessor, IActWrapper actWrapper, IBenchmarkRepository benchmarkRepository)
	{
		_processManager = processManager;
		_serverTimeProcessor = serverTime;
		_zoneProcessor = zoneProcessor;
		_mobArrayProcessor = mobArrayProcessor;
		_combatantManager = combatantManager;
		_playerProcessor = playerProcessor;
		_partyProcessor = partyProcessor;
		_logProcessor = logProcessor;
		_actWrapper = actWrapper;
		_settingsManager = settingsManager;
		_benchmarkRepository = benchmarkRepository;
		ScanThread = new Thread(Run)
		{
			IsBackground = true,
			Name = GetType().FullName
		};
	}

	public void Run(object parameter)
	{
		CancellationToken cancellationToken = (CancellationToken)parameter;
		while (!_actWrapper.IsMainFormVisible())
		{
			if (cancellationToken.WaitHandle.WaitOne(100))
			{
				return;
			}
		}
		while (!cancellationToken.WaitHandle.WaitOne(10))
		{
			try
			{
				if (_settingsManager.DataCollectionSettings == null || !_processManager.Verify())
				{
					if (cancellationToken.WaitHandle.WaitOne(1000))
					{
						break;
					}
					continue;
				}
				_benchmarkRepository.Measure("RefreshServerTime", () =>
				{
					_serverTimeProcessor.Refresh();
				});
				_benchmarkRepository.Measure("RefreshMobArray", () =>
				{
					_mobArrayProcessor.Refresh();
				});
				if (!(_mobArrayProcessor.PrimaryPlayerPointer != IntPtr.Zero))
				{
					continue;
				}
				uint zoneID = _zoneProcessor.ZoneID;
				_benchmarkRepository.Measure("RefreshZone", () =>
				{
					_zoneProcessor.Refresh();
				});
				if (_zoneProcessor.ZoneID == 0)
				{
					continue;
				}
				if (zoneID != _zoneProcessor.ZoneID)
				{
					_benchmarkRepository.Measure("RescanCombatants", () =>
					{
						_combatantManager.Rescan();
					});
				}
				else
				{
					_benchmarkRepository.Measure("RefreshCombatants", () =>
					{
						_combatantManager.Refresh();
					});
				}
				_benchmarkRepository.Measure("RefreshPlayer", () =>
				{
					_playerProcessor.Refresh();
				});
				_benchmarkRepository.Measure("RefreshParty", () =>
				{
					_partyProcessor.Refresh();
				});
				_benchmarkRepository.Measure("RefreshChatlog", () =>
				{
					_logProcessor.Refresh();
				});
			}
			catch (ThreadAbortException)
			{
				break;
			}
			catch (Exception ex2)
			{
				Trace.WriteLine("ScanCombatant Error: " + ex2.ToString().Replace(Environment.NewLine, " "), "FFXIV_ACT_Plugin");
				if (cancellationToken.WaitHandle.WaitOne(100))
				{
					break;
				}
			}
		}
	}
}
