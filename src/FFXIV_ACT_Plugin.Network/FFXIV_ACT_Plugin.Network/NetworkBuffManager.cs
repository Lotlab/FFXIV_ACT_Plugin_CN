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
		lock (_combatantManager.CombatantLock)
		{
			Combatant combatantById = _combatantManager.GetCombatantById(TargetId);
			if (combatantById == null)
			{
				return;
			}
			NetworkBuff networkBuff = combatantById.NetworkBuffs[index];
			if (networkBuff == null)
			{
				networkBuff = (combatantById.NetworkBuffs[index] = new NetworkBuff());
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
			if (networkBuff.BuffID == 0)
			{
				flag = true;
			}
			else if (BuffId == networkBuff.BuffID && ActorId == networkBuff.ActorID)
			{
				if (Duration != 9999f)
				{
					double num = Math.Abs((packetDate.AddSeconds(Duration) - networkBuff.Timestamp.AddSeconds(networkBuff.Duration)).TotalSeconds);
					if (num > 2.0)
					{
						flag = true;
					}
					else if (networkBuff.RefreshPending && num > 0.2)
					{
						flag = true;
						networkBuff.RefreshPending = false;
					}
				}
				if (BuffExtra != networkBuff.BuffExtra)
				{
					flag = true;
				}
			}
			else if (missingInfo)
			{
				if (BuffExtra != networkBuff.BuffExtra)
				{
					flag = true;
					ActorId = networkBuff.ActorID;
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
					networkBuff.ActorID = ActorId;
					networkBuff.ActorName = combatantById2?.Name;
					networkBuff.Duration = Duration;
					networkBuff.Timestamp = packetDate;
				}
				networkBuff.BuffExtra = BuffExtra;
				networkBuff.BuffID = BuffId;
				networkBuff.TargetID = TargetId;
				networkBuff.TargetName = combatantById.Name;
				string text = _logFormat.FormatNetworkBuffMessage(networkBuff.BuffID, _buffList.GetStatusNameByID(networkBuff.BuffID), networkBuff.Duration, networkBuff.ActorID, networkBuff.ActorName, networkBuff.TargetID, networkBuff.TargetName, networkBuff.BuffExtra, combatantById.MaxHP, combatantById2?.MaxHP);
				_logOutput.WriteLine(LogMessageType.StatusAdd, packetDate, text);
				_dataEvent.OnParsedLogLine(LogMessageType.StatusAdd, text);
			}
		}
	}

	private void RemoveBuff(uint TargetId, int index, DateTime packetDate)
	{
		Combatant combatantById = _combatantManager.GetCombatantById(TargetId);
		if (combatantById != null)
		{
			NetworkBuff networkBuff = combatantById.NetworkBuffs[index];
			if (networkBuff != null)
			{
				Combatant combatantById2 = _combatantManager.GetCombatantById(networkBuff.ActorID);
				string text = _logFormat.FormatNetworkBuffMessage(networkBuff.BuffID, _buffList.GetStatusNameByID(networkBuff.BuffID), 0f, networkBuff.ActorID, networkBuff.ActorName, networkBuff.TargetID, networkBuff.TargetName, networkBuff.BuffExtra, combatantById?.MaxHP, combatantById2?.MaxHP);
				ClearBuff(networkBuff);
				_logOutput.WriteLine(LogMessageType.StatusRemove, packetDate, text);
				_dataEvent.OnParsedLogLine(LogMessageType.StatusRemove, text);
			}
		}
	}

	public void ReplaceAllBuffsForTarget(uint TargetId, NetworkBuff[] buffs, DateTime packetDate)
	{
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
					if (combatantById.NetworkBuffs[b]?.BuffID > 0)
					{
						RemoveBuff(TargetId, b, packetDate);
					}
				}
				else if (!_buffList.IsIgnoreStatus(buffs[b].BuffID))
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
