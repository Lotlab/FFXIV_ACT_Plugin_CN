using System;

namespace FFXIV_ACT_Plugin.Memory.MemoryReader;

public class ReadMapId : IReadMapId
{
	private readonly ISignatureManager _signatureManager;

	private readonly IReadMemory _readMemory;

	public ReadMapId(ISignatureManager signatureManager, IReadMemory readMemory)
	{
		_signatureManager = signatureManager;
		_readMemory = readMemory;
	}

	public uint Read()
	{
		IntPtr intPtr = _signatureManager.Read(SignatureType.MapID);
		if (intPtr != IntPtr.Zero)
		{
			return _readMemory.ReadUInt32(intPtr);
		}
		return 0u;
	}
}
