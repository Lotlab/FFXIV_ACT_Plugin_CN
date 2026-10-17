using System;
using System.Runtime.CompilerServices;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class PresetWaymark : IPacketHandler
{
	private readonly INetworkMarkerManager _markerManager;

	public uint Opcode => (uint)(Server_MessageType.PresetWaymark);

	public Type PacketType => typeof(Server_PresetWaymark);

	public unsafe int PacketLength => sizeof(Server_PresetWaymark);

	public PresetWaymark(INetworkMarkerManager markerManager)
	{
		_markerManager = markerManager;
	}

	public unsafe void Process(byte[] buffer, DateTime packetDate)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Invalid comparison between Unknown and I4
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		fixed (byte* ptr = buffer)
		{
			Server_PresetWaymark val = *(Server_PresetWaymark*)ptr;
			for (byte b = 0; b < 8; b++)
			{
				byte b2 = (byte)(1 << (int)b);
				bool flag = ((byte)val.WaymarkType & b2) == b2;
				float x = (float)val.PosX[b] / 1000f;
				float y = (float)val.PosY[b] / 1000f;
				float z = (float)val.PosZ[b] / 1000f;
				_markerManager.UpdateWaymark(packetDate, (WaymarkType)b, (Server_Waymark.WaymarkStatus)(flag ? 1 : 0), val.MessageHeader.ActorID, x, y, z);
			}
		}
	}
}
