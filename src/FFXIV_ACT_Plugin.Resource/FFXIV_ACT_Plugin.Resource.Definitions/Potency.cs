namespace FFXIV_ACT_Plugin.Resource.Definitions;

public class Potency
{
	public enum EffectType
	{
		DamageAddPotency = 1,
		DamageDoneMultiplier,
		PotencyMultiplier,
		DamageReceivedMultiplier,
		HealDoneMultiplier,
		HealReceiveMultiplier
	}

	public EffectType Type { get; set; }

	public int Amount { get; set; }

	public byte? AmountByte { get; set; }

	public bool IsStacked { get; set; }

	public string LimitTo { get; set; }

	public uint[] LimitToActionIds { get; set; }

	public DamageType LimitToDamageType { get; set; }

	public ActionCategory LimitToActionCategory { get; set; }

	public uint? LimitToZoneId { get; set; }
}
