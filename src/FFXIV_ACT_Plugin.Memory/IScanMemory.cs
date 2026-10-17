using System.Threading;

namespace FFXIV_ACT_Plugin.Memory;

public interface IScanMemory
{
	Thread ScanThread { get; }
}
