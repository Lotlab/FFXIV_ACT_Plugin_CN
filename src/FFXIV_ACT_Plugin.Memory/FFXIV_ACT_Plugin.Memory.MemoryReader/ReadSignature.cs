#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FFXIV_ACT_Plugin.Memory.MemoryReader;

public class ReadSignature : IReadSignature
{
	private readonly IProcessManager _processManager;

	private readonly IReadMemory _readMemory;

	private readonly byte[] _buffer = new byte[65536];

	public ReadSignature(IProcessManager processManager, IReadMemory readMemory)
	{
		_processManager = processManager;
		_readMemory = readMemory;
	}

	public Dictionary<SignatureType, uint> Read(Dictionary<SignatureType, int[]> signatures)
	{
		Dictionary<SignatureType, uint> dictionary = new Dictionary<SignatureType, uint>();
		List<SignatureType> list = new List<SignatureType>((SignatureType[])Enum.GetValues(typeof(SignatureType)));
		if (_processManager.Current.ClientMode != FFXIVClientMode.FFXIV_64)
		{
			return dictionary;
		}
		IntPtr baseAddress = _processManager.Current.BaseAddress;
		IntPtr intPtr = IntPtr.Add(baseAddress, _processManager.Current.MaxModuleSize);
		IntPtr intPtr2 = baseAddress;
		int num = signatures.Values.Max((int[] x) => x.Length);
		while (intPtr2.ToInt64() < intPtr.ToInt64())
		{
			int num2 = _buffer.Length;
			if (IntPtr.Add(intPtr2, num2).ToInt64() > intPtr.ToInt64())
			{
				num2 = (int)(intPtr.ToInt64() - intPtr2.ToInt64());
			}
			_readMemory.ReadBuffer(intPtr2, _buffer, num2);
			for (int num3 = list.Count - 1; num3 >= 0; num3--)
			{
				int firstSignatureOccurrence = GetFirstSignatureOccurrence(signatures[list[num3]], _buffer, num2);
				if (firstSignatureOccurrence > 0)
				{
					uint signaturefromOffset = GetSignaturefromOffset(intPtr2, _buffer, firstSignatureOccurrence);
					dictionary.Add(list[num3], signaturefromOffset);
					Trace.WriteLine($"Found Signature [{list[num3]}] at offset [{signaturefromOffset:X8}]", "FFXIV_ACT_Plugin");
					list.Remove(list[num3]);
				}
			}
			intPtr2 = ((_buffer.Length - num <= 0) ? IntPtr.Add(intPtr2, _buffer.Length) : IntPtr.Add(intPtr2, _buffer.Length - num));
		}
		if (list.Count != 0)
		{
			for (int num4 = 0; num4 < list.Count; num4++)
			{
				Trace.WriteLine($"ERROR: Missing Signature [{list[num4]}].", "FFXIV_ACT_Plugin");
			}
		}
		return dictionary;
	}

	private int GetFirstSignatureOccurrence(int[] signature, byte[] buffer, int length)
	{
		int num = length - signature.Length - 4 + 1;
		for (int i = 0; i < num; i++)
		{
			int num2 = 0;
			for (int j = 0; j < signature.Length; j++)
			{
				if (signature[j] == -1)
				{
					num2++;
					continue;
				}
				if (signature[j] != buffer[i + j])
				{
					break;
				}
				num2++;
			}
			if (num2 == signature.Length)
			{
				return i + signature.Length;
			}
		}
		return 0;
	}

	private uint GetSignaturefromOffset(IntPtr currentAddress, byte[] buffer, int startIndex)
	{
		IntPtr intPtr = new IntPtr(BitConverter.ToInt32(buffer, startIndex));
		return (uint)(new IntPtr(currentAddress.ToInt64() + startIndex + 4 + intPtr.ToInt64()).ToInt64() - _processManager.Current.BaseAddress.ToInt64());
	}
}
