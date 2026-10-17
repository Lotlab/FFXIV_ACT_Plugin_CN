using System;

namespace FFXIV_ACT_Plugin.Resource.Definitions;

public class JsonParseException : ApplicationException
{
	public JsonParseException(string message)
		: base(message)
	{
	}
}
