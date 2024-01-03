using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Timers;
using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace Duels.components;

public partial class DuelState
{
	public enum DuelState_t
	{
		DUEL_VOTE,
		DUEL_NOT,
		DUEL_START,
		DUEL_IN_PROCESS,
		DUEL_END
	}
	
	public Dictionary<string, List<List<float>>> duelPositions = new Dictionary<string, List<List<float>>>();
	private readonly Dictionary<int, bool> VoteState = new Dictionary<int, bool>();
	private readonly Dictionary<int, string> WeaponChoose = new Dictionary<int, string>();
	public readonly Dictionary<int, List<string>> returnWeapon = new Dictionary<int, List<string>>();
	public DuelState_t state = DuelState_t.DUEL_NOT;
	private CCSPlayerController? FirstDuelist;
	private CCSPlayerController? SecondDuelist;

	private void StartDuel(CCSPlayerController player, CCSPlayerController opponent)
	{
		if(state is DuelState_t.DUEL_NOT)
		{
			VoteState.Clear();

			FirstDuelist = player;
			SecondDuelist = opponent;

			FirstDuelist!.PlayerPawn.Value!.Health = 100;
			SecondDuelist!.PlayerPawn.Value!.Health = 100;
			
			Utilities.SetStateChanged(FirstDuelist!.PlayerPawn.Value, "CBaseEntity", "m_iHealth");
			Utilities.SetStateChanged(SecondDuelist!.PlayerPawn.Value, "CBaseEntity", "m_iHealth");

			var rand = new List<int>
			{
				FirstDuelist.Slot,
				SecondDuelist.Slot
			};
			
			foreach (var wpn in FirstDuelist.PlayerPawn.Value!.WeaponServices!.MyWeapons)
			{
				if (wpn is { IsValid: true, Value.IsValid: true })
				{
					if(wpn.Value.AttributeManager.Item.ItemDefinitionIndex != (int)ItemDefinition.C4_EXPLOSIVE)
					{
						if (wpn.Value.AttributeManager.Item.ItemDefinitionIndex == (int)ItemDefinition.USP_S)
						{
							returnWeapon[FirstDuelist.Slot].Add("weapon_usp_silencer");
						} else
						{
							returnWeapon[FirstDuelist.Slot].Add(wpn.Value.DesignerName);
						}
					}
					wpn.Value.Remove();
				}
			}
			
			foreach (var wpn in SecondDuelist.PlayerPawn.Value!.WeaponServices!.MyWeapons)
			{
				if (wpn is { IsValid: true, Value.IsValid: true })
				{
					if(wpn.Value.AttributeManager.Item.ItemDefinitionIndex != (int)ItemDefinition.C4_EXPLOSIVE)
					{
						if (wpn.Value.AttributeManager.Item.ItemDefinitionIndex == (int)ItemDefinition.USP_S)
						{
							returnWeapon[SecondDuelist.Slot].Add("weapon_usp_silencer");
						} else
						{
							returnWeapon[SecondDuelist.Slot].Add(wpn.Value.DesignerName);
						}
					}
					wpn.Value.Remove();
				}
			}

			var weapon = rand[new Random().Next(rand.Count)];
			
			FirstDuelist.GiveNamedItem(WeaponChoose[weapon]);
			SecondDuelist.GiveNamedItem(WeaponChoose[weapon]);
			
			WeaponChoose.Clear();
			
			state = DuelState_t.DUEL_START;

			var timer = new Timer(0.2f, () =>
			{
				TeleportDuelists();
			}, TimerFlags.STOP_ON_MAPCHANGE);

			
		}
	}

