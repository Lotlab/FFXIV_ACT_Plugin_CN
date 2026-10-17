namespace FFXIV_ACT_Plugin.Resource;

public class ResourceManager
{
	public IActionList _actionList;

	public IDefinitionRepository _definitionRepo;

	public IMapList _mapList;

	public INameResource _nameResource;

	public IStatusList _statusList;

	public ITerritoryList _territoryList;

	public IWorldList _worldList;

	public ResourceManager(IActionList actionList, IDefinitionRepository definitionRepo, IMapList mapList, INameResource nameResource, IStatusList statusList, ITerritoryList territoryList, IWorldList worldList)
	{
		_actionList = actionList;
		_definitionRepo = definitionRepo;
		_mapList = mapList;
		_nameResource = nameResource;
		_statusList = statusList;
		_territoryList = territoryList;
		_worldList = worldList;
	}

	public void LoadResources()
	{
		_definitionRepo.LoadResources();
		_actionList.LoadResources();
		_statusList.LoadResources();
		_mapList.LoadResources();
		_territoryList.LoadResources();
		_worldList.LoadResources();
	}
}
