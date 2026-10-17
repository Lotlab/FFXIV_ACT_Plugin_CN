using System.Collections.ObjectModel;
using System.Linq;
using FFXIV_ACT_Plugin.Common.Models;
using FFXIV_ACT_Plugin.Memory.MemoryReader;
using FFXIV_ACT_Plugin.Memory.Models.Global;

namespace FFXIV_ACT_Plugin.Memory.MemoryProcessors;

public class PartyProcessor : IPartyProcessor
{
	private readonly int c_MaxPartyRecordCount = 28;

	private readonly Party _party;

	private readonly IReadParty _readParty;

	private readonly DataEvent _dataEvent;

	private readonly ICombatantManager _combatantManager;

	private readonly IProcessManager _processManager;

	public ReadOnlyCollection<uint> PartyMemberIds => new ReadOnlyCollection<uint>(_party.MemberIDs);

	public int PartySize => _party.PartySize;

	public PartyProcessor(IReadParty readParty, DataEvent dataEvent, ICombatantManager combatantManager, IProcessManager processManager)
	{
		_readParty = readParty;
		_dataEvent = dataEvent;
		_combatantManager = combatantManager;
		_processManager = processManager;
		_party = new Party
		{
			MemberIDs = new uint[c_MaxPartyRecordCount]
		};
	}

	public void Refresh()
	{
		bool flag = Refresh64();
		UpdatePartyFlag();
		if (flag)
		{
			_dataEvent.OnPartyListChanged(_party.MemberIDs.Where((uint x) => x != 0 && x != 3758096384u).ToList().AsReadOnly(), _party.PartySize);
		}
	}

	private unsafe bool Refresh64()
	{
		bool result = false;
		PartyStruct64* ptr = (PartyStruct64*)(void*)_readParty.Read();
		bool flag = ptr != null && ptr->PartyFlag1 == 1;
		if (flag)
		{
			result = flag != _party.IsInParty;
			_party.IsInParty = true;
			_party.PartySize = ptr->PartySize;
			PartyEntryStruct64* ptr2 = (PartyEntryStruct64*)ptr->Entries;
			for (int i = 0; i < _party.MemberIDs.Length; i++)
			{
				if ((ptr2->Flags & 1) == 0)
				{
					if (_party.MemberIDs[i] != 0)
					{
						result = true;
						_party.MemberIDs[i] = 0u;
					}
				}
				else if (_party.MemberIDs[i] != ptr2->ID)
				{
					result = true;
					_party.MemberIDs[i] = ptr2->ID;
				}
				ptr2++;
			}
		}
		else if (_party.PartySize > 0)
		{
			result = true;
			Clear();
		}
		return result;
	}

	private void Clear()
	{
		_party.IsInParty = false;
		_party.PartySize = 0;
		for (int i = 0; i < _party.MemberIDs.Length; i++)
		{
			_party.MemberIDs[i] = 0u;
		}
	}

	private void UpdatePartyFlag()
	{
		for (int i = 0; i < _combatantManager.Combatants.Count; i++)
		{
			Combatant combatant = _combatantManager.Combatants[i];
			if (combatant == null || (combatant.type != 1 && combatant.type != 2))
			{
				continue;
			}
			bool flag = false;
			for (int j = 0; j < _party.MemberIDs.Length; j++)
			{
				if (_party.MemberIDs[j] == combatant.ID)
				{
					flag = true;
					combatant.PartyType = ((j < _party.PartySize) ? PartyType.Party : PartyType.Alliance);
				}
			}
			if (!flag)
			{
				combatant.PartyType = PartyType.None;
			}
		}
	}
}
