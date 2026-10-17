using System;

namespace FFXIV_ACT_Plugin.Parse;

public class LookupException : ApplicationException
{
	public string[] LineData { get; set; }

	public LookupException(string Message)
		: base(Message)
	{
	}
}
