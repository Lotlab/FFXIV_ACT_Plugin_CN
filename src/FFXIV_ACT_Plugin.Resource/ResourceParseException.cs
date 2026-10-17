using System;

namespace FFXIV_ACT_Plugin.Resource;

public class ResourceParseException : ApplicationException
{
	public ResourceParseException(string message)
		: base(message)
	{
	}
}
