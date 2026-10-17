using System;

namespace FFXIV_ACT_Plugin.Parse;

public class LineParseException : ApplicationException
{
	public string[] LineData { get; set; }

	public LineParseException(string Message)
		: base(Message)
	{
	}
}
