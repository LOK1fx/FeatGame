using UnityEngine;

namespace LOK1game.UI
{
    [RequireComponent(typeof(CanvasGroupFade))]
    public class UILoadingScreen : MonoBehaviour
    {
        private CanvasGroupFade _canvas;

        private void Awake()
        {
            _canvas = GetComponent<CanvasGroupFade>();
        }

        public void Show()
        {
            _canvas.Show();
        }

        public void Hide()
        {
            _canvas.Hide();
        }
    }
}
