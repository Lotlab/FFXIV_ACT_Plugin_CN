#define TRACE
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Memory.MemoryReader;

namespace FFXIV_ACT_Plugin.Memory;

public class ProcessManager : IProcessManager, IDisposable
{
	private readonly object _processLock = new object();

	private readonly ISettingsMediator _settingsManager;

	private readonly IReadProcesses _readProcesses;

	private bool disposedValue;

	public CurrentProcess Current { get; set; }

	public event EventHandler ProcessChanged;

	public ProcessManager(ISettingsMediator settingsManager, IReadProcesses readProcesses)
	{
		_settingsManager = settingsManager;
		_readProcesses = readProcesses;
		Current = new CurrentProcess
		{
			Handle = IntPtr.Zero
		};
		ClearCurrent();
		SetIsAdministratorFlag();
	}

	public bool Verify()
	{
		int num = 0;
		FFXIVClientMode clientMode = FFXIVClientMode.Unknown;
		if (_settingsManager.DataCollectionSettings == null)
		{
			return false;
		}
		int selectedProcessId = _settingsManager.DataCollectionSettings.ProcessID;
		lock (_processLock)
		{
			if (Current.ProcessId != 0)
			{
				if (selectedProcessId > 0 && Current.ProcessId != selectedProcessId)
				{
					num = 0;
				}
				else if (_readProcesses.Verify(Current.Handle))
				{
					num = Current.ProcessId;
				}
			}
			if (num == 0)
			{
				clientMode = FFXIVClientMode.FFXIV_64;
				num = ((selectedProcessId != 0) ? _readProcesses.Read64(verifyMainModule: true).FirstOrDefault((int x) => x == selectedProcessId) : _readProcesses.Read64(verifyMainModule: true).FirstOrDefault());
			}
			if (num != Current.ProcessId)
			{
				if (num > 0)
				{
					LoadCurrentProcessFromId(num, clientMode);
				}
				else
				{
					ClearCurrent();
				}
				ProcessChanged?.Invoke(this, new EventArgs());
			}
			return Current.ProcessId > 0;
		}
	}

	public void ClearCurrent()
	{
		IntPtr handle = Current.Handle;
		Current.ClientMode = FFXIVClientMode.Unknown;
		Current.Handle = IntPtr.Zero;
		Current.ProcessId = 0;
		Current.BaseAddress = IntPtr.Zero;
		Current.MaxModuleSize = 0;
		Current.GameVersion = "";
		Current.GameFile = "";
		if (handle != IntPtr.Zero)
		{
			NativeMethods.CloseHandle(handle);
		}
	}

	public void LoadCurrentProcessFromId(int processId, FFXIVClientMode clientMode)
	{
		Current.ClientMode = clientMode;
		Current.Handle = NativeMethods.OpenProcess(NativeMethods.ProcessAccessFlags.VirtualMemoryRead | NativeMethods.ProcessAccessFlags.QueryInformation | NativeMethods.ProcessAccessFlags.Synchronize, bInheritHandle: false, processId);
		Current.ProcessId = processId;
		Current.BaseAddress = Current.Process.MainModule.BaseAddress;
		Current.MaxModuleSize = Current.Process.MainModule.ModuleMemorySize;
		Current.GameVersion = "";
		try
		{
			Current.GameFile = Current.Process.MainModule.FileName;
			if (File.Exists(Current.GameFile))
			{
				string text = Path.GetDirectoryName(Current.GameFile) + "\\ffxivgame.ver";
				if (File.Exists(text))
				{
					Current.GameVersion = File.ReadAllText(text);
				}
				else
				{
					Trace.WriteLine("Game Version file not found: " + text, "FFXIV_ACT_Plugin");
				}
			}
			else
			{
				Trace.WriteLine("Game executable file not found: " + Current.GameFile, "FFXIV_ACT_Plugin");
			}
		}
		catch (Exception ex)
		{
			Current.GameVersion = "";
			Trace.WriteLine("Unable to read game version file: " + ex.ToString().Replace(Environment.NewLine, " "), "FFXIV_ACT_Plugin");
		}
	}

	private void SetIsAdministratorFlag()
	{
		Current.IsAdministrator = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing && Current.Handle != IntPtr.Zero)
			{
				NativeMethods.CloseHandle(Current.Handle);
				Current.Handle = IntPtr.Zero;
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
