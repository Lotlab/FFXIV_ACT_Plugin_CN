using System;
using System.Collections.ObjectModel;
using FFXIV_ACT_Plugin.Common.Models;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory.MemoryProcessors;
using FFXIV_ACT_Plugin.Resource;

namespace FFXIV_ACT_Plugin.Memory;

public class DataEventProcessor : IDisposable
{
	private readonly DataEvent _dataEvent;

	private readonly DataSubscription _dataSubscription;

	private readonly ILogOutput _logOutput;

	private readonly ILogFormat _logFormat;

	private readonly ICombatantManager _combatantManager;

	private readonly IMobArrayProcessor _mobArrayProcessor;

	private readonly IZoneMapProcessor _zoneProcessor;

	private readonly IWorldList _worldList;

	private readonly ITerritoryList _zoneList;

	private readonly IMapList _mapList;

	private readonly ISettingsMediator _settingsManager;

	private readonly IServerTimeProcessor _serverTime;

	private readonly IProcessManager _processManager;

	private readonly IStatusList _statusList;

	private uint _parsedLogLineSequence;

	private static readonly DateTime _MinEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	private bool disposedValue;

	public DataEventProcessor(DataEvent dataEvent, DataSubscription dataSubscription, ILogOutput logOutput, ILogFormat logFormat, ICombatantManager combatantManager, IWorldList worldList, ITerritoryList zoneList, IMapList mapList, IMobArrayProcessor mobArrayProcessor, IZoneMapProcessor zoneProcessor, ISettingsMediator settingsManager, IServerTimeProcessor serverTime, IProcessManager processManager, IStatusList statusList)
	{
		_dataEvent = dataEvent;
		_dataSubscription = dataSubscription;
		_logOutput = logOutput;
		_logFormat = logFormat;
		_combatantManager = combatantManager;
		_worldList = worldList;
		_zoneList = zoneList;
		_mapList = mapList;
		_mobArrayProcessor = mobArrayProcessor;
		_zoneProcessor = zoneProcessor;
		_serverTime = serverTime;
		_settingsManager = settingsManager;
		_processManager = processManager;
		_statusList = statusList;
		DataEvent dataEvent2 = _dataEvent;
		dataEvent2.CombatantAdded = (DataEvent.CombatantAddedHandler)Delegate.Combine(dataEvent2.CombatantAdded, new DataEvent.CombatantAddedHandler(OnCombatantAdded));
		DataEvent dataEvent3 = _dataEvent;
		dataEvent3.CombatantRemoved = (DataEvent.CombatantRemovedHandler)Delegate.Combine(dataEvent3.CombatantRemoved, new DataEvent.CombatantRemovedHandler(OnCombatantRemoved));
		DataEvent dataEvent4 = _dataEvent;
		dataEvent4.PartyListChanged = (DataEvent.PartyListChangedHandler)Delegate.Combine(dataEvent4.PartyListChanged, new DataEvent.PartyListChangedHandler(OnPartyListChanged));
		DataEvent dataEvent5 = _dataEvent;
		dataEvent5.PlayerStatsChanged = (DataEvent.PlayerStatsChangedHandler)Delegate.Combine(dataEvent5.PlayerStatsChanged, new DataEvent.PlayerStatsChangedHandler(OnPlayerStatsChanged));
		DataEvent dataEvent6 = _dataEvent;
		dataEvent6.PrimaryPlayerChanged = (DataEvent.PrimaryPlayerHandler)Delegate.Combine(dataEvent6.PrimaryPlayerChanged, new DataEvent.PrimaryPlayerHandler(OnPrimaryPlayerChanged));
		DataEvent dataEvent7 = _dataEvent;
		dataEvent7.ZoneChanged = (DataEvent.ZoneChangedHandler)Delegate.Combine(dataEvent7.ZoneChanged, new DataEvent.ZoneChangedHandler(OnZoneChanged));
		DataEvent dataEvent8 = _dataEvent;
		dataEvent8.MapChanged = (DataEvent.MapChangedHandler)Delegate.Combine(dataEvent8.MapChanged, new DataEvent.MapChangedHandler(OnMapChanged));
		DataEvent dataEvent9 = _dataEvent;
		dataEvent9.LogLine = (DataEvent.LogLineHandler)Delegate.Combine(dataEvent9.LogLine, new DataEvent.LogLineHandler(OnLogLine));
		DataEvent dataEvent10 = _dataEvent;
		dataEvent10.NetworkReceived = (DataEvent.NetworkReceivedHandler)Delegate.Combine(dataEvent10.NetworkReceived, new DataEvent.NetworkReceivedHandler(OnNetworkReceived));
		DataEvent dataEvent11 = _dataEvent;
		dataEvent11.NetworkSent = (DataEvent.NetworkSentHandler)Delegate.Combine(dataEvent11.NetworkSent, new DataEvent.NetworkSentHandler(OnNetworkSent));
		DataEvent dataEvent12 = _dataEvent;
		dataEvent12.ParsedLogLine = (DataEvent.ParsedLogLineHandler)Delegate.Combine(dataEvent12.ParsedLogLine, new DataEvent.ParsedLogLineHandler(OnParsedLogLine));
		_processManager.ProcessChanged += OnProcessChanged;
	}

