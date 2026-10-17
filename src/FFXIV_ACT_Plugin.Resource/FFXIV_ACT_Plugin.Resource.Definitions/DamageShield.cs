namespace FFXIV_ACT_Plugin.Resource.Definitions;

public class DamageShield
{
	public enum DamageShieldType
	{
		TargetHpPercent = 1,
		Potency,
		HealPercent
	}

	public DamageShieldType Type { get; set; }

	public double Amount { get; set; }
}
