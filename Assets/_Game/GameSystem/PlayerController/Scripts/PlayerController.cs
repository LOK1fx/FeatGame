using LOK1game.Game.Events;
using UnityEngine;

namespace LOK1game
{
    public class PlayerController : Controller
    {
        private bool _isEscapedPressed;

        protected override void Awake()
        {
            EventManager.AddListener<OnDevConsoleStateChangedEvent>(OnDevConsoleStateChanged);
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<OnDevConsoleStateChangedEvent>(OnDevConsoleStateChanged);
        }

        public override void ApplicationUpdate()
        {
            if (IsInputProcessing)
                ControlledPawn?.OnInput(this);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                var isPause = !_isEscapedPressed;

                _isEscapedPressed = !_isEscapedPressed;
                IsInputProcessing = !_isEscapedPressed;

                Cursor.lockState = _isEscapedPressed ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = _isEscapedPressed;

                if (isPause)
                    App.ProjectContext.GameStateManager.SetState(Game.EGameState.Paused);
                else
                    App.ProjectContext.GameStateManager.SetState(Game.EGameState.Gameplay);
            }
        }

        public void ResumeGame()
        {
            _isEscapedPressed = false;
            IsInputProcessing = true;

            Cursor.lockState = Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDevConsoleStateChanged(OnDevConsoleStateChangedEvent evt)
        {
            if (evt.Enabled)
            {
                IsInputProcessing = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else if (_isEscapedPressed == false)
            {
                IsInputProcessing = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}