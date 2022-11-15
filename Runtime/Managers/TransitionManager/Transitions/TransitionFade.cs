using System;
using UnityEngine;
using UnityEngine.UI;

namespace VED.Utilities
{
    public class TransitionFade : Transition
    {
        private Image _image = null;
        private Timer _timer = null;

        private const float ALPHA_MIN = 0f;
        private const float ALPHA_MAX = 1f;

        protected void Awake()
        {
            _image = transform.Find("Image").GetComponent<Image>();
            _timer = new Timer(DEFAULT_DURATION);
        }

        public override void Stop()
        {
            _image.color = Color.clear;
            _image.enabled = false;
            _timer.Reset();
        }

        public override void In(Action callback = null, float duration = DEFAULT_DURATION)
        {
            _image.enabled = true;
            _image.color = Color.black;

            void Update() => _image.color = Color.black * new Color(1f, 1f, 1f, Mathf.Lerp(ALPHA_MIN, ALPHA_MAX, _timer.InverseElapsed));


            callback += () => _image.enabled = false;
            _timer.Set(Update, callback, duration);
        }

        public override void Out(Action callback = null, float duration = DEFAULT_DURATION)
        {
            _image.enabled = true;
            _image.color = Color.clear;

            void Update() => _image.color = Color.black * new Color(1f, 1f, 1f, Mathf.Lerp(ALPHA_MIN, ALPHA_MAX, _timer.Elapsed));

            callback += () => _image.color = Color.black;
            _timer.Set(Update, callback, duration);
        }
    }
}