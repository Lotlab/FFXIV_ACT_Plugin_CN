namespace FFXIV_ACT_Plugin.Common.Models;

public class Player
{
	public uint JobID { get; set; }

	public uint Str { get; set; }

	public uint Dex { get; set; }

	public uint Vit { get; set; }

	public uint Intel { get; set; }

	public uint Mnd { get; set; }

	public uint Pie { get; set; }

	public uint Attack { get; set; }

	public uint DirectHit { get; set; }

	public uint Crit { get; set; }

	public uint AttackMagicPotency { get; set; }

	public uint HealMagicPotency { get; set; }

	public uint Det { get; set; }

	public uint SkillSpeed { get; set; }

	public uint SpellSpeed { get; set; }

	public uint Tenacity { get; set; }

	public ulong LocalContentId { get; set; }

	public Player Clone()
	{
		return (Player)MemberwiseClone();
	}
}
