using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using LOK1game.Tools;
using System;

namespace LOK1game.Game
{
    [Serializable]
    public class SubtitleManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _subtitleText;
        [SerializeField] private float _defaultDisplayTime = 3f;
        [SerializeField] private float _fadeInTime = 0.3f;
        [SerializeField] private float _fadeOutTime = 0.3f;

        private Queue<SubtitleData> _subtitleQueue = new Queue<SubtitleData>();
        private Coroutine _currentSubtitleCoroutine;
        private bool _isDisplayingSubtitle;

        private void Awake()
        {
            _subtitleText.alpha = 0f;
        }

        public void ShowSubtitle(string key, float? displayTime = null)
        {
            var subtitle = new SubtitleData
            {
                Text = LocalisationSystem.GetLocalisedValue(key),
                DisplayTime = displayTime ?? _defaultDisplayTime
            };

            _subtitleQueue.Enqueue(subtitle);

            if (!_isDisplayingSubtitle)
                DisplayNextSubtitle();
        }

        private void DisplayNextSubtitle()
        {
            if (_subtitleQueue.Count == 0)
            {
                _isDisplayingSubtitle = false;
                return;
            }

            _isDisplayingSubtitle = true;
            var subtitle = _subtitleQueue.Dequeue();

            if (_currentSubtitleCoroutine != null)
                Coroutines.StopRoutine(_currentSubtitleCoroutine);

            _currentSubtitleCoroutine = Coroutines.StartRoutine(DisplaySubtitleRoutine(subtitle));
        }

        private IEnumerator DisplaySubtitleRoutine(SubtitleData subtitle)
        {
            _subtitleText.text = subtitle.Text;

            var elapsedTime = 0f;
            while (elapsedTime < _fadeInTime)
            {
                _subtitleText.alpha = Mathf.Lerp(0f, 1f, elapsedTime / _fadeInTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            _subtitleText.alpha = 1f;

            yield return new WaitForSeconds(subtitle.DisplayTime);
            
            elapsedTime = 0f;
            while (elapsedTime < _fadeOutTime)
            {
                _subtitleText.alpha = Mathf.Lerp(1f, 0f, elapsedTime / _fadeOutTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            _subtitleText.alpha = 0f;

            DisplayNextSubtitle();
        }

        private struct SubtitleData
        {
            public string Text;
            public float DisplayTime;
        }
    }
} 