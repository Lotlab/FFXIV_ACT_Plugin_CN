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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Invalid comparison between Unknown and I4
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Expected I4, but got Unknown
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Expected I4, but got Unknown
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		uint actorID = packetMessage.MessageHeader.ActorID;
		Combatant combatantById = _combatantManager.GetCombatantById(actorID);
		uint num;
		if ((int)packetMessage.effectDisplayType == 13)
		{
			num = 218103808 + packetMessage.actionId;
		}
		else
		{
			num = (((int)packetMessage.effectDisplayType != 2) ? packetMessage.actionAnimationId : (33554432 + packetMessage.actionId));
		}
		byte b = packetMessage.effectCount;
		if (b > maxTargetCount)
		{
			b = maxTargetCount;
		}
		Combatant val = null;
		if (combatantById != null && combatantById.OwnerID != 0)
		{
			val = _combatantManager.GetCombatantById(combatantById.OwnerID);
		}
		if (b == 0)
		{
			string text = _logFormat.FormatNetworkAbilityMessage(actorID, (combatantById != null) ? combatantById.Name : null, num, _skillList.GetActionNameByID(num), 3758096384u, "", (combatantById != null) ? new uint?(combatantById.CurrentHP) : ((uint?)null), (combatantById != null) ? new uint?(combatantById.MaxHP) : ((uint?)null), (combatantById != null) ? new uint?(combatantById.CurrentMP) : ((uint?)null), (combatantById != null) ? new uint?(combatantById.MaxMP) : ((uint?)null), (combatantById != null) ? new float?(combatantById.PosX) : ((float?)null), (combatantById != null) ? new float?(combatantById.PosY) : ((float?)null), (combatantById != null) ? new float?(combatantById.PosZ) : ((float?)null), (combatantById != null) ? new float?(combatantById.Heading) : ((float?)null), (uint?)null, (uint?)null, (uint?)null, (uint?)null, (float?)null, (float?)null, (float?)null, (float?)null, 0uL, 0uL, 0uL, 0uL, 0uL, 0uL, 0uL, 0uL, packetMessage.globalEffectCounter, (byte)0, (byte)0, (val != null) ? val.ID : 0u, (val != null) ? val.Name : null, (byte)(int)packetMessage.effectDisplayType, packetMessage.actionId, (uint)packetMessage.actionAnimationId, packetMessage.animationLockTime, packetMessage.rotation);
			_logOutput.WriteLine((LogMessageType)21, packetDate, text);
			_dataEvent.OnParsedLogLine((LogMessageType)21, text);
			return;
		}
		LogMessageType val2 = (LogMessageType)((maxTargetCount == 1) ? 21 : 22);
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
				string text2 = _logFormat.FormatNetworkAbilityMessage(actorID, (combatantById != null) ? combatantById.Name : null, num, _skillList.GetActionNameByID(num), num2, (combatantById2 != null) ? combatantById2.Name : null, (combatantById != null) ? new uint?(combatantById.CurrentHP) : ((uint?)null), (combatantById != null) ? new uint?(combatantById.MaxHP) : ((uint?)null), (combatantById != null) ? new uint?(combatantById.CurrentMP) : ((uint?)null), (combatantById != null) ? new uint?(combatantById.MaxMP) : ((uint?)null), (combatantById != null) ? new float?(combatantById.PosX) : ((float?)null), (combatantById != null) ? new float?(combatantById.PosY) : ((float?)null), (combatantById != null) ? new float?(combatantById.PosZ) : ((float?)null), (combatantById != null) ? new float?(combatantById.Heading) : ((float?)null), (combatantById2 != null) ? new uint?(combatantById2.CurrentHP) : ((uint?)null), (combatantById2 != null) ? new uint?(combatantById2.MaxHP) : ((uint?)null), (combatantById2 != null) ? new uint?(combatantById2.CurrentMP) : ((uint?)null), (combatantById2 != null) ? new uint?(combatantById2.MaxMP) : ((uint?)null), (combatantById2 != null) ? new float?(combatantById2.PosX) : ((float?)null), (combatantById2 != null) ? new float?(combatantById2.PosY) : ((float?)null), (combatantById2 != null) ? new float?(combatantById2.PosZ) : ((float?)null), (combatantById2 != null) ? new float?(combatantById2.Heading) : ((float?)null), ((ulong*)effectPtr)[b4 * 8], ((ulong*)effectPtr)[b4 * 8 + 1], ((ulong*)effectPtr)[b4 * 8 + 2], ((ulong*)effectPtr)[b4 * 8 + 3], ((ulong*)effectPtr)[b4 * 8 + 4], ((ulong*)effectPtr)[b4 * 8 + 5], ((ulong*)effectPtr)[b4 * 8 + 6], ((ulong*)effectPtr)[b4 * 8 + 7], packetMessage.globalEffectCounter, b4, b2, (val != null) ? val.ID : 0u, (val != null) ? val.Name : null, (byte)(int)packetMessage.effectDisplayType, packetMessage.actionId, (uint)packetMessage.actionAnimationId, packetMessage.animationLockTime, packetMessage.rotation);
				_logOutput.WriteLine(val2, packetDate, text2);
				_dataEvent.OnParsedLogLine(val2, text2);
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
