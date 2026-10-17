using System;
using System.Collections.Generic;
using System.Globalization;
using Advanced_Combat_Tracker;

namespace FFXIV_ACT_Plugin;

public static class CombatantDataExtension
{
	public static string Job(this CombatantData combatant)
	{
		for (int i = 0; i < combatant.AllOut.Values.Count; i++)
		{
			if (combatant.AllOut.Values[i].Items.Count > 0 && combatant.AllOut.Values[i].Items[0].Tags.ContainsKey("Job") && !string.IsNullOrWhiteSpace(combatant.AllOut.Values[i].Items[0].Tags["Job"].ToString()))
			{
				return combatant.AllOut.Values[i].Items[0].Tags["Job"].ToString();
			}
		}
		return "";
	}

	public static long Parry(this AttackType attackType)
	{
		long num = 0L;
		for (int i = 0; i < attackType.Items.Count; i++)
		{
			if (attackType.Items[i].Special == "Parried")
			{
				num++;
			}
		}
		return num;
	}

	public static long Block(this AttackType attackType)
	{
		long num = 0L;
		for (int i = 0; i < attackType.Items.Count; i++)
		{
			if (attackType.Items[i].Special == "Blocked")
			{
				num++;
			}
		}
		return num;
	}

	public static long BlockParryCount(this AttackType attackType)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < attackType.Items.Count; i++)
		{
			if ((attackType.Items[i].Special == "Blocked" || attackType.Items[i].Special == "Parried") && !list.Contains(attackType.Items[i].AttackType))
			{
				list.Add(attackType.Items[i].AttackType);
			}
		}
		int num = 0;
		for (int j = 0; j < attackType.Items.Count; j++)
		{
			if (list.Contains(attackType.Items[j].AttackType))
			{
				num++;
			}
		}
		return num;
	}

	public static long DirectHeal(this CombatantData combatant)
	{
		long num = 0L;
		List<MasterSwing> items = combatant.Items[CombatantData.DamageTypeDataOutgoingHealing].Items["All"].Items;
		for (int i = 0; i < items.Count; i++)
		{
			if (items[i].DamageType != "DamageShield" && items[i].DamageType != "Absorb")
			{
				num = (long)((long)(num) + items[i].Damage);
			}
		}
		return num;
	}

	public static long Overheal(this AttackType attackType)
	{
		long num = 0L;
		for (int i = 0; i < attackType.Items.Count; i++)
		{
			if (attackType.Items[i].Tags.ContainsKey("overheal") && long.TryParse(attackType.Items[i].Tags["overheal"].ToString(), NumberStyles.Integer, null, out var result))
			{
				num += result;
			}
		}
		return num;
	}

	public static long DirectHitCount(this AttackType attackType)
	{
		long num = 0L;
		for (int i = 0; i < attackType.Items.Count; i++)
		{
			if (attackType.Items[i].Tags.ContainsKey("DirectHit") && attackType.Items[i].Tags["DirectHit"].ToString() == "True")
			{
				num++;
			}
		}
		return num;
	}

	public static long CritDirectHitCount(this AttackType attackType)
	{
		long num = 0L;
		for (int i = 0; i < attackType.Items.Count; i++)
		{
			if (attackType.Items[i].Tags.ContainsKey("DirectHit") && attackType.Items[i].Tags["DirectHit"].ToString() == "True" && attackType.Items[i].Critical)
			{
				num++;
			}
		}
		return num;
	}

	public static double LastNDPS(this CombatantData combatant, int N)
	{
		long num = 0L;
		DateTime dateTime = ActGlobals.oFormActMain.LastKnownTime.Subtract(new TimeSpan(0, 0, N));
		List<MasterSwing> items = combatant.Items[CombatantData.DamageTypeDataOutgoingDamage].Items["All"].Items;
		for (int i = 0; i < items.Count; i++)
		{
			if (items[i].Time > dateTime)
			{
				num += items[i].Damage.Number;
			}
		}
		return (double)num / ((combatant.Duration.TotalSeconds < (double)N) ? combatant.Duration.TotalSeconds : ((double)N));
	}

	public static double LastNDPS(this EncounterData encounter, List<CombatantData> SelectiveAllies, int N)
	{
		long num = 0L;
		DateTime dateTime = ActGlobals.oFormActMain.LastKnownTime.Subtract(new TimeSpan(0, 0, N));
		for (int i = 0; i < SelectiveAllies.Count; i++)
		{
			List<MasterSwing> items = SelectiveAllies[i].Items[CombatantData.DamageTypeDataOutgoingDamage].Items["All"].Items;
			for (int j = 0; j < items.Count; j++)
			{
				if (items[j].Time > dateTime)
				{
					num += items[j].Damage.Number;
				}
			}
		}
		return (double)num / ((encounter.Duration.TotalSeconds < 10.0) ? encounter.Duration.TotalSeconds : 10.0);
	}
}
