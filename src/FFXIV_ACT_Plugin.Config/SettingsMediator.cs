using System;

namespace FFXIV_ACT_Plugin.Config;

public class SettingsMediator : ISettingsMediator
{
	private DataCollectionSettingsEventArgs _dataCollectionSettings;

	public Action RestartDataCollection { get; set; }

	public Action<DateTime, string> ProcessException { get; set; }

	public ParseSettings ParseSettings { get; set; }

	public DataCollectionSettingsEventArgs DataCollectionSettings
	{
		get
		{
			return _dataCollectionSettings;
		}
		set
		{
			bool num = _dataCollectionSettings != null && (value?.LogFileFolder != _dataCollectionSettings?.LogFileFolder || value?.ProcessID != _dataCollectionSettings?.ProcessID || value?.RegionID != _dataCollectionSettings?.RegionID || value?.NetworkIP != _dataCollectionSettings?.NetworkIP || value?.UseWinPCap != _dataCollectionSettings?.UseWinPCap || value?.UseSocketFilter != _dataCollectionSettings?.UseSocketFilter || value?.UseDeucalion != _dataCollectionSettings?.UseDeucalion);
			_dataCollectionSettings = value;
			if (num)
			{
				RestartDataCollection?.Invoke();
			}
		}
	}

	public void AddExceptionMessage(DateTime timestamp, string message)
	{
		ProcessException?.Invoke(timestamp, message);
	}
}
