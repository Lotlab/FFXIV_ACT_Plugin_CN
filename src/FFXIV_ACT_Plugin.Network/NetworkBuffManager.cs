using System;
using FFXIV_ACT_Plugin.Common.Models;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory;
using FFXIV_ACT_Plugin.Resource;

namespace FFXIV_ACT_Plugin.Network;

public class NetworkBuffManager : INetworkBuffManager
{
	private readonly ICombatantManager _combatantManager;

	private readonly ILogOutput _logOutput;

	private readonly IStatusList _buffList;

	private readonly ILogFormat _logFormat;

	private readonly DataEvent _dataEvent;

	public NetworkBuffManager(ILogOutput logOutput, IStatusList buffList, ICombatantManager combatantManager, ILogFormat logFormat, DataEvent dataEvent)
	{
		_logOutput = logOutput;
		_logFormat = logFormat;
		_dataEvent = dataEvent;
		_buffList = buffList;
		_combatantManager = combatantManager;
	}

	public void AddUpdateBuffForTarget(uint ActorId, uint TargetId, ushort BuffId, ushort BuffExtra, float Duration, DateTime packetDate, byte index, bool missingInfo)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected Obj, but got Unknown
		//IL_0049: Expected Obj, but got Unknown
		lock (_combatantManager.CombatantLock)
		{
			Combatant combatantById = _combatantManager.GetCombatantById(TargetId);
			if (combatantById == null)
			{
				return;
			}
			NetworkBuff val = combatantById.NetworkBuffs[index];
			if (val == null)
			{
				NetworkBuff[] networkBuffs = combatantById.NetworkBuffs;
				byte b = index;
				NetworkBuff val2 = new NetworkBuff();
				NetworkBuff val3 = val2;
				networkBuffs[b] = val2;
				val = val3;
			}
			bool flag = false;
			if (Duration == 0f)
			{
				Duration = 9999f;
			}
			if (Duration > 9999f)
			{
				Duration = 9999f;
			}
			if (val.BuffID == 0)
			{
				flag = true;
			}
			else if (BuffId == val.BuffID && ActorId == val.ActorID)
			{
				if (Duration != 9999f)
				{
					double num = Math.Abs((packetDate.AddSeconds(Duration) - val.Timestamp.AddSeconds(val.Duration)).TotalSeconds);
					if (num > 2.0)
					{
						flag = true;
					}
					else if (val.RefreshPending && num > 0.2)
					{
						flag = true;
						val.RefreshPending = false;
					}
				}
				if (BuffExtra != val.BuffExtra)
				{
					flag = true;
				}
			}
			else if (missingInfo)
			{
				if (BuffExtra != val.BuffExtra)
				{
					flag = true;
					ActorId = val.ActorID;
				}
			}
			else
			{
				RemoveBuff(TargetId, index, packetDate);
				flag = true;
			}
			if (flag)
			{
				Combatant combatantById2 = _combatantManager.GetCombatantById(ActorId);
				if (!missingInfo)
				{
					val.ActorID = ActorId;
					val.ActorName = ((combatantById2 != null) ? combatantById2.Name : null);
					val.Duration = Duration;
					val.Timestamp = packetDate;
				}
				val.BuffExtra = BuffExtra;
				val.BuffID = BuffId;
				val.TargetID = TargetId;
				val.TargetName = combatantById.Name;
				string text = _logFormat.FormatNetworkBuffMessage(val.BuffID, _buffList.GetStatusNameByID((uint)val.BuffID), val.Duration, val.ActorID, val.ActorName, val.TargetID, val.TargetName, val.BuffExtra, (uint?)combatantById.MaxHP, (combatantById2 != null) ? new uint?(combatantById2.MaxHP) : ((uint?)null));
				_logOutput.WriteLine((LogMessageType)26, packetDate, text);
				_dataEvent.OnParsedLogLine((LogMessageType)26, text);
			}
		}
	}

	private void RemoveBuff(uint TargetId, int index, DateTime packetDate)
	{
		Combatant combatantById = _combatantManager.GetCombatantById(TargetId);
		if (combatantById != null)
		{
			NetworkBuff val = combatantById.NetworkBuffs[index];
			if (val != null)
			{
				Combatant combatantById2 = _combatantManager.GetCombatantById(val.ActorID);
				string text = _logFormat.FormatNetworkBuffMessage(val.BuffID, _buffList.GetStatusNameByID((uint)val.BuffID), 0f, val.ActorID, val.ActorName, val.TargetID, val.TargetName, val.BuffExtra, (combatantById != null) ? new uint?(combatantById.MaxHP) : ((uint?)null), (combatantById2 != null) ? new uint?(combatantById2.MaxHP) : ((uint?)null));
				ClearBuff(val);
				_logOutput.WriteLine((LogMessageType)30, packetDate, text);
				_dataEvent.OnParsedLogLine((LogMessageType)30, text);
			}
		}
	}

	public void ReplaceAllBuffsForTarget(uint TargetId, NetworkBuff[] buffs, DateTime packetDate)
	{
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Expected Obj, but got Unknown
		lock (_combatantManager.CombatantLock)
		{
			Combatant combatantById = _combatantManager.GetCombatantById(TargetId);
			if (combatantById == null)
			{
				return;
			}
			if (buffs.Length > combatantById.NetworkBuffs.Length)
			{
				Array.Resize(ref combatantById.NetworkBuffs, buffs.Length);
			}
			for (byte b = 0; b < combatantById.NetworkBuffs.Length; b++)
			{
				if (b >= buffs.Length)
				{
					NetworkBuff obj = combatantById.NetworkBuffs[b];
					if (((obj != null) ? new ushort?(obj.BuffID) : ((ushort?)null)) > 0)
					{
						RemoveBuff(TargetId, b, packetDate);
					}
				}
				else if (!_buffList.IsIgnoreStatus((uint)buffs[b].BuffID))
				{
					if (combatantById.NetworkBuffs[b] == null)
					{
						combatantById.NetworkBuffs[b] = new NetworkBuff();
					}
					if (buffs[b].BuffID != 0)
					{
						AddUpdateBuffForTarget(buffs[b].ActorID, TargetId, buffs[b].BuffID, buffs[b].BuffExtra, buffs[b].Duration, packetDate, b, missingInfo: false);
					}
					else if (combatantById.NetworkBuffs[b].BuffID != 0)
					{
						RemoveBuff(TargetId, b, packetDate);
					}
				}
			}
		}
	}

	private void ClearBuff(NetworkBuff buff)
	{
		buff.ActorID = 0u;
		buff.ActorName = "";
		buff.BuffExtra = 0;
		buff.BuffID = 0;
		buff.Duration = 0f;
		buff.TargetID = 0u;
		buff.TargetName = "";
		buff.Timestamp = DateTime.MinValue;
	}
}
