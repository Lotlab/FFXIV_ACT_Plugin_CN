using System;
using System.Runtime.CompilerServices;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class Waymark : IPacketHandler
{
	private readonly INetworkMarkerManager _markerManager;

	public uint Opcode => (uint)(Server_MessageType.Waymark);

	public Type PacketType => typeof(Server_Waymark);

	public unsafe int PacketLength => sizeof(Server_Waymark);

	public Waymark(INetworkMarkerManager markerManager)
	{
		_markerManager = markerManager;
	}

	public unsafe void Process(byte[] buffer, DateTime packetDate)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		fixed (byte* ptr = buffer)
		{
			Server_Waymark val = *(Server_Waymark*)ptr;
			float x = (float)val.PosX / 1000f;
			float y = (float)val.PosY / 1000f;
			float z = (float)val.PosZ / 1000f;
			_markerManager.UpdateWaymark(packetDate, val.Waymark, val.Status, val.MessageHeader.ActorID, x, y, z);
		}
	}
}
