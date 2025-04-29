using LOK1game.PlayerDomain;
using LOK1game.UI;
using System.Collections;
using UnityEngine;

namespace LOK1game.Game
{
    public sealed class DefaultGameMode : BaseGameMode
    {
        public override EGameModeId Id => EGameModeId.Default;

        public override IEnumerator OnStart()
        {
            State = EGameModeState.Starting;

            SpawnGameModeObject(CameraPrefab);

            //So strange code
            var player = SpawnGameModeObject(PlayerPrefab.gameObject);
            player.transform.position = GetRandomSpawnPointPosition();

            var controller = CreatePlayerController(player.GetComponent<Pawn>());

            var ui = SpawnGameModeObject(UiPrefab);

            if (ui.TryGetComponent<IPlayerUI>(out var playerUI))
                playerUI.Bind(controller, player.GetComponent<Player>());

            State = EGameModeState.Started;

            yield return null;
        }

        public override IEnumerator OnEnd()
        {
            State = EGameModeState.Ending;
            
            yield return DestroyAllGameModeObjects();

            State = EGameModeState.Ended;
        }
    }
}