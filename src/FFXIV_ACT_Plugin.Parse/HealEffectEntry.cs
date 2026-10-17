using FFXIV_ACT_Plugin.Resource;

namespace FFXIV_ACT_Plugin.Parse;

public class HealEffectEntry : EffectEntry, IEffectAppliable
{
	public uint Amount => (uint)(Value + (((Flags2 & 0x40) != 0) ? (Flags1 * 65536) : 0));

	public bool IsCritical => (Param1 & 0x20) != 0;

	public DamageType DamageType => (DamageType)0;

	public string ActionName { get; set; }

	public string ProcActionName { get; set; }

	public uint ProcSourceId { get; set; }

	public uint ProcTargetId { get; set; }

	public string SourceNameOverride { get; set; }

	public bool IsHeal => true;

	public double CalculatedPotency { get; set; }

	public double CalculatedPotencyMultiplier { get; set; } = 1.0;

	public double AppliedPotencyMultiplier { get; set; }

	public string AppliedStatusEffects { get; set; }

	public double SourceCritRate { get; set; }

	public string AppliedCritStatusList { get; set; }

	public uint OverHealAmount { get; set; }

	public double CritBuffAmount { get; set; }

	public double CalculatedDirectHitRate { get; set; }

	public string AppliedDirectHitStatusList { get; set; }

	public double DirectHitBuffs { get; set; }
}
