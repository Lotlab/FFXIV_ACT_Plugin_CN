using System;
using System.Collections.ObjectModel;
using FFXIV_ACT_Plugin.Common.Models;
using FFXIV_ACT_Plugin.Memory.MemoryProcessors;

namespace FFXIV_ACT_Plugin.Memory;

public class CombatantManager : ICombatantManager
{
	private Combatant[] _combatants;

	private ReadOnlyCollection<Combatant> _readonlyCombatants;

	private DateTime _lastCombatantRefresh;

	private readonly ICombatantProcessor _combatantProcessor;

	private readonly IMobArrayProcessor _mobArrayProcessor;

	private readonly IProcessManager _processManager;

	public object CombatantLock { get; } = new object();

	public ReadOnlyCollection<Combatant> Combatants
	{
		get
		{
			lock (CombatantLock)
			{
				return _readonlyCombatants;
			}
		}
	}

	public CombatantManager(ICombatantProcessor combatantProcessor, IMobArrayProcessor mobArrayProcessor, IProcessManager processManager)
	{
		_combatantProcessor = combatantProcessor;
		_mobArrayProcessor = mobArrayProcessor;
		_processManager = processManager;
		_processManager.ProcessChanged += OnProcessChanged;
		_combatants = Array.Empty<Combatant>();
		_readonlyCombatants = Array.AsReadOnly(_combatants);
		_lastCombatantRefresh = DateTime.MinValue;
	}

	private void OnProcessChanged(object sender, EventArgs e)
	{
		lock (CombatantLock)
		{
			_combatants = new Combatant[_mobArrayProcessor.MobArray.Count];
			_readonlyCombatants = Array.AsReadOnly(_combatants);
			Reset();
		}
	}

	public Combatant GetCombatantById(uint CombatantID)
	{
		lock (CombatantLock)
		{
			if (_combatants == null)
			{
				return null;
			}
			for (int i = 0; i < _combatants.Length; i++)
			{
				if (_combatants[i].ID == CombatantID && _combatants[i].Address != IntPtr.Zero)
				{
					return _combatants[i];
				}
			}
		}
		return null;
	}

	public Combatant GetCombatantByPointer(IntPtr combatantPtr)
	{
		lock (CombatantLock)
		{
			if (_combatants == null)
			{
				return null;
			}
			for (int i = 0; i < _combatants.Length; i++)
			{
				if (_combatants[i].Address == combatantPtr && _combatants[i].ID != 0 && _combatants[i].ID != 3758096384u)
				{
					return _combatants[i];
				}
			}
		}
		return null;
	}

	public void Rescan()
	{
		lock (CombatantLock)
		{
			Combatant[] oldCombatants = (Combatant[])_combatants.Clone();
			Reset();
			for (int i = 0; i < _mobArrayProcessor.MobArray.Count; i++)
			{
				IntPtr address = _mobArrayProcessor.MobArray[i];
				Combatant combatant = Combatants[i];
				_combatantProcessor.RefreshAndAddCombatant(combatant, address, oldCombatants);
			}
		}
	}

	public void Refresh()
	{
		bool flag = false;
		if (DateTime.UtcNow.Subtract(_lastCombatantRefresh).TotalMilliseconds > 100.0)
		{
			_lastCombatantRefresh = DateTime.UtcNow;
			flag = true;
		}
		ReadOnlyCollection<IntPtr> mobArray = _mobArrayProcessor.MobArray;
		lock (CombatantLock)
		{
			for (int i = 0; i < mobArray.Count; i++)
			{
				IntPtr intPtr = mobArray[i];
				Combatant combatant = Combatants[i];
				if (flag || intPtr != combatant.Address)
				{
					_combatantProcessor.RefreshCombatant(combatant, intPtr, flag);
				}
			}
		}
	}

	private void Reset()
	{
		for (int i = 0; i < _combatants.Length; i++)
		{
			_combatants[i] = new Combatant
			{
				Order = i
			};
		}
		_lastCombatantRefresh = DateTime.MinValue;
	}
}
