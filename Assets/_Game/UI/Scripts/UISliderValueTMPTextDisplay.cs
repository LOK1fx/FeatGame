using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LOK1game
{
    [RequireComponent(typeof(Slider))]
    public class UISliderValueTMPTextDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _displayText;

        private Slider _slider;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        private void Start()
        {
            _slider.onValueChanged.AddListener(OnValueChanged);
            OnValueChanged(_slider.value);
        }

        private void OnDestroy()
        {
            _slider.onValueChanged.RemoveListener(OnValueChanged);
        }
        
        private void OnValueChanged(float value)
        {
            _displayText.text = value.ToString("0.0");
        }
    }
}
