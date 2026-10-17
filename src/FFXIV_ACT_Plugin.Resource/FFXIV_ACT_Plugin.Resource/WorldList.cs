#define TRACE
using System.Collections.Generic;
using System.Diagnostics;

namespace FFXIV_ACT_Plugin.Resource;

public class WorldList : IWorldList
{
	private SortedDictionary<uint, string> _Worlds;

	public void LoadResources()
	{
		_Worlds = new ResourceFile("Generated.WorldList_English.txt", isIdHex: false).Dictionary;
	}

	public string GetWorldNameById(uint WorldID)
	{
		if (_Worlds == null)
		{
			return $"Unknown_{WorldID:X2}";
		}
		if (WorldID == 0)
		{
			return "";
		}
		if (!_Worlds.ContainsKey(WorldID))
		{
			Trace.WriteLine($"UNKNOWN WORLD ID [{WorldID:X2}]", "FFXIV_ACT_Plugin");
			return $"Unknown_{WorldID:X2}";
		}
		return _Worlds[WorldID];
	}
}
