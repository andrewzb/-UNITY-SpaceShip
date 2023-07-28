
using System.Collections;
using SpaceShip.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceShip.Managers
{
    public class LoadingFadeEffect: SingletonPersistent<LoadingFadeEffect>
    {
        [SerializeField] public static bool isCanRun;

        [SerializeField] private Image _image;

        [SerializeField] [Range(0, 2)] private float _fadeTime;
        [SerializeField] private float _fadeGap;

        private IEnumerator FadeAll()
        {
            yield return StartCoroutine(FadeIn());

            yield return new WaitForSeconds(_fadeGap);

            yield return StartCoroutine(FadeOut());
        }
        
        private IEnumerator FadeIn()
        {
            var backgroundColor = _image.color;
            backgroundColor.a = 0;

            _image.color = backgroundColor;
            _image.gameObject.SetActive(true);
        
            while (backgroundColor.a <= 1)
            {
                yield return new WaitForSeconds(Time.deltaTime);
                backgroundColor.a += (1 / _fadeTime) * Time.deltaTime;
                _image.color = backgroundColor;
            }
            isCanRun = true;
        }

        private IEnumerator FadeOut()
        {
            isCanRun = false;
            var backgroundColor = _image.color;
            backgroundColor.a = 1;
            while (backgroundColor.a >= 0)
            {
                yield return new WaitForSeconds(Time.deltaTime);
                backgroundColor.a -= (1 / _fadeTime) * Time.deltaTime;
                _image.color = backgroundColor;
            }

            _image.gameObject.SetActive(false);
        }

        public void RunFadeIn()
        {
            StartCoroutine(FadeIn());
        }

        public void RunFadeOut()
        {
            StartCoroutine(FadeOut());
        }

        public void RunFadeAll()
        {
            StartCoroutine(FadeAll());
        }
        
    }
}