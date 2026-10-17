using System;

namespace FFXIV_ACT_Plugin.Memory;

public interface IProcessManager : IDisposable
{
	CurrentProcess Current { get; }

	event EventHandler ProcessChanged;

	bool Verify();
}
