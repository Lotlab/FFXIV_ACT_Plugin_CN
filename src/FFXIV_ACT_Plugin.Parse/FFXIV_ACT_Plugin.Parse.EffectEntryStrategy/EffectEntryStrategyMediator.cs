using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FFXIV_ACT_Plugin.Common;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public class EffectEntryStrategyMediator : IEffectEntryStrategyMediator
{
	private readonly IServiceProvider _serviceProvider;

	private readonly IBenchmarkRepository _benchmarkRepository;

	private readonly Dictionary<Type, List<IEffectEntryStrategy>> _strategies = new Dictionary<Type, List<IEffectEntryStrategy>>();

	public EffectEntryStrategyMediator(IServiceProvider serviceProvider, IBenchmarkRepository benchmarkRepository)
	{
		_serviceProvider = serviceProvider;
		_benchmarkRepository = benchmarkRepository;
		LoadStrategies();
	}

	private void LoadStrategies()
	{
		foreach (Type item in Assembly.GetExecutingAssembly().ExportedTypes.Where((Type x) => Enumerable.Contains(x.GetInterfaces(), typeof(IEffectEntryStrategy))))
		{
			IEffectEntryStrategy effectEntryStrategy = _serviceProvider.GetService(item) as IEffectEntryStrategy;
			Type[] effectEntryTypes = effectEntryStrategy.EffectEntryTypes;
			foreach (Type key in effectEntryTypes)
			{
				if (!_strategies.ContainsKey(key))
				{
					_strategies.Add(key, new List<IEffectEntryStrategy>());
				}
				_strategies[key].Add(effectEntryStrategy);
			}
		}
		foreach (Type item2 in _strategies.Keys.ToList())
		{
			_strategies[item2] = _strategies[item2].OrderBy((IEffectEntryStrategy x) => x.Order).ToList();
		}
	}

	public void ApplyStrategies<T>(EnvironmentState state, ActionEffect effect, T entry) where T : EffectEntry
	{
		if (!_strategies.ContainsKey(typeof(T)))
		{
			return;
		}
		foreach (IEffectEntryStrategy strategy in _strategies[typeof(T)])
		{
			_benchmarkRepository.Measure("ParseStrategy" + strategy.GetType().Name, (Action)(() =>
			{
				strategy.Apply(state, effect, entry);
			}));
		}
	}
}
