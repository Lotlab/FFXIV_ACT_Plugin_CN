using System.Runtime.InteropServices;

namespace FFXIV_ACT_Plugin.Memory.Models.Global;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct CombatantStruct
{
	[FieldOffset(0)]
	public CombatantHeader Header;

	[FieldOffset(3536)]
	public uint PCTargetID;

	[FieldOffset(8968)]
	public uint NPCTargetID;

	[FieldOffset(9016)]
	public uint BNpcNameID;

	[FieldOffset(9056)]
	public ushort CurrentWorldID;

	[FieldOffset(9058)]
	public ushort HomeWorldID;

	[FieldOffset(428)]
	public uint CurrentHP;

	[FieldOffset(432)]
	public uint MaxHP;

	[FieldOffset(436)]
	public uint CurrentMP;

	[FieldOffset(444)]
	public ushort CurrentGP;

	[FieldOffset(446)]
	public ushort MaxGP;

	[FieldOffset(448)]
	public ushort CurrentCP;

	[FieldOffset(450)]
	public ushort MaxCP;

	[FieldOffset(458)]
	public byte Job;

	[FieldOffset(459)]
	public byte Level;

	[FieldOffset(10128)]
	public byte IsCasting1;

	[FieldOffset(10129)]
	public byte IsCasting2;

	[FieldOffset(10132)]
	public uint CastBuffID;

	[FieldOffset(10144)]
	public uint CastTargetID;

	[FieldOffset(10180)]
	public float CastDurationCurrent;

	[FieldOffset(10184)]
	public float CastDurationMax;
}
