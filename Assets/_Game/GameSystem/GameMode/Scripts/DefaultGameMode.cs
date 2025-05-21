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

            var player = SpawnGameModeObject(PlayerPrefab.gameObject);
            var spawnPointPosition = GetRandomSpawnPointPosition();
            var controller = CreatePlayerController(player.GetComponent<Pawn>());
            var ui = SpawnGameModeObject(UiPrefab);

            if (ui.TryGetComponent<IPlayerUI>(out var playerUI))
                playerUI.Bind(controller, player.GetComponent<Player>());

            var playerRigidbody = player.GetComponent<Rigidbody>();
            playerRigidbody.isKinematic = true;
            playerRigidbody.Sleep();

            player.transform.position = spawnPointPosition;

            playerRigidbody.isKinematic = false;
            playerRigidbody.WakeUp();

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