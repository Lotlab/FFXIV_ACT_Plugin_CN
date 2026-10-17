using System;
using System.Runtime.CompilerServices;
using FFXIV_ACT_Plugin.Common.Models;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory;
using FFXIV_ACT_Plugin.Resource;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class ActorCast : IPacketHandler
{
	private readonly ILogOutput _logOutput;

	private readonly ILogFormat _logFormat;

	private readonly IActionList _skillList;

	private readonly ICombatantManager _combatantManager;

	private readonly DataEvent _dataEvent;

	public uint Opcode => (uint)(Server_MessageType.ActorCast);

	public Type PacketType => typeof(Server_ActorCast);

	public unsafe int PacketLength => sizeof(Server_ActorCast);

	public ActorCast(ILogOutput logOutput, ILogFormat logFormat, DataEvent dataEvent, IActionList skillList, ICombatantManager combatantManager)
	{
		_logOutput = logOutput;
		_logFormat = logFormat;
		_skillList = skillList;
		_combatantManager = combatantManager;
		_dataEvent = dataEvent;
	}

	public unsafe void Process(byte[] buffer, DateTime packetDate)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		fixed (byte* ptr = buffer)
		{
			Server_ActorCast val = *(Server_ActorCast*)ptr;
			uint actorID = val.MessageHeader.ActorID;
			Combatant combatantById = _combatantManager.GetCombatantById(actorID);
			uint targetID = val.TargetID;
			Combatant combatantById2 = _combatantManager.GetCombatantById(targetID);
			string text = _logFormat.FormatNetworkCastMessage(actorID, (combatantById != null) ? combatantById.Name : null, targetID, (combatantById2 != null) ? combatantById2.Name : null, (uint)val.ActionID, _skillList.GetActionNameByID((uint)val.ActionID), val.CastTime, (combatantById != null) ? new float?(combatantById.PosX) : ((float?)null), (combatantById != null) ? new float?(combatantById.PosY) : ((float?)null), (combatantById != null) ? new float?(combatantById.PosZ) : ((float?)null), (combatantById != null) ? new float?(combatantById.Heading) : ((float?)null));
			_logOutput.WriteLine((LogMessageType)20, packetDate, text);
			_dataEvent.OnParsedLogLine((LogMessageType)20, text);
		}
	}
}
