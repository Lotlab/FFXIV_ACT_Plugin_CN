using System;
using System.Threading;

namespace FFXIV_ACT_Plugin.Logfile;

public interface ILogOutput
{
	Thread ScanThread { get; }

	void WriteLine(LogMessageType messageType, DateTime ServerDate, string line);
}
