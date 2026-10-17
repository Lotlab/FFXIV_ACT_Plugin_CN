using System;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class SystemLogMessage : IPacketHandler
{
	private readonly ILogOutput _logOutput;

	private readonly ILogFormat _logFormat;

	private readonly DataEvent _dataEvent;

	public uint Opcode => (ushort)Server_MessageType.SystemLogMessage;

	public Type PacketType => typeof(Server_SystemLogMessage);

	public unsafe int PacketLength => sizeof(Server_SystemLogMessage);

	public SystemLogMessage(ILogOutput logOutput, ILogFormat logFormat, DataEvent dataEvent)
	{
		_logOutput = logOutput;
		_logFormat = logFormat;
		_dataEvent = dataEvent;
	}

	public unsafe void Process(byte[] buffer, DateTime packetDate)
	{
		fixed (byte* ptr = buffer)
		{
			Server_SystemLogMessage server_SystemLogMessage = *(Server_SystemLogMessage*)ptr;
			string text = _logFormat.FormatUIntArray(server_SystemLogMessage.unknown1, server_SystemLogMessage.logmessageid, server_SystemLogMessage.param1, server_SystemLogMessage.param2, server_SystemLogMessage.param3);
			_logOutput.WriteLine(LogMessageType.SystemLogMessage, packetDate, text);
			_dataEvent.OnParsedLogLine(LogMessageType.SystemLogMessage, text);
		}
	}
}
