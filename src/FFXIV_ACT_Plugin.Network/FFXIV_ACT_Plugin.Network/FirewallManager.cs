#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WindowsFirewallHelper;

namespace FFXIV_ACT_Plugin.Network;

/// <summary>
/// Re-creates the Windows Firewall allow rules for the process hosting ACT.
///
/// This is only needed by the raw-socket capture path: Windows asks for
/// permission the first time ACT opens a raw socket, and a stale or missing rule
/// makes the capture silently receive nothing.  Npcap and deucalion do not use
/// the firewall, so the settings page also exposes this as a manual repair.
/// </summary>
public class FirewallManager
{
	private const string RuleName = "Advanced Combat Tracker";

	/// <summary>
	/// Removes every firewall rule named after ACT and adds a fresh
	/// allow-everything rule for the current executable on all profiles.
	/// </summary>
	public void FixFirewallRules()
	{
		try
		{
			string executable = Process.GetCurrentProcess().MainModule.FileName;
			List<IRule> stale = WindowsFirewallHelper.FirewallManager.Instance.Rules
				.Where(rule => rule.Name == RuleName).ToList();
			for (int i = stale.Count - 1; i >= 0; i--)
			{
				WindowsFirewallHelper.FirewallManager.Instance.Rules.Remove(stale[i]);
			}

			IRule rule = WindowsFirewallHelper.FirewallManager.Instance.CreateApplicationRule(
				FirewallProfiles.Domain | FirewallProfiles.Private | FirewallProfiles.Public,
				RuleName, FirewallAction.Allow, executable);
			WindowsFirewallHelper.FirewallManager.Instance.Rules.Add(rule);

			Trace.WriteLine($"Firewall rules for [{executable}] were re-created.", "FFXIV_ACT_Plugin");
		}
		catch (Exception ex)
		{
			Trace.WriteLine("Error updating firewall.  " +
				ex.ToString().Replace(Environment.NewLine, " "), "FFXIV_ACT_Plugin");
		}
	}

	/// <summary>
	/// True when a firewall this library can manage is present.
	/// </summary>
	public bool? IsFirewallEnabled()
	{
		return WindowsFirewallHelper.FirewallManager.Instance.IsSupported;
	}
}
