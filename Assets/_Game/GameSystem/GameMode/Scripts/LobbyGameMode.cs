using LOK1game.Game;
using System.Collections;
using UnityEngine;

namespace LOK1game
{
    public class LobbyGameMode : BaseGameMode
    {
        public override EGameModeId Id => EGameModeId.Lobby;

        public override IEnumerator OnEnd()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            yield return null;
        }

        public override IEnumerator OnStart()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            yield return null;
        }
    }
}
