using FFXIV_ACT_Plugin.Memory.MemoryReader;
using FFXIV_ACT_Plugin.Memory.Models.Global;

namespace FFXIV_ACT_Plugin.Memory.MemoryProcessors;

public class ZoneMapProcessor : IZoneMapProcessor
{
	private readonly object _zoneLock = new object();

	private uint _zoneID;

	private uint _mapID;

	private readonly IReadZoneMap _readZoneMap;

	private readonly IReadMapId _readMapId;

	private readonly DataEvent _dataEvent;

	public uint ZoneID
	{
		get
		{
			lock (_zoneLock)
			{
				return _zoneID;
			}
		}
	}

	public uint MapID
	{
		get
		{
			lock (_zoneLock)
			{
				return _mapID;
			}
		}
	}

	public ZoneMapProcessor(IReadZoneMap readZoneMap, IReadMapId readMapId, DataEvent dataEvent)
	{
		_readZoneMap = readZoneMap;
		_readMapId = readMapId;
		_dataEvent = dataEvent;
	}

	public unsafe void Refresh()
	{
		ZoneMapStruct* ptr = (ZoneMapStruct*)(void*)_readZoneMap.Read();
		if (ptr == null)
		{
			return;
		}
		uint num = ptr->ZoneId;
		uint num2 = 0u;
		if (num > 4095)
		{
			num = 0u;
		}
		if (ZoneID != num)
		{
			lock (_zoneLock)
			{
				_zoneID = num;
			}
			if (ZoneID != 0)
			{
				_dataEvent.OnZoneChanged(ZoneID);
			}
		}
		if (num != 0)
		{
			num2 = ptr->MapId;
			uint num3 = _readMapId.Read();
			if ((num3 != 0) & (num3 < 4095))
			{
				num2 = num3;
			}
		}
		if (MapID != num2)
		{
			lock (_zoneLock)
			{
				_mapID = num2;
			}
			if (MapID != 0)
			{
				_dataEvent.OnMapChanged(MapID);
			}
		}
	}
}
