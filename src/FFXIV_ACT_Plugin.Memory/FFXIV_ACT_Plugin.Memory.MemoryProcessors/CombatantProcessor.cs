using System;
using System.Linq;
using System.Text;
using FFXIV_ACT_Plugin.Common.Models;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Memory.MemoryReader;
using FFXIV_ACT_Plugin.Memory.Models.Global;
using FFXIV_ACT_Plugin.Memory.Models.TraditionalChinese;
using FFXIV_ACT_Plugin.Resource;

namespace FFXIV_ACT_Plugin.Memory.MemoryProcessors;

public class CombatantProcessor : ICombatantProcessor
{
	private readonly DataEvent _dataEvent;

	private readonly IProcessManager _processManager;

	private readonly IReadCombatant _readCombatant;

	private readonly IWorldList _worldList;

	private readonly ISettingsMediator _settingsMediator;

	public CombatantProcessor(IReadCombatant readCombatant, IProcessManager processManager, DataEvent dataEvent, IWorldList worldList, ISettingsMediator settingsMediator)
	{
		_processManager = processManager;
		_readCombatant = readCombatant;
		_dataEvent = dataEvent;
		_worldList = worldList;
		_settingsMediator = settingsMediator;
	}

	public void RefreshCombatant(Combatant combatant, IntPtr address, bool refresh)
	{
		if (address != combatant.Address)
		{
			if (combatant.Address != IntPtr.Zero)
			{
				_dataEvent.OnCombatantRemoved(combatant);
				ClearCombatant(combatant);
			}
			if (address != IntPtr.Zero)
			{
				RefreshCombatantInternal(combatant, address, refresh);
				if (combatant.ID != 0)
				{
					_dataEvent.OnCombatantAdded(combatant);
				}
			}
		}
		else
		{
			if (!(address != IntPtr.Zero))
			{
				return;
			}
			int job = combatant.Job;
			int level = combatant.Level;
			uint iD = combatant.ID;
			string name = combatant.Name;
			RefreshCombatantInternal(combatant, address, refresh);
			if (iD != combatant.ID || name != combatant.Name)
			{
				if (combatant.Order == 0)
				{
					_dataEvent.OnPrimaryPlayerChanged();
				}
				_dataEvent.OnCombatantAdded(combatant);
			}
			else if (job != combatant.Job || level != combatant.Level)
			{
				_dataEvent.OnCombatantAdded(combatant);
			}
		}
	}

	public void RefreshAndAddCombatant(Combatant combatant, IntPtr address, Combatant[] oldCombatants)
	{
		if (address == IntPtr.Zero)
		{
			ClearCombatant(combatant);
			return;
		}
		RefreshCombatantInternal(combatant, address, refresh: true);
		if (combatant.Order == 0)
		{
			_dataEvent.OnPrimaryPlayerChanged();
		}
		if (combatant.ID != 0)
		{
			Combatant combatant2 = oldCombatants.FirstOrDefault((Combatant x) => x.ID == combatant.ID);
			if (combatant2?.NetworkBuffs != null)
			{
				combatant.NetworkBuffs = combatant2.NetworkBuffs;
			}
			_dataEvent.OnCombatantAdded(combatant);
		}
	}

	private void RefreshCombatantInternal(Combatant oldCombatant, IntPtr combatantPointer, bool refresh)
	{
		if (!(oldCombatant.Address == combatantPointer) || refresh)
		{
			bool flag;
			if (_settingsMediator.DataCollectionSettings.RegionID == Region.Korean)
			{
				flag = CopyCombatant_Korean(oldCombatant, combatantPointer);
			}
			else
			{
				flag = ((_settingsMediator.DataCollectionSettings.RegionID != Region.TraditionalChinese) ? CopyCombatant_Global(oldCombatant, combatantPointer) : CopyCombatant_TraditionalChinese(oldCombatant, combatantPointer));
			}
			oldCombatant.Address = combatantPointer;
			if (oldCombatant.WorldID == 65535)
			{
				oldCombatant.WorldID = 0u;
			}
			if (oldCombatant.CurrentWorldID == 65535)
			{
				oldCombatant.CurrentWorldID = 0u;
			}
			if (oldCombatant.OwnerID == 3758096384u)
			{
				oldCombatant.OwnerID = 0u;
			}
			if (flag)
			{
				ClearNetworkBuff(oldCombatant);
			}
			if (oldCombatant.type == 1 || oldCombatant.type == 2)
			{
				oldCombatant.WorldName = _worldList.GetWorldNameById(oldCombatant.WorldID);
			}
			else
			{
				oldCombatant.WorldName = "";
			}
		}
	}

