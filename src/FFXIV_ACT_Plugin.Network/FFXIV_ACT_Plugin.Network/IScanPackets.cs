using System;

namespace FFXIV_ACT_Plugin.Network;

public interface IScanPackets : IDisposable
{
	void StartScan();

	void StopScan();

	int IsNetworkDataValid();
}
