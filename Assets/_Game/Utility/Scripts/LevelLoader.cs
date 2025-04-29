using UnityEngine;
using UnityEngine.SceneManagement;

namespace LOK1game
{
    public class LevelLoader : MonoBehaviour
    {
        public void LoadLevel(string levelName)
        {
            SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
        }
    }
}
