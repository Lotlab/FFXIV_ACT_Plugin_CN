using System;

namespace FFXIV_ACT_Plugin.Common.Models;

public class Combatant
{
	public NetworkBuff[] NetworkBuffs;

	public uint ID { get; set; }

	public uint OwnerID { get; set; }

	public byte type { get; set; }

	public int Job { get; set; }

	public int Level { get; set; }

	public string Name { get; set; }

	public uint CurrentHP { get; set; }

	public uint MaxHP { get; set; }

	public uint CurrentMP { get; set; }

	public uint MaxMP { get; set; }

	public uint CurrentCP { get; set; }

	public uint MaxCP { get; set; }

	public uint CurrentGP { get; set; }

	public uint MaxGP { get; set; }

	public bool IsCasting { get; set; }

	public uint CastBuffID { get; set; }

	public uint CastTargetID { get; set; }

	public float CastDurationCurrent { get; set; }

	public float CastDurationMax { get; set; }

	public float PosX { get; set; }

	public float PosY { get; set; }

	public float PosZ { get; set; }

	public float Heading { get; set; }

	public uint CurrentWorldID { get; set; }

	public uint WorldID { get; set; }

	public string WorldName { get; set; }

	public uint BNpcNameID { get; set; }

	public uint BNpcID { get; set; }

	public uint TargetID { get; set; }

	public byte EffectiveDistance { get; set; }

	public PartyType PartyType { get; set; }

	public IntPtr Address { get; set; }

	public int Order { get; set; }

	public Combatant()
	{
		NetworkBuffs = new NetworkBuff[30];
	}

	public Combatant Clone()
	{
		Combatant combatant = (Combatant)MemberwiseClone();
		if (NetworkBuffs != null)
		{
			combatant.NetworkBuffs = new NetworkBuff[NetworkBuffs.Length];
			for (int i = 0; i < NetworkBuffs.Length; i++)
			{
				combatant.NetworkBuffs[i] = NetworkBuffs[i].Clone();
			}
		}
		return combatant;
	}
}
