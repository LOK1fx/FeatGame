using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOK1game
{
    public class TakeDamageEffect : MonoBehaviour
    {
        [SerializeField] private Color _damageColor = Color.red;
        [SerializeField] private float _effectDuration = 0.2f;
        [SerializeField] private float _emissionIntensity = 5f;

        private Renderer[] _meshRenderers;
        private Material[] _originalMaterials;
        private Material[] _damageMaterials;
        private bool _isEffectActive = false;

        private void Awake()
        {
            _meshRenderers = GetComponentsInChildren<Renderer>(true);
            _originalMaterials = new Material[_meshRenderers.Length];
            _damageMaterials = new Material[_meshRenderers.Length];

            for (int i = 0; i < _meshRenderers.Length; i++)
            {
                _originalMaterials[i] = _meshRenderers[i].material;
                _damageMaterials[i] = new Material(_originalMaterials[i]);
                _damageMaterials[i].EnableKeyword("_EMISSION");
                _damageMaterials[i].SetColor("_EmissionColor", _damageColor * _emissionIntensity);
                _damageMaterials[i].color = _damageColor;
            }
        }

        public void PlayEffect()
        {
            if (_isEffectActive)
                ResetEffect();

            _isEffectActive = true;

            for (int i = 0; i < _meshRenderers.Length; i++)
            {
                _meshRenderers[i].material = _damageMaterials[i];
            }

            Invoke(nameof(ResetEffect), _effectDuration);
        }

        private void ResetEffect()
        {
            for (int i = 0; i < _meshRenderers.Length; i++)
            {
                _meshRenderers[i].material = _originalMaterials[i];
            }

            _isEffectActive = false;
        }

        private void OnDestroy()
        {
            if (_meshRenderers != null)
            {
                for (int i = 0; i < _meshRenderers.Length; i++)
                {
                    if (_meshRenderers[i] != null)
                    {
                        _meshRenderers[i].material = _originalMaterials[i];
                    }
                }
            }
        }
    }
}
