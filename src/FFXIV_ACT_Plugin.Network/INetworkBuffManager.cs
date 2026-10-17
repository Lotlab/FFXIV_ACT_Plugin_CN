using System;
using FFXIV_ACT_Plugin.Common.Models;

namespace FFXIV_ACT_Plugin.Network;

public interface INetworkBuffManager
{
	void AddUpdateBuffForTarget(uint ActorId, uint TargetId, ushort BuffId, ushort BuffExtra, float Duration, DateTime packetDate, byte index, bool missingInfo);

	void ReplaceAllBuffsForTarget(uint TargetId, NetworkBuff[] buffs, DateTime packetDate);
}
