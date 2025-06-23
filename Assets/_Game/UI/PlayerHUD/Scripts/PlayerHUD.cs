using LOK1game.Game.Events;
using LOK1game.PlayerDomain;
using LOK1game.UI;
using UnityEngine;

namespace LOK1game
{
    public class PlayerHUD : MonoBehaviour, IPlayerUI, IApplicationUpdatable
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
            _player.OnStaminaChanged += OnStaminaChanged;

            _player.Interaction.OnInteractionFound += OnInteractionFound;
            _player.Interaction.OnInteractionLost += OnInteractionLost;

            _player.Health.OnHealthChanged += OnHealthChanged;
            _player.OnTakeDamage += OnPlayerTakeDamage;

            _pauseMenu.Construct(controller);
            EventManager.AddListener<OnGameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnDestroy()
        {
            _player.OnStaminaRecovered -= OnStaminaRecovered;
            _player.OnStaminaOut -= OnStaminaOut;
            _player.OnStaminaChanged -= OnStaminaChanged;

            _player.Interaction.OnInteractionFound -= OnInteractionFound;
            _player.Interaction.OnInteractionLost -= OnInteractionLost;

            _player.Health.OnHealthChanged -= OnHealthChanged;
            _player.OnTakeDamage -= OnPlayerTakeDamage;

            EventManager.RemoveListener<OnGameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnEnable()
        {
            ApplicationUpdateManager.Register(this);
        }

        private void OnDisable()
        {
            ApplicationUpdateManager.Unregister(this);
        }

        public void ApplicationUpdate()
        {
            
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

        private void OnStaminaChanged(float stamina)
        {
            _staminaBar.SetValue(stamina, 0, _player.MaxStamina);
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

        private void OnGameStateChanged(OnGameStateChangedEvent evt)
        {
            switch (evt.NewState)
            {
                case Game.EGameState.Gameplay:
                    _pauseMenu.Resume();
                    break;
                case Game.EGameState.Paused:
                    _pauseMenu.Show();
                    break;
                default:
                    break;
            }
        }
    }
}
