using System;
using System.Diagnostics;

namespace FFXIV_ACT_Plugin.Memory;

public class CurrentProcess
{
	private int _processId;

	public FFXIVClientMode ClientMode { get; set; }

	public IntPtr Handle { get; set; }

	public int ProcessId
	{
		get
		{
			return _processId;
		}
		set
		{
			_processId = value;
			if (_processId == 0)
			{
				Process = null;
			}
			else if (Process == null || Process.Id != _processId)
			{
				Process = Process.GetProcessById(ProcessId);
			}
		}
	}

	public IntPtr BaseAddress { get; set; }

	public int MaxModuleSize { get; set; }

	public bool IsAdministrator { get; set; }

	public Process Process { get; set; }

	public string GameVersion { get; set; }

	public string GameFile { get; set; }
}
