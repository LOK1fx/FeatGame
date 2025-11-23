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
            {
                InputContext.MovementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

                var cameraInputX = 0f;
                var cameraInputY = 0f;

                if (!Application.isMobilePlatform && Cursor.lockState == CursorLockMode.Locked)
                {
                    cameraInputX = Input.GetAxisRaw("Mouse X");
                    cameraInputY = Input.GetAxisRaw("Mouse Y");
                }
                else
                {
                    if (Input.touchCount >= 1)
                    {
                        cameraInputX = Input.GetTouch(0).deltaPosition.x;
                        cameraInputY = Input.GetTouch(0).deltaPosition.y;
                    }
                }

                InputContext.LookInput = new Vector2(cameraInputX, cameraInputY);

                if (Input.GetKeyDown(KeyCode.LeftControl))
                    InputContext.FireCrouchButtonDown();
                if (Input.GetKeyUp(KeyCode.LeftControl))
                    InputContext.FireCrouchButtonUp();

                if (Input.GetKeyDown(KeyCode.LeftShift))
                    InputContext.FireSprintButtonDown();
                if (Input.GetKeyUp(KeyCode.LeftShift))
                    InputContext.FireSprintButtonUp();

                if (Input.GetKeyDown(KeyCode.Space))
                    InputContext.FireJumpButtonDown();

                if (Input.GetKeyDown(KeyCode.Mouse0))
                    InputContext.FireFireButtonDown();

                if (Input.GetKeyDown(KeyCode.F))
                    InputContext.FireInteractionButtonDown();


                ControlledPawn?.OnInput(this, InputContext);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isEscapedPressed)
                    ResumeGame();
                else
                    PauseGame();
            }
        }

        public void PauseGame()
        {
            _isEscapedPressed = true;
            IsInputProcessing = false;

            App.ProjectContext.GameStateManager.SetState(Game.EGameStateId.Paused);
        }

        public void ResumeGame()
        {
            _isEscapedPressed = false;
            IsInputProcessing = true;

            App.ProjectContext.GameStateManager.SetState(Game.EGameStateId.Gameplay);
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