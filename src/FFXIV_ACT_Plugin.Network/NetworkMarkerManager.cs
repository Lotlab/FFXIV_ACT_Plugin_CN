#define TRACE
using System;
using System.Diagnostics;
using FFXIV_ACT_Plugin.Common.Models;
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
		string text = "";
		if (actorId != 0 && actorId != 3758096384u)
		{
			Combatant combatantById = _combatantManager.GetCombatantById(actorId);
			text = ((combatantById != null) ? combatantById.Name : null);
		}
		string text2 = "";
		if (targetId != 0 && targetId != 3758096384u)
		{
			Combatant combatantById2 = _combatantManager.GetCombatantById(targetId);
			text2 = ((combatantById2 != null) ? combatantById2.Name : null);
		}
		lock (_signLock)
		{
			if (_signs[signId].TargetID != 0)
			{
				Combatant combatantById3 = _combatantManager.GetCombatantById(_signs[signId].TargetID);
				text2 = ((combatantById3 != null) ? combatantById3.Name : null);
				string text3 = _logFormat.FormatNetworkSignMessage($"{NetworkMarker.MarkerEvent.Delete}", (uint)signId, actorId, text, (uint?)_signs[signId].TargetID, text2);
				_logOutput.WriteLine((LogMessageType)29, packetDate, text3);
				_dataEvent.OnParsedLogLine((LogMessageType)29, text3);
			}
			if (targetId == 3758096384u || targetId == 0)
			{
				_signs[signId].ActorID = 0u;
				_signs[signId].TargetID = 0u;
				return;
			}
			string text4 = _logFormat.FormatNetworkSignMessage($"{NetworkMarker.MarkerEvent.Add}", (uint)signId, actorId, text, (uint?)targetId, text2);
			_logOutput.WriteLine((LogMessageType)29, packetDate, text4);
			_dataEvent.OnParsedLogLine((LogMessageType)29, text4);
			_signs[signId].MarkerType = NetworkMarker.MarkerTarget.Target;
			_signs[signId].Marker = signId;
			_signs[signId].ActorID = actorId;
			_signs[signId].TargetID = targetId;
		}
	}

	public void UpdateWaymark(DateTime packetDate, WaymarkType waymark, Server_Waymark.WaymarkStatus status, uint actorId, float x, float y, float z)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected I4, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Expected I4, but got Unknown
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		if ((int)waymark > (int)MAX_WAYMARKS)
		{
			Trace.WriteLine($"Network Waymark [{waymark}] is larger than expected.", "FFXIV_ACT_Plugin");
			return;
		}
		if ((int)status != 1 && (int)status != 0)
		{
			Trace.WriteLine($"Network Waymark [{waymark}] has unexpected status value {status}.", "FFXIV_ACT_Plugin");
			return;
		}
		string text = "";
		if (actorId != 0 && actorId != 3758096384u)
		{
			Combatant combatantById = _combatantManager.GetCombatantById(actorId);
			text = ((combatantById != null) ? combatantById.Name : null);
		}
		lock (_waymarkLock)
		{
			if (_waymarks[(int)waymark].MarkerType == NetworkMarker.MarkerTarget.Ground)
			{
				string text2 = _logFormat.FormatNetworkWaymarkMessage($"{NetworkMarker.MarkerEvent.Delete}", (uint)(int)waymark, actorId, text, (float?)x, (float?)y, (float?)z);
				_logOutput.WriteLine((LogMessageType)28, packetDate, text2);
				_dataEvent.OnParsedLogLine((LogMessageType)28, text2);
			}
			if ((int)status == 0)
			{
				_waymarks[(int)waymark].MarkerType = (NetworkMarker.MarkerTarget)0;
				_waymarks[(int)waymark].ActorID = 0u;
				_waymarks[(int)waymark].PosX = 0f;
				_waymarks[(int)waymark].PosY = 0f;
				_waymarks[(int)waymark].PosZ = 0f;
			}
			else
			{
				string text3 = _logFormat.FormatNetworkWaymarkMessage($"{NetworkMarker.MarkerEvent.Add}", (uint)(int)waymark, actorId, text, (float?)x, (float?)y, (float?)z);
				_logOutput.WriteLine((LogMessageType)28, packetDate, text3);
				_dataEvent.OnParsedLogLine((LogMessageType)28, text3);
				_waymarks[(int)waymark].MarkerType = NetworkMarker.MarkerTarget.Ground;
				_waymarks[(int)waymark].ActorID = actorId;
				_waymarks[(int)waymark].PosX = x;
				_waymarks[(int)waymark].PosY = y;
				_waymarks[(int)waymark].PosZ = z;
			}
		}
	}
}
