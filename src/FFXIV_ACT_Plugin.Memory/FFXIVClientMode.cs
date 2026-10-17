using System;

namespace FFXIV_ACT_Plugin.Memory;

public enum FFXIVClientMode
{
	Unknown,
	[Obsolete("32-bit client not supported")]
	FFXIV_32,
	FFXIV_64
}
