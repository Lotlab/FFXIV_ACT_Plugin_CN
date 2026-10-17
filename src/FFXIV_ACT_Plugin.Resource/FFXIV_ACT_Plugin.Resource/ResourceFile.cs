#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FFXIV_ACT_Plugin.Resource;

public class ResourceFile
{
	private readonly bool _idIsHex;

	private readonly string _fileName;

	public string Content => ReadFileAsString();

	public SortedDictionary<uint, string> Dictionary => LoadResourceDictionary();

	public ResourceFile(string fileName, bool isIdHex = true)
	{
		_idIsHex = isIdHex;
		_fileName = fileName;
	}

	private string ReadFileAsString()
	{
		string name = "FFXIV_ACT_Plugin.Resource." + _fileName;
		using Stream stream = typeof(ResourceFile).Module.Assembly.GetManifestResourceStream(name);
		using StreamReader streamReader = new StreamReader(stream);
		return streamReader.ReadToEnd();
	}

	private SortedDictionary<uint, string> LoadResourceDictionary()
	{
		SortedDictionary<uint, string> sortedDictionary = null;
		byte idBase = (byte)(_idIsHex ? 16u : 10u);
		try
		{
			sortedDictionary = new SortedDictionary<uint, string>((from x in (from x in Content.Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
					select x.Split(new string[1] { "|" }, StringSplitOptions.RemoveEmptyEntries)).ToArray()
				where x.Length == 2
				select x).ToDictionary((string[] x) => Convert.ToUInt32(x[0], idBase), (string[] x) => x[1]));
		}
		catch (Exception ex)
		{
			Trace.WriteLine("Unable to load resource file [" + _fileName + "]. Error: " + ex.ToString().Replace(Environment.NewLine, " "), "FFXIV_ACT_Plugin");
		}
		try
		{
			if (_fileName.StartsWith("generated.statuslist_", StringComparison.OrdinalIgnoreCase) || _fileName.StartsWith("generated.actionlist_", StringComparison.OrdinalIgnoreCase))
			{
				foreach (string item in from x in typeof(ResourceFile).Module.Assembly.GetManifestResourceNames()
					where x.StartsWith("ffxiv_act_plugin.resource.generated.override.global_", StringComparison.OrdinalIgnoreCase)
					select x)
				{
					foreach (KeyValuePair<uint, string> item2 in new SortedDictionary<string, string>((from x in (from x in new ResourceFile(item.Substring("ffxiv_act_plugin.resource.".Length), isIdHex: false).Content.Split(new string[3]
							{
								Environment.NewLine,
								"\r",
								"\n"
							}, StringSplitOptions.RemoveEmptyEntries)
							select x.Split(new string[1] { "|" }, StringSplitOptions.RemoveEmptyEntries)).ToArray()
						where x.Length == 2
						select x).ToDictionary((string[] x) => x[0], (string[] x) => x[1])).Join(sortedDictionary, (KeyValuePair<string, string> x) => x.Key.ToLowerInvariant(), (KeyValuePair<uint, string> x) => x.Value.ToLowerInvariant(), (KeyValuePair<string, string> x, KeyValuePair<uint, string> y) => new KeyValuePair<uint, string>(y.Key, x.Value)).ToList())
					{
						sortedDictionary[item2.Key] = item2.Value;
					}
				}
			}
		}
		catch (Exception ex2)
		{
			Trace.WriteLine("Unable to load override resource file for [" + _fileName + "]. Error: " + ex2.ToString().Replace(Environment.NewLine, " "), "FFXIV_ACT_Plugin");
		}
		foreach (KeyValuePair<uint, string> item3 in sortedDictionary.Where((KeyValuePair<uint, string> x) => x.Value.StartsWith("_rsv_", StringComparison.OrdinalIgnoreCase)))
		{
			Trace.WriteLine($"Resource File [{_fileName}] contains unobfuscated data: {item3.Key}|{item3.Value}");
		}
		return sortedDictionary;
	}
}
