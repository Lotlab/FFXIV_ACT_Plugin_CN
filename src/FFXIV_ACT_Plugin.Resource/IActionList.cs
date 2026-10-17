namespace FFXIV_ACT_Plugin.Resource;

public interface IActionList
{
	string GetActionNameByID(uint actionId);

	ActionCategory GetActionCategory(uint actionId);

	uint[] GetAbilityIdsByEnglishName(string actionName);

	void LoadResources();
}
