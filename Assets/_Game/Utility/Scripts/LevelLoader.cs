using UnityEngine;
using UnityEngine.SceneManagement;

namespace LOK1game
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private float _delay = 1.0f;

        private string _levelName = string.Empty;

        public void LoadLevel(string levelName)
        {
            _levelName = levelName;

            Invoke(nameof(Load), _delay);
        }

        private void Load()
        {
            SceneManager.LoadSceneAsync(_levelName, LoadSceneMode.Single);
        }
    }
}
