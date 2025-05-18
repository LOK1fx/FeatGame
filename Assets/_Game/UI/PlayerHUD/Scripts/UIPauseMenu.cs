using LOK1game.UI;
using UnityEngine;

namespace LOK1game
{
    [RequireComponent(typeof(CanvasGroupFade))]
    public class UIPauseMenu : MonoBehaviour
    {
        [SerializeField] private LevelData _mainMenuLevelData;

        private PlayerController _playerController;
        private CanvasGroupFade _fade;

        private void Awake()
        {
            _fade = GetComponent<CanvasGroupFade>();
        }

        public void Construct(PlayerController playerController)
        {
            _playerController = playerController;
        }

        public void Show()
        {
            _fade.Show();
        }

        public void Resume()
        {
            _playerController.ResumeGame();
            _fade.Hide();
        }

        public void RestartLevel()
        {
            LevelManager.RestartLevel();
        }

        public void LoadMainMenuLevel()
        {
            LevelManager.LoadLevel(_mainMenuLevelData);
        }
    }
}
