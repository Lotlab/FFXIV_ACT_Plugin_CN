using System.Runtime.InteropServices;

namespace FFXIV_ACT_Plugin.Memory.Models.Global;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct ZoneMapStruct
{
	[FieldOffset(0)]
	public uint ZoneId;

	[FieldOffset(24)]
	public uint MapId;
}
