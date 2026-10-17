using System;
using System.Runtime.CompilerServices;
using FFXIV_ACT_Plugin.Common.Models;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory;
using FFXIV_ACT_Plugin.Resource;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public class ActorControl : IPacketHandler
{
	private readonly ILogOutput _logOutput;

	private readonly ILogFormat _logFormat;

	private readonly IActionList _skillList;

	private readonly ICombatantManager _combatantManager;

	private readonly INetworkBuffManager _buffManager;

	private readonly DataEvent _dataEvent;

	public uint Opcode => (uint)(Server_MessageType.ActorControl);

	public Type PacketType => typeof(Server_ActorControl);

	public unsafe int PacketLength => sizeof(Server_ActorControl);

	public ActorControl(ILogOutput logOutput, ILogFormat logFormat, DataEvent dataEvent, IActionList skillList, ICombatantManager combatantManager, INetworkBuffManager buffManager)
	{
		_logOutput = logOutput;
		_logFormat = logFormat;
		_skillList = skillList;
		_combatantManager = combatantManager;
		_buffManager = buffManager;
		_dataEvent = dataEvent;
	}

	public unsafe void Process(byte[] buffer, DateTime packetDate)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Invalid comparison between Unknown and I4
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Invalid comparison between Unknown and I4
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Invalid comparison between Unknown and I4
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Invalid comparison between Unknown and I4
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Invalid comparison between Unknown and I4
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0650: Unknown result type (might be due to invalid IL or missing references)
		//IL_0657: Invalid comparison between Unknown and I4
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ab: Invalid comparison between Unknown and I4
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		//IL_072e: Invalid comparison between Unknown and I4
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0733: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_076f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0775: Unknown result type (might be due to invalid IL or missing references)
		//IL_077b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0781: Unknown result type (might be due to invalid IL or missing references)
		//IL_0787: Unknown result type (might be due to invalid IL or missing references)
		//IL_078d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		fixed (byte* ptr = buffer)
		{
			Server_ActorControl val = *(Server_ActorControl*)ptr;
			uint actorID = val.MessageHeader.ActorID;
			Combatant combatantById = _combatantManager.GetCombatantById(actorID);
			if ((int)val.category == 1541)
			{
				uint param = val.param3;
				Combatant val2 = null;
				if (param != 0 && param != 3758096384u)
				{
					val2 = _combatantManager.GetCombatantById(param);
				}
				string text = _logFormat.FormatNetworkDoTMessage(actorID, (combatantById != null) ? combatantById.Name : null, false, val.param1, val.param2, (combatantById != null) ? new uint?(combatantById.CurrentHP) : ((uint?)null), (combatantById != null) ? new uint?(combatantById.MaxHP) : ((uint?)null), (combatantById != null) ? new uint?(combatantById.CurrentMP) : ((uint?)null), (combatantById != null) ? new uint?(combatantById.MaxMP) : ((uint?)null), (combatantById != null) ? new float?(combatantById.PosX) : ((float?)null), (combatantById != null) ? new float?(combatantById.PosY) : ((float?)null), (combatantById != null) ? new float?(combatantById.PosZ) : ((float?)null), (combatantById != null) ? new float?(combatantById.Heading) : ((float?)null), (uint?)param, (val2 != null) ? val2.Name : null, val.param4, (val2 != null) ? new uint?(val2.CurrentHP) : ((uint?)null), (val2 != null) ? new uint?(val2.MaxHP) : ((uint?)null), (val2 != null) ? new uint?(val2.CurrentMP) : ((uint?)null), (val2 != null) ? new uint?(val2.MaxMP) : ((uint?)null), (val2 != null) ? new float?(val2.PosX) : ((float?)null), (val2 != null) ? new float?(val2.PosY) : ((float?)null), (val2 != null) ? new float?(val2.PosZ) : ((float?)null), (val2 != null) ? new float?(val2.Heading) : ((float?)null));
				_logOutput.WriteLine((LogMessageType)24, packetDate, text);
				_dataEvent.OnParsedLogLine((LogMessageType)24, text);
			}
			else if ((int)val.category == 1540)
			{
				uint param2 = val.param3;
				Combatant val3 = null;
				if (param2 != 0 && param2 != 3758096384u)
				{
					val3 = _combatantManager.GetCombatantById(param2);
				}
				string text2 = _logFormat.FormatNetworkDoTMessage(actorID, (combatantById != null) ? combatantById.Name : null, true, val.param1, val.param2, (combatantById != null) ? new uint?(combatantById.CurrentHP) : ((uint?)null), (combatantById != null) ? new uint?(combatantById.MaxHP) : ((uint?)null), (combatantById != null) ? new uint?(combatantById.CurrentMP) : ((uint?)null), (combatantById != null) ? new uint?(combatantById.MaxMP) : ((uint?)null), (combatantById != null) ? new float?(combatantById.PosX) : ((float?)null), (combatantById != null) ? new float?(combatantById.PosY) : ((float?)null), (combatantById != null) ? new float?(combatantById.PosZ) : ((float?)null), (combatantById != null) ? new float?(combatantById.Heading) : ((float?)null), (uint?)param2, (val3 != null) ? val3.Name : null, val.param4, (val3 != null) ? new uint?(val3.CurrentHP) : ((uint?)null), (val3 != null) ? new uint?(val3.MaxHP) : ((uint?)null), (val3 != null) ? new uint?(val3.CurrentMP) : ((uint?)null), (val3 != null) ? new uint?(val3.MaxMP) : ((uint?)null), (val3 != null) ? new float?(val3.PosX) : ((float?)null), (val3 != null) ? new float?(val3.PosY) : ((float?)null), (val3 != null) ? new float?(val3.PosZ) : ((float?)null), (val3 != null) ? new float?(val3.Heading) : ((float?)null));
				_logOutput.WriteLine((LogMessageType)24, packetDate, text2);
				_dataEvent.OnParsedLogLine((LogMessageType)24, text2);
			}
			else if ((int)val.category == 15)
			{
				string text3 = _logFormat.FormatNetworkCancelMessage(actorID, (combatantById != null) ? combatantById.Name : null, val.param3, _skillList.GetActionNameByID(val.param3), val.param4 == 0, val.param4 == 1);
				_logOutput.WriteLine((LogMessageType)23, packetDate, text3);
				_dataEvent.OnParsedLogLine((LogMessageType)23, text3);
			}
			else if ((int)val.category == 6)
			{
				uint param3 = val.param1;
				Combatant combatantById2 = _combatantManager.GetCombatantById(param3);
				string text4 = _logFormat.FormatNetworkDeathMessage(actorID, (combatantById != null) ? combatantById.Name : null, param3, (combatantById2 != null) ? combatantById2.Name : null);
				_logOutput.WriteLine((LogMessageType)25, packetDate, text4);
				_dataEvent.OnParsedLogLine((LogMessageType)25, text4);
			}
			else if ((int)val.category == 34)
			{
				string text5 = _logFormat.FormatNetworkTargetIconMessage(actorID, (combatantById != null) ? combatantById.Name : null, val.MessageHeader.Unknown4, (uint)val.padding, val.param1, val.param2, val.param3, val.param4);
				_logOutput.WriteLine((LogMessageType)27, packetDate, text5);
				_dataEvent.OnParsedLogLine((LogMessageType)27, text5);
			}
			else if ((int)val.category == 22)
			{
				ushort buffId = (ushort)(val.param2 & 0xFFFF);
				ushort buffExtra = (ushort)(val.param3 & 0xFFFF);
				byte index = (byte)val.param1;
				_buffManager.AddUpdateBuffForTarget(3758096384u, actorID, buffId, buffExtra, 0f, packetDate, index, missingInfo: true);
			}
			else if ((int)val.category == 54)
			{
				uint actorID2 = val.MessageHeader.ActorID;
				Combatant combatantById3 = _combatantManager.GetCombatantById(actorID2);
				string text6 = _logFormat.FormatNetworkTargettableMessage(actorID, (combatantById != null) ? combatantById.Name : null, actorID2, (combatantById3 != null) ? combatantById3.Name : null, (byte)(val.param1 & 0xFF));
				_logOutput.WriteLine((LogMessageType)34, packetDate, text6);
				_dataEvent.OnParsedLogLine((LogMessageType)34, text6);
			}
			else if ((int)val.category == 35)
			{
				uint param4 = val.param3;
				Combatant combatantById4 = _combatantManager.GetCombatantById(param4);
				string text7 = _logFormat.FormatNetworkTetherMessage(actorID, (combatantById != null) ? combatantById.Name : null, param4, (combatantById4 != null) ? combatantById4.Name : null, (uint)val.padding, val.param1, val.param2, val.param3, val.param4, val.padding1);
				_logOutput.WriteLine((LogMessageType)35, packetDate, text7);
				_dataEvent.OnParsedLogLine((LogMessageType)35, text7);
			}
		}
	}
}
