using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace Duels.components;

public static class Stock
{
	public static Dictionary<string, string> weaponList = new Dictionary<string, string>
	{
		{"weapon_knife", "Knife"},
		{"weapon_ak47", "AK-47"},
		{"weapon_m4a1", "M4A4"},
		{"weapon_m4a1_silencer", "M4A1-S"},
		{"weapon_famas", "FAMAS"},
		{"weapon_galilar", "Galil AR"},
		{"weapon_aug", "AUG"},
		{"weapon_sg556", "SG 556"},
		{"weapon_awp", "AWP"},
		{"weapon_ssg08", "SSG 08"},
		{"weapon_scar20", "SCAR-20"},
		{"weapon_g3sg1", "G3SG1"},
		{"weapon_ump45", "UMP-45"},
		{"weapon_p90", "P90"},
		{"weapon_mp7", "MP7"},
		{"weapon_mp5sd", "MP5-SD"},
		{"weapon_mp9", "MP9"},
		{"weapon_bizon", "Bizon"},
		{"weapon_mac10", "Mac-10"},
		{"weapon_negev", "Negev"},
		{"weapon_m249", "M249"},
		{"weapon_glock", "Glock-18"},
		{"weapon_usp_silencer", "USP-S"},
		{"weapon_hkp2000", "P2000"},
		{"weapon_p250", "P250"},
		{"weapon_fiveseven", "Five-SeveN"},
		{"weapon_tec9", "Tec-9"},
		{"weapon_elite", "Dual Berettas"},
		{"weapon_deagle", "Deagle"},
		{"weapon_revolver", "Revolver"},
		{"weapon_cz75a", "CZ-Auto"},
		{"weapon_mag7", "Mag-7"},
		{"weapon_nova", "Nova"},
		{"weapon_sawedoff", "Sawed-off"},
		{"weapon_xm1014", "XM1014"}
	};
	
	public static bool IsValidPlayer(CCSPlayerController? player)
	{
		return player is not null && player.IsValid && !player.IsBot;
	}

	public static void ChangePlayerMoveType(CCSPlayerController player, MoveType_t type)
	{
		player.PlayerPawn.Value!.MoveType = type;
		player.MoveType = type;
		
		Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseEntity", "m_MoveType");
		Utilities.SetStateChanged(player, "CBaseEntity", "m_MoveType");
	}
}