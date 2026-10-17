using System;
using System.Linq;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Resource;
using FFXIV_ACT_Plugin.Resource.Definitions;

namespace FFXIV_ACT_Plugin.Parse;

public class DamageShieldSimulator
{
	private readonly IStatusList _statusList;

	private readonly IPotencyStatusApplication _statusApplication;

	private readonly IReportCombatData _reportCombatData;

	private readonly ISettingsMediator _settingsMediator;

	public DamageShieldSimulator(IStatusList statusList, IPotencyStatusApplication potencyStatusApplication, IReportCombatData reportCombatData, ISettingsMediator settingsMediator)
	{
		_statusApplication = potencyStatusApplication;
		_statusList = statusList;
		_reportCombatData = reportCombatData;
		_settingsMediator = settingsMediator;
	}

	public void ProcessDamageShield(EnvironmentState state, StatusEffect effect, CombatantState source, CombatantState target)
	{
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Invalid comparison between Unknown and I4
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Invalid comparison between Unknown and I4
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Invalid comparison between Unknown and I4
		if (_settingsMediator.ParseSettings.DisableDamageShield)
		{
			return;
		}
		FFXIV_ACT_Plugin.Resource.Definitions.StatusEffect definition = effect.Definition;
		if (((definition != null) ? definition.DamageShield : null) == null || source == null || target == null)
		{
			return;
		}
		DamageShieldEntry damageShieldEntry = new DamageShieldEntry
		{
			SourceId = source.Id,
			TargetId = target.Id
		};
		if (effect.ActionEffect?.ActionId == ActionId.DeploymentTactics && source.ActionEffects.Any((ActionEffect x) => x.ActionId == ActionId.Adloquium || x.ActionId == ActionId.Succor))
		{
			effect.ActionEffect = source.ActionEffects.OrderByDescending((ActionEffect x) => x.Timestamp).FirstOrDefault((ActionEffect x) => x.ActionId == ActionId.Adloquium || x.ActionId == ActionId.Succor);
		}
		if ((int)effect.Definition.DamageShield.Type == 1)
		{
			if (effect.ActionEffect != null || target != null)
			{
				double amount = effect.Definition.DamageShield.Amount;
				ActionEffect actionEffect = effect.ActionEffect;
				damageShieldEntry.Amount = (uint)((double)((actionEffect != null && actionEffect.TargetMaxHp != 0) ? effect.ActionEffect.TargetMaxHp : target.MaxHp) * amount / 100.0);
			}
		}
		else if ((int)effect.Definition.DamageShield.Type == 2)
		{
			if (source != null && target != null)
			{
				double valueOrDefault = source.Tracking.HealPotency.GetValueOrDefault();
				if (valueOrDefault == 0.0)
				{
					valueOrDefault = source.Tracking.DamagePotency.GetValueOrDefault();
				}
				damageShieldEntry.CalculatedPotency = (uint)effect.Definition.DamageShield.Amount;
				_statusApplication.ApplyStatusEffects(effect.Timestamp, source, target, damageShieldEntry, state.ZoneId, effect.ActionEffect?.ActionId ?? 0, state.EchoBuff);
				damageShieldEntry.Amount = (uint)Math.Round(valueOrDefault * damageShieldEntry.CalculatedPotency * damageShieldEntry.CalculatedPotencyMultiplier);
			}
		}
		else if ((int)effect.Definition.DamageShield.Type == 3)
		{
			if (effect.ActionEffect?.Effects?.FirstOrDefault((EffectEntry x) => x.EntryType == EffectEntryType.Heal) is HealEffectEntry healEffectEntry)
			{
				if (effect.ActionEffect.ActionId == ActionId.Succor)
				{
					damageShieldEntry.Amount = (uint)(healEffectEntry.Value * 160 / 100);
				}
				else if (effect.ActionEffect.ActionId == ActionId.AspectedBenefic)
				{
					damageShieldEntry.Amount = (uint)(healEffectEntry.Value * 250 / 100);
				}
				else
				{
					damageShieldEntry.Amount = (uint)((double)(int)healEffectEntry.Value * effect.Definition.DamageShield.Amount / 100.0);
				}
			}
			else if (effect.ActionEffect?.ActionId == ActionId.HeartOfCorundrum && effect.StatusId == StatusId.BrutalShell)
			{
				HealEffectEntry healEffectEntry2 = source.StatusEffects.OrderByDescending((StatusEffect x) => x.Timestamp).FirstOrDefault((StatusEffect x) => x.StatusId == StatusId.BrutalShell)?.ActionEffect.Effects?.OfType<HealEffectEntry>().FirstOrDefault();
				if (healEffectEntry2 != null)
				{
					damageShieldEntry.Amount = (uint)((double)(int)healEffectEntry2.Value * effect.Definition.DamageShield.Amount / 100.0);
				}
			}
		}
		string statusNameByID = _statusList.GetStatusNameByID(effect.StatusId);
		damageShieldEntry.ActionName = statusNameByID + " (*)";
		_reportCombatData.AddDamageShieldEntry(state, effect.Timestamp, damageShieldEntry);
		if (damageShieldEntry.Amount == 0)
		{
			_settingsMediator.AddExceptionMessage(effect.Timestamp, $"Unable to simulate Damage Shield for Status Id {effect.StatusId:X2} on Target {target.Id:X4}.");
		}
	}
}
