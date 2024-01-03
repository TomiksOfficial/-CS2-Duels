using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using Duels.components;
using Microsoft.Extensions.Logging;

namespace Duels;

public partial class Duel : BasePlugin
{
	public override string ModuleName => "Duel";
	public override string ModuleVersion => "0.1.0";
	public override string ModuleDescription => "Duels between players";
	public override string ModuleAuthor => "Tomiks(vk.com/tomiksofficial)";

	private readonly DuelManager duelManager = new DuelManager();
	
	private void Log(string logMessage)
	{
		Logger.LogInformation($"[DUELS] {logMessage}");
	}

	public override void Load(bool hotReload)
	{
		Log("Plugin Start");
		
		VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Hook(OnTakeDamage, HookMode.Pre);
		
		RegisterListener<Listeners.OnMapStart>(OnMapStart);
	}
	
	public override void Unload(bool hotReload)
	{
		Log("Plugin Unload");
		
		VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Unhook(OnTakeDamage, HookMode.Pre);
	}

	[ConsoleCommand("duel_setpos", "Set the position to teleport the player in duel in current map")]
	[RequiresPermissions("@css/root")]
	public void OnSetPosCommand(CCSPlayerController? player, CommandInfo command)
	{
		if (player is null || !player.IsValid || !player.PawnIsAlive || !player.PlayerPawn.Value!.IsValid || player.PlayerPawn.Value!.AbsOrigin is null)
		{
			return;
		}
		
		if (command.ArgCount < 1 || !int.TryParse(command.GetArg(1), out int position))
		{
			command.ReplyToCommand("Command need 1 argument: number of position(1 or 2)");
			return;
		}

		if (position <= 0 || position > 2)
		{
			command.ReplyToCommand("Command need 1 argument: number of position(1 or 2)");
			return;
		}

		if (!duelManager.duelPositions.ContainsKey(Server.MapName))
		{
			duelManager.duelPositions[Server.MapName] = new List<List<float>>();
			
			duelManager.duelPositions[Server.MapName].Add(new List<float> {
				player.PlayerPawn.Value!.AbsOrigin.X,
				player.PlayerPawn.Value!.AbsOrigin.Y,
				player.PlayerPawn.Value!.AbsOrigin.Z
			});
		} else
		{
			if (duelManager.duelPositions[Server.MapName].Count < 2)
			{
				duelManager.duelPositions[Server.MapName].Add(new List<float> {
					player.PlayerPawn.Value!.AbsOrigin.X,
					player.PlayerPawn.Value!.AbsOrigin.Y,
					player.PlayerPawn.Value!.AbsOrigin.Z
				});
				// Log($"{duelManager.duelPositions[Server.MapName][0]}");
			} else
			{
				duelManager.duelPositions[Server.MapName][position - 1] = new List<float> {
					player.PlayerPawn.Value!.AbsOrigin.X,
					player.PlayerPawn.Value!.AbsOrigin.Y,
					player.PlayerPawn.Value!.AbsOrigin.Z
				};
			}
		}
		
		player.PrintToChat($"Successfully set position {position}");
	}
    
	[ConsoleCommand("duel_savepos", "Save positions")]
	[RequiresPermissions("@css/root")]
	public void OnSavePosCommand(CCSPlayerController? player, CommandInfo command)
	{
		if (player is null || !player.IsValid)
		{
			return;
		}

		File.WriteAllText($"{ModuleDirectory + "/map_positions.json"}", JsonSerializer.Serialize(duelManager.duelPositions.Where(x => x.Value.Count == 2).ToDictionary(x => x.Key, x => x.Value)));
		
		player.PrintToChat($"Successfully save data for all map where set 2 positions");
	}
    
	[ConsoleCommand("duel_start", "Save positions")]
	[RequiresPermissions("@css/root")]
	public void OnStartDuelCommand(CCSPlayerController? player, CommandInfo command)
	{
		var alivePlayers = Utilities.GetPlayers().Where(x => !x.IsBot && x.PawnIsAlive).ToList();
		
		if (alivePlayers.Count != 2 || alivePlayers[0].TeamNum == alivePlayers[1].TeamNum)
		{
			if (duelManager.state is not DuelState.DuelState_t.DUEL_NOT)
			{
				duelManager.DuelEnd();
			}
			
			return;
		}
		
		if(duelManager.state is DuelState.DuelState_t.DUEL_NOT && alivePlayers.Count == 2)
		{
			Stock.ChangePlayerMoveType(alivePlayers[0], MoveType_t.MOVETYPE_NONE);
			Stock.ChangePlayerMoveType(alivePlayers[1], MoveType_t.MOVETYPE_NONE);
			
			duelManager.OnDuelVoteMenu(alivePlayers[0], alivePlayers[1]);
			duelManager.OnDuelVoteMenu(alivePlayers[1], alivePlayers[0]);
		} else
		{
			duelManager.DuelEnd();
		}
	}
	
	[ConsoleCommand("duel_load", "Save positions")]
	[RequiresPermissions("@css/root")]
	public void OnLoadMapConfig(CCSPlayerController? player, CommandInfo command)
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
    
	[ConsoleCommand("weapon_check", "Check Weapon Name")]
	[RequiresPermissions("@css/root")]
	public void OnWeaponCheckCommand(CCSPlayerController? player, CommandInfo command)
	{
		if (player is null || !player.IsValid)
		{
			return;
		}
		
		if(EnumUtils.GetEnumMemberAttributeValue(
			   (CsItem)player.PlayerPawn.Value!.WeaponServices!.ActiveWeapon.Value!.AttributeManager.Item
				   .ItemDefinitionIndex) is not null)
		{
			Log(EnumUtils.GetEnumMemberAttributeValue(
				(CsItem)player.PlayerPawn.Value!.WeaponServices!.ActiveWeapon.Value!.AttributeManager.Item
					.ItemDefinitionIndex)!);
		}
		
		Log(player.PlayerPawn.Value!.WeaponServices!.ActiveWeapon.Value!.AttributeManager.Item
			.ItemDefinitionIndex.ToString());
	}
}