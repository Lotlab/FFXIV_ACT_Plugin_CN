using System;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class ActorControlTarget : IPacketHandler
{
	private readonly INetworkMarkerManager _markerManager;

	public uint Opcode => (ushort)Server_MessageType.ActorControlTarget;

	public Type PacketType => typeof(Server_ActorControlTarget);

	public unsafe int PacketLength => sizeof(Server_ActorControlTarget);

	public ActorControlTarget(INetworkMarkerManager markerManager)
	{
		_markerManager = markerManager;
	}

	public unsafe void Process(byte[] buffer, DateTime packetDate)
	{
		fixed (byte* ptr = buffer)
		{
			Server_ActorControlTarget server_ActorControlTarget = *(Server_ActorControlTarget*)ptr;
			if (server_ActorControlTarget.category != Server_ActorControlCategory.SetTargetSign)
			{
				return;
			}
			uint num = server_ActorControlTarget.param2;
			if (num == 3758096384u)
			{
				num = 0u;
			}
			uint targetID = server_ActorControlTarget.TargetID;
			byte signId = (byte)(server_ActorControlTarget.param1 & 0xFF);
			_markerManager.UpdateSign(packetDate, num, targetID, signId);
		}
	}
}
