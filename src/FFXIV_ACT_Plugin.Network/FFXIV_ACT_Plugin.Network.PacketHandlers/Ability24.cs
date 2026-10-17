using System;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory;
using FFXIV_ACT_Plugin.Resource;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class Ability24 : Ability, IPacketHandler
{
	public uint Opcode => (ushort)Server_MessageType.Ability24;

	public Type PacketType => typeof(Server_ActionEffect24);

	public unsafe int PacketLength => sizeof(Server_ActionEffect24);

	public Ability24(ILogOutput logOutput, ILogFormat logFormat, DataEvent dataEvent, IActionList skillList, ICombatantManager combatantManager, INetworkBuffManager buffManager)
		: base(logOutput, logFormat, dataEvent, skillList, combatantManager, buffManager)
	{
	}

	public unsafe void Process(byte[] buffer, DateTime packetDate)
	{
		fixed (byte* ptr = buffer)
		{
			Server_ActionEffectHeader packetMessage = *(Server_ActionEffectHeader*)ptr;
			Server_ActionEffect24 server_ActionEffect = *(Server_ActionEffect24*)ptr;
			ProcessAbility(packetDate, packetMessage, server_ActionEffect.TargetID, 24, server_ActionEffect.Effects);
		}
	}
}
