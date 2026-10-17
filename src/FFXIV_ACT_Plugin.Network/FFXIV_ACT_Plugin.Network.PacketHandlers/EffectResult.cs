using System;
using System.Runtime.CompilerServices;
using FFXIV_ACT_Plugin.Common.Models;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class EffectResult : IPacketHandler
{
	private readonly INetworkBuffManager _buffManager;

	private readonly ILogFormat _logFormat;

	private readonly ICombatantManager _combatantManager;

	private readonly ILogOutput _logOutput;

	private readonly DataEvent _dataEvent;

	public uint Opcode => (uint)(Server_MessageType.EffectResult);

	public Type PacketType => typeof(Server_EffectResult);

	public unsafe int PacketLength => sizeof(Server_EffectResult);

	public EffectResult(INetworkBuffManager buffManager, ICombatantManager combatantManager, ILogFormat logFormat, ILogOutput logOutput, DataEvent dataEvent)
	{
		_combatantManager = combatantManager;
		_buffManager = buffManager;
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
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		fixed (byte* ptr = buffer)
		{
			Server_EffectResult val = *(Server_EffectResult*)ptr;
			uint actorID = val.MessageHeader.ActorID;
			byte b = val.EffectCount;
			if (b > 4)
			{
				b = 4;
			}
			Server_EffectResultEntry* ptr2 = (Server_EffectResultEntry*)(val.Effects);
			uint[] array = new uint[20]
			{
				val.Unknown3, 0u, val.Unknown6, val.EffectCount, 0u, 0u, 0u, 0u, 0u, 0u,
				0u, 0u, 0u, 0u, 0u, 0u, 0u, 0u, 0u, 0u
			};
			for (int i = 0; i < b; i++)
			{
				array[4 + i * 4] = (uint)((*( ptr2 + i)).EffectID + ((*( ptr2 + i)).unknown1 << 16) + ((*( ptr2 + i)).EffectIndex << 24));
				array[4 + i * 4 + 1] = (uint)((*( ptr2 + i)).unknown2 + ((*( ptr2 + i)).unknown3 << 16));
				array[4 + i * 4 + 2] = BitConverter.ToUInt32(BitConverter.GetBytes((*( ptr2 + i)).duration), 0);
				array[4 + i * 4 + 3] = (*( ptr2 + i)).SourceActorID;
			}
			Combatant combatantById = _combatantManager.GetCombatantById(actorID);
			string text = _logFormat.FormatEffectResultMessage(actorID, (combatantById != null) ? combatantById.Name : null, val.RelatedActionSequence, val.CurrentHP, (uint?)val.MaxHP, (uint?)val.CurrentMP, (uint?)10000u, (byte?)val.DamageShield, (combatantById != null) ? new float?(combatantById.PosX) : ((float?)null), (combatantById != null) ? new float?(combatantById.PosY) : ((float?)null), (combatantById != null) ? new float?(combatantById.PosZ) : ((float?)null), (combatantById != null) ? new float?(combatantById.Heading) : ((float?)null), array);
			_logOutput.WriteLine((LogMessageType)37, packetDate, text);
			_dataEvent.OnParsedLogLine((LogMessageType)37, text);
			for (int j = 0; j < b; j++)
			{
				ushort effectID = (*( ptr2 + j)).EffectID;
				if (effectID > 0)
				{
					ushort unknown = (*( ptr2 + j)).unknown2;
					uint sourceActorID = (*( ptr2 + j)).SourceActorID;
					ushort effectIndex = (*( ptr2 + j)).EffectIndex;
					if (effectIndex >= (combatantById?.NetworkBuffs?.Length ?? 30))
					{
						text = $"ProcessEffectResult: invalid status index {(*( ptr2 + j)).EffectIndex} for effect index {j}.";
						_logOutput.WriteLine((LogMessageType)254, packetDate, text);
					}
					else
					{
						float duration = Math.Abs((*( ptr2 + j)).duration);
						_buffManager.AddUpdateBuffForTarget(sourceActorID, actorID, effectID, unknown, duration, packetDate, (byte)effectIndex, missingInfo: false);
					}
				}
			}
		}
	}
}
