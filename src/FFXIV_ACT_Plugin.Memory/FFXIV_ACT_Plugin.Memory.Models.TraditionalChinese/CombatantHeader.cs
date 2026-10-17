using System.Runtime.InteropServices;

namespace FFXIV_ACT_Plugin.Memory.Models.TraditionalChinese;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct CombatantHeader
{
	[FieldOffset(48)]
	public unsafe fixed byte Name[64];

	[FieldOffset(116)]
	public uint ID;

	[FieldOffset(128)]
	public uint BNpcID;

	[FieldOffset(132)]
	public uint OwnerID;

	[FieldOffset(140)]
	public byte Type;

	[FieldOffset(146)]
	public byte EffectiveDistance;

	[FieldOffset(160)]
	public float PosX;

	[FieldOffset(164)]
	public float PosZ;

	[FieldOffset(168)]
	public float PosY;

	[FieldOffset(176)]
	public float Heading;
}
