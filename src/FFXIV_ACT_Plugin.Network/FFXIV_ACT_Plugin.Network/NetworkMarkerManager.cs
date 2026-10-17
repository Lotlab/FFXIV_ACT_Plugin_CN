#define TRACE
using System;
using System.Diagnostics;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network;

public class NetworkMarkerManager : INetworkMarkerManager
{
	private readonly ILogFormat _logFormat;

	private readonly DataEvent _dataEvent;

	private readonly ILogOutput _logOutput;

	private readonly ICombatantManager _combatantManager;

	private readonly byte MAX_SIGNS = 32;

	private readonly byte MAX_WAYMARKS = 32;

	private readonly NetworkMarker[] _signs;

	private readonly NetworkMarker[] _waymarks;

	private readonly object _signLock = new object();

	private readonly object _waymarkLock = new object();

	public NetworkMarkerManager(ILogOutput logOutput, ILogFormat logFormat, DataEvent dataEvent, ICombatantManager combatantManager)
	{
		_logOutput = logOutput;
		_logFormat = logFormat;
		_dataEvent = dataEvent;
		_combatantManager = combatantManager;
		_signs = new NetworkMarker[MAX_SIGNS];
		for (int i = 0; i < MAX_SIGNS; i++)
		{
			_signs[i] = new NetworkMarker();
		}
		_waymarks = new NetworkMarker[MAX_WAYMARKS];
		for (int j = 0; j < MAX_WAYMARKS; j++)
		{
			_waymarks[j] = new NetworkMarker();
		}
	}

	public void UpdateSign(DateTime packetDate, uint actorId, uint targetId, byte signId)
	{
		if (signId > MAX_SIGNS)
		{
			Trace.WriteLine($"Network Sign [{signId}] is larger than expected.", "FFXIV_ACT_Plugin");
			return;
		}
		string sourceName = "";
		if (actorId != 0 && actorId != 3758096384u)
		{
			sourceName = _combatantManager.GetCombatantById(actorId)?.Name;
		}
		string targetName = "";
		if (targetId != 0 && targetId != 3758096384u)
		{
			targetName = _combatantManager.GetCombatantById(targetId)?.Name;
		}
		lock (_signLock)
		{
			if (_signs[signId].TargetID != 0)
			{
				targetName = _combatantManager.GetCombatantById(_signs[signId].TargetID)?.Name;
				string text = _logFormat.FormatNetworkSignMessage($"{NetworkMarker.MarkerEvent.Delete}", signId, actorId, sourceName, _signs[signId].TargetID, targetName);
				_logOutput.WriteLine(LogMessageType.SignMarker, packetDate, text);
				_dataEvent.OnParsedLogLine(LogMessageType.SignMarker, text);
			}
			if (targetId == 3758096384u || targetId == 0)
			{
				_signs[signId].ActorID = 0u;
				_signs[signId].TargetID = 0u;
				return;
			}
			string text2 = _logFormat.FormatNetworkSignMessage($"{NetworkMarker.MarkerEvent.Add}", signId, actorId, sourceName, targetId, targetName);
			_logOutput.WriteLine(LogMessageType.SignMarker, packetDate, text2);
			_dataEvent.OnParsedLogLine(LogMessageType.SignMarker, text2);
			_signs[signId].MarkerType = NetworkMarker.MarkerTarget.Target;
			_signs[signId].Marker = signId;
			_signs[signId].ActorID = actorId;
			_signs[signId].TargetID = targetId;
		}
	}

	public void UpdateWaymark(DateTime packetDate, WaymarkType waymark, Server_Waymark.WaymarkStatus status, uint actorId, float x, float y, float z)
	{
		if ((int)waymark > (int)MAX_WAYMARKS)
		{
			Trace.WriteLine($"Network Waymark [{waymark}] is larger than expected.", "FFXIV_ACT_Plugin");
			return;
		}
		if (status != Server_Waymark.WaymarkStatus.On && status != Server_Waymark.WaymarkStatus.Off)
		{
			Trace.WriteLine($"Network Waymark [{waymark}] has unexpected status value {status}.", "FFXIV_ACT_Plugin");
			return;
		}
		string sourceName = "";
		if (actorId != 0 && actorId != 3758096384u)
		{
			sourceName = _combatantManager.GetCombatantById(actorId)?.Name;
		}
		lock (_waymarkLock)
		{
			if (_waymarks[(uint)waymark].MarkerType == NetworkMarker.MarkerTarget.Ground)
			{
				string text = _logFormat.FormatNetworkWaymarkMessage($"{NetworkMarker.MarkerEvent.Delete}", (uint)waymark, actorId, sourceName, x, y, z);
				_logOutput.WriteLine(LogMessageType.WaymarkMarker, packetDate, text);
				_dataEvent.OnParsedLogLine(LogMessageType.WaymarkMarker, text);
			}
			if (status == Server_Waymark.WaymarkStatus.Off)
			{
				_waymarks[(uint)waymark].MarkerType = (NetworkMarker.MarkerTarget)0;
				_waymarks[(uint)waymark].ActorID = 0u;
				_waymarks[(uint)waymark].PosX = 0f;
				_waymarks[(uint)waymark].PosY = 0f;
				_waymarks[(uint)waymark].PosZ = 0f;
			}
			else
			{
				string text2 = _logFormat.FormatNetworkWaymarkMessage($"{NetworkMarker.MarkerEvent.Add}", (uint)waymark, actorId, sourceName, x, y, z);
				_logOutput.WriteLine(LogMessageType.WaymarkMarker, packetDate, text2);
				_dataEvent.OnParsedLogLine(LogMessageType.WaymarkMarker, text2);
				_waymarks[(uint)waymark].MarkerType = NetworkMarker.MarkerTarget.Ground;
				_waymarks[(uint)waymark].ActorID = actorId;
				_waymarks[(uint)waymark].PosX = x;
				_waymarks[(uint)waymark].PosY = y;
				_waymarks[(uint)waymark].PosZ = z;
			}
		}
	}
}
