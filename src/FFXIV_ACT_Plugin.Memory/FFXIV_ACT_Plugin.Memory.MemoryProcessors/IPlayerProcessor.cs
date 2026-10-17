using FFXIV_ACT_Plugin.Common.Models;

namespace FFXIV_ACT_Plugin.Memory.MemoryProcessors;

public interface IPlayerProcessor
{
	Player Player { get; }

	void Refresh();

	void Clear();
}
