using UnityEngine;

namespace LOK1game
{
    public class UIFillBar : MonoBehaviour
    {
        [SerializeField] private RectTransform _bar;

        public void SetValue(float value, float minValue = 0, float maxValue = 100)
        {
            var normalizedValue = Mathf.Clamp01((value - minValue) / (maxValue - minValue));
            
            _bar.localScale = new Vector3(normalizedValue, 1f, 1f);
        }
    }
}
