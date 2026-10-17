using System;
using System.Runtime.CompilerServices;
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

	public uint Opcode => (uint)(Server_MessageType.BossStatusEffectList);

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
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		fixed (byte* ptr = buffer)
		{
			Server_BossStatusEffectList val = *(Server_BossStatusEffectList*)ptr;
			uint actorID = val.MessageHeader.ActorID;
			uint[] array = new uint[183];
			array[0] = val.Unknown1;
			array[1] = val.Unknown2;
			Server_StatusEffectListEntry* ptr2 = (Server_StatusEffectListEntry*)val.Effects1;
			for (byte b = 0; b < 30; b++)
			{
				if ((*(ptr2 + (int)b)).EffectID != 0)
				{
					array[3 + b * 3] = (uint)((*(ptr2 + (int)b)).EffectID + ((*(ptr2 + (int)b)).OtherInfo << 16));
					array[3 + b * 3 + 1] = BitConverter.ToUInt32(BitConverter.GetBytes((*(ptr2 + (int)b)).Duration), 0);
					array[3 + b * 3 + 2] = (*(ptr2 + (int)b)).ActorID;
				}
			}
			Server_StatusEffectListEntry* ptr3 = (Server_StatusEffectListEntry*)(val.Effects2);
			for (byte b2 = 0; b2 < 30; b2++)
			{
				if ((*(ptr3 + (int)b2)).EffectID != 0)
				{
					array[93 + b2 * 3] = (uint)((*(ptr3 + (int)b2)).EffectID + ((*(ptr3 + (int)b2)).OtherInfo << 16));
					array[93 + b2 * 3 + 1] = BitConverter.ToUInt32(BitConverter.GetBytes((*(ptr3 + (int)b2)).Duration), 0);
					array[93 + b2 * 3 + 2] = (*(ptr3 + (int)b2)).ActorID;
				}
			}
			Combatant combatantById = _combatantManager.GetCombatantById(val.MessageHeader.ActorID);
			uint num = (uint)(val.JobID + (val.Level1 << 8) + (val.Level2 << 16) + (val.Level3 << 24));
			string text = _logFormat.FormatStatusListMessage(val.MessageHeader.ActorID, (combatantById != null) ? combatantById.Name : null, num, val.CurrentHP, val.MaxHP, (uint)val.CurrentMP, (uint)val.MaxMP, val.DamageShield, (combatantById != null) ? new float?(combatantById.PosX) : ((float?)null), (combatantById != null) ? new float?(combatantById.PosY) : ((float?)null), (combatantById != null) ? new float?(combatantById.PosZ) : ((float?)null), (combatantById != null) ? new float?(combatantById.Heading) : ((float?)null), array);
			_logOutput.WriteLine((LogMessageType)38, packetDate, text);
			_dataEvent.OnParsedLogLine((LogMessageType)38, text);
			NetworkBuff[] buffs = ConvertNetworkBuffs((Server_StatusEffectListEntry*)val.Effects1, (Server_StatusEffectListEntry*)(val.Effects2), packetDate);
			_buffManager.ReplaceAllBuffsForTarget(actorID, buffs, packetDate);
		}
	}

	private unsafe NetworkBuff[] ConvertNetworkBuffs(Server_StatusEffectListEntry* effects1, Server_StatusEffectListEntry* effects2, DateTime packetDate)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected Obj, but got Unknown
		NetworkBuff[] array = new NetworkBuff[60];
		for (byte b = 0; b < 30; b++)
		{
			array[b] = new NetworkBuff();
			array[b].BuffID = (*( effects1 + (int)b)).EffectID;
			if (array[b].BuffID != 0)
			{
				array[b].BuffExtra = (*( effects1 + (int)b)).OtherInfo;
				array[b].ActorID = (*( effects1 + (int)b)).ActorID;
				array[b].Duration = Math.Abs((*( effects1 + (int)b)).Duration);
				array[b].Timestamp = packetDate;
			}
		}
		for (byte b2 = 0; b2 < 30; b2++)
		{
			array[b2 + 30] = new NetworkBuff();
			array[b2 + 30].BuffID = (*( effects2 + (int)b2)).EffectID;
			if (array[b2 + 30].BuffID != 0)
			{
				array[b2 + 30].BuffExtra = (*( effects2 + (int)b2)).OtherInfo;
				array[b2 + 30].ActorID = (*( effects2 + (int)b2)).ActorID;
				array[b2 + 30].Duration = Math.Abs((*( effects2 + (int)b2)).Duration);
				array[b2 + 30].Timestamp = packetDate;
			}
		}
		return array;
	}
}
