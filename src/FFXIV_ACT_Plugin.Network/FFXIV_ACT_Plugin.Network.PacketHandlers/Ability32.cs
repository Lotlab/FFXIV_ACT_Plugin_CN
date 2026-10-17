using System;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory;
using FFXIV_ACT_Plugin.Resource;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class Ability32 : Ability, IPacketHandler
{
	public uint Opcode => (ushort)Server_MessageType.Ability32;

	public Type PacketType => typeof(Server_ActionEffect32);

	public unsafe int PacketLength => sizeof(Server_ActionEffect32);

	public Ability32(ILogOutput logOutput, ILogFormat logFormat, DataEvent dataEvent, IActionList skillList, ICombatantManager combatantManager, INetworkBuffManager buffManager)
		: base(logOutput, logFormat, dataEvent, skillList, combatantManager, buffManager)
	{
	}

	public unsafe void Process(byte[] buffer, DateTime packetDate)
	{
		fixed (byte* ptr = buffer)
		{
			Server_ActionEffectHeader packetMessage = *(Server_ActionEffectHeader*)ptr;
			Server_ActionEffect32 server_ActionEffect = *(Server_ActionEffect32*)ptr;
			ProcessAbility(packetDate, packetMessage, server_ActionEffect.TargetID, 32, server_ActionEffect.Effects);
		}
	}
}
