using System;
using System.Collections.ObjectModel;
using FFXIV_ACT_Plugin.Memory.MemoryReader;

namespace FFXIV_ACT_Plugin.Memory.MemoryProcessors;

public class MobArrayProcessor : IMobArrayProcessor
{
	private IntPtr[] _internalMmobArray;

	private readonly IProcessManager _processManager;

	private readonly IReadMobArray _readMobArray;

	public ReadOnlyCollection<IntPtr> MobArray { get; private set; }

	public IntPtr PrimaryPlayerPointer
	{
		get
		{
			if (MobArray.Count != 0)
			{
				return MobArray[0];
			}
			return IntPtr.Zero;
		}
	}

	public MobArrayProcessor(IProcessManager processManager, IReadMobArray readMobArrayy)
	{
		_processManager = processManager;
		_readMobArray = readMobArrayy;
		_internalMmobArray = Array.Empty<IntPtr>();
		MobArray = Array.AsReadOnly(_internalMmobArray);
		processManager.ProcessChanged += OnProcessChanged;
	}

	private void OnProcessChanged(object sender, EventArgs e)
	{
		int num = 200;
		_internalMmobArray = new IntPtr[num];
		MobArray = Array.AsReadOnly(_internalMmobArray);
	}

	public unsafe void Refresh()
	{
		ulong* ptr = (ulong*)(void*)_readMobArray.Read64();
		for (int i = 0; i < _internalMmobArray.Length; i++)
		{
			_internalMmobArray[i] = ((ptr == null) ? IntPtr.Zero : new IntPtr((long)ptr[i]));
		}
	}
}
