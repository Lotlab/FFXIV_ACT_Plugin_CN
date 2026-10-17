using System;
using System.Runtime.InteropServices;

namespace FFXIV_ACT_Plugin.Memory.MemoryReader;

public class ReadMobArray : IReadMobArray, IDisposable
{
	private readonly byte[] _buffer;

	private readonly GCHandle _bufferHandle;

	private readonly ISignatureManager _signatureManager;

	private readonly IReadMemory _readMemory;

	private bool disposedValue;

	public ReadMobArray(ISignatureManager signatureManager, IReadMemory readMemory)
	{
		_signatureManager = signatureManager;
		_readMemory = readMemory;
		_buffer = new byte[1600];
		_bufferHandle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
	}

	public unsafe IntPtr Read64()
	{
		IntPtr intPtr = _signatureManager.Read(SignatureType.MobArray);
		if (intPtr == IntPtr.Zero)
		{
			return IntPtr.Zero;
		}
		_readMemory.ReadBuffer(intPtr, _buffer, 1600);
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
