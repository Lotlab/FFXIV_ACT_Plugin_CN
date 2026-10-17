using System;

namespace FFXIV_ACT_Plugin.Config;

public class DataCollectionSettingsEventArgs : EventArgs
{
	public int ProcessID;

	public string LogFileFolder;

	public bool LogAllNetworkData;

	public bool DisableCombatLog;

	public Region RegionID;

	/// <summary>
	/// Local address to capture on, empty to capture on every interface.
	/// </summary>
	public string NetworkIP;

	/// <summary>
	/// Capture with Npcap instead of a raw socket.  Requires the Npcap driver.
	/// </summary>
	public bool UseWinPCap;

	/// <summary>
	/// Only capture traffic to the game server rather than every remote address.
	/// </summary>
	public bool UseSocketFilter;

	/// <summary>
	/// Capture by injecting deucalion into the game process instead of sniffing
	/// the network.  Deucalion delivers already deobfuscated packets.
	/// </summary>
	public bool UseDeucalion;
}
