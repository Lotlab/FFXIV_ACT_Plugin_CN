using System;
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

	public uint Opcode => (ushort)Server_MessageType.ActorCast;

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
		fixed (byte* ptr = buffer)
		{
			Server_ActorCast server_ActorCast = *(Server_ActorCast*)ptr;
			uint actorID = server_ActorCast.MessageHeader.ActorID;
			Combatant combatantById = _combatantManager.GetCombatantById(actorID);
			uint targetID = server_ActorCast.TargetID;
			Combatant combatantById2 = _combatantManager.GetCombatantById(targetID);
			string text = _logFormat.FormatNetworkCastMessage(actorID, combatantById?.Name, targetID, combatantById2?.Name, server_ActorCast.ActionID, _skillList.GetActionNameByID(server_ActorCast.ActionID), server_ActorCast.CastTime, combatantById?.PosX, combatantById?.PosY, combatantById?.PosZ, combatantById?.Heading);
			_logOutput.WriteLine(LogMessageType.StartsCasting, packetDate, text);
			_dataEvent.OnParsedLogLine(LogMessageType.StartsCasting, text);
		}
	}
}
