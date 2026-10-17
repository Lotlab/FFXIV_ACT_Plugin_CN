using System;

namespace FFXIV_ACT_Plugin.Config;

public class DataCollectionSettingsEventArgs : EventArgs
{
	public int ProcessID;

	public string LogFileFolder;

	public bool LogAllNetworkData;

	public bool DisableCombatLog;

	public Region RegionID;
}
