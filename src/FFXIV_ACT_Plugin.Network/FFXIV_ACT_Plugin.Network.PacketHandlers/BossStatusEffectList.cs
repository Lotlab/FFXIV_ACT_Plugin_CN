using System;
using FFXIV_ACT_Plugin.Common.Models;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class BossStatusEffectList : IPacketHandler
{
	private readonly INetworkBuffManager _buffManager;

	private readonly ILogFormat _logFormat;

	private readonly ICombatantManager _combatantManager;

	private readonly ILogOutput _logOutput;

	private readonly DataEvent _dataEvent;

	public uint Opcode => (ushort)Server_MessageType.BossStatusEffectList;

	public Type PacketType => typeof(Server_BossStatusEffectList);

	public unsafe int PacketLength => sizeof(Server_BossStatusEffectList);

	public BossStatusEffectList(INetworkBuffManager buffManager, ICombatantManager combatantManager, ILogFormat logFormat, ILogOutput logOutput, DataEvent dataEvent)
	{
		_buffManager = buffManager;
		_combatantManager = combatantManager;
		_logFormat = logFormat;
		_logOutput = logOutput;
		_dataEvent = dataEvent;
	}

	public unsafe void Process(byte[] buffer, DateTime packetDate)
	{
		fixed (byte* ptr = buffer)
		{
			Server_BossStatusEffectList server_BossStatusEffectList = *(Server_BossStatusEffectList*)ptr;
			uint actorID = server_BossStatusEffectList.MessageHeader.ActorID;
			uint[] array = new uint[183];
			array[0] = server_BossStatusEffectList.Unknown1;
			array[1] = server_BossStatusEffectList.Unknown2;
			Server_StatusEffectListEntry* ptr2 = (Server_StatusEffectListEntry*)server_BossStatusEffectList.Effects1;
			for (byte b = 0; b < 30; b++)
			{
				if (ptr2[(int)b].EffectID != 0)
				{
					array[3 + b * 3] = (uint)(ptr2[(int)b].EffectID + (ptr2[(int)b].OtherInfo << 16));
					array[3 + b * 3 + 1] = BitConverter.ToUInt32(BitConverter.GetBytes(ptr2[(int)b].Duration), 0);
					array[3 + b * 3 + 2] = ptr2[(int)b].ActorID;
				}
			}
			Server_StatusEffectListEntry* ptr3 = (Server_StatusEffectListEntry*)server_BossStatusEffectList.Effects2;
			for (byte b2 = 0; b2 < 30; b2++)
			{
				if (ptr3[(int)b2].EffectID != 0)
				{
					array[93 + b2 * 3] = (uint)(ptr3[(int)b2].EffectID + (ptr3[(int)b2].OtherInfo << 16));
					array[93 + b2 * 3 + 1] = BitConverter.ToUInt32(BitConverter.GetBytes(ptr3[(int)b2].Duration), 0);
					array[93 + b2 * 3 + 2] = ptr3[(int)b2].ActorID;
				}
			}
			Combatant combatantById = _combatantManager.GetCombatantById(server_BossStatusEffectList.MessageHeader.ActorID);
			uint jobLevels = (uint)(server_BossStatusEffectList.JobID + (server_BossStatusEffectList.Level1 << 8) + (server_BossStatusEffectList.Level2 << 16) + (server_BossStatusEffectList.Level3 << 24));
			string text = _logFormat.FormatStatusListMessage(server_BossStatusEffectList.MessageHeader.ActorID, combatantById?.Name, jobLevels, server_BossStatusEffectList.CurrentHP, server_BossStatusEffectList.MaxHP, server_BossStatusEffectList.CurrentMP, server_BossStatusEffectList.MaxMP, server_BossStatusEffectList.DamageShield, combatantById?.PosX, combatantById?.PosY, combatantById?.PosZ, combatantById?.Heading, array);
			_logOutput.WriteLine(LogMessageType.StatusList, packetDate, text);
			_dataEvent.OnParsedLogLine(LogMessageType.StatusList, text);
			NetworkBuff[] buffs = ConvertNetworkBuffs((Server_StatusEffectListEntry*)server_BossStatusEffectList.Effects1, (Server_StatusEffectListEntry*)server_BossStatusEffectList.Effects2, packetDate);
			_buffManager.ReplaceAllBuffsForTarget(actorID, buffs, packetDate);
		}
	}

	private unsafe NetworkBuff[] ConvertNetworkBuffs(Server_StatusEffectListEntry* effects1, Server_StatusEffectListEntry* effects2, DateTime packetDate)
	{
		NetworkBuff[] array = new NetworkBuff[60];
		for (byte b = 0; b < 30; b++)
		{
			array[b] = new NetworkBuff();
			array[b].BuffID = effects1[(int)b].EffectID;
			if (array[b].BuffID != 0)
			{
				array[b].BuffExtra = effects1[(int)b].OtherInfo;
				array[b].ActorID = effects1[(int)b].ActorID;
				array[b].Duration = Math.Abs(effects1[(int)b].Duration);
				array[b].Timestamp = packetDate;
			}
		}
		for (byte b2 = 0; b2 < 30; b2++)
		{
			array[b2 + 30] = new NetworkBuff();
			array[b2 + 30].BuffID = effects2[(int)b2].EffectID;
			if (array[b2 + 30].BuffID != 0)
			{
				array[b2 + 30].BuffExtra = effects2[(int)b2].OtherInfo;
				array[b2 + 30].ActorID = effects2[(int)b2].ActorID;
				array[b2 + 30].Duration = Math.Abs(effects2[(int)b2].Duration);
				array[b2 + 30].Timestamp = packetDate;
			}
		}
		return array;
	}
}