	private void TeleportDuelists()
	{
		if (!Stock.IsValidPlayer(FirstDuelist) || !FirstDuelist!.PawnIsAlive || !Stock.IsValidPlayer(SecondDuelist) || !SecondDuelist!.PawnIsAlive)
		{
			state = DuelState_t.DUEL_END;
			DuelEnd();
		}
		
		if (duelPositions.TryGetValue(Server.MapName, out var positions) && positions.Count == 2)
		{
			FirstDuelist!.PlayerPawn.Value!.Teleport(new Vector(duelPositions[Server.MapName][0][0], duelPositions[Server.MapName][0][1], duelPositions[Server.MapName][0][2]), FirstDuelist.PlayerPawn.Value.AngVelocity, new Vector(0f, 0f, 0f));
			FirstDuelist!.Teleport(new Vector(duelPositions[Server.MapName][0][0], duelPositions[Server.MapName][0][1], duelPositions[Server.MapName][0][2]), FirstDuelist.PlayerPawn.Value.AngVelocity, new Vector(0f, 0f, 0f));
			
			SecondDuelist!.PlayerPawn.Value!.Teleport(new Vector(duelPositions[Server.MapName][1][0], duelPositions[Server.MapName][1][1], duelPositions[Server.MapName][1][2]), SecondDuelist.PlayerPawn.Value.AngVelocity, new Vector(0f, 0f, 0f));
			SecondDuelist!.Teleport(new Vector(duelPositions[Server.MapName][1][0], duelPositions[Server.MapName][1][1], duelPositions[Server.MapName][1][2]), SecondDuelist.PlayerPawn.Value.AngVelocity, new Vector(0f, 0f, 0f));
		}

		Stock.ChangePlayerMoveType(FirstDuelist!, MoveType_t.MOVETYPE_WALK);
		Stock.ChangePlayerMoveType(SecondDuelist!, MoveType_t.MOVETYPE_WALK);
		
		// FirstDuelist!.PlayerPawn.Value!.MoveType = MoveType_t.MOVETYPE_WALK;
		// FirstDuelist!.MoveType = MoveType_t.MOVETYPE_WALK;
		//
		// SecondDuelist!.PlayerPawn.Value!.MoveType = MoveType_t.MOVETYPE_WALK;
		// SecondDuelist!.MoveType = MoveType_t.MOVETYPE_WALK;
			
		state = DuelState_t.DUEL_IN_PROCESS;
	}

	public void DuelEnd()
	{
		if(Stock.IsValidPlayer(FirstDuelist) && FirstDuelist!.PawnIsAlive && FirstDuelist!.MoveType == MoveType_t.MOVETYPE_NONE)
		{
			// FirstDuelist!.PlayerPawn.Value!.MoveType = MoveType_t.MOVETYPE_WALK;
			// FirstDuelist!.MoveType = MoveType_t.MOVETYPE_WALK;
			
			Stock.ChangePlayerMoveType(FirstDuelist!, MoveType_t.MOVETYPE_WALK);
		}
		
		if(Stock.IsValidPlayer(SecondDuelist) && SecondDuelist!.PawnIsAlive && SecondDuelist!.MoveType == MoveType_t.MOVETYPE_NONE)
		{
			// SecondDuelist!.PlayerPawn.Value!.MoveType = MoveType_t.MOVETYPE_WALK;
			// SecondDuelist!.MoveType = MoveType_t.MOVETYPE_WALK;
			
			Stock.ChangePlayerMoveType(SecondDuelist!, MoveType_t.MOVETYPE_WALK);
		}

		ReturnWeapons();
		
		state = DuelState_t.DUEL_NOT;
	}

	private void ReturnWeapons()
	{
		if (Stock.IsValidPlayer(FirstDuelist) && FirstDuelist!.PlayerPawn.IsValid && FirstDuelist!.PawnIsAlive && returnWeapon.TryGetValue(FirstDuelist!.Slot, out var weapF) && weapF.Count > 0)
		{
			foreach (var weapon in FirstDuelist.PlayerPawn.Value!.WeaponServices!.MyWeapons)
			{
				if (weapon is { IsValid: true, Value.IsValid: true } && weapon.Value.AttributeManager.Item.ItemDefinitionIndex != (int)ItemDefinition.C4_EXPLOSIVE)
				{
					weapon.Value.Remove();
				}
			}

			if(returnWeapon.TryGetValue(FirstDuelist.Slot, out var weapons))
			{
				foreach (var weapon in weapons)
				{
					FirstDuelist.GiveNamedItem(weapon);
				}
			}
		}
		
		if (Stock.IsValidPlayer(SecondDuelist) && SecondDuelist!.PlayerPawn.IsValid && SecondDuelist!.PawnIsAlive && returnWeapon.TryGetValue(SecondDuelist!.Slot, out var weapS) && weapS.Count > 0)
		{
			foreach (var weapon in SecondDuelist.PlayerPawn.Value!.WeaponServices!.MyWeapons)
			{
				if (weapon is { IsValid: true, Value.IsValid: true } && weapon.Value.AttributeManager.Item.ItemDefinitionIndex != (int)ItemDefinition.C4_EXPLOSIVE)
				{
					weapon.Value.Remove();
				}
			}

			if(returnWeapon.TryGetValue(SecondDuelist.Slot, out var weapons))
			{
				foreach (var weapon in weapons)
				{
					SecondDuelist.GiveNamedItem(weapon);
				}
			}
		}
	}
}