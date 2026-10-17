using System;

namespace FFXIV_ACT_Plugin.Parse;

public class DeferredEvent
{
	public enum EventType
	{
		HoT = 1,
		DoT
	}

	public EventType Type { get; set; }

	public DateTime Timestamp { get; set; }

	public uint TargetId { get; set; }

	public uint StatusId { get; set; }

	public uint Amount { get; set; }

	public uint TargetCurrentHp { get; set; }

	public uint TargetMaxHp { get; set; }

	public uint SourceId { get; set; }
}
