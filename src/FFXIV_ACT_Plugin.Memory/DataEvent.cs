using System;
using System.Collections.ObjectModel;
using FFXIV_ACT_Plugin.Logfile;

namespace FFXIV_ACT_Plugin.Memory;

public class DataEvent
{
	public delegate void NetworkReceivedHandler(string connection, long epoch, byte[] message);

	public delegate void NetworkSentHandler(string connection, long epoch, byte[] message);

	public delegate void CombatantAddedHandler(object Combatant);

	public delegate void CombatantRemovedHandler(object Combatant);

	public delegate void PrimaryPlayerHandler();

	public delegate void ZoneChangedHandler(uint ZoneID);

	public delegate void MapChangedHandler(uint MapID);

	public delegate void PlayerStatsChangedHandler(object playerStats);

	public delegate void PartyListChangedHandler(ReadOnlyCollection<uint> partyList, int partySize);

	public delegate void LogLineHandler(DateTime timestamp, uint eventType, uint seconds, string player, string logline);

	public delegate void ParsedLogLineHandler(LogMessageType messageType, string message);

	public NetworkReceivedHandler NetworkReceived;

	public NetworkSentHandler NetworkSent;

	public CombatantAddedHandler CombatantAdded;

	public CombatantRemovedHandler CombatantRemoved;

	public PrimaryPlayerHandler PrimaryPlayerChanged;

	public ZoneChangedHandler ZoneChanged;

	public MapChangedHandler MapChanged;

	public PlayerStatsChangedHandler PlayerStatsChanged;

	public PartyListChangedHandler PartyListChanged;

	public LogLineHandler LogLine;

	public ParsedLogLineHandler ParsedLogLine;

	public void OnNetworkReceived(string connection, long epoch, byte[] message)
	{
		NetworkReceived?.Invoke(connection, epoch, message);
	}

	public void OnNetworkSent(string connection, long epoch, byte[] message)
	{
		NetworkSent?.Invoke(connection, epoch, message);
	}

	internal void OnCombatantAdded(object Combatant)
	{
		CombatantAdded?.Invoke(Combatant);
	}

	internal void OnCombatantRemoved(object Combatant)
	{
		CombatantRemoved?.Invoke(Combatant);
	}

	internal void OnPrimaryPlayerChanged()
	{
		PrimaryPlayerChanged?.Invoke();
	}

	internal void OnZoneChanged(uint ZoneID)
	{
		ZoneChanged?.Invoke(ZoneID);
	}

	internal void OnMapChanged(uint MapID)
	{
		MapChanged?.Invoke(MapID);
	}

	internal void OnPlayerStatsChanged(object playerStats)
	{
		PlayerStatsChanged?.Invoke(playerStats);
	}

	internal void OnPartyListChanged(ReadOnlyCollection<uint> partyList, int partySize)
	{
		PartyListChanged?.Invoke(partyList, partySize);
	}

	internal void OnLogLine(DateTime timestamp, uint eventType, uint seconds, string player, string logline)
	{
		LogLine?.Invoke(timestamp, eventType, seconds, player, logline);
	}

	public void OnParsedLogLine(LogMessageType messageType, string message)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		ParsedLogLine?.Invoke(messageType, message);
	}
}
