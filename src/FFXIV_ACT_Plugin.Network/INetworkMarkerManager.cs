using System;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network;

public interface INetworkMarkerManager
{
	void UpdateSign(DateTime packetDate, uint actorId, uint targetId, byte signId);

	void UpdateWaymark(DateTime packetDate, WaymarkType waymark, Server_Waymark.WaymarkStatus status, uint actorId, float x, float y, float z);
}
