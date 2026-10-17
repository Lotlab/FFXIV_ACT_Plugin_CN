using System;
using System.IO;
using Advanced_Combat_Tracker;

namespace FFXIV_ACT_Plugin;

public class ACTWrapper
{
	internal static FormActMain.AttackTypeGraphGenerator GenerateAttackTypeGraph
	{
		get
		{
			return ActGlobals.oFormActMain?.GenerateAttackTypeGraph;
		}
		set
		{
			if (ActGlobals.oFormActMain?.GenerateAttackTypeGraph != null)
			{
				ActGlobals.oFormActMain.GenerateAttackTypeGraph = value;
			}
		}
	}

	internal static DirectoryInfo AppDataFolder => ActGlobals.oFormActMain?.AppDataFolder;

	internal static void AddUpdateCheckClickedDelegate(FormActMain.NullDelegate method)
	{
		ActGlobals.oFormActMain.UpdateCheckClicked += method;
	}

	internal static void RemoveUpdateCheckClickedDelegate(FormActMain.NullDelegate method)
	{
		ActGlobals.oFormActMain.UpdateCheckClicked -= method;
	}

	internal static ActPluginData PluginGetSelfData(IActPluginV1 MyPluginInstance)
	{
		return ActGlobals.oFormActMain.PluginGetSelfData(MyPluginInstance);
	}

	internal static FileInfo PluginDownload(int MyPluginId)
	{
		return ActGlobals.oFormActMain.PluginDownload(MyPluginId);
	}

	internal static void RunOnACTUIThread(Action code)
	{
		if (ActGlobals.oFormActMain.InvokeRequired)
		{
			if (!ActGlobals.oFormActMain.IsDisposed && !ActGlobals.oFormActMain.Disposing)
			{
				ActGlobals.oFormActMain.Invoke(code);
			}
		}
		else
		{
			code();
		}
	}

	internal static string PluginGetRemoteVersion(int MyPluginId)
	{
		return ActGlobals.oFormActMain.PluginGetRemoteVersion(MyPluginId);
	}

	internal static bool GetAutomaticUpdatesAllowed()
	{
		return ActGlobals.oFormActMain.GetAutomaticUpdatesAllowed();
	}
}
