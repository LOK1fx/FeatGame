using UnityEngine;
using TMPro;
using System.Linq;

namespace LOK1game
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class QualityChanger : MonoBehaviour
    {
        private TMP_Dropdown _dropdown;

        private void Start()
        {
            _dropdown = GetComponent<TMP_Dropdown>();
            _dropdown.ClearOptions();
            _dropdown.AddOptions(QualitySettings.names.ToList());

            _dropdown.value = QualitySettings.GetQualityLevel();
        }

        public void SetQuality(int index)
        {
            QualitySettings.SetQualityLevel(index);
        }
    }
}