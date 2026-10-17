#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Memory.MemoryReader;

namespace FFXIV_ACT_Plugin.Memory;

public class SignatureManager : ISignatureManager
{
	private Dictionary<SignatureType, uint> _offsets;

	private Dictionary<SignatureType, IntPtr> _vtableSignatures;

	private readonly object _signatureLock = new object();

	private Dictionary<SignatureType, int[]> _signatures64;

	private readonly IProcessManager _processManager;

	private readonly IReadSignature _readSignature;

	private readonly IReadVtable _readVtable;

	private readonly ISettingsMediator _settingsMediator;

	public SignatureManager(IProcessManager processManager, IReadSignature readSignature, IReadVtable readVtable, ISettingsMediator settingsMediator)
	{
		_processManager = processManager;
		_readSignature = readSignature;
		_readVtable = readVtable;
		_settingsMediator = settingsMediator;
		_processManager.ProcessChanged += OnProcessChanged;
		ISettingsMediator settingsMediator2 = _settingsMediator;
		settingsMediator2.RestartDataCollection = (Action)Delegate.Combine(settingsMediator2.RestartDataCollection, new Action(OnRestartDataCollection));
	}

	private void OnRestartDataCollection()
	{
		OnProcessChanged(null, null);
	}

	private void OnProcessChanged(object sender, EventArgs e)
	{
		LoadSignatureResource();
		RefreshSignatures();
		RefreshVtable();
	}

	public IntPtr Read(SignatureType signatureType)
	{
		lock (_signatureLock)
		{
			uint value3;
			if (signatureType == SignatureType.ChatLog)
			{
				if (_vtableSignatures != null && _vtableSignatures.TryGetValue(signatureType, out var value))
				{
					return value;
				}
				RefreshVtable();
				if (_vtableSignatures != null && _vtableSignatures.TryGetValue(signatureType, out var value2))
				{
					return value2;
				}
			}
			else if (_offsets != null && _offsets.TryGetValue(signatureType, out value3))
			{
				uint offset = value3;
				return IntPtr.Add(_processManager.Current.BaseAddress, (int)offset);
			}
		}
		return IntPtr.Zero;
	}

	public string ValidateWithMessage()
	{
		lock (_signatureLock)
		{
			if (_offsets == null)
			{
				return "FFXIV Signatures not loaded.";
			}
			SignatureType[] array = (SignatureType[])Enum.GetValues(typeof(SignatureType));
			for (int i = 0; i < array.Length; i++)
			{
				if (!_offsets.ContainsKey(array[i]))
				{
					return $"One or more FFXIV Signatures not found - {array[i]}";
				}
			}
			if (_vtableSignatures == null)
			{
				return "Cannot find FFXIV Chatlog VTable signature.";
			}
			if (!_vtableSignatures.ContainsKey(SignatureType.ChatLog))
			{
				return "Cannot find FFXIV Chatlog memory offset.";
			}
		}
		return string.Empty;
	}

	private void RefreshSignatures()
	{
		Dictionary<SignatureType, uint> offsets = _readSignature.Read(_signatures64);
		lock (_signatureLock)
		{
			_vtableSignatures = null;
			_offsets = offsets;
		}
	}

	private void RefreshVtable()
	{
		lock (_signatureLock)
		{
			if (_processManager.Current.ClientMode == FFXIVClientMode.FFXIV_64)
			{
				_vtableSignatures = _readVtable.Read(_offsets, new List<SignatureType> { SignatureType.ChatLog });
			}
			else
			{
				_vtableSignatures = null;
			}
		}
	}

	private void LoadSignatureResource()
	{
		try
		{
			string name = "FFXIV_ACT_Plugin.Memory.Signatures_Global.txt";
			if (_settingsMediator.DataCollectionSettings.RegionID == Region.TraditionalChinese)
			{
				name = "FFXIV_ACT_Plugin.Memory.Signatures_TraditionalChinese.txt";
			}
			using Stream stream = typeof(SignatureManager).Module.Assembly.GetManifestResourceStream(name);
			using StreamReader streamReader = new StreamReader(stream);
			string[][] source = (from x in streamReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
				select x.Split(new string[1] { "|" }, StringSplitOptions.RemoveEmptyEntries) into x
				where x.Length == 2
				select x).ToArray();
			_signatures64 = source.ToDictionary((string[] x) => (SignatureType)Enum.Parse(typeof(SignatureType), x[0]), (string[] x) => SignatureStringToByteArray(x[1]));
		}
		catch (Exception ex)
		{
			Trace.WriteLine("Unable to load signature resource data: " + ex.ToString().Replace(Environment.NewLine, " "), "FFXIV_ACT_Plugin");
		}
	}

	private int[] SignatureStringToByteArray(string pattern)
	{
		int[] array = new int[pattern.Length / 2];
		for (int i = 0; i < pattern.Length / 2; i++)
		{
			string text = pattern.Substring(i * 2, 2);
			if (text == "**")
			{
				array[i] = -1;
			}
			else
			{
				array[i] = Convert.ToByte(text, 16);
			}
		}
		return array;
	}
}
