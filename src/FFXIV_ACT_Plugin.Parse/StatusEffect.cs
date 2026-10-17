using System;
using FFXIV_ACT_Plugin.Resource.Definitions;

namespace FFXIV_ACT_Plugin.Parse;

public class StatusEffect
{
	public DateTime Timestamp { get; set; }

	public uint SourceId { get; set; }

	public uint StatusId { get; set; }

	public double Duration { get; set; }

	public ushort Param1 { get; set; }

	public byte? EffectByte0 { get; set; }

	public byte? EffectByte1 { get; set; }

	public byte? EffectByte2 { get; set; }

	public FFXIV_ACT_Plugin.Resource.Definitions.StatusEffect Definition { get; set; }

	public SimulatedDamageState SimulatedDamage { get; set; }

	public ActionEffect ActionEffect { get; set; }

	public DateTime AppliedTimestamp { get; set; }
}
