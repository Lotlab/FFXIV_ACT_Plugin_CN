using System;
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

	public uint Opcode => (ushort)Server_MessageType.UpdateHpMpTp;

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
		fixed (byte* ptr = buffer)
		{
			Server_UpdateHpMpTp server_UpdateHpMpTp = *(Server_UpdateHpMpTp*)ptr;
			uint actorID = server_UpdateHpMpTp.MessageHeader.ActorID;
			Combatant combatantById = _combatantManager.GetCombatantById(actorID);
			string text = _logFormat.FormatUpdateHpMpTp(actorID, combatantById?.Name, server_UpdateHpMpTp.CurrentHp, combatantById?.MaxHP, server_UpdateHpMpTp.CurrentMp, combatantById?.MaxMP, combatantById?.PosX, combatantById?.PosY, combatantById?.PosZ, combatantById?.Heading);
			_logOutput.WriteLine(LogMessageType.UpdateHp, packetDate, text);
			_dataEvent.OnParsedLogLine(LogMessageType.UpdateHp, text);
		}
	}
}
