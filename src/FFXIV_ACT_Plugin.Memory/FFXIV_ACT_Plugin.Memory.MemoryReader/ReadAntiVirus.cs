#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Win32;

namespace FFXIV_ACT_Plugin.Memory.MemoryReader;

public class ReadAntiVirus : IReadAntiVirus
{
	public string[] Read()
	{
		List<string> list = new List<string>();
		try
		{
			using RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);
			using RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\Security Center\\Provider\\Av");
			string[] subKeyNames = registryKey2.GetSubKeyNames();
			foreach (string name in subKeyNames)
			{
				using RegistryKey registryKey3 = registryKey2.OpenSubKey(name);
				if (registryKey3 != null)
				{
					string item = registryKey3.GetValue("DISPLAYNAME")?.ToString();
					list.Add(item);
				}
			}
		}
		catch (Exception arg)
		{
			Trace.WriteLine($"Error when reading registry: {arg}", "FFXIV_ACT_Plugin");
		}
		return list.ToArray();
	}
}
