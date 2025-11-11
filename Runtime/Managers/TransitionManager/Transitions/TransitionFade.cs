using System;
using UnityEngine;
using UnityEngine.UI;

namespace VED.Utilities
{
    public class TransitionFade : Transition
    {
        private TimerRealtime _timer = null;

        private const float ALPHA_MIN = 000.000f;
        private const float ALPHA_MAX = 001.000f;
        private const float DURATION  = 000.500f;

        [SerializeField] private RawImage _rawImage = null;

        [SerializeField] private Easing.Controller _easingController = new Easing.Controller(Easing.Shape.SINE, Easing.Extent.INOUT);

        protected override void Awake()
        {
            base.Awake();
            _timer = new TimerRealtime(DURATION);
        }

        public override void Stop()
        {
            _rawImage.color = Color.clear;
            _timer.Reset();
        }

        public override void In(Action callback = null)
        {
            _rawImage.color = Color.black;
            void Update() => _rawImage.color = Color.black * new Color(1f, 1f, 1f, _easingController.Ease(ALPHA_MIN, ALPHA_MAX, _timer.InverseElapsed));
            callback += () => _rawImage.color = Color.clear;
            _timer.Set(Update, callback);
        }

        public override void Out(Action callback = null)
        {
            _rawImage.color = Color.clear;
            void Update() => _rawImage.color = Color.black * new Color(1f, 1f, 1f, _easingController.Ease(ALPHA_MIN, ALPHA_MAX, _timer.Elapsed));
            callback += () => _rawImage.color = Color.black;
            _timer.Set(Update, callback);
        }

        public override void SetIn()
        {
            _rawImage.color = Color.clear;
        }

        public override void SetOut()
        {
            _rawImage.color = Color.black;
        }
    }
}