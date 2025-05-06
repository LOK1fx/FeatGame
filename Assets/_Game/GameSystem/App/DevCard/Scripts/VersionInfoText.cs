using TMPro;
using UnityEngine;

namespace LOK1game
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class VersionInfoText : MonoBehaviour
    {
        private TextMeshProUGUI _text;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();

            _text.text = $"{Application.unityVersion} | {Application.version}\n{Application.buildGUID}";
        }
    }
}
