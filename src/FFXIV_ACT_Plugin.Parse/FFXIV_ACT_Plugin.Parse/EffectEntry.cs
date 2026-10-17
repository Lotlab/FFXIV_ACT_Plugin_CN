namespace FFXIV_ACT_Plugin.Parse;

public class EffectEntry
{
	public EffectEntryType EntryType { get; set; }

	public byte Param0 { get; set; }

	public byte Param1 { get; set; }

	public byte Param2 { get; set; }

	public byte Flags2 { get; set; }

	public byte Flags1 { get; set; }

	public ushort Value { get; set; }

	public bool IsSourceEntry => (Flags2 & 0x80) != 0;

	public byte EffectIndex { get; set; }
}
