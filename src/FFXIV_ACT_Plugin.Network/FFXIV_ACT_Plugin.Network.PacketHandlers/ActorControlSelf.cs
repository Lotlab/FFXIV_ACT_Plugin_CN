using System;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class ActorControlSelf : IPacketHandler
{
	private readonly ILogOutput _logOutput;

	private readonly ILogFormat _logFormat;

	private readonly DataEvent _dataEvent;

	public uint Opcode => (ushort)Server_MessageType.ActorControlSelf;

	public Type PacketType => typeof(Server_ActorControlSelf);

	public unsafe int PacketLength => sizeof(Server_ActorControlSelf);

	public ActorControlSelf(ILogOutput logOutput, ILogFormat logFormat, DataEvent dataEvent)
	{
		_logOutput = logOutput;
		_logFormat = logFormat;
		_dataEvent = dataEvent;
	}

	public unsafe void Process(byte[] buffer, DateTime packetDate)
	{
		fixed (byte* ptr = buffer)
		{
			Server_ActorControlSelf server_ActorControlSelf = *(Server_ActorControlSelf*)ptr;
			if (server_ActorControlSelf.category == Server_ActorControlCategory.DirectorUpdate)
			{
				if ((server_ActorControlSelf.param2 & 0xFFFF) != 0 && (server_ActorControlSelf.param2 & 0xFF000000u) != 0)
				{
					string text = _logFormat.FormatUIntArray(server_ActorControlSelf.param1, server_ActorControlSelf.param2, server_ActorControlSelf.param3, server_ActorControlSelf.param4, server_ActorControlSelf.param5, server_ActorControlSelf.param6);
					_logOutput.WriteLine(LogMessageType.Director, packetDate, text);
					_dataEvent.OnParsedLogLine(LogMessageType.Director, text);
				}
			}
			else if (server_ActorControlSelf.category == Server_ActorControlCategory.LimitBreak)
			{
				string text2 = _logFormat.FormatNetworkLimitBreakfMessage((byte)(server_ActorControlSelf.param1 & 0xFF), server_ActorControlSelf.param2 & 0xFFFF);
				_logOutput.WriteLine(LogMessageType.LimitBreak, packetDate, text2);
				_dataEvent.OnParsedLogLine(LogMessageType.LimitBreak, text2);
			}
		}
	}
}
