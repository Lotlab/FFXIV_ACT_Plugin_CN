using System;
using System.Collections.Generic;
using System.Linq;
using FFXIV_ACT_Plugin.Common;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Network.PacketHandlers;
using Machina.FFXIV;
using Machina.FFXIV.Headers;
using Machina.Infrastructure;

namespace FFXIV_ACT_Plugin.Network;

public class PacketHandlerMediator
{
	private readonly Dictionary<GameRegion, Dictionary<uint, IPacketHandler>> _packetHandlers = new Dictionary<GameRegion, Dictionary<uint, IPacketHandler>>();

	private readonly ISettingsMediator _settingsMediator;

	private readonly IBenchmarkRepository _benchmarkRepository;

	private readonly IServiceProvider _serviceProvider;

	public PacketHandlerMediator(ISettingsMediator settingsMediator, IBenchmarkRepository benchmarkRepository, IServiceProvider serviceProvider)
	{
		_settingsMediator = settingsMediator;
		_benchmarkRepository = benchmarkRepository;
		_serviceProvider = serviceProvider;
	}

	public void LoadPacketHandlers()
	{
		_packetHandlers.Clear();
		foreach (GameRegion value in Enum.GetValues(typeof(GameRegion)))
		{
			_packetHandlers.Add(value, new Dictionary<uint, IPacketHandler>());
		}
		foreach (Type item in typeof(IPacketHandler).Assembly.ExportedTypes.Where((Type x) => x.GetInterfaces().Contains(typeof(IPacketHandler))))
		{
			IPacketHandler packetHandler = (IPacketHandler)_serviceProvider.GetService(item);
			if (packetHandler.Opcode != 0)
			{
				GameRegion key2 = GameRegion.Global;
				if (item.Namespace.ToLowerInvariant().EndsWith("tc"))
				{
					key2 = GameRegion.TraditionalChinese;
				}
				else if (item.Namespace.ToLowerInvariant().EndsWith("kr"))
				{
					key2 = GameRegion.Korean;
				}
				else if (item.Namespace.ToLowerInvariant().EndsWith("cn"))
				{
					key2 = GameRegion.Chinese;
				}
				_packetHandlers[key2].Add(packetHandler.Opcode, packetHandler);
			}
		}
	}

	public unsafe bool ProcessPacket(long epoch, byte[] message)
	{
		if (message.Length < sizeof(Server_MessageHeader))
		{
			return false;
		}
		DateTime packetDate = ConversionUtility.EpochToDateTime(epoch).ToLocalTime();
		ushort messageType;
		fixed (byte* ptr = message)
		{
			messageType = ((Server_MessageHeader*)ptr)->MessageType;
		}
		IPacketHandler packetHandler;
		if (_packetHandlers[(GameRegion)_settingsMediator.DataCollectionSettings.RegionID].ContainsKey(messageType))
		{
			packetHandler = _packetHandlers[(GameRegion)_settingsMediator.DataCollectionSettings.RegionID][messageType];
		}
		else
		{
			if (!_packetHandlers[GameRegion.Global].ContainsKey(messageType))
			{
				return false;
			}
			packetHandler = _packetHandlers[GameRegion.Global][messageType];
		}
		if (message.Length < packetHandler.PacketLength)
		{
			throw new PacketLengthException($"ProcessPacket: message length {message.Length} is less than {packetHandler.PacketLength} for handler {packetHandler.GetType().Name}");
		}
		_benchmarkRepository.Measure("Network" + packetHandler.GetType().Name, () =>
		{
			packetHandler.Process(message, packetDate);
		});
		return true;
	}
}
