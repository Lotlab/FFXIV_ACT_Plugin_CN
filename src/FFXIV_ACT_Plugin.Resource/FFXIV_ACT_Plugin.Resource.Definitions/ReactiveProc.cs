namespace FFXIV_ACT_Plugin.Resource.Definitions;

public class ReactiveProc
{
	public enum ReactiveProcType
	{
		DamageOnDamageReceived = 1,
		HealOnDamageDealt,
		HealOnDamageReceived,
		HealOnHealCast
	}

	public ReactiveProcType Type { get; set; }

	public uint? amount { get; set; }
}
