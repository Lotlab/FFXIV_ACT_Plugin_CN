namespace FFXIV_ACT_Plugin.Memory.Models.Global;

public class Party
{
	public bool IsInParty { get; set; }

	public byte PartySize { get; set; }

	public uint[] MemberIDs { get; set; }
}
