using System;
using System.Runtime.CompilerServices;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class ActorControlSelf : IPacketHandler
{
	private readonly ILogOutput _logOutput;

	private readonly ILogFormat _logFormat;

	private readonly DataEvent _dataEvent;

	public uint Opcode => (uint)(Server_MessageType.ActorControlSelf);

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
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Invalid comparison between Unknown and I4
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Invalid comparison between Unknown and I4
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		fixed (byte* ptr = buffer)
		{
			Server_ActorControlSelf val = *(Server_ActorControlSelf*)ptr;
			if ((int)val.category == 109)
			{
				if ((val.param2 & 0xFFFF) != 0 && (val.param2 & 0xFF000000u) != 0)
				{
					string text = _logFormat.FormatUIntArray(new uint[6] { val.param1, val.param2, val.param3, val.param4, val.param5, val.param6 });
					_logOutput.WriteLine((LogMessageType)33, packetDate, text);
					_dataEvent.OnParsedLogLine((LogMessageType)33, text);
				}
			}
			else if ((int)val.category == 505)
			{
				string text2 = _logFormat.FormatNetworkLimitBreakfMessage((byte)(val.param1 & 0xFF), val.param2 & 0xFFFF);
				_logOutput.WriteLine((LogMessageType)36, packetDate, text2);
				_dataEvent.OnParsedLogLine((LogMessageType)36, text2);
			}
		}
	}
}
