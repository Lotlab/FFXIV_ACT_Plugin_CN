using System;

namespace FFXIV_ACT_Plugin.Memory.MemoryReader;

public class ReadServerTime : IReadServerTime
{
	private readonly ISignatureManager _signatureManager;

	private readonly IReadMemory _readMemory;

	public ReadServerTime(ISignatureManager signatureManager, IReadMemory readMemory)
	{
		_signatureManager = signatureManager;
		_readMemory = readMemory;
	}

	public ulong Read()
	{
		int offset = 5752;
		int offset2 = 8;
		int offset3 = 2900;
		IntPtr intPtr = _signatureManager.Read(SignatureType.ServerTime);
		if (intPtr != IntPtr.Zero)
		{
			intPtr = _readMemory.ReadPointer(intPtr);
			if (intPtr != IntPtr.Zero)
			{
				intPtr = _readMemory.ReadPointer(IntPtr.Add(intPtr, offset));
				if (intPtr != IntPtr.Zero)
				{
					intPtr = _readMemory.ReadPointer(IntPtr.Add(intPtr, offset2));
				}
			}
		}
		if (intPtr == IntPtr.Zero)
		{
			return 0uL;
		}
		intPtr = IntPtr.Add(intPtr, offset3);
		return _readMemory.ReadUInt64(intPtr);
	}
}
