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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected I4, but got Unknown
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Invalid comparison between Unknown and I4
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected I4, but got Unknown
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		switch ((int)messageType)
		{
		default:
			switch ((int)messageType - 249)
			{
			case 3:
				break;
			case 0:
			case 1:
			case 2:
			case 4:
			case 5:
				return _serviceProvider.GetService(typeof(ParseStrategyDebugMessage)) as IParseStrategy;
			default:
				goto end_IL_0001;
			}
			goto case 12;
		case 1:
			return _serviceProvider.GetService(typeof(ParseStrategyZoneId)) as IParseStrategy;
		case 2:
			return _serviceProvider.GetService(typeof(ParseStrategyCurrentPlayer)) as IParseStrategy;
		case 3:
			return _serviceProvider.GetService(typeof(ParseStrategyAddCombatant)) as IParseStrategy;
		case 4:
			return _serviceProvider.GetService(typeof(ParseStrategyRemoveCombatant)) as IParseStrategy;
		case 21:
		case 22:
			return _serviceProvider.GetService(typeof(ParseStrategyActionEffect)) as IParseStrategy;
		case 25:
			return _serviceProvider.GetService(typeof(ParseStrategyDeath)) as IParseStrategy;
		case 26:
			return _serviceProvider.GetService(typeof(ParseStrategyAddStatus)) as IParseStrategy;
		case 30:
			return _serviceProvider.GetService(typeof(ParseStrategyRemoveStatus)) as IParseStrategy;
		case 24:
			return _serviceProvider.GetService(typeof(ParseStrategyDoT)) as IParseStrategy;
		case 0:
			return _serviceProvider.GetService(typeof(ParseStrategyLogLine)) as IParseStrategy;
		case 23:
			return _serviceProvider.GetService(typeof(ParseStrategyCancelAbility)) as IParseStrategy;
		case 11:
			return _serviceProvider.GetService(typeof(ParseStrategyParty)) as IParseStrategy;
		case 20:
			return _serviceProvider.GetService(typeof(ParseStrategyStartsCasting)) as IParseStrategy;
		case 12:
		case 27:
		case 28:
		case 29:
		case 31:
		case 32:
		case 33:
		case 34:
		case 35:
		case 36:
		case 37:
		case 38:
		case 39:
		case 40:
		case 41:
		case 42:
		case 43:
			return _serviceProvider.GetService(typeof(ParseStrategyDoNothing)) as IParseStrategy;
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
		case 10:
		case 13:
		case 14:
		case 15:
		case 16:
		case 17:
		case 18:
		case 19:
			break;
			end_IL_0001:
			break;
		}
		if ((int)messageType >= 256)
		{
			return _serviceProvider.GetService(typeof(ParseStrategyDoNothing)) as IParseStrategy;
		}
		throw new NotImplementedException($"Unsupported LogMessageType {messageType}.");
	}
}
