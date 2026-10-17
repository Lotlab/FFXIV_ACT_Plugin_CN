using System;
using System.Runtime.InteropServices;

namespace FFXIV_ACT_Plugin.Memory.MemoryReader;

public class ReadLog : IReadLog, IDisposable
{
	private readonly byte[] _headerBuffer;

	private readonly GCHandle _headerBufferHandle;

	private readonly byte[] _lengthBuffer;

	private readonly GCHandle _lengthBufferHandle;

	private byte[] _messageBuffer;

	private GCHandle _messageBufferHandle;

	private readonly IReadMemory _readMemory;

	private readonly ISignatureManager _signatureManager;

	private readonly IProcessManager _processManager;

	private bool disposedValue;

	public ReadLog(IReadMemory readMemory, ISignatureManager signatureManager, IProcessManager processManager)
	{
		_readMemory = readMemory;
		_signatureManager = signatureManager;
		_processManager = processManager;
		_headerBuffer = new byte[1024];
		_headerBufferHandle = GCHandle.Alloc(_headerBuffer, GCHandleType.Pinned);
		_lengthBuffer = new byte[16384];
		_lengthBufferHandle = GCHandle.Alloc(_lengthBuffer, GCHandleType.Pinned);
		_messageBuffer = new byte[65536];
		_messageBufferHandle = GCHandle.Alloc(_messageBuffer, GCHandleType.Pinned);
	}

	public unsafe IntPtr ReadHeader()
	{
		IntPtr intPtr = _signatureManager.Read(SignatureType.ChatLog);
		if (intPtr == IntPtr.Zero)
		{
			return IntPtr.Zero;
		}
		_readMemory.ReadBuffer(intPtr, _headerBuffer, _headerBuffer.Length);
		fixed (byte* headerBuffer = _headerBuffer)
		{
			return (IntPtr)headerBuffer;
		}
	}

	public unsafe IntPtr ReadLineLengthArray(IntPtr startPointer, int length)
	{
		_readMemory.ReadBuffer(startPointer, _lengthBuffer, length);
		fixed (byte* lengthBuffer = _lengthBuffer)
		{
			return (IntPtr)lengthBuffer;
		}
	}

	public unsafe IntPtr ReadMessageArray(IntPtr startPointer, int length)
	{
		if (_messageBuffer.Length < length)
		{
			_messageBufferHandle.Free();
			_messageBuffer = new byte[length];
			_messageBufferHandle = GCHandle.Alloc(_messageBuffer, GCHandleType.Pinned);
		}
		_readMemory.ReadBuffer(startPointer, _messageBuffer, length);
		fixed (byte* messageBuffer = _messageBuffer)
		{
			return (IntPtr)messageBuffer;
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
			GCHandle lengthBufferHandle = _lengthBufferHandle;
			if (lengthBufferHandle.IsAllocated)
			{
				_lengthBufferHandle.Free();
			}
			if (_messageBufferHandle.IsAllocated)
			{
				_messageBufferHandle.Free();
			}
			lengthBufferHandle = _headerBufferHandle;
			if (lengthBufferHandle.IsAllocated)
			{
				_headerBufferHandle.Free();
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
