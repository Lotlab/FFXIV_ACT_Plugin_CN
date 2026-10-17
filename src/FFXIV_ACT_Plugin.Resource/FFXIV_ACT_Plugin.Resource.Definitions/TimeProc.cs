namespace FFXIV_ACT_Plugin.Resource.Definitions;

public class TimeProc
{
	public enum TickType
	{
		DoT = 1,
		HoT,
		GroundDamage,
		GroundHeal
	}

	public TickType Type { get; set; }

	public uint Potency { get; set; }

	public DamageType DamageType { get; set; }

	public byte MaxTicks { get; set; }
}
