using System;

namespace FFXIV_ACT_Plugin.Common;

public class BenchmarkStats
{
	public string BenchmarkType { get; set; }

	public TimeSpan LastMeasuredTime { get; set; }

	public TimeSpan AverageTime { get; set; }

	public TimeSpan MaxTime { get; set; }

	public TimeSpan TotalTime { get; set; }

	public uint Count { get; set; }

	public Tuple<DateTime, double>[] History { get; } = new Tuple<DateTime, double>[1000];
}
