using System;
using System.Runtime.InteropServices;

namespace FFXIV_ACT_Plugin.Memory;

internal static class NativeMethods
{
	public struct MEMORY_BASIC_INFORMATION64
	{
		public ulong BaseAddress;

		public ulong AllocationBase;

		public int AllocationProtect;

		public int __alignment1;

		public ulong RegionSize;

		public int State;

		public int Protect;

		public int Type;

		public int __alignment2;
	}

	public struct MEMORY_BASIC_INFORMATION32
	{
		public IntPtr BaseAddress;

		public IntPtr AllocationBase;

		public uint AllocationProtect;

		public IntPtr RegionSize;

		public uint State;

		public uint Protect;

		public uint Type;
	}

	public enum AllocationProtect : uint
	{
		PAGE_EXECUTE = 16u,
		PAGE_EXECUTE_READ = 32u,
		PAGE_EXECUTE_READWRITE = 64u,
		PAGE_EXECUTE_WRITECOPY = 128u,
		PAGE_NOACCESS = 1u,
		PAGE_READONLY = 2u,
		PAGE_READWRITE = 4u,
		PAGE_WRITECOPY = 8u,
		PAGE_GUARD = 256u,
		PAGE_NOCACHE = 512u,
		PAGE_WRITECOMBINE = 1024u
	}

	[Flags]
	public enum ProcessAccessFlags : uint
	{
		All = 0x1F0FFFu,
		Terminate = 1u,
		CreateThread = 2u,
		VirtualMemoryOperation = 8u,
		VirtualMemoryRead = 0x10u,
		VirtualMemoryWrite = 0x20u,
		DuplicateHandle = 0x40u,
		CreateProcess = 0x80u,
		SetQuota = 0x100u,
		SetInformation = 0x200u,
		QueryInformation = 0x400u,
		QueryLimitedInformation = 0x1000u,
		Synchronize = 0x100000u
	}

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool GetExitCodeProcess(IntPtr hHandle, out uint lpExitCode);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, IntPtr nSize, ref IntPtr lpNumberOfBytesRead);

	[DllImport("kernel32.dll", EntryPoint = "VirtualQueryEx", SetLastError = true)]
	public static extern int VirtualQueryEx64(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION64 lpBuffer, uint dwLength);

	[DllImport("kernel32.dll", EntryPoint = "VirtualQueryEx", SetLastError = true)]
	public static extern int VirtualQueryEx32(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION32 lpBuffer, uint dwLength);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool CloseHandle(IntPtr hObject);
}
