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

            var player = SpawnGameModeObject(PlayerPrefab.gameObject).GetComponent<Player>();
            var spawnPoint = GetRandomSpawnPoint(true);

            var playerRigidbody = player.Movement.Rigidbody;
            playerRigidbody.isKinematic = true;

            player.transform.position = spawnPoint.Position;
            player.ApplyYaw(spawnPoint.Yaw);

            var controller = CreatePlayerController(player.GetComponent<Pawn>());
            var ui = SpawnGameModeObject(UiPrefab);

            if (ui.TryGetComponent<IPlayerUI>(out var playerUI))
                playerUI.Bind(controller, player);

            yield return null;

            playerRigidbody.isKinematic = false;

            State = EGameModeState.Started;
        }

        public override IEnumerator OnEnd()
        {
            State = EGameModeState.Ending;
            
            yield return DestroyAllGameModeObjects();

            State = EGameModeState.Ended;
        }
    }
}