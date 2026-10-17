using System;

namespace FFXIV_ACT_Plugin.Common;

public interface IBenchmarkRepository
{
	void Measure(string benchmarkType, Action action);

	T Measure<T>(string benchmarkType, Func<T> action);

	BenchmarkStats GetBenchmarkStats(string benchmarkType);

	BenchmarkStats[] GetAllBenchmarkStats();

	void ResetBenchmarkStats(string benchmarkType);
}
