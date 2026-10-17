namespace FFXIV_ACT_Plugin.Common;

public interface IDataSubscription
{
	event NetworkReceivedDelegate NetworkReceived;

	event NetworkSentDelegate NetworkSent;

	event CombatantAddedDelegate CombatantAdded;

	event CombatantRemovedDelegate CombatantRemoved;

	event PrimaryPlayerDelegate PrimaryPlayerChanged;

	event ZoneChangedDelegate ZoneChanged;

	event PlayerStatsChangedDelegate PlayerStatsChanged;

	event PartyListChangedDelegate PartyListChanged;

	event LogLineDelegate LogLine;

	event ParsedLogLineDelegate ParsedLogLine;

	event ProcessChangedDelegate ProcessChanged;
}
