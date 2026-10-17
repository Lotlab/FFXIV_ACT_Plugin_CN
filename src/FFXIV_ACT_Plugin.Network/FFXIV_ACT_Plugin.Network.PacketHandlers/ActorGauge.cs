using System;
using System.Runtime.CompilerServices;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class ActorGauge : IPacketHandler
{
	private readonly ILogOutput _logOutput;

	private readonly ILogFormat _logFormat;

	private readonly DataEvent _dataEvent;

	public uint Opcode => (uint)(Server_MessageType.ActorGauge);

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
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		fixed (byte* ptr = buffer)
		{
			Server_ActorGauge val = *(Server_ActorGauge*)ptr;
			uint actorID = val.MessageHeader.ActorID;
			string text = _logFormat.FormatUIntArray(new uint[5] { actorID, val.param1, val.param2, val.param3, val.param4 });
			_logOutput.WriteLine((LogMessageType)31, packetDate, text);
			_dataEvent.OnParsedLogLine((LogMessageType)31, text);
		}
	}
}
