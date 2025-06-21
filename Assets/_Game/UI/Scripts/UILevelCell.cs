using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LOK1game.UI
{
    [RequireComponent(typeof(Button))]
    public class UILevelCell : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelName;
        [SerializeField] private Image _levelImage;

        private Button _button;
        private LevelData _levelData;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void Start()
        {
            _button.onClick.AddListener(OnClicked);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        public void Initialize(LevelData data)
        {
            _levelName.text = data.DisplayName;
            _levelImage.sprite = data.LevelImage;

            _levelData = data;
        }

        private void OnClicked()
        {
            StartCoroutine(LevelManager.LoadLevel(_levelData));
        }
    }
}
