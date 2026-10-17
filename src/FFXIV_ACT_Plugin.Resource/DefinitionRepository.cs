using System;
using System.Collections.Generic;
using System.Linq;
using FFXIV_ACT_Plugin.Resource.Definitions;
using Newtonsoft.Json;

namespace FFXIV_ACT_Plugin.Resource;

public class DefinitionRepository : IDefinitionRepository
{
	private readonly string _definitionFolderName = "FFXIV_ACT_Plugin.Resource.Definitions";

	private Dictionary<uint, FFXIV_ACT_Plugin.Resource.Definitions.Action> _actionEffectDefinitions;

	private Dictionary<uint, StatusEffect> _statusEffectDefinitions;

	public FFXIV_ACT_Plugin.Resource.Definitions.Action GetActionById(uint actionId)
	{
		if (_actionEffectDefinitions.ContainsKey(actionId))
		{
			return _actionEffectDefinitions[actionId];
		}
		return null;
	}

	public StatusEffect GetStatusEffectById(uint statusId)
	{
		if (_statusEffectDefinitions.ContainsKey(statusId))
		{
			return _statusEffectDefinitions[statusId];
		}
		return null;
	}

	public uint[] GetStatusIdsByReactiveProcType(ReactiveProc.ReactiveProcType procType)
	{
		return (from x in _statusEffectDefinitions.Where((KeyValuePair<uint, StatusEffect> x) =>
			{
				ReactiveProc reactiveProc = x.Value.ReactiveProc;
				return reactiveProc != null && reactiveProc.Type == procType;
			})
			select x.Key).ToArray();
	}

	public void LoadResources()
	{
		_actionEffectDefinitions = new Dictionary<uint, FFXIV_ACT_Plugin.Resource.Definitions.Action>();
		_statusEffectDefinitions = new Dictionary<uint, StatusEffect>();
		string[] allJobDefinitionFileNames = GetAllJobDefinitionFileNames();
		foreach (string fileName in allJobDefinitionFileNames)
		{
			JobDefinition jobDefinition = ReadJobDefinition(fileName);
			foreach (FFXIV_ACT_Plugin.Resource.Definitions.Action value in jobDefinition.actions.Values)
			{
				if (_actionEffectDefinitions.ContainsKey(value.Id))
				{
					throw new ResourceParseException($"Action Id {value.Id:X2} found for Job {jobDefinition.job} but already added.");
				}
				_actionEffectDefinitions.Add(value.Id, value);
			}
			foreach (StatusEffect value2 in jobDefinition.statuseffects.Values)
			{
				if (_statusEffectDefinitions.ContainsKey(value2.Id))
				{
					throw new ResourceParseException($"Status Id {value2.Id:X2} found for Job {jobDefinition.job} but already added.");
				}
				_statusEffectDefinitions.Add(value2.Id, value2);
			}
		}
		PopulateLimitToActionIds();
	}

	public void PopulateLimitToActionIds()
	{
		foreach (Potency item in _statusEffectDefinitions.Values.SelectMany((StatusEffect x) => x.PotencyEffects))
		{
			if (!string.IsNullOrWhiteSpace(item.LimitTo))
			{
				List<uint> list = new List<uint>();
				string[] array = item.LimitTo.Split(new char[1] { ',' });
				foreach (string abilityName in array)
				{
					uint[] collection = (from x in _actionEffectDefinitions
						where x.Value.Name.Equals(abilityName, StringComparison.OrdinalIgnoreCase)
						select x.Key).ToArray();
					list.AddRange(collection);
				}
				item.LimitToActionIds = list.ToArray();
			}
			else
			{
				item.LimitToActionIds = Array.Empty<uint>();
			}
		}
		foreach (Multiplier item2 in _statusEffectDefinitions.Values.SelectMany((StatusEffect x) => x.Multipliers))
		{
			if (!string.IsNullOrWhiteSpace(item2.LimitTo))
			{
				List<uint> list2 = new List<uint>();
				string[] array = item2.LimitTo.Split(new char[1] { ',' });
				foreach (string abilityName2 in array)
				{
					uint[] collection2 = (from x in _actionEffectDefinitions
						where x.Value.Name.Equals(abilityName2, StringComparison.OrdinalIgnoreCase)
						select x.Key).ToArray();
					list2.AddRange(collection2);
				}
				item2.LimitToActionIds = list2.ToArray();
			}
			else
			{
				item2.LimitToActionIds = Array.Empty<uint>();
			}
		}
	}

	public string[] GetAllJobDefinitionFileNames()
	{
		return (from x in typeof(DefinitionRepository).Module.Assembly.GetManifestResourceNames()
			where x.StartsWith(_definitionFolderName, StringComparison.OrdinalIgnoreCase)
			select "Definitions" + x.Substring(_definitionFolderName.Length)).ToArray();
	}

	public JobDefinition ReadJobDefinition(string fileName)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected Obj, but got Unknown
		string content = new ResourceFile(fileName).Content;
		try
		{
			return JsonConvert.DeserializeObject<JobDefinition>(content, new JsonSerializerSettings
			{
				MissingMemberHandling = (MissingMemberHandling)1
			});
		}
		catch (Exception arg)
		{
			throw new ResourceParseException($"Error while parsing definition file {fileName}: {arg}");
		}
	}
}
