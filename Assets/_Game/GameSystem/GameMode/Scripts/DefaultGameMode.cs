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

            if (CameraPrefab == null)
            {
                GetLogger().PushError("CameraPrefab is not assigned in DefaultGameMode");
                yield break;
            }
            SpawnGameModeObject(CameraPrefab);

            if (PlayerPrefab == null)
            {
                GetLogger().PushError("PlayerPrefab is not assigned in DefaultGameMode");
                yield break;
            }
            var player = SpawnGameModeObject(PlayerPrefab.gameObject).GetComponent<Player>();
            var spawnPoint = GetRandomSpawnPoint(true);

            var playerRigidbody = player.Movement.Rigidbody;
            playerRigidbody.isKinematic = true;

            if (spawnPoint != null )
            {
                player.transform.position = spawnPoint.Position;
                player.ApplyYaw(spawnPoint.Yaw);
            }
            else
            {
                player.transform.position = Vector3.zero;
                player.ApplyYaw(0);
            }

            var controller = CreatePlayerController(player.GetComponent<Pawn>());
            
            if (controller == null)
            {
                GetLogger().PushError("Failed to create PlayerController in DefaultGameMode");
                yield break;
            }
            
            if (UiPrefab == null)
            {
                GetLogger().PushError("UiPrefab is not assigned in DefaultGameMode");
                yield break;
            }
            var ui = SpawnGameModeObject(UiPrefab);

            if (ui != null && ui.TryGetComponent<IPlayerUI>(out var playerUI))
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