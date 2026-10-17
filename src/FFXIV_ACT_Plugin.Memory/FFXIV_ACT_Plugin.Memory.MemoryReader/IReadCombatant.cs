using System;

namespace FFXIV_ACT_Plugin.Memory.MemoryReader;

public interface IReadCombatant : IDisposable
{
	IntPtr Read(IntPtr address);

	IntPtr ReadHeader(IntPtr address);
}
