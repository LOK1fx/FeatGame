using UnityEngine;

namespace LOK1game
{
    [RequireComponent(typeof(Sway))]
    public class WeaponSway : MonoBehaviour
    {
        private Sway _sway;
        private float _sensitivity;

        private void Awake()
        {
            _sway = GetComponent<Sway>();
        }

        private void Start()
        {
            Settings.TryGetSensivity(out _sensitivity);
            Settings.OnSensivityChanged += OnPlayerSensitivityChanged;
        }

        private void OnDestroy()
        {
            Settings.OnSensivityChanged -= OnPlayerSensitivityChanged;
        }

        private void OnPlayerSensitivityChanged(float newSens)
        {
            _sensitivity = newSens;
        }

        public void OnInput(PlayerCharacterInputContext input)
        {
            _sway.SetInputDelta(input.LookInput * _sensitivity);
        }
    }
}
