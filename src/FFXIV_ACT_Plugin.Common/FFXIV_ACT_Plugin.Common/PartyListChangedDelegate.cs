using System.Collections.ObjectModel;

namespace FFXIV_ACT_Plugin.Common;

public delegate void PartyListChangedDelegate(ReadOnlyCollection<uint> partyList, int partySize);
