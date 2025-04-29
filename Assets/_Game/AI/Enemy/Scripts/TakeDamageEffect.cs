using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOK1game
{
    public class TakeDamageEffect : MonoBehaviour
    {
        [SerializeField] private float _flashIntensity = 5f;
        [SerializeField] private float _fadeSpeed = 2f;

        private MeshRenderer[] _meshRenderers;
        private List<Material[]> _originalMaterials = new List<Material[]>();
        private Coroutine _flashCoroutine;

        private void Start()
        {
            _meshRenderers = GetComponentsInChildren<MeshRenderer>();

            foreach (var renderer in _meshRenderers)
            {
                _originalMaterials.Add(renderer.materials);
            }
        }

        public void Flash()
        {
            if (_flashCoroutine != null)
            {
                StopCoroutine(_flashCoroutine);
                ReturnOriginals();
            }  

            foreach (var renderer in _meshRenderers)
            {
                var instancedMaterials = new Material[renderer.materials.Length];
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    instancedMaterials[i] = new Material(renderer.materials[i]);
                }
                renderer.materials = instancedMaterials;
            }

            _flashCoroutine = StartCoroutine(FlashEffect());
        }

        private IEnumerator FlashEffect()
        {
            var currentIntensity = _flashIntensity;
            var baseColors = new Color[_meshRenderers.Length * 4];

            var index = 0;
            foreach (var renderer in _meshRenderers)
            {
                foreach (var material in renderer.materials)
                {
                    baseColors[index++] = material.color;
                    material.color = Color.white * _flashIntensity;
                    material.EnableKeyword("_EMISSION");
                    material.SetColor("_EmissionColor", Color.white * _flashIntensity);
                }
            }

            while (currentIntensity > 0f)
            {
                currentIntensity = Mathf.Max(0f, currentIntensity - _fadeSpeed * Time.deltaTime);

                index = 0;
                foreach (var renderer in _meshRenderers)
                {
                    foreach (var material in renderer.materials)
                    {
                        var targetColor = baseColors[index] * Mathf.LinearToGammaSpace(currentIntensity + 1f);
                        material.color = Color.Lerp(baseColors[index], targetColor, currentIntensity / _flashIntensity);
                        material.SetColor("_EmissionColor", Color.white * currentIntensity);
                        index++;
                    }
                }
                yield return null;
            }

            ReturnOriginals();
        }

        private void ReturnOriginals()
        {
            for (int i = 0; i < _meshRenderers.Length; i++)
            {
                _meshRenderers[i].materials = _originalMaterials[i];
            }
        }

        private void OnDestroy()
        {
            if (_meshRenderers != null)
            {
                for (int i = 0; i < _meshRenderers.Length; i++)
                {
                    if (_meshRenderers[i] != null)
                    {
                        _meshRenderers[i].materials = _originalMaterials[i];
                    }
                }
            }
        }
    }
}
