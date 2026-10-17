using System;
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

	public uint Opcode => (ushort)Server_MessageType.ActorControl;

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
		fixed (byte* ptr = buffer)
		{
			Server_ActorControl server_ActorControl = *(Server_ActorControl*)ptr;
			uint actorID = server_ActorControl.MessageHeader.ActorID;
			Combatant combatantById = _combatantManager.GetCombatantById(actorID);
			if (server_ActorControl.category == Server_ActorControlCategory.DoT)
			{
				uint param = server_ActorControl.param3;
				Combatant combatant = null;
				if (param != 0 && param != 3758096384u)
				{
					combatant = _combatantManager.GetCombatantById(param);
				}
				string text = _logFormat.FormatNetworkDoTMessage(actorID, combatantById?.Name, isHeal: false, server_ActorControl.param1, server_ActorControl.param2, combatantById?.CurrentHP, combatantById?.MaxHP, combatantById?.CurrentMP, combatantById?.MaxMP, combatantById?.PosX, combatantById?.PosY, combatantById?.PosZ, combatantById?.Heading, param, combatant?.Name, server_ActorControl.param4, combatant?.CurrentHP, combatant?.MaxHP, combatant?.CurrentMP, combatant?.MaxMP, combatant?.PosX, combatant?.PosY, combatant?.PosZ, combatant?.Heading);
				_logOutput.WriteLine(LogMessageType.DoTHoT, packetDate, text);
				_dataEvent.OnParsedLogLine(LogMessageType.DoTHoT, text);
			}
			else if (server_ActorControl.category == Server_ActorControlCategory.HoT)
			{
				uint param2 = server_ActorControl.param3;
				Combatant combatant2 = null;
				if (param2 != 0 && param2 != 3758096384u)
				{
					combatant2 = _combatantManager.GetCombatantById(param2);
				}
				string text2 = _logFormat.FormatNetworkDoTMessage(actorID, combatantById?.Name, isHeal: true, server_ActorControl.param1, server_ActorControl.param2, combatantById?.CurrentHP, combatantById?.MaxHP, combatantById?.CurrentMP, combatantById?.MaxMP, combatantById?.PosX, combatantById?.PosY, combatantById?.PosZ, combatantById?.Heading, param2, combatant2?.Name, server_ActorControl.param4, combatant2?.CurrentHP, combatant2?.MaxHP, combatant2?.CurrentMP, combatant2?.MaxMP, combatant2?.PosX, combatant2?.PosY, combatant2?.PosZ, combatant2?.Heading);
				_logOutput.WriteLine(LogMessageType.DoTHoT, packetDate, text2);
				_dataEvent.OnParsedLogLine(LogMessageType.DoTHoT, text2);
			}
			else if (server_ActorControl.category == Server_ActorControlCategory.CancelAbility)
			{
				string text3 = _logFormat.FormatNetworkCancelMessage(actorID, combatantById?.Name, server_ActorControl.param3, _skillList.GetActionNameByID(server_ActorControl.param3), server_ActorControl.param4 == 0, server_ActorControl.param4 == 1);
				_logOutput.WriteLine(LogMessageType.CancelAction, packetDate, text3);
				_dataEvent.OnParsedLogLine(LogMessageType.CancelAction, text3);
			}
			else if (server_ActorControl.category == Server_ActorControlCategory.Death)
			{
				uint param3 = server_ActorControl.param1;
				Combatant combatantById2 = _combatantManager.GetCombatantById(param3);
				string text4 = _logFormat.FormatNetworkDeathMessage(actorID, combatantById?.Name, param3, combatantById2?.Name);
				_logOutput.WriteLine(LogMessageType.Death, packetDate, text4);
				_dataEvent.OnParsedLogLine(LogMessageType.Death, text4);
			}
			else if (server_ActorControl.category == Server_ActorControlCategory.TargetIcon)
			{
				string text5 = _logFormat.FormatNetworkTargetIconMessage(actorID, combatantById?.Name, server_ActorControl.MessageHeader.Unknown4, server_ActorControl.padding, server_ActorControl.param1, server_ActorControl.param2, server_ActorControl.param3, server_ActorControl.param4);
				_logOutput.WriteLine(LogMessageType.TargetIcon, packetDate, text5);
				_dataEvent.OnParsedLogLine(LogMessageType.TargetIcon, text5);
			}
			else if (server_ActorControl.category == Server_ActorControlCategory.UpdateEffect)
			{
				ushort buffId = (ushort)(server_ActorControl.param2 & 0xFFFF);
				ushort buffExtra = (ushort)(server_ActorControl.param3 & 0xFFFF);
				byte index = (byte)server_ActorControl.param1;
				_buffManager.AddUpdateBuffForTarget(3758096384u, actorID, buffId, buffExtra, 0f, packetDate, index, missingInfo: true);
			}
			else if (server_ActorControl.category == Server_ActorControlCategory.Targetable)
			{
				uint actorID2 = server_ActorControl.MessageHeader.ActorID;
				Combatant combatantById3 = _combatantManager.GetCombatantById(actorID2);
				string text6 = _logFormat.FormatNetworkTargettableMessage(actorID, combatantById?.Name, actorID2, combatantById3?.Name, (byte)(server_ActorControl.param1 & 0xFF));
				_logOutput.WriteLine(LogMessageType.NameToggle, packetDate, text6);
				_dataEvent.OnParsedLogLine(LogMessageType.NameToggle, text6);
			}
			else if (server_ActorControl.category == Server_ActorControlCategory.Tether)
			{
				uint param4 = server_ActorControl.param3;
				Combatant combatantById4 = _combatantManager.GetCombatantById(param4);
				string text7 = _logFormat.FormatNetworkTetherMessage(actorID, combatantById?.Name, param4, combatantById4?.Name, server_ActorControl.padding, server_ActorControl.param1, server_ActorControl.param2, server_ActorControl.param3, server_ActorControl.param4, server_ActorControl.padding1);
				_logOutput.WriteLine(LogMessageType.Tether, packetDate, text7);
				_dataEvent.OnParsedLogLine(LogMessageType.Tether, text7);
			}
		}
	}
}
