using UnityEngine;

namespace LOK1game
{
    public class UIHpBar : MonoBehaviour
    {
        [SerializeField] private RectTransform _bar;

        public void SetHealth(int health)
        {
            _bar.localScale = new Vector3(health * 0.01f, 1f, 1f);
        }
    }
}
