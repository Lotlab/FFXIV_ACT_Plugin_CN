#define TRACE
using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Memory;
using Machina.FFXIV;
using Machina.FFXIV.Deucalion;
using Machina.FFXIV.Headers.Opcodes;
using Machina.FFXIV.Oodle;
using Machina.Infrastructure;

namespace FFXIV_ACT_Plugin.Network;

public class ScanPackets : IScanPackets, IDisposable
{
	private readonly ISettingsMediator _settingsMediator;

	private readonly IProcessManager _processManager;

	private readonly PacketHandlerMediator _packetHandlerMediator;

	private readonly DataEvent _dataEvent;

	private readonly INetworkPacketUnscrambler _unscrambler;

	private FFXIVNetworkMonitor _monitor;

	/// <summary>
	/// True while capturing with a plain socket/Npcap monitor, i.e. while the
	/// packets still carry the client obfuscation.  Deucalion already delivers
	/// plaintext, so unscrambling must stay off in that mode.
	/// </summary>
	private bool _unscramblePackets;

	private DateTime _lastDataProcessedTimestamp = DateTime.MinValue;

	private DateTime _lastDataReceivedTimestamp = DateTime.MinValue;

	private bool disposedValue;

	public ScanPackets(ISettingsMediator settingsManager, IProcessManager processManager, DataEvent dataEvent, PacketHandlerMediator packetHandlerMediator, INetworkPacketUnscrambler unscrambler)
	{
		_settingsMediator = settingsManager;
		_processManager = processManager;
		_dataEvent = dataEvent;
		_packetHandlerMediator = packetHandlerMediator;
		_unscrambler = unscrambler;
		_processManager.ProcessChanged += OnProcessChanged;
	}

	private void OnProcessChanged(object sender, EventArgs e)
	{
		int processId = _processManager.Current.ProcessId;
		if (processId == 0 || _processManager.Current.ClientMode == FFXIVClientMode.Unknown)
		{
			StopScan();
		}
		else if (_monitor == null || _monitor.ProcessID != processId)
		{
			StartScan();
		}
	}

	public void StartScan()
	{
		try
		{
			StopScan();
			_lastDataReceivedTimestamp = DateTime.MinValue;
			_lastDataProcessedTimestamp = DateTime.MinValue;
			int processId = _processManager.Current.ProcessId;
			if (_settingsMediator.DataCollectionSettings != null && processId != 0)
			{
				DataCollectionSettingsEventArgs settings = _settingsMediator.DataCollectionSettings;
				OpcodeManager.Instance.SetRegion((GameRegion)settings.RegionID);
				DeucalionInjector.GameRegion = (GameRegion)settings.RegionID;
				_packetHandlerMediator.LoadPacketHandlers();
				_monitor = new FFXIVNetworkMonitor();
				_monitor.MonitorType = (settings.UseWinPCap ? NetworkMonitorType.WinPCap : NetworkMonitorType.RawSocket);
				_monitor.ProcessID = (uint)processId;
				_monitor.ProcessIDList.Add((uint)processId);
				if (!string.IsNullOrWhiteSpace(settings.NetworkIP))
				{
					if (IPAddress.TryParse(settings.NetworkIP, out IPAddress localIp))
					{
						_monitor.LocalIP = localIp;
					}
					else
					{
						Trace.WriteLine("Ignoring unparsable capture address [" + settings.NetworkIP + "].", "FFXIV_ACT_Plugin");
					}
				}
				if (settings.UseSocketFilter)
				{
					_monitor.UseRemoteIpFilter = true;
				}
				_monitor.OodleImplementation = OodleImplementation.FfxivTcp;
				string gameFile = _processManager.Current.GameFile;
				if (!string.IsNullOrEmpty(gameFile))
				{
					_monitor.OodlePath = (gameFile.EndsWith("\\ffxiv.exe", StringComparison.OrdinalIgnoreCase) ? gameFile.Replace("\\ffxiv.exe", "\\ffxiv_dx11.exe") : gameFile);
				}
				_monitor.UseDeucalion = settings.UseDeucalion;
				// Deucalion reads the packets after the game has already deobfuscated
				// them; only a plain capture needs the obfuscation reversed.
				_unscramblePackets = !settings.UseDeucalion;
				if (_unscramblePackets)
				{
					_unscrambler.Initialize(gameFile);
				}
				_monitor.MessageReceivedEventHandler = (TCPConnection connection, long epoch, byte[] message) =>
				{
					NetworkMessageReceived(connection, epoch, message);
				};
				_monitor.MessageSentEventHandler = (TCPConnection connection, long epoch, byte[] message) =>
				{
					NetworkMessageSent(connection, epoch, message);
				};
				_monitor.Start();
			}
		}
		catch (Exception ex)
		{
			Trace.WriteLine("Unexpected error trying to read network data.  Please restart ACT.  " + ex.ToString().Replace(Environment.NewLine, " "), "FFXIV_ACT_Plugin");
		}
	}

