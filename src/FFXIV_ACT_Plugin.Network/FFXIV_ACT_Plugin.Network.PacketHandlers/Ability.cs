using System;
using FFXIV_ACT_Plugin.Common.Models;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory;
using FFXIV_ACT_Plugin.Resource;
using Machina.FFXIV.Headers;

namespace FFXIV_ACT_Plugin.Network.PacketHandlers;

public abstract class Ability
{
	private readonly ILogOutput _logOutput;

	private readonly ILogFormat _logFormat;

	private readonly IActionList _skillList;

	private readonly ICombatantManager _combatantManager;

	private readonly INetworkBuffManager _buffManager;

	private readonly DataEvent _dataEvent;

	public Ability(ILogOutput logOutput, ILogFormat logFormat, DataEvent dataEvent, IActionList skillList, ICombatantManager combatantManager, INetworkBuffManager buffManager)
	{
		_logOutput = logOutput;
		_logFormat = logFormat;
		_skillList = skillList;
		_combatantManager = combatantManager;
		_buffManager = buffManager;
		_dataEvent = dataEvent;
	}

	public unsafe void ProcessAbility(DateTime packetDate, Server_ActionEffectHeader packetMessage, ulong* targetIDPtr, byte maxTargetCount, uint* effectPtr)
	{
		uint actorID = packetMessage.MessageHeader.ActorID;
		Combatant combatantById = _combatantManager.GetCombatantById(actorID);
		uint num;
		if (packetMessage.effectDisplayType == Server_ActionEffectDisplayType.MountName)
		{
			num = 218103808 + packetMessage.actionId;
		}
		else
		{
			num = ((packetMessage.effectDisplayType != Server_ActionEffectDisplayType.ShowItemName) ? packetMessage.actionAnimationId : (33554432 + packetMessage.actionId));
		}
		byte b = packetMessage.effectCount;
		if (b > maxTargetCount)
		{
			b = maxTargetCount;
		}
		Combatant combatant = null;
		if ((combatantById?.OwnerID ?? 0) != 0)
		{
			combatant = _combatantManager.GetCombatantById(combatantById.OwnerID);
		}
		if (b == 0)
		{
			string text = _logFormat.FormatNetworkAbilityMessage(actorID, combatantById?.Name, num, _skillList.GetActionNameByID(num), 3758096384u, "", combatantById?.CurrentHP, combatantById?.MaxHP, combatantById?.CurrentMP, combatantById?.MaxMP, combatantById?.PosX, combatantById?.PosY, combatantById?.PosZ, combatantById?.Heading, null, null, null, null, null, null, null, null, 0uL, 0uL, 0uL, 0uL, 0uL, 0uL, 0uL, 0uL, packetMessage.globalEffectCounter, 0, 0, combatant?.ID ?? 0, combatant?.Name, (byte)packetMessage.effectDisplayType, packetMessage.actionId, packetMessage.actionAnimationId, packetMessage.animationLockTime, packetMessage.rotation);
			_logOutput.WriteLine(LogMessageType.ActionEffect, packetDate, text);
			_dataEvent.OnParsedLogLine(LogMessageType.ActionEffect, text);
			return;
		}
		LogMessageType messageType = ((maxTargetCount == 1) ? LogMessageType.ActionEffect : LogMessageType.AOEActionEffect);
		byte b2 = 0;
		for (byte b3 = 0; b3 < b; b3++)
		{
			if ((int)(targetIDPtr[(int)b3] & 0xFFFFFFFFu) != 0)
			{
				b2++;
			}
		}
		for (byte b4 = 0; b4 < b; b4++)
		{
			uint num2 = (uint)(targetIDPtr[(int)b4] & 0xFFFFFFFFu);
			if (num2 != 0)
			{
				Combatant combatantById2 = _combatantManager.GetCombatantById(num2);
				string text2 = _logFormat.FormatNetworkAbilityMessage(actorID, combatantById?.Name, num, _skillList.GetActionNameByID(num), num2, combatantById2?.Name, combatantById?.CurrentHP, combatantById?.MaxHP, combatantById?.CurrentMP, combatantById?.MaxMP, combatantById?.PosX, combatantById?.PosY, combatantById?.PosZ, combatantById?.Heading, combatantById2?.CurrentHP, combatantById2?.MaxHP, combatantById2?.CurrentMP, combatantById2?.MaxMP, combatantById2?.PosX, combatantById2?.PosY, combatantById2?.PosZ, combatantById2?.Heading, ((ulong*)effectPtr)[b4 * 8], ((ulong*)effectPtr)[b4 * 8 + 1], ((ulong*)effectPtr)[b4 * 8 + 2], ((ulong*)effectPtr)[b4 * 8 + 3], ((ulong*)effectPtr)[b4 * 8 + 4], ((ulong*)effectPtr)[b4 * 8 + 5], ((ulong*)effectPtr)[b4 * 8 + 6], ((ulong*)effectPtr)[b4 * 8 + 7], packetMessage.globalEffectCounter, b4, b2, combatant?.ID ?? 0, combatant?.Name, (byte)packetMessage.effectDisplayType, packetMessage.actionId, packetMessage.actionAnimationId, packetMessage.animationLockTime, packetMessage.rotation);
				_logOutput.WriteLine(messageType, packetDate, text2);
				_dataEvent.OnParsedLogLine(messageType, text2);
				if (num == ActionId.IronJaws)
				{
					for (int i = 0; i < combatantById2.NetworkBuffs.Length; i++)
					{
						if (combatantById2.NetworkBuffs[i].BuffID == StatusId.StormBite || combatantById2.NetworkBuffs[i].BuffID == StatusId.CausticBite)
						{
							combatantById2.NetworkBuffs[i].RefreshPending = true;
						}
					}
				}
			}
		}
	}
}
