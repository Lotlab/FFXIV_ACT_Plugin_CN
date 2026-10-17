using System;
using System.Linq;
using FFXIV_ACT_Plugin.Resource;

namespace FFXIV_ACT_Plugin.Parse.EffectEntryStrategy;

public class ActionNameSourceRedirect : IEffectEntryStrategy
{
	private readonly IActionList _actionlist;

	private readonly uint[] _actionIdRedirectList = new uint[7]
	{
		ActionId.OmegaJammer,
		ActionId.Exhaust,
		ActionId.Exhaust2,
		ActionId.HeatShock1,
		ActionId.HeatShock2,
		ActionId.ColdShock1,
		ActionId.ColdShock2
	};

	public Type[] EffectEntryTypes => new Type[2]
	{
		typeof(DamageEffectEntry),
		typeof(HealEffectEntry)
	};

	public int Order => 1;

	public ActionNameSourceRedirect(IActionList actionList)
	{
		_actionlist = actionList;
	}

	public void Apply<T>(EnvironmentState state, ActionEffect actionEffect, T entry) where T : EffectEntry
	{
		if (Enumerable.Contains(_actionIdRedirectList, actionEffect.ActionId))
		{
			string actionNameByID = _actionlist.GetActionNameByID(actionEffect.ActionId);
			if (entry is DamageEffectEntry damageEffectEntry)
			{
				damageEffectEntry.SourceNameOverride = actionNameByID;
			}
			else if (entry is HealEffectEntry healEffectEntry)
			{
				healEffectEntry.SourceNameOverride = actionNameByID;
			}
		}
	}
}
