using System;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class Waymark : IPacketHandler
{
	private readonly INetworkMarkerManager _markerManager;

	public uint Opcode => (ushort)Server_MessageType.Waymark;

	public Type PacketType => typeof(Server_Waymark);

	public unsafe int PacketLength => sizeof(Server_Waymark);

	public Waymark(INetworkMarkerManager markerManager)
	{
		_markerManager = markerManager;
	}

	public unsafe void Process(byte[] buffer, DateTime packetDate)
	{
		fixed (byte* ptr = buffer)
		{
			Server_Waymark server_Waymark = *(Server_Waymark*)ptr;
			float x = (float)server_Waymark.PosX / 1000f;
			float y = (float)server_Waymark.PosY / 1000f;
			float z = (float)server_Waymark.PosZ / 1000f;
			_markerManager.UpdateWaymark(packetDate, server_Waymark.Waymark, server_Waymark.Status, server_Waymark.MessageHeader.ActorID, x, y, z);
		}
	}
}
