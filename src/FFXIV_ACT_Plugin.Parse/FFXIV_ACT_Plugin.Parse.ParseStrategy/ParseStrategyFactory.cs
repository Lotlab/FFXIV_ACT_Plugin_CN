using System;
using FFXIV_ACT_Plugin.Logfile;

namespace FFXIV_ACT_Plugin.Parse.ParseStrategy;

public class ParseStrategyFactory
{
	private readonly IServiceProvider _serviceProvider;

	public ParseStrategyFactory(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public IParseStrategy GetParseStrategy(LogMessageType messageType)
	{
		switch (messageType)
		{
		case LogMessageType.Territory:
			return _serviceProvider.GetService(typeof(ParseStrategyZoneId)) as IParseStrategy;
		case LogMessageType.ChangePrimaryPlayer:
			return _serviceProvider.GetService(typeof(ParseStrategyCurrentPlayer)) as IParseStrategy;
		case LogMessageType.AddCombatant:
			return _serviceProvider.GetService(typeof(ParseStrategyAddCombatant)) as IParseStrategy;
		case LogMessageType.RemoveCombatant:
			return _serviceProvider.GetService(typeof(ParseStrategyRemoveCombatant)) as IParseStrategy;
		case LogMessageType.ActionEffect:
		case LogMessageType.AOEActionEffect:
			return _serviceProvider.GetService(typeof(ParseStrategyActionEffect)) as IParseStrategy;
		case LogMessageType.Death:
			return _serviceProvider.GetService(typeof(ParseStrategyDeath)) as IParseStrategy;
		case LogMessageType.StatusAdd:
			return _serviceProvider.GetService(typeof(ParseStrategyAddStatus)) as IParseStrategy;
		case LogMessageType.StatusRemove:
			return _serviceProvider.GetService(typeof(ParseStrategyRemoveStatus)) as IParseStrategy;
		case LogMessageType.DoTHoT:
			return _serviceProvider.GetService(typeof(ParseStrategyDoT)) as IParseStrategy;
		case LogMessageType.ChatLog:
			return _serviceProvider.GetService(typeof(ParseStrategyLogLine)) as IParseStrategy;
		case LogMessageType.CancelAction:
			return _serviceProvider.GetService(typeof(ParseStrategyCancelAbility)) as IParseStrategy;
		case LogMessageType.PartyList:
			return _serviceProvider.GetService(typeof(ParseStrategyParty)) as IParseStrategy;
		case LogMessageType.StartsCasting:
			return _serviceProvider.GetService(typeof(ParseStrategyStartsCasting)) as IParseStrategy;
		case LogMessageType.PlayerStats:
		case LogMessageType.TargetIcon:
		case LogMessageType.WaymarkMarker:
		case LogMessageType.SignMarker:
		case LogMessageType.Gauge:
		case LogMessageType.World:
		case LogMessageType.Director:
		case LogMessageType.NameToggle:
		case LogMessageType.Tether:
		case LogMessageType.LimitBreak:
		case LogMessageType.EffectResult:
		case LogMessageType.StatusList:
		case LogMessageType.UpdateHp:
		case LogMessageType.ChangeMap:
		case LogMessageType.SystemLogMessage:
		case LogMessageType.StatusList3:
		case LogMessageType.StatusListForay3:
		case LogMessageType.PacketDump:
			return _serviceProvider.GetService(typeof(ParseStrategyDoNothing)) as IParseStrategy;
		case LogMessageType.Settings:
		case LogMessageType.Process:
		case LogMessageType.Debug:
		case LogMessageType.Version:
		case LogMessageType.Error:
			return _serviceProvider.GetService(typeof(ParseStrategyDebugMessage)) as IParseStrategy;
		default:
			if (messageType >= (LogMessageType)256)
			{
				return _serviceProvider.GetService(typeof(ParseStrategyDoNothing)) as IParseStrategy;
			}
			throw new NotImplementedException($"Unsupported LogMessageType {messageType}.");
		}
	}
}
