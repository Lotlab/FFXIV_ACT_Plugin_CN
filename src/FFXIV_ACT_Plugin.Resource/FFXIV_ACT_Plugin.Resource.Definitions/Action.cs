using System.Linq;

namespace FFXIV_ACT_Plugin.Resource.Definitions;

public class Action
{
	public uint Id { get; set; }

	public string Name { get; set; }

	public DamageEntry[] Damage { get; set; }

	public HealEntry[] Heal { get; set; }

	public bool IsDamage => Damage?.Any() ?? false;

	public bool IsHeal => Heal?.Any() ?? false;

	public uint GetHealPotency(int targetIndex)
	{
		if (Heal != null && Heal.Any((HealEntry x) => x.TargetIndex == targetIndex))
		{
			return Heal.First((HealEntry x) => x.TargetIndex == targetIndex).Potency;
		}
		return (Heal?.FirstOrDefault((HealEntry x) => !x.TargetIndex.HasValue)?.Potency).GetValueOrDefault();
	}

	public uint GetDamagePotency(int targetIndex, bool isCombo)
	{
		if (Damage.Any((DamageEntry x) => x.TargetIndex == targetIndex))
		{
			return Damage.First((DamageEntry x) => x.TargetIndex == targetIndex).Potency;
		}
		DamageEntry damageEntry = Damage.FirstOrDefault((DamageEntry x) => !x.TargetIndex.HasValue);
		if (isCombo && damageEntry != null && damageEntry.Combo.HasValue)
		{
			return damageEntry.Combo.Value;
		}
		return damageEntry?.Potency ?? 0;
	}
}
