using System;

namespace FFXIV_ACT_Plugin.Memory.MemoryReader;

public interface IReadMobArray : IDisposable
{
	IntPtr Read64();
}
