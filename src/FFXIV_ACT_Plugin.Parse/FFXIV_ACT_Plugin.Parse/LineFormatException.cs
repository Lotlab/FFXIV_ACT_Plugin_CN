using System;

namespace FFXIV_ACT_Plugin.Parse;

public class LineFormatException : ApplicationException
{
	public string[] LineData { get; set; }

	public byte ExpectedSegmentCount { get; set; }

	public LineFormatException(string Message)
		: base(Message)
	{
	}
}
