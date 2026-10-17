using System;

namespace FFXIV_ACT_Plugin.Parse;

public interface IReportCombatData
{
	void AddDamageEntry(EnvironmentState state, DateTime timestamp, uint sourceId, uint targetId, DamageEffectEntry entry);

	void AddHealEntry(EnvironmentState state, DateTime timestamp, uint sourceId, uint targetId, HealEffectEntry entry);

	void AddDeathEntry(EnvironmentState state, DateTime timestamp, uint sourceId, uint targetId);

	void AddStatusEffect(EnvironmentState state, DateTime timestamp, string statusName, uint sourceId, uint targetId, double Duration, ushort param, byte? extraByte0, byte? extraByte1, byte? extraByte2);

	void AddCancelEntry(EnvironmentState state, DateTime timestamp, uint sourceId, uint targetId, string actionName, string cancelType, bool isHeal, bool isDamage);

	void AddDoTEntry(EnvironmentState state, DateTime timestamp, uint sourceId, uint targetId, DoTEffectEntry entry, bool isHoT);

	void AddActionEntry(EnvironmentState state, DateTime timestamp, uint sourceId, uint targetId, string actionName);

	void AddDamageShieldEntry(EnvironmentState state, DateTime timestamp, DamageShieldEntry shieldEntry);

	void AddResourceEntry(EnvironmentState state, DateTime timestamp, uint sourceId, uint targetId, ResourceEffectEntry resourceEntry);

	void ChangeTerritory(string territoryName);

	void EchoCommand(string command);

	void StopCombat(bool actExport);
}
