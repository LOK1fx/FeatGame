using System;
using UnityEngine;

namespace LOK1game
{
    public class PlayerController : Controller
    {
        public event Action OnPauseKeyPressed;
        public event Action OnResumeKeyPressed;

        private bool _isEscapedPressed;

        protected override void Awake()
        {
            
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
                    OnPauseKeyPressed?.Invoke();
                else
                    OnResumeKeyPressed?.Invoke();
            }
        }

        public void ResumeGame()
        {
            _isEscapedPressed = false;
            IsInputProcessing = true;

            Cursor.lockState = Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}