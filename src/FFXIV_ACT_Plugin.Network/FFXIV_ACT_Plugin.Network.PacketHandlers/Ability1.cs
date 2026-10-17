using System;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory;
using FFXIV_ACT_Plugin.Resource;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class Ability1 : Ability, IPacketHandler
{
	public uint Opcode => (ushort)Server_MessageType.Ability1;

	public Type PacketType => typeof(Server_ActionEffect1);

	public unsafe int PacketLength => sizeof(Server_ActionEffect1);

	public Ability1(ILogOutput logOutput, ILogFormat logFormat, DataEvent dataEvent, IActionList skillList, ICombatantManager combatantManager, INetworkBuffManager buffManager)
		: base(logOutput, logFormat, dataEvent, skillList, combatantManager, buffManager)
	{
	}

	public unsafe void Process(byte[] buffer, DateTime packetDate)
	{
		fixed (byte* ptr = buffer)
		{
			Server_ActionEffectHeader packetMessage = *(Server_ActionEffectHeader*)ptr;
			Server_ActionEffect1 server_ActionEffect = *(Server_ActionEffect1*)ptr;
			ProcessAbility(packetDate, packetMessage, server_ActionEffect.TargetID, 1, server_ActionEffect.Effects);
		}
	}
}
