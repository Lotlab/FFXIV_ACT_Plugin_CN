using System;
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

	public uint Opcode => (ushort)Server_MessageType.EffectResultBasic;

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
		fixed (byte* ptr = buffer)
		{
			Server_EffectResultBasic server_EffectResultBasic = *(Server_EffectResultBasic*)ptr;
			uint actorID = server_EffectResultBasic.MessageHeader.ActorID;
			uint[] data = Array.Empty<uint>();
			Combatant combatantById = _combatantManager.GetCombatantById(actorID);
			string text = _logFormat.FormatEffectResultMessage(actorID, combatantById?.Name, server_EffectResultBasic.RelatedActionSequence, server_EffectResultBasic.CurrentHP, null, null, null, null, combatantById?.PosX, combatantById?.PosY, combatantById?.PosZ, combatantById?.Heading, data);
			_logOutput.WriteLine(LogMessageType.EffectResult, packetDate, text);
			_dataEvent.OnParsedLogLine(LogMessageType.EffectResult, text);
		}
	}
}
