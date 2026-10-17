using System;

namespace FFXIV_ACT_Plugin.Memory.MemoryReader;

public interface IReadZoneMap : IDisposable
{
	IntPtr Read();
}
