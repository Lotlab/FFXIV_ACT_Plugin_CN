using System;

namespace FFXIV_ACT_Plugin.Memory.MemoryReader;

public interface IReadPlayer : IDisposable
{
	IntPtr Read();
}
