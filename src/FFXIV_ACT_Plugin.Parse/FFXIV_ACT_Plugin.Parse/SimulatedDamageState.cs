using System;
using FFXIV_ACT_Plugin.Resource;
using FFXIV_ACT_Plugin.Resource.Definitions;

namespace FFXIV_ACT_Plugin.Parse;

public class SimulatedDamageState : IEffectAppliable
{
	public DateTime Timestamp { get; set; }

	public uint StatusId { get; set; }

	public uint SourceId { get; set; }

	public uint TargetId { get; set; }

	public byte? CritLowByte { get; set; }

	public byte? AmountLowByte { get; set; }

	public TimeProc.TickType TickType { get; set; }

	public DamageType DamageType { get; set; }

	public string ActionName { get; set; }

	public bool IsHeal
	{
		get
		{
			if (TickType != TimeProc.TickType.HoT)
			{
				return TickType == TimeProc.TickType.GroundHeal;
			}
			return true;
		}
	}

	public double CalculatedPotency { get; set; }

	public double CalculatedPotencyMultiplier { get; set; }

	public double AppliedPotencyMultiplier { get; set; }

	public double CalculatedCritRate { get; set; }

	public double CritBuffAmount { get; set; }

	public double DirectHitBuffs { get; set; }

	public double CalculatedDirectHitRate { get; set; }

	public string AppliedStatusEffects { get; set; }

	public string AppliedCritStatusList { get; set; }

	public string AppliedDirectHitStatusList { get; set; }

	public uint SimulatedAmount { get; set; }

	public byte MaxTicks { get; set; }

	public byte UsedTicks { get; set; }

	public DateTime LastTickTimestamp { get; set; }
}
