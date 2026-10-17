namespace FFXIV_ACT_Plugin.Network;

public class NetworkMarker
{
	public enum MarkerEvent
	{
		Add = 1,
		Update,
		Delete
	}

	public enum MarkerTarget
	{
		Ground = 1,
		Target
	}

	public MarkerTarget MarkerType { get; set; }

	public uint Marker { get; set; }

	public float PosX { get; set; }

	public float PosY { get; set; }

	public float PosZ { get; set; }

	public uint TargetID { get; set; }

	public uint ActorID { get; set; }
}
