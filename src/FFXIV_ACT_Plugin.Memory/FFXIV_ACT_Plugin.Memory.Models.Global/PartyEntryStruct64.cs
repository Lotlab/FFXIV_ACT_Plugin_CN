using System.Runtime.InteropServices;

namespace FFXIV_ACT_Plugin.Memory.Models.Global;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 1168)]
public struct PartyEntryStruct64
{
	[FieldOffset(1024)]
	public uint ID;

	[FieldOffset(1052)]
	public unsafe fixed byte Name[64];

	[FieldOffset(1156)]
	public byte Flags;
}
