using FFXIV_ACT_Plugin.Resource;

namespace FFXIV_ACT_Plugin.Parse;

public interface IEffectAppliable
{
	bool IsHeal { get; }

	DamageType DamageType { get; }

	string ActionName { get; set; }

	double CalculatedPotencyMultiplier { get; set; }

	string AppliedStatusEffects { get; set; }

	double CalculatedPotency { get; set; }

	double AppliedPotencyMultiplier { get; set; }

	string AppliedCritStatusList { get; set; }

	double CritBuffAmount { get; set; }

	string AppliedDirectHitStatusList { get; set; }

	double DirectHitBuffs { get; set; }

	double CalculatedDirectHitRate { get; set; }
}
