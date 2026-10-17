using System.Runtime.InteropServices;

namespace FFXIV_ACT_Plugin.Memory.Models.Global;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct PlayerStruct
{
	[FieldOffset(104)]
	public ulong LocalContentId;

	[FieldOffset(126)]
	public byte Job;

	[FieldOffset(428)]
	public uint Str;

	[FieldOffset(432)]
	public uint Dex;

	[FieldOffset(436)]
	public uint Vit;

	[FieldOffset(440)]
	public uint Int;

	[FieldOffset(444)]
	public uint Mnd;

	[FieldOffset(448)]
	public uint Pie;

	[FieldOffset(500)]
	public uint Tenacity;

	[FieldOffset(504)]
	public uint Attack;

	[FieldOffset(512)]
	public uint DirectHit;

	[FieldOffset(532)]
	public uint Crit;

	[FieldOffset(556)]
	public uint AttackMagicPotency;

	[FieldOffset(560)]
	public uint HealMagicPotency;

	[FieldOffset(600)]
	public uint Det;

	[FieldOffset(604)]
	public uint SkillSpeed;

	[FieldOffset(608)]
	public uint SpellSpeed;
}
