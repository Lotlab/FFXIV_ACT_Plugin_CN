#define TRACE
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Advanced_Combat_Tracker;

namespace FFXIV_ACT_Plugin;

public class ACT_PluginUpdate : IDisposable
{
	private readonly int m_PluginId = 73;

	private Thread _updateThread;

	private bool disposedValue;

	public IActPluginV1 Plugin { get; set; }

	public ACT_PluginUpdate()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected Obj, but got Unknown
		ACTWrapper.AddUpdateCheckClickedDelegate(UpdateCheckClicked_Version);
	}

	public void StartUpdateCheck()
	{
		if (ACTWrapper.GetAutomaticUpdatesAllowed())
		{
			_updateThread = new Thread(UpdateCheckClicked_Version);
			_updateThread.IsBackground = true;
			_updateThread.Start();
		}
	}

	public void UpdateCheckClicked_Version()
	{
	}

	public void FinalizeDeucalionDistrib()
	{
		string[] files = Directory.GetFiles(ACTWrapper.PluginGetSelfData(Plugin).pluginFile.DirectoryName);
		foreach (string text in files)
		{
			string name = new FileInfo(text).Name;
			if (!name.StartsWith("deucalion", StringComparison.InvariantCultureIgnoreCase) || !name.EndsWith(".distrib.dll", StringComparison.InvariantCultureIgnoreCase))
			{
				continue;
			}
			string text2 = text.ToLowerInvariant().Replace(".distrib.dll", ".dll");
			try
			{
				if (File.Exists(text2))
				{
					File.Delete(text2);
					Trace.WriteLine("已删除原本的deucalion的dll文件[" + text2 + "]以进行替换更新", "FFXIV_ACT_Plugin");
				}
			}
			catch (Exception arg)
			{
				Trace.WriteLine(string.Format("无法删除deucalion的dll文件[{0}]并进行替换更新，", text2, arg), "FFXIV_ACT_Plugin");
			}
			try
			{
				File.Move(text, text2);
				Trace.WriteLine("已重命名原文件[" + text + "]为[" + text2 + "]。", "FFXIV_ACT_Plugin");
			}
			catch (Exception arg2)
			{
				Trace.WriteLine(string.Format("无法移动新版本deucalion的dll文件[{0}]到[{1}]。", text, text2, arg2), "FFXIV_ACT_Plugin");
			}
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected Obj, but got Unknown
		if (!disposedValue)
		{
			if (disposing)
			{
				ACTWrapper.RemoveUpdateCheckClickedDelegate(UpdateCheckClicked_Version);
			}
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
