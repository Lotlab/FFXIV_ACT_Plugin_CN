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
			string message = _logFormat.FormatProcess($"{_processManager.Current.ClientMode}", _processManager.Current.ProcessId, _processManager.Current.IsAdministrator, _processManager.Current.GameVersion);
			WriteLineToLog(LogMessageType.Process, DateTime.MinValue, message);
		}
		_dataSubscription.OnProcessChanged(_processManager.Current.Process);
	}

	private void OnNetworkSent(string connection, long epoch, byte[] message)
	{
		_dataSubscription.OnNetworkSent(connection, epoch, message);
	}

	private void OnNetworkReceived(string connection, long epoch, byte[] message)
	{
		DateTime timestamp = _MinEpoch.AddMilliseconds(epoch).ToLocalTime();
		if (_settingsManager.DataCollectionSettings.LogAllNetworkData)
		{
			WriteLineToLog(LogMessageType.PacketDump, timestamp, _logFormat.FormatByteArray(message));
		}
		_dataSubscription.OnNetworkReceived(connection, epoch, message);
	}

	private void OnCombatantAdded(object combatant)
	{
		Combatant combatant2 = (Combatant)combatant;
		string worldName = "";
		if ((combatant2.type == 1 || combatant2.type == 2) && combatant2.WorldID != 0)
		{
			worldName = _worldList.GetWorldNameById(combatant2.WorldID);
		}
		DateTime serverTime = _serverTime.ServerTime;
		string message = _logFormat.FormatCombatantMessage(combatant2.ID, combatant2.OwnerID, combatant2.Name, combatant2.Job, combatant2.Level, combatant2.WorldID, worldName, combatant2.BNpcNameID, combatant2.BNpcID, combatant2.CurrentHP, combatant2.MaxHP, combatant2.CurrentMP, combatant2.MaxMP, combatant2.PosX, combatant2.PosY, combatant2.PosZ, combatant2.Heading);
		if (combatant2.type == 1 || combatant2.type == 2)
		{
			WriteLineToLog(LogMessageType.AddCombatant, serverTime, message);
			if (combatant2.NetworkBuffs != null)
			{
				for (int i = 0; i < combatant2.NetworkBuffs.Length; i++)
				{
					NetworkBuff networkBuff = combatant2.NetworkBuffs[i];
					if (networkBuff != null && networkBuff.BuffID > 0)
					{
						Combatant combatantById = _combatantManager.GetCombatantById(networkBuff.ActorID);
						string statusNameByID = _statusList.GetStatusNameByID(networkBuff.BuffID);
						message = _logFormat.FormatNetworkBuffMessage(networkBuff.BuffID, statusNameByID, networkBuff.Duration, networkBuff.ActorID, networkBuff.ActorName, networkBuff.TargetID, networkBuff.TargetName, networkBuff.BuffExtra, combatant2.MaxHP, combatantById?.MaxHP ?? 0);
						WriteLineToLog(LogMessageType.StatusAdd, serverTime, message);
					}
				}
			}
		}
		_dataSubscription.OnCombatantAdded(combatant);
	}

	private void OnCombatantRemoved(object combatant)
	{
		Combatant combatant2 = (Combatant)combatant;
		string worldName = "";
		if ((combatant2.type == 1 || combatant2.type == 2) && combatant2.WorldID != 0)
		{
			worldName = _worldList.GetWorldNameById(combatant2.WorldID);
		}
		DateTime serverTime = _serverTime.ServerTime;
		string message = _logFormat.FormatCombatantMessage(combatant2.ID, combatant2.OwnerID, combatant2.Name, combatant2.Job, combatant2.Level, combatant2.WorldID, worldName, combatant2.BNpcNameID, combatant2.BNpcID, combatant2.CurrentHP, combatant2.MaxHP, combatant2.CurrentMP, combatant2.MaxMP, combatant2.PosX, combatant2.PosY, combatant2.PosZ, combatant2.Heading);
		if (combatant2.type == 1 || combatant2.type == 2)
		{
			WriteLineToLog(LogMessageType.RemoveCombatant, serverTime, message);
		}
		_dataSubscription.OnCombatantRemoved(combatant);
	}

	private void OnPrimaryPlayerChanged()
	{
		Combatant combatantByPointer = _combatantManager.GetCombatantByPointer(_mobArrayProcessor.PrimaryPlayerPointer);
		if (_zoneProcessor.ZoneID != 0 && _mobArrayProcessor.PrimaryPlayerPointer != IntPtr.Zero)
		{
			DateTime serverTime = _serverTime.ServerTime;
			string message = _logFormat.FormatChangePrimaryPlayerMessage(combatantByPointer?.ID, combatantByPointer?.Name);
			WriteLineToLog(LogMessageType.ChangePrimaryPlayer, serverTime, message);
			_dataSubscription.OnPrimaryPlayerChanged();
		}
	}

	private void OnPartyListChanged(ReadOnlyCollection<uint> party, int partySize)
	{
		DateTime serverTime = _serverTime.ServerTime;
		string message = _logFormat.FormatPartyMessage(partySize, party);
		WriteLineToLog(LogMessageType.PartyList, serverTime, message);
		_dataSubscription.OnPartyListChanged(party, partySize);
	}

	private void OnPlayerStatsChanged(object player)
	{
		if (player is Player { JobID: >0u } player2)
		{
			DateTime serverTime = _serverTime.ServerTime;
			string message = _logFormat.FormatPlayerStatsMessage(player2.LocalContentId, player2.JobID, player2.Str, player2.Dex, player2.Vit, player2.Intel, player2.Mnd, player2.Pie, player2.Attack, player2.DirectHit, player2.Crit, player2.AttackMagicPotency, player2.HealMagicPotency, player2.Det, player2.SkillSpeed, player2.SpellSpeed, player2.Tenacity);
			WriteLineToLog(LogMessageType.PlayerStats, serverTime, message);
			_dataSubscription.OnPlayerStatsChanged(player2);
		}
	}

	private void OnZoneChanged(uint ZoneID)
	{
		if (ZoneID != 0)
		{
			DateTime serverTime = _serverTime.ServerTime;
			string territoryNameById = _zoneList.GetTerritoryNameById(ZoneID);
			string message = _logFormat.FormatChangeZoneMessage(ZoneID, territoryNameById);
			WriteLineToLog(LogMessageType.Territory, serverTime, message);
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
			WriteLineToLog(LogMessageType.ChangeMap, serverTime, message);
		}
	}

	private void OnLogLine(DateTime timestamp, uint eventType, uint seconds, string player, string logline)
	{
		if (!_settingsManager.DataCollectionSettings.DisableCombatLog || eventType == 56 || eventType == 57)
		{
			string message = _logFormat.FormatChatMessage(eventType, player, logline);
			WriteLineToLog(LogMessageType.ChatLog, timestamp, message);
			_dataSubscription.OnLogLine(eventType, seconds, player + "\u001f" + logline);
		}
	}

	private void WriteLineToLog(LogMessageType messageType, DateTime timestamp, string message)
	{
		_logOutput.WriteLine(messageType, timestamp, message);
		OnParsedLogLine(messageType, message);
	}

	private void OnParsedLogLine(LogMessageType messageType, string message)
	{
		if (messageType != LogMessageType.Debug && messageType != LogMessageType.Error && messageType != LogMessageType.PacketDump && messageType != LogMessageType.Process && messageType != LogMessageType.Settings && messageType != LogMessageType.Version)
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
