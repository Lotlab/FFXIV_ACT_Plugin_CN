using System;

namespace FFXIV_ACT_Plugin.Memory.MemoryReader;

public interface IReadLog : IDisposable
{
	IntPtr ReadHeader();

	IntPtr ReadLineLengthArray(IntPtr startPointer, int length);

	IntPtr ReadMessageArray(IntPtr startPointer, int length);
}
