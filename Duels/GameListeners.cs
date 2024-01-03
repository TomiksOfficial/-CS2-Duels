using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using Duels.components;

namespace Duels;

public partial class Duel
{
	public void OnMapStart(string mapName)
	{
		string path = ModuleDirectory + "/map_positions.json";
		if (!File.Exists(path))
		{
			return;
		}
		
		var data = JsonSerializer.Deserialize<Dictionary<string, List<List<float>>>>(File.ReadAllText(path));
		
		if(data != null)
		{
			duelManager.duelPositions = data;
		}
	}
	
	private HookResult OnTakeDamage(DynamicHook hook)    
	{
		var entIndex = hook.GetParam<CEntityInstance>(0).Index;
       
		if (entIndex == 0)
			return HookResult.Continue;

		var pawn = Utilities.GetEntityFromIndex<CCSPlayerPawn>((int)entIndex);
        
		if (!pawn.IsValid || !pawn.OriginalController.IsValid)
			return HookResult.Continue;
		
		if (duelManager.state is DuelState.DuelState_t.DUEL_VOTE)
			hook.GetParam<CTakeDamageInfo>(1).Damage = 0;
        
		return HookResult.Continue;
	}
}