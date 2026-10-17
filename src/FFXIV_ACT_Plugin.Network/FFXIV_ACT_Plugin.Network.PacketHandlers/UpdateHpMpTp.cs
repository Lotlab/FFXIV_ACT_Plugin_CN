using System;
using System.Runtime.CompilerServices;
using FFXIV_ACT_Plugin.Common.Models;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class UpdateHpMpTp : IPacketHandler
{
	private readonly ILogOutput _logOutput;

	private readonly ILogFormat _logFormat;

	private readonly ICombatantManager _combatantManager;

	private readonly DataEvent _dataEvent;

	public uint Opcode => (uint)(Server_MessageType.UpdateHpMpTp);

	public Type PacketType => typeof(Server_UpdateHpMpTp);

	public unsafe int PacketLength => sizeof(Server_UpdateHpMpTp);

	public UpdateHpMpTp(ILogOutput logOutput, ILogFormat logFormat, DataEvent dataEvent, ICombatantManager combatantManager)
	{
		_logOutput = logOutput;
		_logFormat = logFormat;
		_combatantManager = combatantManager;
		_dataEvent = dataEvent;
	}

	public unsafe void Process(byte[] buffer, DateTime packetDate)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		fixed (byte* ptr = buffer)
		{
			Server_UpdateHpMpTp val = *(Server_UpdateHpMpTp*)ptr;
			uint actorID = val.MessageHeader.ActorID;
			Combatant combatantById = _combatantManager.GetCombatantById(actorID);
			string text = _logFormat.FormatUpdateHpMpTp(actorID, (combatantById != null) ? combatantById.Name : null, val.CurrentHp, (combatantById != null) ? new uint?(combatantById.MaxHP) : ((uint?)null), (uint)val.CurrentMp, (combatantById != null) ? new uint?(combatantById.MaxMP) : ((uint?)null), (combatantById != null) ? new float?(combatantById.PosX) : ((float?)null), (combatantById != null) ? new float?(combatantById.PosY) : ((float?)null), (combatantById != null) ? new float?(combatantById.PosZ) : ((float?)null), (combatantById != null) ? new float?(combatantById.Heading) : ((float?)null));
			_logOutput.WriteLine((LogMessageType)39, packetDate, text);
			_dataEvent.OnParsedLogLine((LogMessageType)39, text);
		}
	}
}
