using FFXIV_ACT_Plugin.Resource;

namespace FFXIV_ACT_Plugin.Parse;

public class DamageShieldEntry : IEffectAppliable
{
	public uint Amount { get; set; }

	public uint SourceId { get; set; }

	public uint TargetId { get; set; }

	public bool IsHeal => true;

	public DamageType DamageType => (DamageType)0;

	public string ActionName { get; set; }

	public double CalculatedPotencyMultiplier { get; set; } = 1.0;

	public string AppliedStatusEffects { get; set; }

	public double CalculatedPotency { get; set; }

	public double AppliedPotencyMultiplier { get; set; }

	public double CritBuffAmount { get; set; }

	public double CalculatedDirectHitRate { get; set; }

	public string AppliedCritStatusList { get; set; }

	public string AppliedDirectHitStatusList { get; set; }

	public double DirectHitBuffs { get; set; }
}
