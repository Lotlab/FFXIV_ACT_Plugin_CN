using FFXIV_ACT_Plugin.Common.Models;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Memory.MemoryReader;
using FFXIV_ACT_Plugin.Memory.Models.Global;
using FFXIV_ACT_Plugin.Memory.Models.TraditionalChinese;

namespace FFXIV_ACT_Plugin.Memory.MemoryProcessors;

public class PlayerProcessor : IPlayerProcessor
{
	private readonly object _playerLock = new object();

	private readonly Player _player;

	private readonly DataEvent _dataEvent;

	private readonly IReadPlayer _readPlayer;

	private readonly ISettingsMediator _settingsMediator;

	public Player Player
	{
		get
		{
			lock (_playerLock)
			{
				return _player;
			}
		}
	}

	public PlayerProcessor(IReadPlayer readPlayer, DataEvent dataEvent, ISettingsMediator settingsMediator)
	{
		_readPlayer = readPlayer;
		_dataEvent = dataEvent;
		_settingsMediator = settingsMediator;
		_player = new Player();
	}

	public void Refresh()
	{
		bool flag;
		if (_settingsMediator.DataCollectionSettings.RegionID == Region.Korean)
		{
			flag = Refresh_Korean();
		}
		else
		{
			flag = ((_settingsMediator.DataCollectionSettings.RegionID != Region.TraditionalChinese) ? Refresh_Global() : Refresh_TraditionalChinese());
		}
		if (flag)
		{
			_dataEvent.OnPlayerStatsChanged(_player);
		}
	}

	private unsafe bool Refresh_Global()
	{
		bool flag = false;
		FFXIV_ACT_Plugin.Memory.Models.Global.PlayerStruct* ptr = (FFXIV_ACT_Plugin.Memory.Models.Global.PlayerStruct*)(void*)_readPlayer.Read();
		if (ptr == null)
		{
			return flag;
		}
		lock (_playerLock)
		{
			flag |= _player.LocalContentId != ptr->LocalContentId;
			_player.LocalContentId = ptr->LocalContentId;
			flag |= _player.JobID != ptr->Job;
			_player.JobID = ptr->Job;
			flag |= _player.Str != ptr->Str;
			_player.Str = ptr->Str;
			flag |= _player.Dex != ptr->Dex;
			_player.Dex = ptr->Dex;
			flag |= _player.Vit != ptr->Vit;
			_player.Vit = ptr->Vit;
			flag |= _player.Intel != ptr->Int;
			_player.Intel = ptr->Int;
			flag |= _player.Mnd != ptr->Mnd;
			_player.Mnd = ptr->Mnd;
			flag |= _player.Pie != ptr->Pie;
			_player.Pie = ptr->Pie;
			flag |= _player.Attack != ptr->Attack;
			_player.Attack = ptr->Attack;
			flag |= _player.DirectHit != ptr->DirectHit;
			_player.DirectHit = ptr->DirectHit;
			flag |= _player.Crit != ptr->Crit;
			_player.Crit = ptr->Crit;
			flag |= _player.AttackMagicPotency != ptr->AttackMagicPotency;
			_player.AttackMagicPotency = ptr->AttackMagicPotency;
			flag |= _player.HealMagicPotency != ptr->HealMagicPotency;
			_player.HealMagicPotency = ptr->HealMagicPotency;
			flag |= _player.Det != ptr->Det;
			_player.Det = ptr->Det;
			flag |= _player.SkillSpeed != ptr->SkillSpeed;
			_player.SkillSpeed = ptr->SkillSpeed;
			flag |= _player.SpellSpeed != ptr->SpellSpeed;
			_player.SpellSpeed = ptr->SpellSpeed;
			flag |= _player.Tenacity != ptr->Tenacity;
			_player.Tenacity = ptr->Tenacity;
			return flag;
		}
	}

	private bool Refresh_Korean()
	{
		return Refresh_Global();
	}

	private unsafe bool Refresh_TraditionalChinese()
	{
		bool flag = false;
		FFXIV_ACT_Plugin.Memory.Models.TraditionalChinese.PlayerStruct* ptr = (FFXIV_ACT_Plugin.Memory.Models.TraditionalChinese.PlayerStruct*)(void*)_readPlayer.Read();
		if (ptr == null)
		{
			return flag;
		}
		lock (_playerLock)
		{
			flag |= _player.LocalContentId != ptr->LocalContentId;
			_player.LocalContentId = ptr->LocalContentId;
			flag |= _player.JobID != ptr->Job;
			_player.JobID = ptr->Job;
			flag |= _player.Str != ptr->Str;
			_player.Str = ptr->Str;
			flag |= _player.Dex != ptr->Dex;
			_player.Dex = ptr->Dex;
			flag |= _player.Vit != ptr->Vit;
			_player.Vit = ptr->Vit;
			flag |= _player.Intel != ptr->Int;
			_player.Intel = ptr->Int;
			flag |= _player.Mnd != ptr->Mnd;
			_player.Mnd = ptr->Mnd;
			flag |= _player.Pie != ptr->Pie;
			_player.Pie = ptr->Pie;
			flag |= _player.Attack != ptr->Attack;
			_player.Attack = ptr->Attack;
			flag |= _player.DirectHit != ptr->DirectHit;
			_player.DirectHit = ptr->DirectHit;
			flag |= _player.Crit != ptr->Crit;
			_player.Crit = ptr->Crit;
			flag |= _player.AttackMagicPotency != ptr->AttackMagicPotency;
			_player.AttackMagicPotency = ptr->AttackMagicPotency;
			flag |= _player.HealMagicPotency != ptr->HealMagicPotency;
			_player.HealMagicPotency = ptr->HealMagicPotency;
			flag |= _player.Det != ptr->Det;
			_player.Det = ptr->Det;
			flag |= _player.SkillSpeed != ptr->SkillSpeed;
			_player.SkillSpeed = ptr->SkillSpeed;
			flag |= _player.SpellSpeed != ptr->SpellSpeed;
			_player.SpellSpeed = ptr->SpellSpeed;
			flag |= _player.Tenacity != ptr->Tenacity;
			_player.Tenacity = ptr->Tenacity;
			return flag;
		}
	}

	public void Clear()
	{
		lock (_playerLock)
		{
			Player player = _player;
			Player player2 = _player;
			Player player3 = _player;
			Player player4 = _player;
			Player player5 = _player;
			Player player6 = _player;
			Player player7 = _player;
			Player player8 = _player;
			Player player9 = _player;
			Player player10 = _player;
			Player player11 = _player;
			Player player12 = _player;
			Player player13 = _player;
			Player player14 = _player;
			Player player15 = _player;
			Player player16 = _player;
			uint num = (_player.Tenacity = 0u);
			uint num3 = (player16.SpellSpeed = num);
			uint num5 = (player15.SkillSpeed = num3);
			uint num7 = (player14.Det = num5);
			uint num9 = (player13.HealMagicPotency = num7);
			uint num11 = (player12.AttackMagicPotency = num9);
			uint num13 = (player11.Crit = num11);
			uint num15 = (player10.DirectHit = num13);
			uint num17 = (player9.Attack = num15);
			uint num19 = (player8.Pie = num17);
			uint num21 = (player7.Mnd = num19);
			uint num23 = (player6.Intel = num21);
			uint num25 = (player5.Vit = num23);
			uint num27 = (player4.Dex = num25);
			uint num29 = (player3.Str = num27);
			uint num31 = (player2.JobID = num29);
			player.LocalContentId = num31;
		}
	}
}
