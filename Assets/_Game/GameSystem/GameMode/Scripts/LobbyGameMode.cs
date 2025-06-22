using LOK1game.Game;
using System.Collections;
using UnityEngine;

namespace LOK1game
{
    public class LobbyGameMode : BaseGameMode
    {
        public override EGameModeId Id => EGameModeId.Lobby;

        [SerializeField] private GameObject _characterPrefab;
        [SerializeField] private Vector3 _characterPosition = new(-4.5f, 0, 3.25f);
        [SerializeField] private Vector3 _characterRotation = new(0f, -113f, 0f);

        public override IEnumerator OnEnd()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            yield return DestroyAllGameModeObjects();
        }

        public override IEnumerator OnStart()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            var character = SpawnGameModeObject(_characterPrefab);
            character.transform.SetPositionAndRotation(_characterPosition, Quaternion.Euler(_characterRotation));

            SpawnGameModeObject(CameraPrefab);
            SpawnGameModeObject(UiPrefab);

            yield return null;
        }
    }
}
