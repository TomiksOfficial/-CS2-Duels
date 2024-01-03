using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;

namespace Duels.components;

public partial class DuelState
{
	public void OnDuelVoteMenu(CCSPlayerController player, CCSPlayerController opponent)
	{
		state = DuelState_t.DUEL_VOTE;
		
		ChatMenu menu = new ChatMenu($"Duel Vote [{opponent.PlayerName}]");

		menu.AddMenuOption("Accept", (ply, option) =>
		{
			VoteState[ply.Slot] = true;

			if (VoteState.TryGetValue(opponent.Slot, out bool val) && val)
			{
				// StartDuel(ply, opponent);

				returnWeapon[ply.Slot] = new List<string>();
				returnWeapon[opponent.Slot] = new List<string>();
				
				OnWeaponChooseMenu(ply, opponent);
				OnWeaponChooseMenu(opponent, ply);
			}
		});
		
		menu.AddMenuOption("Cancel", (ply, option) =>
		{
			Stock.ChangePlayerMoveType(ply, MoveType_t.MOVETYPE_WALK);
			
			if(Stock.IsValidPlayer(opponent) && opponent.PawnIsAlive)
			{
				Stock.ChangePlayerMoveType(opponent, MoveType_t.MOVETYPE_WALK);
			}

			state = DuelState_t.DUEL_NOT;
		});
		
		ChatMenus.OpenMenu(player, menu);
	}

	public void OnWeaponChooseMenu(CCSPlayerController player, CCSPlayerController opponent)
	{
		ChatMenu menu = new ChatMenu("Choose weapon");

		foreach (var (weaponDifinition, weaponName) in Stock.weaponList)
		{
			menu.AddMenuOption(weaponName, (ply, option) =>
			{
				// Add save previous weapon
				
				// ply.GiveNamedItem(weaponDifinition);

				WeaponChoose[ply.Slot] = weaponDifinition;
				
				if(WeaponChoose.TryGetValue(opponent.Slot, out var val) && val.Length > 0)
				{
					StartDuel(ply, opponent);
				}
			});
		}
		
		ChatMenus.OpenMenu(player, menu);
	}
}