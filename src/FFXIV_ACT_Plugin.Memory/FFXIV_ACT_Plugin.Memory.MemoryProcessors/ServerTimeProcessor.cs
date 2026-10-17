using System;
using FFXIV_ACT_Plugin.Memory.MemoryReader;

namespace FFXIV_ACT_Plugin.Memory.MemoryProcessors;

public class ServerTimeProcessor : IServerTimeProcessor
{
	private readonly IReadServerTime _readServerTime;

	public DateTime Date1970 { get; }

	public int MaxDate { get; }

	public DateTime ServerTime { get; set; }

	public ServerTimeProcessor(IReadServerTime readServerTime)
	{
		_readServerTime = readServerTime;
		Date1970 = DateTime.MinValue.AddYears(1969);
		MaxDate = Convert.ToInt32((Date1970.AddYears(60) - Date1970).TotalSeconds);
	}

	public void Refresh()
	{
		ulong num = _readServerTime.Read();
		uint num2 = (uint)(num & 0xFFFFFFFFu);
		uint num3 = (uint)(num >> 32);
		if (num2 != 0 && num2 < MaxDate)
		{
			ServerTime = Date1970.AddSeconds(num2).AddMilliseconds(num3).ToLocalTime();
		}
	}
}
