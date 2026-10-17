using System.Runtime.InteropServices;

namespace FFXIV_ACT_Plugin.Memory.Models.TraditionalChinese;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct CombatantStruct
{
	[FieldOffset(0)]
	public CombatantHeader Header;

	[FieldOffset(3392)]
	public uint PCTargetID;

	[FieldOffset(8824)]
	public uint NPCTargetID;

	[FieldOffset(8868)]
	public uint BNpcNameID;

	[FieldOffset(8904)]
	public ushort CurrentWorldID;

	[FieldOffset(8906)]
	public ushort HomeWorldID;

	[FieldOffset(412)]
	public uint CurrentHP;

	[FieldOffset(416)]
	public uint MaxHP;

	[FieldOffset(420)]
	public uint CurrentMP;

	[FieldOffset(428)]
	public ushort CurrentGP;

	[FieldOffset(430)]
	public ushort MaxGP;

	[FieldOffset(432)]
	public ushort CurrentCP;

	[FieldOffset(434)]
	public ushort MaxCP;

	[FieldOffset(442)]
	public byte Job;

	[FieldOffset(443)]
	public byte Level;

	[FieldOffset(9984)]
	public byte IsCasting1;

	[FieldOffset(9986)]
	public byte IsCasting2;

	[FieldOffset(9988)]
	public uint CastBuffID;

	[FieldOffset(10000)]
	public uint CastTargetID;

	[FieldOffset(10036)]
	public float CastDurationCurrent;

	[FieldOffset(10040)]
	public float CastDurationMax;
}
