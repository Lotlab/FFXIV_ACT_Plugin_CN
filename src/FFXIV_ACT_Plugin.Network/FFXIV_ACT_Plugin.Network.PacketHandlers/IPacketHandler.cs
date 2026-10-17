using System;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public interface IPacketHandler
{
	uint Opcode { get; }

	Type PacketType { get; }

	int PacketLength { get; }

	void Process(byte[] buffer, DateTime packetDate);
}
