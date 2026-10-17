using FFXIV_ACT_Plugin.Resource.Definitions;

namespace FFXIV_ACT_Plugin.Resource;

public interface IDefinitionRepository
{
	Action GetActionById(uint actionId);

	string[] GetAllJobDefinitionFileNames();

	StatusEffect GetStatusEffectById(uint statusId);

	uint[] GetStatusIdsByReactiveProcType(ReactiveProc.ReactiveProcType procType);

	void LoadResources();
}
