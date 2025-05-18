using LOK1game.PlayerDomain;
using LOK1game.UI;
using UnityEngine;

namespace LOK1game
{
    public class PlayerHUD : MonoBehaviour, IPlayerUI
    {
        private Player _player;
        private PlayerController _controller;

        [SerializeField] private StaminaScreen _staminaScreen;
        [SerializeField] private CanvasGroupFade _interactionTextFader;
        [SerializeField] private CanvasGroupFade _staminaTextFader;
        [SerializeField] private UIFillBar _hpBar;
        [SerializeField] private UIFillBar _staminaBar;
        [SerializeField] private CanvasGroupFade _damageOverlay;
        [SerializeField] private UIPauseMenu _pauseMenu;

        private int _staminaTextCount = 0;

        public void Bind(PlayerController controller, Player player)
        {
            _controller = controller;
            _player = player;

            _player.OnStaminaRecovered += OnStaminaRecovered;
            _player.OnStaminaOut += OnStaminaOut;

            _player.Interaction.OnInteractionFound += OnInteractionFound;
            _player.Interaction.OnInteractionLost += OnInteractionLost;

            _player.Health.OnHealthChanged += OnHealthChanged;
            _player.OnTakeDamage += OnPlayerTakeDamage;

            _pauseMenu.Construct(controller);
            _controller.OnPauseKeyPressed += OnPlayerControllerPausePressed;
            _controller.OnResumeKeyPressed += OnPlayerControllerResumePressed;
        }

        private void OnDestroy()
        {
            _player.OnStaminaRecovered -= OnStaminaRecovered;
            _player.OnStaminaOut -= OnStaminaOut;

            _player.Interaction.OnInteractionFound -= OnInteractionFound;
            _player.Interaction.OnInteractionLost -= OnInteractionLost;

            _player.Health.OnHealthChanged -= OnHealthChanged;
            _player.OnTakeDamage -= OnPlayerTakeDamage;

            _controller.OnPauseKeyPressed -= OnPlayerControllerPausePressed;
            _controller.OnResumeKeyPressed -= OnPlayerControllerResumePressed;
        }

        private void Update()
        {
            if (_player == null)
                return;

            _staminaBar.SetValue(_player.Stamina, 0, _player.MaxStamina);
        }

        private void OnPlayerTakeDamage()
        {
            _damageOverlay.Canvas.alpha = 1;
        }

        private void OnStaminaOut()
        {
            _staminaScreen.StartDrop();

            if (_staminaTextCount == 0 || _staminaTextCount % 3 == 0)
                _staminaTextFader.Show();
            _staminaTextCount++;

            _staminaBar.SetValue(0);
        }

        private void OnStaminaRecovered()
        {
            _staminaScreen.EndDrop();

            _staminaTextFader.Hide();

            _staminaBar.SetValue(100);
        }

        private void OnInteractionLost()
        {
            _interactionTextFader.Hide();
        }

        private void OnInteractionFound()
        {
            _interactionTextFader.Show();
        }

        private void OnHealthChanged(int newHealth)
        {
            _hpBar.SetValue(newHealth);
        }

        private void OnPlayerControllerPausePressed()
        {
            _pauseMenu.Show();
        }

        private void OnPlayerControllerResumePressed()
        {
            _pauseMenu.Resume();
        }
    }
}
