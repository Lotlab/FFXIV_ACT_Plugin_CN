#define TRACE
using System;
using System.Diagnostics;

namespace FFXIV_ACT_Plugin.Memory.MemoryReader;

public class ReadMemory : IReadMemory
{
	private readonly IProcessManager _processManager;

	public ReadMemory(IProcessManager processManager)
	{
		_processManager = processManager;
	}

	public IntPtr ReadPointer(IntPtr Offset)
	{
		if (_processManager.Current.Handle == IntPtr.Zero)
		{
			return IntPtr.Zero;
		}
		int num = 8;
		byte[] array = new byte[num];
		IntPtr lpNumberOfBytesRead = IntPtr.Zero;
		if (!NativeMethods.ReadProcessMemory(_processManager.Current.Handle, Offset, array, new IntPtr(num), ref lpNumberOfBytesRead))
		{
			return IntPtr.Zero;
		}
		return new IntPtr(BitConverter.ToInt64(array, 0));
	}

	public int ReadInt32(IntPtr Offset)
	{
		if (_processManager.Current.Handle == IntPtr.Zero)
		{
			return 0;
		}
		byte[] array = new byte[4];
		IntPtr lpNumberOfBytesRead = IntPtr.Zero;
		if (!NativeMethods.ReadProcessMemory(_processManager.Current.Handle, Offset, array, new IntPtr(4), ref lpNumberOfBytesRead))
		{
			return 0;
		}
		return BitConverter.ToInt32(array, 0);
	}

	public ulong ReadUInt64(IntPtr Offset)
	{
		if (_processManager.Current.Handle == IntPtr.Zero)
		{
			return 0uL;
		}
		byte[] array = new byte[8];
		IntPtr lpNumberOfBytesRead = IntPtr.Zero;
		if (!NativeMethods.ReadProcessMemory(_processManager.Current.Handle, Offset, array, new IntPtr(8), ref lpNumberOfBytesRead))
		{
			return 0uL;
		}
		return BitConverter.ToUInt64(array, 0);
	}

	public uint ReadUInt32(IntPtr Offset)
	{
		if (_processManager.Current.Handle == IntPtr.Zero)
		{
			return 0u;
		}
		byte[] array = new byte[4];
		IntPtr lpNumberOfBytesRead = IntPtr.Zero;
		if (!NativeMethods.ReadProcessMemory(_processManager.Current.Handle, Offset, array, new IntPtr(4), ref lpNumberOfBytesRead))
		{
			return 0u;
		}
		return BitConverter.ToUInt32(array, 0);
	}

	public void ReadBuffer(IntPtr Offset, byte[] buffer, int Length)
	{
		if (_processManager.Current.Handle == IntPtr.Zero)
		{
			return;
		}
		IntPtr lpNumberOfBytesRead = IntPtr.Zero;
		if (Length > 0 && Length <= 100000 && !(Offset == IntPtr.Zero) && buffer != null)
		{
			if (!NativeMethods.ReadProcessMemory(_processManager.Current.Handle, Offset, buffer, new IntPtr(Length), ref lpNumberOfBytesRead))
			{
				Trace.WriteLine($"IReadMemory.ReadBuffer() - ReadProcessMemory failed. Offset: {Offset.ToInt64():X4}, Length:{Length:X4}", "FFXIV_ACT_Plugin");
			}
			else if (lpNumberOfBytesRead != (IntPtr)Length)
			{
				Trace.WriteLine($"IReadMemory.ReadBuffer() - failed.  Read: {lpNumberOfBytesRead:X2}, Expected: {Length:X2}", "FFXIV_ACT_Plugin");
			}
		}
	}
}
