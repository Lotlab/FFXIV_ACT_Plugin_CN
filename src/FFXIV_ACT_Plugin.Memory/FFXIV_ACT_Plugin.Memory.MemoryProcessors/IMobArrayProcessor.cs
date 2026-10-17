using System;
using System.Collections.ObjectModel;

namespace FFXIV_ACT_Plugin.Memory.MemoryProcessors;

public interface IMobArrayProcessor
{
	ReadOnlyCollection<IntPtr> MobArray { get; }

	IntPtr PrimaryPlayerPointer { get; }

	void Refresh();
}
