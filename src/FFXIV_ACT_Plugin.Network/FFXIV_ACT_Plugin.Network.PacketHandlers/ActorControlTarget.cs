using System;
using System.Runtime.CompilerServices;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class ActorControlTarget : IPacketHandler
{
	private readonly INetworkMarkerManager _markerManager;

	public uint Opcode => (uint)(Server_MessageType.ActorControlTarget);

	public Type PacketType => typeof(Server_ActorControlTarget);

	public unsafe int PacketLength => sizeof(Server_ActorControlTarget);

	public ActorControlTarget(INetworkMarkerManager markerManager)
	{
		_markerManager = markerManager;
	}

	public unsafe void Process(byte[] buffer, DateTime packetDate)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		fixed (byte* ptr = buffer)
		{
			Server_ActorControlTarget val = *(Server_ActorControlTarget*)ptr;
			if ((int)val.category != 502)
			{
				return;
			}
			uint num = val.param2;
			if (num == 3758096384u)
			{
				num = 0u;
			}
			uint targetID = val.TargetID;
			byte signId = (byte)(val.param1 & 0xFF);
			_markerManager.UpdateSign(packetDate, num, targetID, signId);
		}
	}
}
