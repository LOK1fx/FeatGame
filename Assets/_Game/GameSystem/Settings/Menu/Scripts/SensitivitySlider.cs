using UnityEngine;
using UnityEngine.UI;

namespace LOK1game
{
    [RequireComponent(typeof(Slider))]
    public class SensitivitySlider : MonoBehaviour
    {
        private Slider _slider;

        private void Start()
        {
            _slider = GetComponent<Slider>();

            Settings.TryGetSensivity(out var sensitivity);
            _slider.value = sensitivity;

            _slider.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDestroy()
        {
            _slider.onValueChanged.RemoveListener(OnValueChanged);
        }

        public void OnValueChanged(float value)
        {
            Settings.SetSensitivity(value);
        }
    }
}