using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace RetakeDefuse
{
    public class RetakeDefuse : BasePlugin
    {
        public override string ModuleAuthor => "TICHOJEBEC";
        public override string ModuleName => "Defuser Fix";
        public override string ModuleVersion => "v1.1";

        public override void Load(bool hotReload)
        {
            RegisterEventHandler<EventBombPlanted>(OnBombPlanted);
        }

        private static List<CCSPlayerController> GetValidPlayers()
        {
            return Utilities.GetPlayers()
                .Where(player => player is { IsValid: true, PawnIsAlive: true })
                .ToList();
        }

        private static bool HasDefuser(CCSPlayerController? player)
        {
            if (player is null or { IsValid: false } or { PawnIsAlive: false })
                return false;

            var pawn = player.PlayerPawn.Value;
            return pawn?.WeaponServices?.MyWeapons
                .Any(weapon => weapon.Value?.IsValid == true && weapon.Value.DesignerName.Contains("item_defuser")) ?? false;
        }

        private HookResult OnBombPlanted(EventBombPlanted @event, GameEventInfo info)
        {
            var players = GetValidPlayers();
            foreach (var player in players)
            {
                if (player.Team == CsTeam.CounterTerrorist && !HasDefuser(player))
                {
                    player.GiveNamedItem("item_defuser");
                }
            }
            return HookResult.Continue;
        }
    }
}