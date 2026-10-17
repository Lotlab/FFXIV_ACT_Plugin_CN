using System;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class PresetWaymark : IPacketHandler
{
	private readonly INetworkMarkerManager _markerManager;

	public uint Opcode => (ushort)Server_MessageType.PresetWaymark;

	public Type PacketType => typeof(Server_PresetWaymark);

	public unsafe int PacketLength => sizeof(Server_PresetWaymark);

	public PresetWaymark(INetworkMarkerManager markerManager)
	{
		_markerManager = markerManager;
	}

	public unsafe void Process(byte[] buffer, DateTime packetDate)
	{
		fixed (byte* ptr = buffer)
		{
			Server_PresetWaymark server_PresetWaymark = *(Server_PresetWaymark*)ptr;
			for (byte b = 0; b < 8; b++)
			{
				byte b2 = (byte)(1 << (int)b);
				bool flag = ((uint)server_PresetWaymark.WaymarkType & (uint)b2) == b2;
				float x = (float)server_PresetWaymark.PosX[(int)b] / 1000f;
				float y = (float)server_PresetWaymark.PosY[(int)b] / 1000f;
				float z = (float)server_PresetWaymark.PosZ[(int)b] / 1000f;
				_markerManager.UpdateWaymark(packetDate, (WaymarkType)b, flag ? Server_Waymark.WaymarkStatus.On : Server_Waymark.WaymarkStatus.Off, server_PresetWaymark.MessageHeader.ActorID, x, y, z);
			}
		}
	}
}
