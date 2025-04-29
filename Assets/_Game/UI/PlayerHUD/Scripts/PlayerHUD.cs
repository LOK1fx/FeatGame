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

        private int _staminaTextCount = 0;

        public void Bind(PlayerController controller, Player player)
        {
            _controller = controller;
            _player = player;

            _player.OnStaminaRecovered += OnStaminaRecovered;
            _player.OnStaminaOut += OnStaminaOut;

            _player.Interaction.OnInteractionFound += OnInteractionFound;
            _player.Interaction.OnInteractionLost += OnInteractionLost;
        }

        private void OnDestroy()
        {
            _player.OnStaminaRecovered -= OnStaminaRecovered;
            _player.OnStaminaOut -= OnStaminaOut;

            _player.Interaction.OnInteractionFound -= OnInteractionFound;
            _player.Interaction.OnInteractionLost -= OnInteractionLost;
        }

        private void OnStaminaOut()
        {
            _staminaScreen.StartDrop();

            if (_staminaTextCount == 0 || _staminaTextCount % 3 == 0)
                _staminaTextFader.Show();
            _staminaTextCount++;
        }

        private void OnStaminaRecovered()
        {
            _staminaScreen.EndDrop();

            _staminaTextFader.Hide();
        }

        private void OnInteractionLost()
        {
            _interactionTextFader.Hide();
        }

        private void OnInteractionFound()
        {
            _interactionTextFader.Show();
        }
    }
}
