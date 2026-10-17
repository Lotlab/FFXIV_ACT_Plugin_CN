namespace FFXIV_ACT_Plugin.Resource;

public interface IStatusList
{
	string GetStatusNameByID(uint statusId);

	bool IsIgnoreStatus(uint statusId);

	void LoadResources();
}
