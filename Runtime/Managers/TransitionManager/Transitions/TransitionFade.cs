using System;
using UnityEngine;
using UnityEngine.UI;

namespace VED.Utilities
{
    public class TransitionFade : Transition
    {
        private TimerRealtime _timer = null;

        private const float ALPHA_MIN = 0f;
        private const float ALPHA_MAX = 1f;

        protected override void Awake()
        {
            base.Awake();

            _timer = new TimerRealtime(DEFAULT_DURATION);
        }

        public override void Stop()
        {
            _uiBlock2D.Color = Color.clear;
            _timer.Reset();
        }

        public override void In(Action callback = null, float duration = DEFAULT_DURATION)
        {
            _uiBlock2D.Color = Color.black;

            void Update() => _uiBlock2D.Color = Color.black * new Color(1f, 1f, 1f, Mathf.Lerp(ALPHA_MIN, ALPHA_MAX, _timer.InverseElapsed));

            callback += () => _uiBlock2D.Color = Color.clear;
            _timer.Set(Update, callback, duration);
        }

        public override void Out(Action callback = null, float duration = DEFAULT_DURATION)
        {
            _uiBlock2D.Color = Color.clear;

            void Update() => _uiBlock2D.Color = Color.black * new Color(1f, 1f, 1f, Mathf.Lerp(ALPHA_MIN, ALPHA_MAX, _timer.Elapsed));

            callback += () => _uiBlock2D.Color = Color.black;
            _timer.Set(Update, callback, duration);
        }
    }
}