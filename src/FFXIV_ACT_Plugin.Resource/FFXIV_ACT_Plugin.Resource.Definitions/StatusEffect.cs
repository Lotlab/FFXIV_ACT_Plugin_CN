namespace FFXIV_ACT_Plugin.Resource.Definitions;

public class StatusEffect
{
	public uint Id { get; set; }

	public string Name { get; set; }

	public Potency[] PotencyEffects { get; set; }

	public TimeProc TimeProc { get; set; }

	public Multiplier[] Multipliers { get; set; }

	public ReactiveProc ReactiveProc { get; set; }

	public DamageShield DamageShield { get; set; }
}
