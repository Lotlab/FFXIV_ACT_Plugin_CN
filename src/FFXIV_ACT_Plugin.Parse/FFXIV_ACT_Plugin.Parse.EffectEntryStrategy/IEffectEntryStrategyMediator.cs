namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public interface IEffectEntryStrategyMediator
{
	void ApplyStrategies<T>(EnvironmentState state, ActionEffect effect, T entry) where T : EffectEntry;
}
