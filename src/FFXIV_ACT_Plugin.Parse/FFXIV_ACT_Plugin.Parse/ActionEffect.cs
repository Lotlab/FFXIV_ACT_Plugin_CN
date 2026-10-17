using System;
using FFXIV_ACT_Plugin.Resource.Definitions;

namespace FFXIV_ACT_Plugin.Parse;

public class ActionEffect
{
	public DateTime Timestamp { get; set; }

	public uint SequenceId { get; set; }

	public uint SourceId { get; set; }

	public uint TargetId { get; set; }

	public uint ActionId { get; set; }

	public int TargetIndex { get; set; }

	public EffectEntry[] Effects { get; set; }

	public uint SourceCurrentHp { get; set; }

	public uint SourceMaxHp { get; set; }

	public uint TargetCurrentHp { get; set; }

	public uint TargetMaxHp { get; set; }

	public FFXIV_ACT_Plugin.Resource.Definitions.Action Definition { get; set; }

	public bool IsKardionApplied { get; set; }
}
