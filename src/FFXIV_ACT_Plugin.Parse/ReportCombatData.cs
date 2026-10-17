using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FFXIV_ACT_Plugin.Common;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Resource;

namespace FFXIV_ACT_Plugin.Parse;

public class ReportCombatData : IReportCombatData
{
	private readonly ISettingsMediator _settingsManager;

	private readonly IActWrapper _actWrapper;

	private readonly INameResource _nameResource;

	private readonly IBenchmarkRepository _benchmarkRepository;

	public ReportCombatData(ISettingsMediator settingsManager, IActWrapper actWrapper, INameResource nameResource, IBenchmarkRepository benchmarkRepository)
	{
		_actWrapper = actWrapper;
		_nameResource = nameResource;
		_settingsManager = settingsManager;
		_benchmarkRepository = benchmarkRepository;
	}

	public void AddDamageEntry(EnvironmentState state, DateTime timestamp, uint sourceId, uint targetId, DamageEffectEntry entry)
	{
		_benchmarkRepository.Measure("ParseAddDamageEntry", (Action)(() =>
		{
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Invalid comparison between Unknown and I4
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			if (CheckParseFilter(state, sourceId, targetId, death: false))
			{
				string text = null;
				if (state.Combatants.TryGetValue(sourceId, out var value))
				{
					text = value.Job.ToString();
				}
				if (entry.SourceNameOverride == "Limit Break")
				{
					text = entry.SourceNameOverride;
				}
				string attacker = entry.SourceNameOverride ?? GetCombatantName(state, sourceId);
				string combatantName = GetCombatantName(state, targetId);
				long damage = entry.Amount;
				if (entry.EntryType == EffectEntryType.Miss)
				{
					damage = _actWrapper.Miss;
				}
				SwingType swingType = ((!entry.IsAutoAttack) ? SwingType.Damage : SwingType.Autoattack);
				string text2 = Enum.GetName(typeof(DamageType), entry.DamageType);
				if ((int)entry.ElementType != 0 && (int)entry.ElementType != 7)
				{
					text2 = text2 + " (" + Enum.GetName(typeof(ElementType), entry.ElementType) + ")";
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>
				{
					{ "potency", entry.AppliedPotencyMultiplier },
					{
						"StatusEffects",
						entry.AppliedStatusEffects ?? ""
					},
					{
						"CritRate",
						(entry.SourceCritRate * 100.0).ToString("0.00", CultureInfo.InvariantCulture)
					},
					{
						"DHRate",
						(entry.SourceDirectHitRate * 100.0).ToString("0.00", CultureInfo.InvariantCulture)
					},
					{
						"Job",
						(text == "Adv") ? "" : (text ?? "")
					},
					{
						"SourceId",
						$"{sourceId:X4}"
					},
					{
						"TargetId",
						$"{targetId:X4}"
					},
					{
						"CritEffects",
						entry.AppliedCritStatusList ?? ""
					},
					{
						"DHEffects",
						entry.AppliedDhStatusList ?? ""
					}
				};
				if (entry.IsDh)
				{
					dictionary.Add("DirectHit", "True");
				}
				AddCombatAction((int)swingType, entry.IsCritical, entry.SpecialDamage, damage, timestamp, _actWrapper.GlobalTimeSorter, entry.ProcActionName ?? entry.ActionName, attacker, text2, combatantName, dictionary);
			}
		}));
	}

	public void AddActionEntry(EnvironmentState state, DateTime timestamp, uint sourceId, uint targetId, string actionName)
	{
		_benchmarkRepository.Measure("ParseAddActionEntry", (Action)(() =>
		{
			if (CheckParseFilter(state, sourceId, targetId, death: false))
			{
				string combatantName = GetCombatantName(state, sourceId);
				string combatantName2 = GetCombatantName(state, targetId);
				if (_actWrapper.InCombat)
				{
					_actWrapper.AddCombatAction(1, false, "", _actWrapper.NoDamage, timestamp, _actWrapper.GlobalTimeSorter, actionName, combatantName, "", combatantName2, (Dictionary<string, object>)null);
				}
			}
		}));
	}

	public void AddDoTEntry(EnvironmentState state, DateTime timestamp, uint sourceId, uint targetId, DoTEffectEntry entry, bool isHoT)
	{
		_benchmarkRepository.Measure("ParseAddDoTEntry", (Action)(() =>
		{
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Invalid comparison between Unknown and I4
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			if (CheckParseFilter(state, sourceId, targetId, death: false) && (!isHoT || _actWrapper.InCombat))
			{
				string text = null;
				if (state.Combatants.TryGetValue(sourceId, out var value))
				{
					text = value.Job.ToString();
				}
				string attacker = entry.SourceNameOverride ?? GetCombatantName(state, sourceId);
				string combatantName = GetCombatantName(state, targetId);
				long damage = entry.Amount;
				if (entry.EntryType == EffectEntryType.Miss)
				{
					damage = _actWrapper.Miss;
				}
				SwingType swingType = (isHoT ? SwingType.HoT : SwingType.DoT);
				string text2 = Enum.GetName(typeof(DamageType), entry.DamageType);
				if ((int)entry.ElementType != 0 && (int)entry.ElementType != 7)
				{
					text2 = text2 + " (" + Enum.GetName(typeof(ElementType), entry.ElementType) + ")";
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>
				{
					{ "potency", entry.AppliedPotencyMultiplier },
					{
						"StatusEffects",
						entry.AppliedStatusEffects ?? ""
					},
					{
						"CritRate",
						(entry.SourceCritRate * 100.0).ToString("0.00", CultureInfo.InvariantCulture)
					},
					{
						"DHRate",
						(entry.SourceDirectHitRate * 100.0).ToString("0.00", CultureInfo.InvariantCulture)
					},
					{
						"Job",
						(text == "Adv") ? "" : (text ?? "")
					},
					{
						"SourceId",
						$"{sourceId:X4}"
					},
					{
						"TargetId",
						$"{targetId:X4}"
					},
					{ "dotbase", entry.DoTBaseDamage },
					{ "dotPotency", "" },
					{
						"CritEffects",
						entry.AppliedCritStatusList ?? ""
					},
					{
						"DHEffects",
						entry.AppliedDhStatusList ?? ""
					},
					{
						"BuffByte2",
						$"{entry.AmountByte:X2}"
					},
					{
						"BuffByte3",
						$"{entry.CritByte:X2}"
					}
				};
				if (entry.IsDh)
				{
					dictionary.Add("DirectHit", "True");
				}
				if (isHoT)
				{
					dictionary.Add("overheal", entry.OverHealAmount.ToString(CultureInfo.InvariantCulture));
				}
				AddCombatAction((int)swingType, entry.IsCritical, entry.SpecialDamage, damage, timestamp, _actWrapper.GlobalTimeSorter, entry.ProcActionName ?? entry.ActionName, attacker, text2, combatantName, dictionary);
			}
		}));
	}

	public void AddHealEntry(EnvironmentState state, DateTime timestamp, uint sourceId, uint targetId, HealEffectEntry entry)
	{
		_benchmarkRepository.Measure("ParseAddHealEntry", (Action)(() =>
		{
			if (_actWrapper.InCombat && CheckParseFilter(state, sourceId, targetId, death: false))
			{
				string text = null;
				if (state.Combatants.TryGetValue(sourceId, out var value))
				{
					text = value.Job.ToString();
				}
				if (entry.SourceNameOverride == "Limit Break")
				{
					text = entry.SourceNameOverride;
				}
				string attacker = entry.SourceNameOverride ?? GetCombatantName(state, sourceId);
				string combatantName = GetCombatantName(state, targetId);
				long damage = entry.Amount;
				if (entry.EntryType == EffectEntryType.Miss)
				{
					damage = _actWrapper.Miss;
				}
				Dictionary<string, object> tags = new Dictionary<string, object>
				{
					{ "potency", entry.AppliedPotencyMultiplier },
					{
						"StatusEffects",
						entry.AppliedStatusEffects ?? ""
					},
					{
						"CritEffects",
						entry.AppliedCritStatusList ?? ""
					},
					{
						"CritRate",
						(entry.SourceCritRate * 100.0).ToString("0.00", CultureInfo.InvariantCulture)
					},
					{
						"Job",
						(text == "Adv") ? "" : (text ?? "")
					},
					{
						"overheal",
						entry.OverHealAmount.ToString(CultureInfo.InvariantCulture)
					}
				};
				AddCombatAction(4, entry.IsCritical, "", damage, timestamp, _actWrapper.GlobalTimeSorter, entry.ProcActionName ?? entry.ActionName, attacker, "", combatantName, tags);
			}
		}));
	}

	public void AddDamageShieldEntry(EnvironmentState state, DateTime timestamp, DamageShieldEntry shieldEntry)
	{
		_benchmarkRepository.Measure("ParseAddDamageShieldEntry", (Action)(() =>
		{
			if (_actWrapper.InCombat && CheckParseFilter(state, shieldEntry.SourceId, shieldEntry.TargetId, death: false))
			{
				string combatantName = GetCombatantName(state, shieldEntry.SourceId);
				string combatantName2 = GetCombatantName(state, shieldEntry.TargetId);
				Dictionary<string, object> tags = new Dictionary<string, object> { 
				{
					"StatusEffects",
					shieldEntry.AppliedStatusEffects ?? ""
				} };
				AddCombatAction(11, Critical: false, "DamageShield", shieldEntry.Amount, timestamp, _actWrapper.GlobalTimeSorter, shieldEntry.ActionName, combatantName, "", combatantName2, tags);
			}
		}));
	}

	public void AddResourceEntry(EnvironmentState state, DateTime timestamp, uint sourceId, uint targetId, ResourceEffectEntry resourceEntry)
	{
		_benchmarkRepository.Measure("ParseAddResourceEntry", (Action)(() =>
		{
			if (_actWrapper.InCombat && CheckParseFilter(state, sourceId, targetId, death: false))
			{
				string combatantName = GetCombatantName(state, sourceId);
				string combatantName2 = GetCombatantName(state, targetId);
				Dictionary<string, object> tags = new Dictionary<string, object>();
				SwingType swingType = SwingType.Autoattack;
				if (resourceEntry.ResourceType == ResourceType.Power && resourceEntry.Increase)
				{
					swingType = SwingType.PowerHealing;
				}
				else if (resourceEntry.ResourceType == ResourceType.Power && !resourceEntry.Increase)
				{
					swingType = SwingType.PowerDrain;
				}
				else if (resourceEntry.ResourceType == ResourceType.Threat)
				{
					swingType = SwingType.Threat;
				}
				AddCombatAction((int)swingType, Critical: false, resourceEntry.ResourceType.ToString(), resourceEntry.Amount, timestamp, _actWrapper.GlobalTimeSorter, resourceEntry.ActionName, combatantName, "", combatantName2, tags);
			}
		}));
	}

	public void AddDeathEntry(EnvironmentState state, DateTime timestamp, uint sourceId, uint targetId)
	{
		_benchmarkRepository.Measure("ParseAddDeathEntry", (Action)(() =>
		{
			if (_actWrapper.InCombat && CheckParseFilter(state, sourceId, targetId, death: true))
			{
				string combatantName = GetCombatantName(state, sourceId);
				string combatantName2 = GetCombatantName(state, targetId);
				if (_settingsManager.ParseSettings.DisableCombinePets || !state.Combatants.TryGetValue(targetId, out var value) || !_nameResource.IsTurret(value.BNpcNameId))
				{
					AddCombatAction(1, Critical: false, "", _actWrapper.Death, timestamp, _actWrapper.GlobalTimeSorter, "Killing", combatantName, "", combatantName2, null);
				}
			}
		}));
	}

	public void AddStatusEffect(EnvironmentState state, DateTime timestamp, string statusName, uint sourceId, uint targetId, double Duration, ushort param, byte? extraByte0, byte? extraByte1, byte? extraByte2)
	{
		_benchmarkRepository.Measure("ParseAddStatusEntry", (Action)(() =>
		{
			if (CheckParseFilter(state, sourceId, targetId, death: false))
			{
				string text = null;
				if (state.Combatants.TryGetValue(sourceId, out var value))
				{
					text = value.Job.ToString();
				}
				string combatantName = GetCombatantName(state, sourceId);
				string combatantName2 = GetCombatantName(state, targetId);
				Dictionary<string, object> dictionary = new Dictionary<string, object>
				{
					{
						"Job",
						(text == "Adv") ? "" : (text ?? "")
					},
					{
						"SourceId",
						$"{sourceId:X4}"
					},
					{
						"TargetId",
						$"{targetId:X4}"
					},
					{ "StatusDuration", Duration },
					{
						"Params",
						$"{param:X2}"
					},
					{
						"BuffByte1",
						$"{extraByte0:X2}"
					},
					{
						"BuffByte2",
						$"{extraByte1:X2}"
					},
					{
						"BuffByte3",
						$"{extraByte2:X2}"
					}
				};
				if (_actWrapper.InCombat)
				{
					_actWrapper.AddCombatAction(8, false, "", _actWrapper.NoDamage, timestamp, _actWrapper.GlobalTimeSorter, statusName, combatantName, "", combatantName2, dictionary);
				}
			}
		}));
	}

	public void AddCancelEntry(EnvironmentState state, DateTime timestamp, uint sourceId, uint targetId, string actionName, string cancelType, bool isHeal, bool isDamage)
	{
		_benchmarkRepository.Measure("ParseAddCancelEntry", (Action)(() =>
		{
			if (_actWrapper.InCombat && CheckParseFilter(state, sourceId, targetId, death: false))
			{
				string combatantName = GetCombatantName(state, sourceId);
				string combatantName2 = GetCombatantName(state, targetId);
				AddCombatAction(isDamage ? 2 : ((!isHeal) ? 1 : 4), Critical: false, cancelType, _actWrapper.NoDamage, timestamp, _actWrapper.GlobalTimeSorter, actionName, combatantName, "", combatantName2, null);
			}
		}));
	}

	public void ChangeTerritory(string territoryName)
	{
		_actWrapper.ChangeZone(territoryName);
	}

	public void EchoCommand(string command)
	{
		_actWrapper.ActCommands(command);
	}

	private string GetCombatantName(EnvironmentState state, uint id)
	{
		if (id == state.PlayerId)
		{
			return _actWrapper.CharName;
		}
		if (state.Combatants.TryGetValue(id, out var combatant))
		{
			string text = (string.IsNullOrWhiteSpace(combatant.Name) ? "Unknown" : combatant.Name);
			if (combatant.BNpcNameId == 0 && !string.IsNullOrWhiteSpace(combatant.WorldName) && state.Combatants.Any((KeyValuePair<uint, CombatantState> x) => x.Key != id && string.Equals(x.Value.Name, combatant.Name, StringComparison.OrdinalIgnoreCase)))
			{
				text = text + "-" + combatant.WorldName;
			}
			string parentName = GetParentName(state, combatant.ParentId);
			if (!_settingsManager.ParseSettings.DisableCombinePets && !string.IsNullOrWhiteSpace(parentName) && _nameResource.IsPet(combatant.BNpcNameId))
			{
				return parentName;
			}
			if (string.IsNullOrWhiteSpace(parentName))
			{
				return text;
			}
			return text + " (" + parentName + ")";
		}
		return "Unknown";
	}

	private string GetParentName(EnvironmentState state, uint parentId)
	{
		string text = null;
		if (parentId != 0 && parentId != 3758096384u)
		{
			CombatantState parent;
			if (parentId == state.PlayerId)
			{
				text = _actWrapper.CharName;
			}
			else if (state.Combatants.TryGetValue(parentId, out parent))
			{
				text = state.Combatants[parentId].Name;
				if (parent.BNpcNameId == 0 && !string.IsNullOrWhiteSpace(parent.WorldName) && state.Combatants.Any((KeyValuePair<uint, CombatantState> x) => x.Key != parentId && string.Equals(x.Value.Name, parent.Name, StringComparison.OrdinalIgnoreCase)))
				{
					text = text + "-" + parent.WorldName;
				}
			}
		}
		return text;
	}

	private bool CheckParseFilter(EnvironmentState state, uint sourceId, uint targetId, bool death)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Invalid comparison between Unknown and I4
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Invalid comparison between Unknown and I4
		if ((int)_settingsManager.ParseSettings.ParseFilter == 0)
		{
			return true;
		}
		CombatantState combatantState = null;
		if (state.Combatants.ContainsKey(sourceId))
		{
			combatantState = state.Combatants[sourceId];
		}
		CombatantState combatantState2 = null;
		if (state.Combatants.ContainsKey(targetId))
		{
			combatantState2 = state.Combatants[targetId];
		}
		if (death && (combatantState?.Name == "unknown" || combatantState2?.Name == "unknown"))
		{
			return true;
		}
		if (combatantState?.Id == state.PlayerId || combatantState2?.Id == state.PlayerId)
		{
			return true;
		}
		if (combatantState?.ParentId == state.PlayerId || combatantState2?.ParentId == state.PlayerId)
		{
			return true;
		}
		if (state.PartyMembers == null || !state.PartyMembers.Any() || state.PartyCount == 0)
		{
			return false;
		}
		if ((int)_settingsManager.ParseSettings.ParseFilter == 2)
		{
			if (combatantState2 != null && state.PartyMembers.Take(state.PartyCount).Contains(combatantState2.Id))
			{
				return true;
			}
			if (combatantState != null && state.PartyMembers.Take(state.PartyCount).Contains(combatantState.Id))
			{
				return true;
			}
			if (combatantState2 != null && state.PartyMembers.Take(state.PartyCount).Contains(combatantState2.ParentId))
			{
				return true;
			}
			if (combatantState != null && state.PartyMembers.Take(state.PartyCount).Contains(combatantState.ParentId))
			{
				return true;
			}
		}
		else if ((int)_settingsManager.ParseSettings.ParseFilter == 3)
		{
			if (combatantState2 != null && state.PartyMembers.Contains(combatantState2.Id))
			{
				return true;
			}
			if (combatantState != null && state.PartyMembers.Contains(combatantState.Id))
			{
				return true;
			}
			if (combatantState2 != null && state.PartyMembers.Contains(combatantState2.ParentId))
			{
				return true;
			}
			if (combatantState != null && state.PartyMembers.Contains(combatantState.ParentId))
			{
				return true;
			}
		}
		return false;
	}

	private void AddCombatAction(int SwingType, bool Critical, string Special, long damage, DateTime Time, int TimeSorter, string theAttackType, string Attacker, string theDamageType, string Victim, Dictionary<string, object> Tags)
	{
		if (_actWrapper.SetEncounter(Time, Attacker, Victim))
		{
			_actWrapper.AddCombatAction(SwingType, Critical, Special, damage, Time, TimeSorter, theAttackType, Attacker, theDamageType, Victim, Tags);
		}
	}

	public void StopCombat(bool actExport)
	{
		if (_actWrapper.InCombat)
		{
			_actWrapper.EndCombat(actExport);
		}
	}
}