	private unsafe bool CopyCombatant_Global(Combatant oldCombatant, IntPtr combatantPointer)
	{
		bool result = false;
		FFXIV_ACT_Plugin.Memory.Models.Global.CombatantHeader* ptr = (FFXIV_ACT_Plugin.Memory.Models.Global.CombatantHeader*)(void*)_readCombatant.ReadHeader(combatantPointer);
		string combatantName = GetCombatantName(ptr->Name);
		if (ptr->ID == 0 || ptr->ID == 3758096384u || string.IsNullOrWhiteSpace(combatantName))
		{
			ClearCombatant(oldCombatant);
			return result;
		}
		if (combatantName != oldCombatant.Name || oldCombatant.ID != ptr->ID)
		{
			result = true;
		}
		FFXIV_ACT_Plugin.Memory.Models.Global.CombatantStruct* ptr2 = (FFXIV_ACT_Plugin.Memory.Models.Global.CombatantStruct*)(void*)_readCombatant.Read(combatantPointer);
		oldCombatant.Name = combatantName;
		oldCombatant.BNpcID = ptr2->Header.BNpcID;
		oldCombatant.BNpcNameID = ptr2->BNpcNameID;
		oldCombatant.CastBuffID = ptr2->CastBuffID;
		oldCombatant.CastDurationCurrent = ptr2->CastDurationCurrent;
		oldCombatant.CastDurationMax = ptr2->CastDurationMax;
		oldCombatant.CastTargetID = ptr2->CastTargetID;
		oldCombatant.CurrentCP = ptr2->CurrentCP;
		oldCombatant.CurrentGP = ptr2->CurrentGP;
		oldCombatant.CurrentHP = ptr2->CurrentHP;
		oldCombatant.CurrentMP = ptr2->CurrentMP;
		oldCombatant.ID = ptr2->Header.ID;
		oldCombatant.IsCasting = ptr2->IsCasting1 == 1 && ptr2->IsCasting2 == 1;
		oldCombatant.Job = ptr2->Job;
		oldCombatant.Level = ptr2->Level;
		oldCombatant.MaxCP = ptr2->MaxCP;
		oldCombatant.MaxGP = ptr2->MaxGP;
		oldCombatant.MaxHP = ptr2->MaxHP;
		oldCombatant.MaxMP = 10000u;
		oldCombatant.OwnerID = ptr2->Header.OwnerID;
		oldCombatant.PosX = ptr2->Header.PosX;
		oldCombatant.PosY = ptr2->Header.PosY;
		oldCombatant.PosZ = ptr2->Header.PosZ;
		oldCombatant.Heading = ptr2->Header.Heading;
		oldCombatant.EffectiveDistance = ptr2->Header.EffectiveDistance;
		if (ptr2->Header.Type == 1 && ptr2->PCTargetID != 3758096384u && ptr2->PCTargetID != 0)
		{
			oldCombatant.TargetID = ptr2->PCTargetID;
		}
		else if (ptr2->NPCTargetID != 3758096384u && ptr2->NPCTargetID != 0)
		{
			oldCombatant.TargetID = ptr2->NPCTargetID;
		}
		else
		{
			oldCombatant.TargetID = 0u;
		}
		oldCombatant.type = ptr2->Header.Type;
		oldCombatant.WorldID = ptr2->HomeWorldID;
		oldCombatant.CurrentWorldID = ptr2->CurrentWorldID;
		return result;
	}

	private bool CopyCombatant_Korean(Combatant oldCombatant, IntPtr combatantPointer)
	{
		return CopyCombatant_Global(oldCombatant, combatantPointer);
	}

