using FFXIV_ACT_Plugin.Resource;

namespace FFXIV_ACT_Plugin.Parse;

public class DamageEffectEntry : EffectEntry, IEffectAppliable
{
	public uint Amount => (uint)(Value + (((Flags2 & 0x40) != 0) ? (Flags1 * 65536) : 0));

	public uint ComboAmount => Param2;

	public bool IsCritical => (Param0 & 0x20) != 0;

	public bool IsDh => (Param0 & 0x40) != 0;

	public bool IsParried => EntryType == EffectEntryType.ParriedDamage;

	public bool IsBlocked => EntryType == EffectEntryType.BlockedDamage;

	public string SpecialDamage
	{
		get
		{
			if (!IsBlocked)
			{
				if (!IsParried)
				{
					if (EntryType != EffectEntryType.Miss)
					{
						return "";
					}
					return "Miss";
				}
				return "Parried";
			}
			return "Blocked";
		}
	}

	public ElementType ElementType => (ElementType)(Param1 >> 4);

	public DamageType DamageType => (DamageType)(Param1 & 0xF);

	public bool IsAutoAttack { get; set; }

	public string ActionName { get; set; }

	public string ProcActionName { get; set; }

	public uint ProcSourceId { get; set; }

	public uint ProcTargetId { get; set; }

	public string SourceNameOverride { get; set; }

	public bool IsHeal => false;

	public double CalculatedPotency { get; set; }

	public double CalculatedPotencyMultiplier { get; set; } = 1.0;

	public double AppliedPotencyMultiplier { get; set; }

	public string AppliedStatusEffects { get; set; }

	public double SourceCritRate { get; set; }

	public string AppliedCritStatusList { get; set; }

	public double SourceDirectHitRate { get; set; }

	public string AppliedDhStatusList { get; set; }

	public double CritBuffAmount { get; set; }

	public double CalculatedDirectHitRate { get; set; }

	public string AppliedDirectHitStatusList { get; set; }

	public double DirectHitBuffs { get; set; }
}