	public void StopScan()
	{
		_unscramblePackets = false;
		if (_monitor != null)
		{
			_monitor.Stop();
			_monitor.MessageReceivedEventHandler = null;
			_monitor.MessageSentEventHandler = null;
			_monitor = null;
		}
	}

	public void NetworkMessageReceived(TCPConnection connection, long epoch, byte[] message)
	{
		_lastDataReceivedTimestamp = DateTime.UtcNow;
		try
		{
			// Reverses the client-side obfuscation before the handlers see the packet.
			if (_unscramblePackets && _unscrambler != null && _unscrambler.Enabled)
			{
				_unscrambler.TryUnscramble(message);
			}
			if (_packetHandlerMediator.ProcessPacket(epoch, message))
			{
				_lastDataProcessedTimestamp = DateTime.UtcNow;
			}
		}
		catch (Exception ex)
		{
			StringBuilder stringBuilder = new StringBuilder(message.Length * 3);
			for (int i = 0; i < message.Length / 4; i++)
			{
				if (i < 16)
				{
					stringBuilder.Append($"{BitConverter.ToUInt32(message, i * 4):X8}|");
				}
			}
			Trace.WriteLine("NetworkMessageReceived Exception.  Message: " + stringBuilder.ToString() + ", Exception: " + ex.ToString().Replace(Environment.NewLine, " "), "FFXIV_ACT_Plugin");
		}
		try
		{
			_dataEvent.OnNetworkReceived(connection.ID, epoch, message);
		}
		catch (Exception ex2)
		{
			StringBuilder stringBuilder2 = new StringBuilder(message.Length * 3);
			for (int j = 0; j < message.Length / 4; j++)
			{
				if (j < 16)
				{
					stringBuilder2.Append($"{BitConverter.ToUInt32(message, j * 4):X8}|");
				}
			}
			Trace.WriteLine("NetworkMessageReceived DataEvent Exception.  Message: " + stringBuilder2.ToString() + ", Exception: " + ex2.ToString().Replace(Environment.NewLine, " "), "FFXIV_ACT_Plugin");
		}
	}

	private void NetworkMessageSent(TCPConnection connection, long epoch, byte[] message)
	{
		try
		{
			_dataEvent.OnNetworkSent(connection.ID, epoch, message);
		}
		catch (Exception ex)
		{
			StringBuilder stringBuilder = new StringBuilder(message.Length * 3);
			for (int i = 0; i < message.Length / 4; i++)
			{
				if (i < 16)
				{
					stringBuilder.Append($"{BitConverter.ToUInt32(message, i * 4):X8}|");
				}
			}
			Trace.WriteLine("NetworkMessageSent Exception.  Message: " + stringBuilder.ToString() + ", Exception: " + ex.ToString().Replace(Environment.NewLine, " "), "FFXIV_ACT_Plugin");
		}
	}

	public int IsNetworkDataValid()
	{
		if (DateTime.UtcNow.Subtract(_lastDataProcessedTimestamp).TotalSeconds < 30.0)
		{
			return 0;
		}
		if (DateTime.UtcNow.Subtract(_lastDataReceivedTimestamp).TotalSeconds < 30.0)
		{
			return 1;
		}
		return 2;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				_processManager.ProcessChanged -= OnProcessChanged;
				_monitor?.Dispose();
				_unscrambler?.Dispose();
			}
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
