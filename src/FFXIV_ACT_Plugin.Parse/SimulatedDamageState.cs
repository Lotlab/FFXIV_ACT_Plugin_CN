using System;
using System.Runtime.CompilerServices;
using FFXIV_ACT_Plugin.Resource;
using FFXIV_ACT_Plugin.Resource.Definitions;
using static FFXIV_ACT_Plugin.Resource.Definitions.TimeProc;

namespace FFXIV_ACT_Plugin.Parse;

public class SimulatedDamageState : IEffectAppliable
{
	public DateTime Timestamp { get; set; }

	public uint StatusId { get; set; }

	public uint SourceId { get; set; }

	public uint TargetId { get; set; }

	public byte? CritLowByte { get; set; }

	public byte? AmountLowByte { get; set; }

	public TickType TickType
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return field;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			field = value;
		}
	}

	public DamageType DamageType
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return field;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			field = value;
		}
	}

	public string ActionName { get; set; }

	public bool IsHeal
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Invalid comparison between Unknown and I4
			if ((int)TickType != 2)
			{
				return (int)TickType == 4;
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
