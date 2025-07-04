﻿using System;
using UnityEngine;

namespace LOK1game.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupFade : MonoBehaviour, IApplicationUpdatable
    {
        public float TargetAlpha;

        public bool IsShowing { get; private set; } = false;

        [SerializeField] [Range(0f, 1f)] private float _startAlpha;
        [SerializeField] private float _fadeSpeed;

        public CanvasGroup Canvas => _canvas;
        [SerializeField] private CanvasGroup _canvas;

        private void Awake()
        {
            _canvas = GetComponent<CanvasGroup>();
            _canvas.alpha = _startAlpha;
            TargetAlpha = _startAlpha;

            if(_startAlpha == 0)
            {
                Hide();
            }
        }

        private void OnEnable()
        {
            ApplicationUpdateManager.Register(this);
        }

        private void OnDisable()
        {
            ApplicationUpdateManager.Unregister(this);
        }

        public void ApplicationUpdate()
        {
            if (Mathf.Abs(_canvas.alpha - TargetAlpha) < 0.01f)
            {
                _canvas.alpha = TargetAlpha;
                return;
            }
            
            _canvas.alpha = Mathf.Lerp(_canvas.alpha, TargetAlpha, _fadeSpeed * Time.deltaTime);
        }

        public void SetAlpha(float alpha)
        {
            alpha = Mathf.Clamp01(alpha);

            if (alpha == 0)
            {
                Hide();
            }
            else if (alpha == 1)
            {
                Show();
            }

            TargetAlpha = alpha;
        }

        public void Show()
        {
            TargetAlpha = 1f;

            _canvas.interactable = true;
            _canvas.blocksRaycasts = true;

            IsShowing = true;
        }

        public void InstaShow()
        {
            TargetAlpha = 1f;
            _canvas.alpha = 1f;

            _canvas.interactable = true;
            _canvas.blocksRaycasts = true;

            IsShowing = true;
        }

        public void Hide()
        {
            TargetAlpha = 0f;

            _canvas.interactable = false;
            _canvas.blocksRaycasts = false;

            IsShowing = false;
        }

        public void InstaHide()
        {
            TargetAlpha = 0f;
            _canvas.alpha = 0f;

            _canvas.interactable = false;
            _canvas.blocksRaycasts = false;

            IsShowing = false;
        }

        public void SetVisible(bool visible)
        {
            if (visible)
                InstaShow();
            else
                InstaHide();
        }
    }
}