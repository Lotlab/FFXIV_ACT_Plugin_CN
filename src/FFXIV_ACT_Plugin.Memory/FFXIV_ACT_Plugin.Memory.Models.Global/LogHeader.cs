using System;

namespace FFXIV_ACT_Plugin.Memory.Models.Global;

public class LogHeader
{
	public IntPtr LengthArrayStart;

	public IntPtr LengthArrayEnd;

	public IntPtr MessageArrayStart;

	public IntPtr MessageArrayEnd;

	public int NextUnprocessedIndex;
}
