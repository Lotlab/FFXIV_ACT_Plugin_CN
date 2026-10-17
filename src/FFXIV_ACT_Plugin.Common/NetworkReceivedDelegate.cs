namespace FFXIV_ACT_Plugin.Common;

public delegate void NetworkReceivedDelegate(string connection, long epoch, byte[] message);
