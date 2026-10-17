using System.Runtime.InteropServices;

namespace FFXIV_ACT_Plugin.Memory.Models.Global;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct LogDetailStruct
{
	[FieldOffset(0)]
	public uint Seconds;

	[FieldOffset(4)]
	public uint EventType;

	[FieldOffset(8)]
	public byte separator1;
}
