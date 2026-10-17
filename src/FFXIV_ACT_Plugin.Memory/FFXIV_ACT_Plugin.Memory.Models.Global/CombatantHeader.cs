using System.Runtime.InteropServices;

namespace FFXIV_ACT_Plugin.Memory.Models.Global;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct CombatantHeader
{
	[FieldOffset(48)]
	public unsafe fixed byte Name[64];

	[FieldOffset(120)]
	public uint ID;

	[FieldOffset(132)]
	public uint BNpcID;

	[FieldOffset(136)]
	public uint OwnerID;

	[FieldOffset(144)]
	public byte Type;

	[FieldOffset(150)]
	public byte EffectiveDistance;

	[FieldOffset(176)]
	public float PosX;

	[FieldOffset(180)]
	public float PosZ;

	[FieldOffset(184)]
	public float PosY;

	[FieldOffset(192)]
	public float Heading;
}
