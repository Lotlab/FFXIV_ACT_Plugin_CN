using System;
using System.Runtime.InteropServices;

namespace FFXIV_ACT_Plugin.Memory.MemoryReader;

public class ReadCombatant : IReadCombatant, IDisposable
{
	private readonly IReadMemory _readMemory;

	private readonly byte[] _buffer;

	private readonly GCHandle _bufferHandle;

	private bool disposedValue;

	public ReadCombatant(IReadMemory readMemory)
	{
		_readMemory = readMemory;
		_buffer = new byte[11520];
		_bufferHandle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
	}

	public unsafe IntPtr Read(IntPtr address)
	{
		if (address == IntPtr.Zero)
		{
			return IntPtr.Zero;
		}
		_readMemory.ReadBuffer(address, _buffer, _buffer.Length);
		fixed (byte* buffer = _buffer)
		{
			return (IntPtr)buffer;
		}
	}

	public unsafe IntPtr ReadHeader(IntPtr address)
	{
		if (address == IntPtr.Zero)
		{
			return IntPtr.Zero;
		}
		_readMemory.ReadBuffer(address, _buffer, 256);
		fixed (byte* buffer = _buffer)
		{
			return (IntPtr)buffer;
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing && _bufferHandle.IsAllocated)
			{
				_bufferHandle.Free();
			}
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
