using System;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceShip.UI.Components
{
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(Image))]
    public class Bar : MonoBehaviour
    {
        [SerializeField] [Range(0, 1)] private float _value;
        [SerializeField] private float _dampening = 5f;
        [SerializeField] private float _updateScaler = 0.5f;

        private float _timeout = 0.0f;
        private Material _material;
        private float _fillTarget = 0.5f;
        private float _delta = 0.0f;
        private bool _isValueChanged;

        private void Awake()
        {
            var renderer = GetComponent<Renderer>();
            var image = GetComponent<Image>();
            if (renderer != null)
            {
                _material = new Material(renderer.material);
                renderer.material = _material;
            }
            else if (image != null)
            {
                _material = new Material(image.material);
                image.material = _material;
            }
            else
            {
                throw new Exception("No Renderer ore Image attach to " + name);
            }
        }

        private void Update()
        {
            
            _timeout += Time.deltaTime * _updateScaler;
            if (_timeout > 1.0f)
            {
                _timeout = 0f;
                var fill = _value;
                _delta -= _fillTarget - fill;
                _fillTarget = fill;
            }

            _delta = Mathf.Lerp(_delta, 0, Time.deltaTime * _dampening);

            _material.SetFloat("_Delta", _delta);
            _material.SetFloat("_Fill", _fillTarget);
        }

        public void Set(float value)
        {
            var clampValue = Mathf.Clamp01(value);
            if (_value != clampValue)
            {
                _value = clampValue;
                //TODO check
                // _isValueChanged = true;
            }

        }
    }
}
