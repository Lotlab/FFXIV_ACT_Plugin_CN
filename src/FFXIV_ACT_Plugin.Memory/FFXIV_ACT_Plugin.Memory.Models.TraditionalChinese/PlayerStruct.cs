using System.Runtime.InteropServices;

namespace FFXIV_ACT_Plugin.Memory.Models.TraditionalChinese;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct PlayerStruct
{
	[FieldOffset(104)]
	public ulong LocalContentId;

	[FieldOffset(126)]
	public byte Job;

	[FieldOffset(396)]
	public uint Str;

	[FieldOffset(400)]
	public uint Dex;

	[FieldOffset(404)]
	public uint Vit;

	[FieldOffset(408)]
	public uint Int;

	[FieldOffset(412)]
	public uint Mnd;

	[FieldOffset(416)]
	public uint Pie;

	[FieldOffset(468)]
	public uint Tenacity;

	[FieldOffset(472)]
	public uint Attack;

	[FieldOffset(480)]
	public uint DirectHit;

	[FieldOffset(500)]
	public uint Crit;

	[FieldOffset(524)]
	public uint AttackMagicPotency;

	[FieldOffset(528)]
	public uint HealMagicPotency;

	[FieldOffset(568)]
	public uint Det;

	[FieldOffset(572)]
	public uint SkillSpeed;

	[FieldOffset(576)]
	public uint SpellSpeed;
}
