using System.Collections;

namespace LOK1game.Game
{
    public class SpectatorsGameMode : BaseGameMode
    {
        public override EGameModeId Id => EGameModeId.Spectators;

        public override IEnumerator OnEnd()
        {
            yield return DestroyAllGameModeObjects();
        }

        public override IEnumerator OnStart()
        {
            if (UiPrefab != null)
                SpawnGameModeObject(UiPrefab);

            SpawnGameModeObject(CameraPrefab);

            yield return null;
        }
    }
}
