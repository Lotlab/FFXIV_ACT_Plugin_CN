using System;

namespace FFXIV_ACT_Plugin.Memory.MemoryProcessors;

public interface IServerTimeProcessor
{
	DateTime Date1970 { get; }

	int MaxDate { get; }

	DateTime ServerTime { get; }

	void Refresh();
}
