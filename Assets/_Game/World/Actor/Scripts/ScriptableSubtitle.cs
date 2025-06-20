using UnityEngine;

namespace LOK1game
{
    public class ScriptableSubtitle : MonoBehaviour
    {
        private float _duration = 1f;

        public void ShowSubtitle(string text)
        {
            App.ProjectContext.SubtitleManager.ShowSubtitle(text, _duration);
        }

        public void SetDuration(float duration)
        {
            _duration = duration;
        }
    }
}
