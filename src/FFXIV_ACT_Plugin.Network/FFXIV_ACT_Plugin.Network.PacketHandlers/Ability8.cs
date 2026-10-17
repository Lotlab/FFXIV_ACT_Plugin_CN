using System;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory;
using FFXIV_ACT_Plugin.Resource;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class Ability8 : Ability, IPacketHandler
{
	public uint Opcode => (ushort)Server_MessageType.Ability8;

	public Type PacketType => typeof(Server_ActionEffect8);

	public unsafe int PacketLength => sizeof(Server_ActionEffect8);

	public Ability8(ILogOutput logOutput, ILogFormat logFormat, DataEvent dataEvent, IActionList skillList, ICombatantManager combatantManager, INetworkBuffManager buffManager)
		: base(logOutput, logFormat, dataEvent, skillList, combatantManager, buffManager)
	{
	}

	public unsafe void Process(byte[] buffer, DateTime packetDate)
	{
		fixed (byte* ptr = buffer)
		{
			Server_ActionEffectHeader packetMessage = *(Server_ActionEffectHeader*)ptr;
			Server_ActionEffect8 server_ActionEffect = *(Server_ActionEffect8*)ptr;
			ProcessAbility(packetDate, packetMessage, server_ActionEffect.TargetID, 8, server_ActionEffect.Effects);
		}
	}
}
