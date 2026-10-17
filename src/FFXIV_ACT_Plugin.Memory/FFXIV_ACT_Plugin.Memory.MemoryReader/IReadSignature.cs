using System.Collections.Generic;

namespace FFXIV_ACT_Plugin.Memory.MemoryReader;

public interface IReadSignature
{
	Dictionary<SignatureType, uint> Read(Dictionary<SignatureType, int[]> signatures);
}
