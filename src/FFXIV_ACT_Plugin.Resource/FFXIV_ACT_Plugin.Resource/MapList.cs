#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FFXIV_ACT_Plugin.Resource;

public class MapList : IMapList
{
	private SortedDictionary<uint, Map> _Maps;

	public void LoadResources()
	{
		string[] source = new ResourceFile("Generated.MapList_English.txt").Content.Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
		_Maps = new SortedDictionary<uint, Map>(source.Select((string x) => x.Split('|')).ToDictionary((string[] x) => Convert.ToUInt32(x[0], 16), (string[] x) => new Map
		{
			Id = Convert.ToUInt32(x[0], 16),
			Region = x[1],
			Name = x[2],
			SubName = x[3]
		}));
	}

	public Map GetMapById(uint MapId)
	{
		if (_Maps == null)
		{
			return null;
		}
		if (!_Maps.ContainsKey(MapId))
		{
			Trace.WriteLine($"UNKNOWN MAP ID [{MapId:x2}]", "FFXIV_ACT_Plugin");
			return null;
		}
		return _Maps[MapId];
	}
}
