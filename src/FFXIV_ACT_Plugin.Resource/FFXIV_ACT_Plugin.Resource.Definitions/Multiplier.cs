namespace FFXIV_ACT_Plugin.Resource.Definitions;

public class Multiplier
{
	public enum MultiplierType
	{
		CriticalHit = 1,
		DirectHit,
		CriticalReceived
	}

	public MultiplierType Type { get; set; }

	public uint Amount { get; set; }

	public string LimitTo { get; set; }

	public uint[] LimitToActionIds { get; set; }
}
