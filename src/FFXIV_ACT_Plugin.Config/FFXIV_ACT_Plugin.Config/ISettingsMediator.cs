using System;

namespace FFXIV_ACT_Plugin.Config;

public interface ISettingsMediator
{
	ParseSettings ParseSettings { get; set; }

	DataCollectionSettingsEventArgs DataCollectionSettings { get; set; }

	Action<DateTime, string> ProcessException { get; set; }

	Action RestartDataCollection { get; set; }

	void AddExceptionMessage(DateTime timestamp, string message);
}
