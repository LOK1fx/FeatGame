using UnityEngine;

namespace LOK1game.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class StaminaScreen : MonoBehaviour
    {
        [SerializeField] private float _dropSpeed = 4;

        private CanvasGroup _canvas;
        private float _targetAlpha;
        private bool _isEntering = false;

        private void Awake()
        {
            _canvas = GetComponent<CanvasGroup>();
        }

        private void Update()
        {
            var multiplier = _dropSpeed;

            if (_isEntering)
                multiplier = _dropSpeed * 4f;
            if (Mathf.Abs(_canvas.alpha - _targetAlpha) < 0.01f)
            {
                _canvas.alpha = _targetAlpha;
                return;
            }
            _canvas.alpha = Mathf.Lerp(_canvas.alpha, _targetAlpha, Time.deltaTime * multiplier);
        }

        public void StartDrop()
        {
            _isEntering = true;

            _targetAlpha = 1f;
        }

        public void EndDrop()
        {
            _isEntering = false;

            _targetAlpha = 0f;
        }
    }
}
