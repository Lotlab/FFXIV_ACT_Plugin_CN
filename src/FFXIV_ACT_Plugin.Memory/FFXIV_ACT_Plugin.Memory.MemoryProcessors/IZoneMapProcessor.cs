namespace FFXIV_ACT_Plugin.Memory.MemoryProcessors;

public interface IZoneMapProcessor
{
	uint ZoneID { get; }

	uint MapID { get; }

	void Refresh();
}
