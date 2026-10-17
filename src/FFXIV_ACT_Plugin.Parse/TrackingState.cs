using System.Collections.Generic;
using System.Linq;

namespace FFXIV_ACT_Plugin.Parse;

public class TrackingState
{
	public double CriticalDamageRate { get; set; }

	public int CriticalDamageRateSwings { get; set; }

	public double DirectHitRate { get; set; }

	public int DirectHitRateSwings { get; set; }

	public int DamagePotencyHistoryIndex { get; set; }

	public double?[] DamagePotencyHistory { get; } = new double?[1000];

	public int HealPotencyHistoryIndex { get; set; }

	public double?[] HealPotencyHistory { get; } = new double?[1000];

	public double? DamagePotency => Median((from x in DamagePotencyHistory
		where x.HasValue && x > 0.0
		select x.Value into x
		orderby x
		select x).ToList());

	public double? HealPotency => Median((from x in HealPotencyHistory
		where x.HasValue && x > 0.0
		select x.Value into x
		orderby x
		select x).ToList());

	public void Clear()
	{
		CriticalDamageRate = 0.0;
		CriticalDamageRateSwings = 0;
		DirectHitRate = 0.0;
		DirectHitRateSwings = 0;
	}

	private static double? Median(IList<double> list)
	{
		if (list == null || !list.Any())
		{
			return null;
		}
		int count = list.Count;
		int num = list.Count / 2;
		return (count % 2 != 0) ? list.ElementAt(num) : ((list.ElementAt(num) + list.ElementAt(num - 1)) / 2.0);
	}
}
