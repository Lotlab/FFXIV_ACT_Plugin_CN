#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FFXIV_ACT_Plugin.Memory.MemoryReader;

public class ReadVtable : IReadVtable
{
	private readonly IProcessManager _processManager;

	private readonly IReadMemory _readMemory;

	private readonly byte[] _buffer = new byte[65536];

	public ReadVtable(IProcessManager processManager, IReadMemory readMemory)
	{
		_processManager = processManager;
		_readMemory = readMemory;
	}

	public unsafe Dictionary<SignatureType, IntPtr> Read(Dictionary<SignatureType, uint> pointers, List<SignatureType> signatureTypes)
	{
		int num = 8;
		Dictionary<SignatureType, IntPtr> dictionary = new Dictionary<SignatureType, IntPtr>();
		List<Tuple<IntPtr, ulong>> list = ((!Environment.Is64BitProcess) ? GetMemoryRegionsToScan32() : GetMemoryRegionsToScan64());
		byte[] array = new byte[65536];
		for (int i = 0; i < signatureTypes.Count; i++)
		{
			if (!pointers.ContainsKey(signatureTypes[i]))
			{
				return dictionary;
			}
		}
		fixed (byte* ptr = array)
		{
			for (int j = 0; j < list.Count; j++)
			{
				if (signatureTypes.Count == 0)
				{
					break;
				}
				int num3;
				for (ulong num2 = 0uL; num2 < list[j].Item2; num2 += (uint)num3)
				{
					num3 = 65536;
					if (num3 > (int)list[j].Item2 - (int)num2)
					{
						num3 = (int)list[j].Item2 - (int)num2;
						if (num3 % num != 0)
						{
							num3 -= num3 % num;
						}
						if (num3 <= 0)
						{
							break;
						}
					}
					_readMemory.ReadBuffer(list[j].Item1 + (int)num2, array, num3);
					for (int num4 = signatureTypes.Count - 1; num4 >= 0; num4--)
					{
						long num5 = _processManager.Current.BaseAddress.ToInt64() + pointers[signatureTypes[num4]];
						byte* ptr2 = ptr;
						for (byte* ptr3 = ptr + num3; ptr2 < ptr3; ptr2 += num)
						{
							bool flag = false;
							if (num5 == *(long*)ptr2)
							{
								flag = true;
							}
							if (flag)
							{
								IntPtr value = new IntPtr(list[j].Item1.ToInt64() + (ptr2 + num2 - ptr));
								Trace.WriteLine("Found VTable for [" + SignatureType.ChatLog.ToString() + "] at address [" + value.ToString("X8") + "]", "FFXIV_ACT_Plugin");
								dictionary.Add(signatureTypes[num4], value);
								signatureTypes.RemoveAt(num4);
							}
						}
					}
				}
			}
		}
		if (signatureTypes.Count > 0)
		{
			string text = "";
			foreach (SignatureType signatureType in signatureTypes)
			{
				text = text + Enum.GetName(typeof(SignatureType), signatureType) + " ";
			}
			Trace.WriteLine("WARNING: Could not locate VTable references for: " + text + ".  If this persists after login, some functionality will not work.", "FFXIV_ACT_Plugin");
		}
		return dictionary;
	}

	private List<Tuple<IntPtr, ulong>> GetMemoryRegionsToScan64()
	{
		List<Tuple<IntPtr, ulong>> list = new List<Tuple<IntPtr, ulong>>();
		long num = 0L;
		uint num2 = (uint)Marshal.SizeOf<NativeMethods.MEMORY_BASIC_INFORMATION64>();
		NativeMethods.MEMORY_BASIC_INFORMATION64 lpBuffer;
		while (num2 == NativeMethods.VirtualQueryEx64(_processManager.Current.Handle, (IntPtr)num, out lpBuffer, num2))
		{
			if ((lpBuffer.Protect & 0x100) != 256 && (lpBuffer.Protect & 4) == 4)
			{
				list.Add(new Tuple<IntPtr, ulong>(new IntPtr((long)lpBuffer.BaseAddress), lpBuffer.RegionSize));
			}
			if (num >= (long)(lpBuffer.BaseAddress + lpBuffer.RegionSize))
			{
				break;
			}
			num = (long)(lpBuffer.BaseAddress + lpBuffer.RegionSize);
		}
		return list;
	}

	private List<Tuple<IntPtr, ulong>> GetMemoryRegionsToScan32()
	{
		List<Tuple<IntPtr, ulong>> list = new List<Tuple<IntPtr, ulong>>();
		long num = 0L;
		uint num2 = (uint)Marshal.SizeOf<NativeMethods.MEMORY_BASIC_INFORMATION32>();
		NativeMethods.MEMORY_BASIC_INFORMATION32 lpBuffer;
		while (num2 == NativeMethods.VirtualQueryEx32(_processManager.Current.Handle, (IntPtr)num, out lpBuffer, num2))
		{
			if ((lpBuffer.Protect & 0x100) != 256 && (lpBuffer.Protect & 4) == 4)
			{
				list.Add(new Tuple<IntPtr, ulong>(new IntPtr((long)lpBuffer.BaseAddress), (ulong)lpBuffer.RegionSize.ToInt64()));
			}
			if (num >= (long)lpBuffer.BaseAddress + (long)lpBuffer.RegionSize)
			{
				break;
			}
			num = (long)lpBuffer.BaseAddress + (long)lpBuffer.RegionSize;
		}
		return list;
	}
}
