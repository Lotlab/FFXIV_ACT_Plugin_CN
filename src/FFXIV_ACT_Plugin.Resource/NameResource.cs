using System.Collections.Generic;
using System.Linq;

namespace FFXIV_ACT_Plugin.Resource;

public class NameResource : INameResource
{
	private readonly IReadOnlyCollection<uint> _turretBNpcNameIds = new List<uint> { 3666u, 3667u, 8230u };

	private readonly IReadOnlyCollection<uint> _petBNpcNameIds = new List<uint>
	{
		1401u, 1400u, 1398u, 1399u, 3666u, 3667u, 6566u, 8228u, 8227u, 5978u,
		9350u, 10010u, 4269u, 8229u, 8230u, 8313u, 10261u, 4149u, 5478u, 1402u,
		1881u, 1403u, 1404u, 10262u, 10264u, 10263u, 2095u, 6565u, 10349u, 13159u
	};

	public bool IsPet(uint bNpcNameId)
	{
		return _petBNpcNameIds.Any((uint x) => x == bNpcNameId);
	}

	public bool IsTurret(uint bNpcNameId)
	{
		return _turretBNpcNameIds.Any((uint x) => x == bNpcNameId);
	}
}
