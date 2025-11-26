using LOK1game.PlayerDomain;
using LOK1game.Utility;
using System.Linq;
using UnityEngine;

namespace LOK1game
{
    [RequireComponent(typeof(Player))]
    public class PlayerCharacterConsoleCommands
    {
        [ConsoleCommand("player_teleport")]
        public static void TeleportPlayer(Vector3 position)
        {
            if (TryToGetLocalPlayer(out var player))
                player.Teleport(position);
        }

        private static bool TryToGetLocalPlayer(out Player player)
        {
            player = Object.FindObjectsByType<Player>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID)
                .Where(p => p.IsLocallyControlled)
                .FirstOrDefault();

            if (player == null || player == default)
                return false;

            return true;
        }
    }
}
