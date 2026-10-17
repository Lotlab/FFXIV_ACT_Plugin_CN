using System;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public interface IEffectEntryStrategy
{
	Type[] EffectEntryTypes { get; }

	int Order { get; }

	void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry;
}
