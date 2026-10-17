namespace FFXIV_ACT_Plugin.Resource;

public interface ITerritoryList
{
	string GetTerritoryNameById(uint territoryId);

	void LoadResources();
}
