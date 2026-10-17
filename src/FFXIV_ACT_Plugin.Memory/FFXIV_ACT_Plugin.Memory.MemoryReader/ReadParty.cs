using System;
using System.Runtime.InteropServices;

namespace FFXIV_ACT_Plugin.Memory.MemoryReader;

public class ReadParty : IReadParty, IDisposable
{
	private readonly ISignatureManager _signatureManager;

	private readonly IReadMemory _readMemory;

	private readonly byte[] _buffer;

	private readonly GCHandle _bufferHandle;

	private bool disposedValue;

	public ReadParty(ISignatureManager signatureManager, IReadMemory readMemory)
	{
		_signatureManager = signatureManager;
		_readMemory = readMemory;
		_buffer = new byte[32767];
		_bufferHandle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
	}

	public unsafe IntPtr Read()
	{
		IntPtr intPtr = _signatureManager.Read(SignatureType.PartyList);
		if (intPtr == IntPtr.Zero)
		{
			return IntPtr.Zero;
		}
		intPtr += 32;
		_readMemory.ReadBuffer(intPtr, _buffer, _buffer.Length);
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
