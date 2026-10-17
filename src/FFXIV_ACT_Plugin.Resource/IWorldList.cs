namespace FFXIV_ACT_Plugin.Resource;

public interface IWorldList
{
	string GetWorldNameById(uint WorldID);

	void LoadResources();
}
