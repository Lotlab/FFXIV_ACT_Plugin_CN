using System;
using System.Runtime.CompilerServices;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class SystemLogMessage : IPacketHandler
{
	private readonly ILogOutput _logOutput;

	private readonly ILogFormat _logFormat;

	private readonly DataEvent _dataEvent;

	public uint Opcode => (uint)(Server_MessageType.SystemLogMessage);

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
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		fixed (byte* ptr = buffer)
		{
			Server_SystemLogMessage val = *(Server_SystemLogMessage*)ptr;
			string text = _logFormat.FormatUIntArray(new uint[5] { val.unknown1, val.logmessageid, val.param1, val.param2, val.param3 });
			_logOutput.WriteLine((LogMessageType)41, packetDate, text);
			_dataEvent.OnParsedLogLine((LogMessageType)41, text);
		}
	}
}
