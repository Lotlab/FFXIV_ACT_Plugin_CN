using System;
using System.Text;
using FFXIV_ACT_Plugin.Memory.MemoryReader;
using FFXIV_ACT_Plugin.Memory.Models.Global;

namespace FFXIV_ACT_Plugin.Memory.MemoryProcessors;

public class LogProcessor : ILogProcessor
{
	private readonly int c_MAX_LENGTH_ARRAY = 4000;

	private readonly int C_MAX_MESSAGE_LENGTH = 4096;

	private LogHeader _log;

	private readonly IProcessManager _processManager;

	private readonly IReadLog _readLog;

	private readonly DataEvent _dataEvent;

	private readonly IMobArrayProcessor _mobArray;

	public LogProcessor(IProcessManager processManager, IReadLog readLog, DataEvent dataEvent, IMobArrayProcessor mobArray)
	{
		_readLog = readLog;
		_dataEvent = dataEvent;
		_processManager = processManager;
		_mobArray = mobArray;
		_log = new LogHeader();
		_processManager.ProcessChanged += OnProcessChanged;
	}

	private void OnProcessChanged(object sender, EventArgs e)
	{
		_log = new LogHeader();
	}

	public unsafe void Refresh()
	{
		LogHeader log = _log;
		IntPtr intPtr = _readLog.ReadHeader();
		if (intPtr == IntPtr.Zero)
		{
			_log = new LogHeader();
			return;
		}
		LogHeaderStruct64* ptr = (LogHeaderStruct64*)(void*)intPtr;
		if (ProcessLogHeaderData(ptr->LenthArrayFirst, ptr->LenthArrayLast, ptr->MessageArrayFirst, ptr->MessageArrayLast, ref log))
		{
			_log.LengthArrayEnd = _log.LengthArrayStart + c_MAX_LENGTH_ARRAY;
			ProcessLog(_log);
		}
		_log = log;
		ProcessLog(_log);
	}

	private bool ProcessLogHeaderData(IntPtr LengthArrayFirst, IntPtr LengthArrayLast, IntPtr MessageArrayFirst, IntPtr MessageArrayLast, ref LogHeader log)
	{
		bool result = false;
		if (LengthArrayLast.ToInt64() >= LengthArrayFirst.ToInt64() && MessageArrayLast.ToInt64() >= MessageArrayFirst.ToInt64())
		{
			if (log.LengthArrayStart == IntPtr.Zero || log.LengthArrayEnd == IntPtr.Zero || log.MessageArrayStart == IntPtr.Zero || log.MessageArrayEnd == IntPtr.Zero)
			{
				log.NextUnprocessedIndex = (int)(LengthArrayLast.ToInt64() - LengthArrayFirst.ToInt64()) / 4;
			}
			else if ((int)(LengthArrayLast.ToInt64() - LengthArrayFirst.ToInt64()) / 4 < log.NextUnprocessedIndex)
			{
				result = _mobArray.PrimaryPlayerPointer != IntPtr.Zero;
				log = new LogHeader
				{
					NextUnprocessedIndex = 0
				};
			}
			log.LengthArrayStart = LengthArrayFirst;
			log.MessageArrayStart = MessageArrayFirst;
			log.LengthArrayEnd = LengthArrayLast;
			log.MessageArrayEnd = MessageArrayLast;
		}
		else
		{
			result = false;
			log = new LogHeader();
		}
		return result;
	}

	private unsafe void ProcessLog(LogHeader log)
	{
		int nextUnprocessedIndex = log.NextUnprocessedIndex;
		int num = (int)(log.LengthArrayEnd.ToInt64() - log.LengthArrayStart.ToInt64()) / 4;
		if (num > c_MAX_LENGTH_ARRAY / 4 || nextUnprocessedIndex == num)
		{
			return;
		}
		int num2 = num - nextUnprocessedIndex;
		uint* ptr = (uint*)(void*)_readLog.ReadLineLengthArray(log.LengthArrayStart + (nextUnprocessedIndex - ((nextUnprocessedIndex != 0) ? 1 : 0)) * 4, (num2 + ((nextUnprocessedIndex != 0) ? 1 : 0)) * 4);
		if (ptr == null)
		{
			return;
		}
		uint num3 = ((nextUnprocessedIndex != 0) ? (*ptr) : 0u);
		uint num4 = ptr[num2 - ((nextUnprocessedIndex == 0) ? 1 : 0)];
		if (num4 < num3)
		{
			return;
		}
		byte* ptr2 = (byte*)(void*)_readLog.ReadMessageArray(log.MessageArrayStart + (int)num3, (int)(num4 - num3));
		if (ptr2 == null)
		{
			return;
		}
		for (int i = 0; i <= num2; i++)
		{
			if (i != 0 || nextUnprocessedIndex == 0)
			{
				uint num5 = ((i != 0) ? ptr[i - 1] : 0u);
				uint num6 = ptr[i];
				if (num6 > num4 || num6 <= num5 || num6 - num5 >= C_MAX_MESSAGE_LENGTH || num6 - num5 <= 9)
				{
					break;
				}
				DecodeAndProcessLogLine(nextUnprocessedIndex + i, (IntPtr)(ptr2 + num5 - num3), (int)(num6 - num5));
			}
		}
		log.NextUnprocessedIndex = num;
	}

	private unsafe void DecodeAndProcessLogLine(int index, IntPtr messagePointer, int length)
	{
		char value = '\u001f';
		LogDetailStruct* ptr = (LogDetailStruct*)(void*)messagePointer;
		string text = ConvertLogLineToString(messagePointer, length);
		string player = "";
		if (text.IndexOf(value) > 0)
		{
			player = text.Substring(0, text.IndexOf(value));
		}
		text = text.Substring(text.IndexOf(value) + 1).Replace('\r', ' ').Replace('\n', ' ')
			.Replace('|', '❘');
		DateTime timestamp = DateTime.MinValue.AddYears(1969).AddSeconds(ptr->Seconds).ToLocalTime();
		if (timestamp.Year > 2020 && timestamp.Year < 2040 && ptr->EventType < 65535)
		{
			_dataEvent.OnLogLine(timestamp, ptr->EventType, ptr->Seconds, player, text);
		}
	}

	private unsafe string ConvertLogLineToString(IntPtr messagePointer, int length)
	{
		byte[] array = new byte[length];
		int num = 0;
		byte* ptr = (byte*)(void*)messagePointer;
		for (int i = sizeof(LogDetailStruct); i < length; i++)
		{
			if (ptr[i] == 2)
			{
				if (i + 2 > length)
				{
					break;
				}
				byte b = ptr[i + 2];
				if (b < 240)
				{
					i += b + 2;
				}
			}
			else
			{
				array[num] = ptr[i];
				num++;
			}
		}
		if (num <= 0)
		{
			return "";
		}
		return Encoding.UTF8.GetString(array, 0, num);
	}
}
