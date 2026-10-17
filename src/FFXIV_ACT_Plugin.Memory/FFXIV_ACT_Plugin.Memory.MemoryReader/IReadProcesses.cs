using System;
using System.Collections.Generic;

namespace FFXIV_ACT_Plugin.Memory.MemoryReader;

public interface IReadProcesses
{
	List<int> Read64(bool verifyMainModule);

	bool Verify(IntPtr processHandle);
}
