using System.Collections.Generic;
using Newtonsoft.Json;

namespace FFXIV_ACT_Plugin.Resource.Definitions;

public class JobDefinition
{
	public string job { get; set; }

	[JsonConverter(typeof(JsonActionConverter))]
	public Dictionary<uint, Action> actions { get; set; }

	[JsonConverter(typeof(JsonStatusEffectConverter))]
	public Dictionary<uint, StatusEffect> statuseffects { get; set; }
}
