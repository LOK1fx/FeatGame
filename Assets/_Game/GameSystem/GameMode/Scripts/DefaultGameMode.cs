using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using LOK1game.PlayerDomain;
using LOK1game.UI;
using System.Collections;

namespace LOK1game.Game
{
    public sealed class DefaultGameMode : BaseGameMode
    {
        private NetworkManager _networkManager;

        public override EGameModeId Id => EGameModeId.Default;

        public override IEnumerator OnStart()
        {
            State = EGameModeState.Starting;

            _networkManager = InstanceFinder.NetworkManager;

            if (CameraPrefab == null)
            {
                GetLogger().PushError("CameraPrefab is not assigned in DefaultGameMode");
                yield break;
            }

            App.Loggers.GetLogger(ELoggerGroup.Networking).Push("network logger test fuck");

            SpawnGameModeObject(CameraPrefab);

            if (PlayerPrefab == null)
            {
                GetLogger().PushError("PlayerPrefab is not assigned in DefaultGameMode");
                yield break;
            }

            _networkManager.SceneManager.OnClientLoadedStartScenes += SceneManager_OnClientLoadedStartScenes;

            App.ProjectContext.GameStateManager.SetState(EGameStateId.Gameplay);

            yield return null;

            State = EGameModeState.Started;
        }

        private void SceneManager_OnClientLoadedStartScenes(NetworkConnection client, bool asServer)
        {
            App.Loggers.GetLogger(ELoggerGroup.Networking).Push("OnClientLoadedStartScenes");

            if (asServer == false)
                return;

            App.Loggers.GetLogger(ELoggerGroup.Networking).Push("OnClientLoadedStartScenes asServer True");

            SpawnPlayers();
        }

        public override IEnumerator OnEnd()
        {
            State = EGameModeState.Ending;
            
            yield return DestroyAllGameModeObjects();

            State = EGameModeState.Ended;
        } 

        [Server]
        private void SpawnPlayers()
        {
            foreach (var client in _networkManager.ServerManager.Clients.Values)
            {
                if (client.IsAuthenticated == false)
                    continue;

                var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

                if (client.Scenes.Contains(scene) == false)
                    _networkManager.SceneManager.AddConnectionToScene(client, scene);

                var player = SpawnPlayer();
                _networkManager.ServerManager.Spawn(player, client, scene);

                var controller = CreatePlayerController(player, false);
                _networkManager.ServerManager.Spawn(controller, client, scene);
            }
        }

        private Player SpawnPlayer()
        {
            var player = Instantiate(PlayerPrefab).GetComponent<Player>();
            var spawnPoint = GetRandomSpawnPoint(true);

            if (spawnPoint != null)
            {
                player.Teleport(spawnPoint.Position);
                player.ApplyYaw(spawnPoint.Yaw);
            }
            else
            {
                player.Teleport(spawnPoint.Position);
                player.ApplyYaw(0);

                GetLogger().PushWarning("Couldn't find a spawn point for player. Spawned at (0, 0, 0).");
            }

            return player;
        }

        private void SetupLocalPlayer(Player player)
        {
            var controller = CreatePlayerController(player.GetComponent<Player>(), true);

            if (UiPrefab == null)
            {
                GetLogger().PushError("UiPrefab is not assigned in DefaultGameMode");
            }
            var ui = SpawnGameModeObject(UiPrefab);

            if (ui != null && ui.TryGetComponent<IPlayerUI>(out var playerUI))
                playerUI.Bind(controller, player);
        }
    }
}