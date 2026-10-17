namespace FFXIV_ACT_Plugin.Resource;

public interface IMapList
{
	Map GetMapById(uint MapId);

	void LoadResources();
}
