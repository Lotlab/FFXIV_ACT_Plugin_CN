using System.Collections.ObjectModel;

namespace FFXIV_ACT_Plugin.Memory.MemoryProcessors;

public interface IPartyProcessor
{
	ReadOnlyCollection<uint> PartyMemberIds { get; }

	int PartySize { get; }

	void Refresh();
}
