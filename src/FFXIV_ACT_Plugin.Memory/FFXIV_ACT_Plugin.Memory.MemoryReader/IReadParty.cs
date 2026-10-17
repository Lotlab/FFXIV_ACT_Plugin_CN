using System;

namespace FFXIV_ACT_Plugin.Memory.MemoryReader;

public interface IReadParty : IDisposable
{
	IntPtr Read();
}
