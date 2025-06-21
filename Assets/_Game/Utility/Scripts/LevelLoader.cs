using LOK1game.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LOK1game
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private LevelData _levelData;
        [SerializeField] private float _delay = 1.0f;

        private string _sceneName = string.Empty;

        public void LoadScene(string sceneName)
        {
            _sceneName = sceneName;

            if (_delay > 0)
                Invoke(nameof(LoadSceneDelayed), Mathf.Abs(_delay));
            else
                LoadSceneDelayed();
        }

        public void LoadLevel()
        {
            if (_delay > 0)
                Invoke(nameof(LoadLevelDelayed), Mathf.Abs(_delay));
            else
                LoadLevelDelayed();
        }

        private void LoadSceneDelayed()
        {
            SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Single); 
        }

        private void LoadLevelDelayed()
        {
            StartCoroutine(LevelManager.LoadLevel(_levelData));
        }
    }
}
