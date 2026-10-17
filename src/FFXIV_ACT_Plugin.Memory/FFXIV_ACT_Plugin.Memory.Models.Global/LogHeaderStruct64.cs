using System;
using System.Runtime.InteropServices;

namespace FFXIV_ACT_Plugin.Memory.Models.Global;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct LogHeaderStruct64
{
	[FieldOffset(72)]
	public IntPtr LenthArrayFirst;

	[FieldOffset(80)]
	public IntPtr LenthArrayLast;

	[FieldOffset(88)]
	public IntPtr LenthArrayEnd;

	[FieldOffset(96)]
	public IntPtr MessageArrayFirst;

	[FieldOffset(104)]
	public IntPtr MessageArrayLast;

	[FieldOffset(112)]
	public IntPtr MessageArrayEnd;
}
