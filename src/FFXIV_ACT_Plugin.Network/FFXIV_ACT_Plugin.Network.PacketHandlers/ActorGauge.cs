using System;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class ActorGauge : IPacketHandler
{
	private readonly ILogOutput _logOutput;

	private readonly ILogFormat _logFormat;

	private readonly DataEvent _dataEvent;

	public uint Opcode => (ushort)Server_MessageType.ActorGauge;

	public Type PacketType => typeof(Server_ActorGauge);

	public unsafe int PacketLength => sizeof(Server_ActorGauge);

	public ActorGauge(ILogOutput logOutput, ILogFormat logFormat, DataEvent dataEvent)
	{
		_logOutput = logOutput;
		_logFormat = logFormat;
		_dataEvent = dataEvent;
	}

	public unsafe void Process(byte[] buffer, DateTime packetDate)
	{
		fixed (byte* ptr = buffer)
		{
			Server_ActorGauge server_ActorGauge = *(Server_ActorGauge*)ptr;
			uint actorID = server_ActorGauge.MessageHeader.ActorID;
			string text = _logFormat.FormatUIntArray(actorID, server_ActorGauge.param1, server_ActorGauge.param2, server_ActorGauge.param3, server_ActorGauge.param4);
			_logOutput.WriteLine(LogMessageType.Gauge, packetDate, text);
			_dataEvent.OnParsedLogLine(LogMessageType.Gauge, text);
		}
	}
}
