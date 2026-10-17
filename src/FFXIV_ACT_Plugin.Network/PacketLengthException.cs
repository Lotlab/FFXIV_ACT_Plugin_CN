using System;

namespace FFXIV_ACT_Plugin.Network;

public class PacketLengthException : ApplicationException
{
	public PacketLengthException(string message)
		: base(message)
	{
	}
}
