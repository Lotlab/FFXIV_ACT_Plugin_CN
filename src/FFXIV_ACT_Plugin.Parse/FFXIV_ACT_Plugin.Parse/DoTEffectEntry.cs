namespace FFXIV_ACT_Plugin.Parse;

public class DoTEffectEntry : DamageEffectEntry
{
	public uint DoTBaseDamage { get; set; }

	public uint OverHealAmount { get; set; }

	public byte AmountByte { get; set; }

	public byte CritByte { get; set; }
}
