using System;
using System.Runtime.CompilerServices;
using FFXIV_ACT_Plugin.Common.Models;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class EffectResultBasic : IPacketHandler
{
	private readonly INetworkBuffManager _buffManager;

	private readonly ILogFormat _logFormat;

	private readonly ICombatantManager _combatantManager;

	private readonly ILogOutput _logOutput;

	private readonly DataEvent _dataEvent;

	public uint Opcode => (uint)(Server_MessageType.EffectResultBasic);

	public Type PacketType => typeof(Server_EffectResultBasic);

	public unsafe int PacketLength => sizeof(Server_EffectResultBasic);

	public EffectResultBasic(INetworkBuffManager buffManager, ICombatantManager combatantManager, ILogFormat logFormat, ILogOutput logOutput, DataEvent dataEvent)
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
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		fixed (byte* ptr = buffer)
		{
			Server_EffectResultBasic val = *(Server_EffectResultBasic*)ptr;
			uint actorID = val.MessageHeader.ActorID;
			uint[] array = Array.Empty<uint>();
			Combatant combatantById = _combatantManager.GetCombatantById(actorID);
			string text = _logFormat.FormatEffectResultMessage(actorID, (combatantById != null) ? combatantById.Name : null, val.RelatedActionSequence, val.CurrentHP, (uint?)null, (uint?)null, (uint?)null, (byte?)null, (combatantById != null) ? new float?(combatantById.PosX) : ((float?)null), (combatantById != null) ? new float?(combatantById.PosY) : ((float?)null), (combatantById != null) ? new float?(combatantById.PosZ) : ((float?)null), (combatantById != null) ? new float?(combatantById.Heading) : ((float?)null), array);
			_logOutput.WriteLine((LogMessageType)37, packetDate, text);
			_dataEvent.OnParsedLogLine((LogMessageType)37, text);
		}
	}
}