	private unsafe bool CopyCombatant_TraditionalChinese(Combatant oldCombatant, IntPtr combatantPointer)
	{
		bool result = false;
		FFXIV_ACT_Plugin.Memory.Models.TraditionalChinese.CombatantHeader* ptr = (FFXIV_ACT_Plugin.Memory.Models.TraditionalChinese.CombatantHeader*)(void*)_readCombatant.ReadHeader(combatantPointer);
		string combatantName = GetCombatantName(ptr->Name);
		if (ptr->ID == 0 || ptr->ID == 3758096384u || string.IsNullOrWhiteSpace(combatantName))
		{
			ClearCombatant(oldCombatant);
			return result;
		}
		if (combatantName != oldCombatant.Name || oldCombatant.ID != ptr->ID)
		{
			result = true;
		}
		FFXIV_ACT_Plugin.Memory.Models.TraditionalChinese.CombatantStruct* ptr2 = (FFXIV_ACT_Plugin.Memory.Models.TraditionalChinese.CombatantStruct*)(void*)_readCombatant.Read(combatantPointer);
		oldCombatant.Name = combatantName;
		oldCombatant.BNpcID = ptr2->Header.BNpcID;
		oldCombatant.BNpcNameID = ptr2->BNpcNameID;
		oldCombatant.CastBuffID = ptr2->CastBuffID;
		oldCombatant.CastDurationCurrent = ptr2->CastDurationCurrent;
		oldCombatant.CastDurationMax = ptr2->CastDurationMax;
		oldCombatant.CastTargetID = ptr2->CastTargetID;
		oldCombatant.CurrentCP = ptr2->CurrentCP;
		oldCombatant.CurrentGP = ptr2->CurrentGP;
		oldCombatant.CurrentHP = ptr2->CurrentHP;
		oldCombatant.CurrentMP = ptr2->CurrentMP;
		oldCombatant.ID = ptr2->Header.ID;
		oldCombatant.IsCasting = ptr2->IsCasting1 == 1 && ptr2->IsCasting2 == 1;
		oldCombatant.Job = ptr2->Job;
		oldCombatant.Level = ptr2->Level;
		oldCombatant.MaxCP = ptr2->MaxCP;
		oldCombatant.MaxGP = ptr2->MaxGP;
		oldCombatant.MaxHP = ptr2->MaxHP;
		oldCombatant.MaxMP = 10000u;
		oldCombatant.OwnerID = ptr2->Header.OwnerID;
		oldCombatant.PosX = ptr2->Header.PosX;
		oldCombatant.PosY = ptr2->Header.PosY;
		oldCombatant.PosZ = ptr2->Header.PosZ;
		oldCombatant.Heading = ptr2->Header.Heading;
		oldCombatant.EffectiveDistance = ptr2->Header.EffectiveDistance;
		if (ptr2->Header.Type == 1 && ptr2->PCTargetID != 3758096384u && ptr2->PCTargetID != 0)
		{
			oldCombatant.TargetID = ptr2->PCTargetID;
		}
		else if (ptr2->NPCTargetID != 3758096384u && ptr2->NPCTargetID != 0)
		{
			oldCombatant.TargetID = ptr2->NPCTargetID;
		}
		else
		{
			oldCombatant.TargetID = 0u;
		}
		oldCombatant.type = ptr2->Header.Type;
		oldCombatant.WorldID = ptr2->HomeWorldID;
		oldCombatant.CurrentWorldID = ptr2->CurrentWorldID;
		return result;
	}

	private void ClearCombatant(Combatant combatant)
	{
		if (!(combatant.Address == IntPtr.Zero))
		{
			combatant.PartyType = PartyType.None;
			combatant.BNpcID = 0u;
			combatant.BNpcNameID = 0u;
			combatant.CastBuffID = 0u;
			combatant.CastDurationCurrent = 0f;
			combatant.CastDurationMax = 0f;
			combatant.CastTargetID = 0u;
			combatant.CurrentCP = 0u;
			combatant.CurrentGP = 0u;
			combatant.CurrentHP = 0u;
			combatant.CurrentMP = 0u;
			combatant.ID = 0u;
			combatant.IsCasting = false;
			combatant.Job = 0;
			combatant.Level = 0;
			combatant.MaxCP = 0u;
			combatant.MaxGP = 0u;
			combatant.MaxHP = 0u;
			combatant.MaxMP = 0u;
			combatant.Name = null;
			combatant.OwnerID = 0u;
			combatant.Address = IntPtr.Zero;
			combatant.PosX = 0f;
			combatant.PosY = 0f;
			combatant.PosZ = 0f;
			combatant.Heading = 0f;
			combatant.EffectiveDistance = 0;
			combatant.TargetID = 0u;
			combatant.type = 0;
			combatant.WorldID = 0u;
			combatant.CurrentWorldID = 0u;
			ClearNetworkBuff(combatant);
		}
	}

	private unsafe string GetCombatantName(byte* data)
	{
		byte[] array = new byte[64];
		for (int i = 0; i < array.Length; i++)
		{
			if (data[i] == 0)
			{
				return Encoding.UTF8.GetString(array, 0, i);
			}
			array[i] = data[i];
		}
		return Encoding.UTF8.GetString(array, 0, array.Length);
	}

	private void ClearNetworkBuff(Combatant combatant)
	{
		for (byte b = 0; b < combatant.NetworkBuffs.Length; b++)
		{
			if (combatant.NetworkBuffs[b] == null)
			{
				combatant.NetworkBuffs[b] = new NetworkBuff();
			}
			else
			{
				combatant.NetworkBuffs[b].ActorID = 0u;
				combatant.NetworkBuffs[b].ActorName = null;
				combatant.NetworkBuffs[b].BuffExtra = 0;
				combatant.NetworkBuffs[b].BuffID = 0;
				combatant.NetworkBuffs[b].Duration = 0f;
				combatant.NetworkBuffs[b].TargetID = 0u;
				combatant.NetworkBuffs[b].TargetName = null;
				combatant.NetworkBuffs[b].Timestamp = default;
			}
		}
	}
}
