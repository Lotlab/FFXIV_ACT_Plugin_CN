namespace FFXIV_ACT_Plugin.Parse;

public class ResourceEffectEntry : EffectEntry
{
	public bool Increase { get; set; }

	public int Amount => (Increase ? 1 : (-1)) * (Value + (((Flags2 & 0x40) != 0) ? (Flags1 * 65536) : 0));

	public string ActionName { get; set; }

	public ResourceType ResourceType { get; set; }
}
