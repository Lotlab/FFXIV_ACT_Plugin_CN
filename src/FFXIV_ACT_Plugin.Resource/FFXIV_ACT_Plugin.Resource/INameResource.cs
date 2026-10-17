namespace FFXIV_ACT_Plugin.Resource;

public interface INameResource
{
	bool IsPet(uint bNpcNameId);

	bool IsTurret(uint bNpcNameId);
}
