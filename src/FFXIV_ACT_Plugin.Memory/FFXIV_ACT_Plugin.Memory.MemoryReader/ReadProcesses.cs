#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace FFXIV_ACT_Plugin.Memory.MemoryReader;

public class ReadProcesses : IReadProcesses
{
	private const string _app_name64 = "ffxiv_dx11";

	private DateTime _lastErrorMessage = DateTime.MinValue;

	private DateTime _lastInvalidHandleErrorMessage = DateTime.MinValue;

	public List<int> Read64(bool verifyMainModule)
	{
		List<int> list = new List<int>();
		try
		{
			Process[] processesByName = Process.GetProcessesByName("ffxiv_dx11");
			foreach (Process process in processesByName)
			{
				IntPtr intPtr = IntPtr.Zero;
				Trace.WriteLine($"Process {process.ProcessName} ({process.Id}) {process.MainWindowTitle} started on {process.StartTime} Found, Verifying.", "FFXIV_ACT_Plugin");
				try
				{
					intPtr = NativeMethods.OpenProcess(NativeMethods.ProcessAccessFlags.VirtualMemoryRead | NativeMethods.ProcessAccessFlags.QueryInformation | NativeMethods.ProcessAccessFlags.Synchronize, bInheritHandle: false, process.Id);
					if (intPtr == IntPtr.Zero && DateTime.UtcNow.Subtract(_lastInvalidHandleErrorMessage).TotalSeconds > 30.0)
					{
						int lastWin32Error = Marshal.GetLastWin32Error();
						_lastInvalidHandleErrorMessage = DateTime.UtcNow;
						Trace.WriteLine($"Found FFXIV Game process id [{process.Id}] but cannot retrieve process handle.  Win32 Error [{lastWin32Error:X8}].", "FFXIV_ACT_Plugin");
					}
					if (Verify(intPtr) && (!verifyMainModule || (process?.MainModule?.ModuleName == "ffxiv_dx11.exe" && HasGameVersion(process.Id, process?.MainModule?.FileName))))
					{
						list.Add(process.Id);
						Trace.WriteLine($"Process {process.ProcessName} ({process.Id}) Verified.", "FFXIV_ACT_Plugin");
					}
				}
				catch (Exception)
				{
					throw;
				}
				finally
				{
					if (intPtr != IntPtr.Zero)
					{
						NativeMethods.CloseHandle(intPtr);
					}
				}
			}
		}
		catch (Exception ex2)
		{
			if (DateTime.UtcNow.Subtract(_lastErrorMessage).TotalSeconds > 10.0)
			{
				_lastErrorMessage = DateTime.UtcNow;
				Trace.WriteLine("ReadProcesses.GetProcesses() error: " + ex2.ToString().Replace(Environment.NewLine, ""), "FFXIV_ACT_Plugin");
			}
		}
		return list;
	}

	public bool Verify(IntPtr processHandle)
	{
		if (processHandle == IntPtr.Zero)
		{
			return false;
		}
		switch (NativeMethods.WaitForSingleObject(processHandle, 0u))
		{
		case 258u:
			return true;
		case uint.MaxValue:
			Marshal.GetLastWin32Error();
			break;
		}
		return false;
	}

	public bool HasGameVersion(int processId, string fileName)
	{
		try
		{
			if (File.Exists(fileName))
			{
				string text = Path.GetDirectoryName(fileName) + "\\ffxivgame.ver";
				if (File.Exists(text))
				{
					Trace.WriteLine($"Found possible FFXIV process {processId} with version file {text}.", "FFXIV_ACT_Plugin");
					return true;
				}
				Trace.WriteLine($"Found possible FFXIV process {processId} but {text} does not exist. Ignoring.", "FFXIV_ACT_Plugin");
			}
			else
			{
				Trace.WriteLine($"Found possible FFXIV process {processId} but {fileName} does not exist.", "FFXIV_ACT_Plugin");
			}
		}
		catch (Exception ex)
		{
			Trace.WriteLine("Unable to find or read game version file: " + ex.ToString().Replace(Environment.NewLine, " "), "FFXIV_ACT_Plugin");
		}
		return false;
	}
}
