using FishNet.Managing.Scened;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LOK1game.Game
{
    public class LevelSceneProcessor : DefaultSceneProcessor
    {
        public override void BeginLoadAsync(string sceneName, LoadSceneParameters parameters)
        {
            var levelData = LevelManager.GetLevelData(sceneName);

            LevelManager.LoadLevel(levelData);
        }
    }
}
