using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using FFXIV_ACT_Plugin.Common;
using FFXIV_ACT_Plugin.Config;

namespace FFXIV_ACT_Plugin.Memory;

public class BenchmarkRepository : IBenchmarkRepository
{
	private readonly ConcurrentDictionary<string, BenchmarkStats> _benchmarkStats = new ConcurrentDictionary<string, BenchmarkStats>();

	private readonly ISettingsMediator _settingsMediator;

	public BenchmarkRepository(ISettingsMediator settingsMediator)
	{
		_settingsMediator = settingsMediator;
	}

	private void RegisterBenchmarkType(string benchmarkType)
	{
		_benchmarkStats.TryAdd(benchmarkType, new BenchmarkStats
		{
			BenchmarkType = benchmarkType
		});
	}

	private void SetBenchmarkStats(string benchmarkType, TimeSpan time)
	{
		if (_benchmarkStats.TryGetValue(benchmarkType, out var value))
		{
			long ticks = (value.AverageTime.Ticks * value.Count + time.Ticks) / (value.Count + 1);
			value.LastMeasuredTime = time;
			value.AverageTime = new TimeSpan(ticks);
			value.TotalTime = value.TotalTime.Add(time);
			if (time > value.MaxTime)
			{
				value.MaxTime = time;
			}
			value.Count++;
			value.History[value.Count % value.History.Length] = new Tuple<DateTime, double>(DateTime.Now, time.TotalMilliseconds);
		}
	}

	public BenchmarkStats GetBenchmarkStats(string benchmarkType)
	{
		if (!_benchmarkStats.TryGetValue(benchmarkType, out var value))
		{
			return null;
		}
		return value;
	}

	public BenchmarkStats[] GetAllBenchmarkStats()
	{
		return _benchmarkStats.Values.ToArray();
	}

	public void ResetBenchmarkStats(string benchmarkType)
	{
		bool flag = string.IsNullOrWhiteSpace(benchmarkType);
		foreach (string key in _benchmarkStats.Keys)
		{
			if ((flag || key.Equals(benchmarkType, StringComparison.OrdinalIgnoreCase)) && _benchmarkStats.TryGetValue(key, out var value))
			{
				value.Count = 0u;
				value.TotalTime = TimeSpan.Zero;
				value.AverageTime = TimeSpan.Zero;
				value.LastMeasuredTime = TimeSpan.Zero;
				value.MaxTime = TimeSpan.Zero;
				for (int i = 0; i < value.History.Length; i++)
				{
					value.History[i] = null;
				}
			}
		}
	}

	public void Measure(string benchmarkType, Action action)
	{
		if (_settingsMediator.ParseSettings.EnableBenchmarks)
		{
			if (!_benchmarkStats.ContainsKey(benchmarkType))
			{
				RegisterBenchmarkType(benchmarkType);
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			action();
			stopwatch.Stop();
			SetBenchmarkStats(benchmarkType, stopwatch.Elapsed);
		}
		else
		{
			action();
		}
	}

	public T Measure<T>(string benchmarkType, Func<T> action)
	{
		if (_settingsMediator.ParseSettings.EnableBenchmarks)
		{
			if (!_benchmarkStats.ContainsKey(benchmarkType))
			{
				RegisterBenchmarkType(benchmarkType);
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			T result = action();
			stopwatch.Stop();
			SetBenchmarkStats(benchmarkType, stopwatch.Elapsed);
			return result;
		}
		return action();
	}
}
