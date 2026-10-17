using System;
using System.Runtime.CompilerServices;
using FFXIV_ACT_Plugin.Common.Models;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class StatusEffectList : IPacketHandler
{
	private readonly INetworkBuffManager _buffManager;

	private readonly ILogFormat _logFormat;

	private readonly ICombatantManager _combatantManager;

	private readonly ILogOutput _logOutput;

	private readonly DataEvent _dataEvent;

	public uint Opcode => (uint)(Server_MessageType.StatusEffectList);

	public Type PacketType => typeof(Server_StatusEffectList);

	public unsafe int PacketLength => sizeof(Server_StatusEffectList);

	public StatusEffectList(INetworkBuffManager buffManager, ICombatantManager combatantManager, ILogFormat logFormat, ILogOutput logOutput, DataEvent dataEvent)
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
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Expected Obj, but got Unknown
		fixed (byte* ptr = buffer)
		{
			Server_StatusEffectList val = *(Server_StatusEffectList*)ptr;
			uint actorID = val.MessageHeader.ActorID;
			Server_StatusEffectListEntry* ptr2 = (Server_StatusEffectListEntry*)(val.Effects);
			uint[] array = new uint[93];
			array[0] = val.Unknown1;
			array[1] = val.Unknown2;
			for (byte b = 0; b < 30; b++)
			{
				if ((*(ptr2 + (int)b)).EffectID != 0)
				{
					array[3 + b * 3] = (uint)((*(ptr2 + (int)b)).EffectID + ((*(ptr2 + (int)b)).OtherInfo << 16));
					array[3 + b * 3 + 1] = BitConverter.ToUInt32(BitConverter.GetBytes((*(ptr2 + (int)b)).Duration), 0);
					array[3 + b * 3 + 2] = (*(ptr2 + (int)b)).ActorID;
				}
			}
			Combatant combatantById = _combatantManager.GetCombatantById(val.MessageHeader.ActorID);
			uint num = (uint)(val.JobID + (val.Level1 << 8) + (val.Level2 << 16) + (val.Level3 << 24));
			string text = _logFormat.FormatStatusListMessage(val.MessageHeader.ActorID, (combatantById != null) ? combatantById.Name : null, num, val.CurrentHP, val.MaxHP, (uint)val.CurrentMP, (uint)val.MaxMP, val.DamageShield, (combatantById != null) ? new float?(combatantById.PosX) : ((float?)null), (combatantById != null) ? new float?(combatantById.PosY) : ((float?)null), (combatantById != null) ? new float?(combatantById.PosZ) : ((float?)null), (combatantById != null) ? new float?(combatantById.Heading) : ((float?)null), array);
			_logOutput.WriteLine((LogMessageType)38, packetDate, text);
			_dataEvent.OnParsedLogLine((LogMessageType)38, text);
			NetworkBuff[] array2 = new NetworkBuff[30];
			for (byte b2 = 0; b2 < 30; b2++)
			{
				array2[b2] = new NetworkBuff();
				array2[b2].BuffID = (*(ptr2 + (int)b2)).EffectID;
				if (array2[b2].BuffID != 0)
				{
					array2[b2].BuffExtra = (*(ptr2 + (int)b2)).OtherInfo;
					array2[b2].ActorID = (*(ptr2 + (int)b2)).ActorID;
					array2[b2].Duration = Math.Abs((*(ptr2 + (int)b2)).Duration);
					array2[b2].Timestamp = packetDate;
				}
			}
			_buffManager.ReplaceAllBuffsForTarget(actorID, array2, packetDate);
		}
	}
}
