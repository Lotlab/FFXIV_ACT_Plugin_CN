using System;
using System.Collections.Generic;

namespace FFXIV_ACT_Plugin.Memory.MemoryReader;

public interface IReadVtable
{
	Dictionary<SignatureType, IntPtr> Read(Dictionary<SignatureType, uint> pointers, List<SignatureType> signatureTypes);
}
