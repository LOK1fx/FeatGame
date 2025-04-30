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

        private MeshRenderer[] _meshRenderers;
        private Material[] _originalMaterials;
        private bool _isEffectActive = false;

        private void Awake()
        {
            _meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
            _originalMaterials = new Material[_meshRenderers.Length];

            for (int i = 0; i < _meshRenderers.Length; i++)
            {
                _originalMaterials[i] = _meshRenderers[i].material;
            }
        }

        public void PlayEffect()
        {
            if (_isEffectActive)
                ResetEffect();

            _isEffectActive = true;

            foreach (var renderer in _meshRenderers)
            {
                var material = renderer.material;
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", _damageColor * _emissionIntensity);
                material.color = _damageColor;
            }

            Invoke(nameof(ResetEffect), _effectDuration);
        }

        private void ResetEffect()
        {
            for (int i = 0; i < _meshRenderers.Length; i++)
            {
                var material = _meshRenderers[i].material;
                material.DisableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", Color.black);
                material.color = _originalMaterials[i].color;
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
                        var material = _meshRenderers[i].material;
                        material.DisableKeyword("_EMISSION");
                        material.SetColor("_EmissionColor", Color.black);
                        material.color = _originalMaterials[i].color;
                    }
                }
            }
        }
    }
}
