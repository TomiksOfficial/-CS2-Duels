using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Timers;
using Duels.components;
using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;

namespace Duels;

public partial class Duel
{
	[GameEventHandler]
	public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
	{
		var timer = new Timer(0.1f, () =>
		{
			var alivePlayers = Utilities.GetPlayers().Where(x => !x.IsBot && x.PawnIsAlive).ToList();
			if (alivePlayers.Count != 2 || alivePlayers[0].TeamNum == alivePlayers[1].TeamNum)
			{
				if (duelManager.state != DuelState.DuelState_t.DUEL_END && duelManager.state != DuelState.DuelState_t.DUEL_NOT)
				{
					duelManager.returnWeapon.Remove(@event.Userid.Slot);
					duelManager.state = DuelState.DuelState_t.DUEL_END;
				}
			
				return;
			}
		
			// Log($"Alive: {alivePlayers.Count} | ply1: {alivePlayers[0].PlayerName} | ply2: {alivePlayers[1].PlayerName}");
		
			if(duelManager.state is DuelState.DuelState_t.DUEL_NOT && alivePlayers.Count == 2)
			{
				// alivePlayers[0].PlayerPawn.Value!.MoveType = MoveType_t.MOVETYPE_NONE;
				// alivePlayers[1].PlayerPawn.Value!.MoveType = MoveType_t.MOVETYPE_NONE;
			
				Stock.ChangePlayerMoveType(alivePlayers[0], MoveType_t.MOVETYPE_NONE);
				Stock.ChangePlayerMoveType(alivePlayers[1], MoveType_t.MOVETYPE_NONE);
			
				duelManager.OnDuelVoteMenu(alivePlayers[0], alivePlayers[1]);
				duelManager.OnDuelVoteMenu(alivePlayers[1], alivePlayers[0]);

				return;
			}
		
			if(duelManager.state != DuelState.DuelState_t.DUEL_END && duelManager.state != DuelState.DuelState_t.DUEL_NOT)
			{
				duelManager.state = DuelState.DuelState_t.DUEL_END;
			}
		}, TimerFlags.STOP_ON_MAPCHANGE);
		
		
		
		return HookResult.Continue;
	}
	
	[GameEventHandler]
	public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
	{
		var alivePlayers = Utilities.GetPlayers().Where(x => x.PawnIsAlive).ToList();
		
		if (alivePlayers.Count == 1 && duelManager.state != DuelState.DuelState_t.DUEL_END && duelManager.state != DuelState.DuelState_t.DUEL_NOT)
		{
			// duelManager.DuelEnd();
			duelManager.state = DuelState.DuelState_t.DUEL_END;
		}
		
		return HookResult.Continue;
	}
	
	[GameEventHandler]
	public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
	{
		if(duelManager.state != DuelState.DuelState_t.DUEL_END && duelManager.state != DuelState.DuelState_t.DUEL_NOT)
		{
			// duelManager.DuelEnd();
			duelManager.state = DuelState.DuelState_t.DUEL_END;
		}

		return HookResult.Continue;
	}
	
	[GameEventHandler]
	public HookResult OnRoundPostStart(EventRoundPoststart @event, GameEventInfo info)
	{
		if(duelManager.state is DuelState.DuelState_t.DUEL_END)
		{
			duelManager.DuelEnd();
			// duelManager.state = DuelState.DuelState_t.DUEL_END;
		}

		return HookResult.Continue;
	}
}