namespace FFXIV_ACT_Plugin.Common;

public delegate void NetworkSentDelegate(string connection, long epoch, byte[] message);
