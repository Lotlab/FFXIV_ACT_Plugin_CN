using System.Runtime.InteropServices;

namespace FFXIV_ACT_Plugin.Memory.Models.Global;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct PartyStruct64
{
	[FieldOffset(0)]
	public unsafe fixed byte Entries[32704];

	[FieldOffset(32724)]
	public byte PartyFlag1;

	[FieldOffset(32732)]
	public byte PartySize;
}
