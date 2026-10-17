using System;

namespace FFXIV_ACT_Plugin.Parse;

public class ActionCast
{
	public DateTime Timestamp { get; set; }

	public uint SourceId { get; set; }

	public uint TargetId { get; set; }

	public uint ActionId { get; set; }
}
