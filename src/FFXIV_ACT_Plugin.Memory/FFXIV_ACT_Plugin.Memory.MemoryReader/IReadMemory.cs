using System;

namespace FFXIV_ACT_Plugin.Memory.MemoryReader;

public interface IReadMemory
{
	IntPtr ReadPointer(IntPtr Offset);

	int ReadInt32(IntPtr Offset);

	ulong ReadUInt64(IntPtr Offset);

	uint ReadUInt32(IntPtr Offset);

	void ReadBuffer(IntPtr Offset, byte[] buffer, int Length);
}
