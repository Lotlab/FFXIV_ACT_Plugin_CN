using System;
using System.Runtime.InteropServices;
using FFXIV_ACT_Plugin.Memory.Models.Global;

namespace FFXIV_ACT_Plugin.Memory.MemoryReader;

public class ReadPlayer : IReadPlayer, IDisposable
{
	private readonly ISignatureManager _signatureManager;

	private readonly IReadMemory _readMemory;

	private readonly byte[] _buffer;

	private readonly GCHandle _bufferHandle;

	private bool disposedValue;

	public unsafe ReadPlayer(ISignatureManager signatureManager, IReadMemory readMemory)
	{
		_signatureManager = signatureManager;
		_readMemory = readMemory;
		_buffer = new byte[sizeof(PlayerStruct)];
		_bufferHandle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
	}

	public unsafe IntPtr Read()
	{
		IntPtr intPtr = _signatureManager.Read(SignatureType.Player);
		if (intPtr == IntPtr.Zero)
		{
			return IntPtr.Zero;
		}
		_readMemory.ReadBuffer(intPtr, _buffer, _buffer.Length);
		fixed (byte* buffer = _buffer)
		{
			return (IntPtr)buffer;
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposedValue)
		{
			return;
		}
		if (disposing)
		{
			GCHandle bufferHandle = _bufferHandle;
			if (bufferHandle.IsAllocated)
			{
				_bufferHandle.Free();
			}
		}
		disposedValue = true;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
