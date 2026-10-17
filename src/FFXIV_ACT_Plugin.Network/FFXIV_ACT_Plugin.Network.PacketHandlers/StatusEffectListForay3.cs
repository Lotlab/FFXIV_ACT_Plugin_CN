using System;
using System.Runtime.CompilerServices;
using FFXIV_ACT_Plugin.Common.Models;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class StatusEffectListForay3 : IPacketHandler
{
	private readonly INetworkBuffManager _buffManager;

	private readonly ILogFormat _logFormat;

	private readonly ICombatantManager _combatantManager;

	private readonly ILogOutput _logOutput;

	private readonly DataEvent _dataEvent;

	public uint Opcode => (uint)(Server_MessageType.StatusEffectListForay3);

	public Type PacketType => typeof(Server_StatusEffectListForay3);

	public unsafe int PacketLength => sizeof(Server_StatusEffectListForay3);

	public StatusEffectListForay3(INetworkBuffManager buffManager, ICombatantManager combatantManager, ILogFormat logFormat, ILogOutput logOutput, DataEvent dataEvent)
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
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Expected Obj, but got Unknown
		fixed (byte* ptr = buffer)
		{
			Server_StatusEffectListForay3 val = *(Server_StatusEffectListForay3*)ptr;
			uint actorID = val.MessageHeader.ActorID;
			Server_StatusEffectListEntry* ptr2 = (Server_StatusEffectListEntry*)(val.Effects);
			uint[] array = new uint[90];
			for (byte b = 0; b < 30; b++)
			{
				if ((*(ptr2 + (int)b)).EffectID != 0)
				{
					array[b * 3] = (uint)((*(ptr2 + (int)b)).EffectID + ((*(ptr2 + (int)b)).OtherInfo << 16));
					array[b * 3 + 1] = BitConverter.ToUInt32(BitConverter.GetBytes((*(ptr2 + (int)b)).Duration), 0);
					array[b * 3 + 2] = (*(ptr2 + (int)b)).ActorID;
				}
			}
			Combatant combatantById = _combatantManager.GetCombatantById(val.MessageHeader.ActorID);
			uint num = (uint)(val.JobID + (val.Level1 << 8) + (val.Level2 << 16) + (val.Level3 << 24));
			string text = _logFormat.FormatStatusListMessage(val.MessageHeader.ActorID, (combatantById != null) ? combatantById.Name : null, num, val.CurrentHP, val.MaxHP, (uint)val.CurrentMP, (uint)val.MaxMP, val.DamageShield, (combatantById != null) ? new float?(combatantById.PosX) : ((float?)null), (combatantById != null) ? new float?(combatantById.PosY) : ((float?)null), (combatantById != null) ? new float?(combatantById.PosZ) : ((float?)null), (combatantById != null) ? new float?(combatantById.Heading) : ((float?)null), array);
			_logOutput.WriteLine((LogMessageType)43, packetDate, text);
			_dataEvent.OnParsedLogLine((LogMessageType)43, text);
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