	private void OnProcessChanged(object sender, EventArgs args)
	{
		if (_processManager.Current.ClientMode != FFXIVClientMode.Unknown)
		{
			string message = _logFormat.FormatProcess($"{_processManager.Current.ClientMode}", (int?)_processManager.Current.ProcessId, _processManager.Current.IsAdministrator, _processManager.Current.GameVersion);
			WriteLineToLog((LogMessageType)250, DateTime.MinValue, message);
		}
		_dataSubscription.OnProcessChanged(_processManager.Current.Process);
	}

	private void OnNetworkSent(string connection, long epoch, byte[] message)
	{
		_dataSubscription.OnNetworkSent(connection, epoch, message);
	}

	private void OnNetworkReceived(string connection, long epoch, byte[] message)
	{
		DateTime minEpoch = _MinEpoch;
		DateTime timestamp = minEpoch.AddMilliseconds(epoch).ToLocalTime();
		if (_settingsManager.DataCollectionSettings.LogAllNetworkData)
		{
			WriteLineToLog((LogMessageType)252, timestamp, _logFormat.FormatByteArray(message));
		}
		_dataSubscription.OnNetworkReceived(connection, epoch, message);
	}

	private void OnCombatantAdded(object combatant)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected Obj, but got Unknown
		Combatant val = (Combatant)combatant;
		string text = "";
		if ((val.type == 1 || val.type == 2) && val.WorldID != 0)
		{
			text = _worldList.GetWorldNameById(val.WorldID);
		}
		DateTime serverTime = _serverTime.ServerTime;
		string message = _logFormat.FormatCombatantMessage(val.ID, val.OwnerID, val.Name, val.Job, val.Level, val.WorldID, text, val.BNpcNameID, val.BNpcID, val.CurrentHP, val.MaxHP, val.CurrentMP, val.MaxMP, val.PosX, val.PosY, val.PosZ, val.Heading);
		if (val.type == 1 || val.type == 2)
		{
			WriteLineToLog((LogMessageType)3, serverTime, message);
			if (val.NetworkBuffs != null)
			{
				for (int i = 0; i < val.NetworkBuffs.Length; i++)
				{
					NetworkBuff val2 = val.NetworkBuffs[i];
					if (val2 != null && val2.BuffID > 0)
					{
						Combatant combatantById = _combatantManager.GetCombatantById(val2.ActorID);
						string statusNameByID = _statusList.GetStatusNameByID((uint)val2.BuffID);
						message = _logFormat.FormatNetworkBuffMessage(val2.BuffID, statusNameByID, val2.Duration, val2.ActorID, val2.ActorName, val2.TargetID, val2.TargetName, val2.BuffExtra, (uint?)val.MaxHP, (uint?)((combatantById != null) ? combatantById.MaxHP : 0u));
						WriteLineToLog((LogMessageType)26, serverTime, message);
					}
				}
			}
		}
		_dataSubscription.OnCombatantAdded(combatant);
	}

	private void OnCombatantRemoved(object combatant)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected Obj, but got Unknown
		Combatant val = (Combatant)combatant;
		string text = "";
		if ((val.type == 1 || val.type == 2) && val.WorldID != 0)
		{
			text = _worldList.GetWorldNameById(val.WorldID);
		}
		DateTime serverTime = _serverTime.ServerTime;
		string message = _logFormat.FormatCombatantMessage(val.ID, val.OwnerID, val.Name, val.Job, val.Level, val.WorldID, text, val.BNpcNameID, val.BNpcID, val.CurrentHP, val.MaxHP, val.CurrentMP, val.MaxMP, val.PosX, val.PosY, val.PosZ, val.Heading);
		if (val.type == 1 || val.type == 2)
		{
			WriteLineToLog((LogMessageType)4, serverTime, message);
		}
		_dataSubscription.OnCombatantRemoved(combatant);
	}

	private void OnPrimaryPlayerChanged()
	{
		Combatant combatantByPointer = _combatantManager.GetCombatantByPointer(_mobArrayProcessor.PrimaryPlayerPointer);
		if (_zoneProcessor.ZoneID != 0 && _mobArrayProcessor.PrimaryPlayerPointer != IntPtr.Zero)
		{
			DateTime serverTime = _serverTime.ServerTime;
			string message = _logFormat.FormatChangePrimaryPlayerMessage((combatantByPointer != null) ? new uint?(combatantByPointer.ID) : ((uint?)null), (combatantByPointer != null) ? combatantByPointer.Name : null);
			WriteLineToLog((LogMessageType)2, serverTime, message);
			_dataSubscription.OnPrimaryPlayerChanged();
		}
	}

	private void OnPartyListChanged(ReadOnlyCollection<uint> party, int partySize)
	{
		DateTime serverTime = _serverTime.ServerTime;
		string message = _logFormat.FormatPartyMessage(partySize, party);
		WriteLineToLog((LogMessageType)11, serverTime, message);
		_dataSubscription.OnPartyListChanged(party, partySize);
	}

	private void OnPlayerStatsChanged(object player)
	{
		Player val = (Player)((player is Player) ? player : null);
		if (val != null && val.JobID != 0)
		{
			DateTime serverTime = _serverTime.ServerTime;
			string message = _logFormat.FormatPlayerStatsMessage(val.LocalContentId, val.JobID, val.Str, val.Dex, val.Vit, val.Intel, val.Mnd, val.Pie, val.Attack, val.DirectHit, val.Crit, val.AttackMagicPotency, val.HealMagicPotency, val.Det, val.SkillSpeed, val.SpellSpeed, val.Tenacity);
			WriteLineToLog((LogMessageType)12, serverTime, message);
			_dataSubscription.OnPlayerStatsChanged(val);
		}
	}

	private void OnZoneChanged(uint ZoneID)
	{
		if (ZoneID != 0)
		{
			DateTime serverTime = _serverTime.ServerTime;
			string territoryNameById = _zoneList.GetTerritoryNameById(ZoneID);
			string message = _logFormat.FormatChangeZoneMessage(ZoneID, territoryNameById);
			WriteLineToLog((LogMessageType)1, serverTime, message);
			_dataSubscription.OnZoneChanged(ZoneID, territoryNameById);
		}
	}

	private void OnMapChanged(uint MapID)
	{
		if (MapID != 0)
		{
			DateTime serverTime = _serverTime.ServerTime;
			Map mapById = _mapList.GetMapById(MapID);
			string message = _logFormat.FormatChangeMapMessage(MapID, mapById.Region, mapById.Name, mapById.SubName);
			WriteLineToLog((LogMessageType)40, serverTime, message);
		}
	}

	private void OnLogLine(DateTime timestamp, uint eventType, uint seconds, string player, string logline)
	{
		if (!_settingsManager.DataCollectionSettings.DisableCombatLog || eventType == 56 || eventType == 57)
		{
			string message = _logFormat.FormatChatMessage(eventType, player, logline);
			WriteLineToLog((LogMessageType)0, timestamp, message);
			_dataSubscription.OnLogLine(eventType, seconds, player + "\u001f" + logline);
		}
	}

	private void WriteLineToLog(LogMessageType messageType, DateTime timestamp, string message)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		_logOutput.WriteLine(messageType, timestamp, message);
		OnParsedLogLine(messageType, message);
	}

	private void OnParsedLogLine(LogMessageType messageType, string message)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Invalid comparison between Unknown and I4
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Invalid comparison between Unknown and I4
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected I4, but got Unknown
		if ((int)messageType != 251 && (int)messageType != 254 && (int)messageType != 252 && (int)messageType != 250 && (int)messageType != 249 && (int)messageType != 253)
		{
			_parsedLogLineSequence++;
			_dataSubscription.OnParsedLogLine(_parsedLogLineSequence, (int)messageType, message);
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				DataEvent dataEvent = _dataEvent;
				dataEvent.CombatantAdded = (DataEvent.CombatantAddedHandler)Delegate.Remove(dataEvent.CombatantAdded, new DataEvent.CombatantAddedHandler(OnCombatantAdded));
				DataEvent dataEvent2 = _dataEvent;
				dataEvent2.CombatantRemoved = (DataEvent.CombatantRemovedHandler)Delegate.Remove(dataEvent2.CombatantRemoved, new DataEvent.CombatantRemovedHandler(OnCombatantRemoved));
				DataEvent dataEvent3 = _dataEvent;
				dataEvent3.PartyListChanged = (DataEvent.PartyListChangedHandler)Delegate.Remove(dataEvent3.PartyListChanged, new DataEvent.PartyListChangedHandler(OnPartyListChanged));
				DataEvent dataEvent4 = _dataEvent;
				dataEvent4.PlayerStatsChanged = (DataEvent.PlayerStatsChangedHandler)Delegate.Remove(dataEvent4.PlayerStatsChanged, new DataEvent.PlayerStatsChangedHandler(OnPlayerStatsChanged));
				DataEvent dataEvent5 = _dataEvent;
				dataEvent5.PrimaryPlayerChanged = (DataEvent.PrimaryPlayerHandler)Delegate.Remove(dataEvent5.PrimaryPlayerChanged, new DataEvent.PrimaryPlayerHandler(OnPrimaryPlayerChanged));
				DataEvent dataEvent6 = _dataEvent;
				dataEvent6.ZoneChanged = (DataEvent.ZoneChangedHandler)Delegate.Remove(dataEvent6.ZoneChanged, new DataEvent.ZoneChangedHandler(OnZoneChanged));
				DataEvent dataEvent7 = _dataEvent;
				dataEvent7.MapChanged = (DataEvent.MapChangedHandler)Delegate.Remove(dataEvent7.MapChanged, new DataEvent.MapChangedHandler(OnMapChanged));
				DataEvent dataEvent8 = _dataEvent;
				dataEvent8.LogLine = (DataEvent.LogLineHandler)Delegate.Remove(dataEvent8.LogLine, new DataEvent.LogLineHandler(OnLogLine));
				DataEvent dataEvent9 = _dataEvent;
				dataEvent9.NetworkReceived = (DataEvent.NetworkReceivedHandler)Delegate.Remove(dataEvent9.NetworkReceived, new DataEvent.NetworkReceivedHandler(OnNetworkReceived));
				DataEvent dataEvent10 = _dataEvent;
				dataEvent10.NetworkSent = (DataEvent.NetworkSentHandler)Delegate.Remove(dataEvent10.NetworkSent, new DataEvent.NetworkSentHandler(OnNetworkSent));
				DataEvent dataEvent11 = _dataEvent;
				dataEvent11.ParsedLogLine = (DataEvent.ParsedLogLineHandler)Delegate.Remove(dataEvent11.ParsedLogLine, new DataEvent.ParsedLogLineHandler(OnParsedLogLine));
				_processManager.ProcessChanged -= OnProcessChanged;
			}
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
