using System;
using UnityEngine;
using UnityEngine.UI;

namespace VED.Utilities
{
    public class TransitionFade : Transition
    {
        private TimerRealtime _timer = new(DURATION);

        private const float ALPHA_MIN = 000.000f;
        private const float ALPHA_MAX = 001.000f;
        private const float DURATION  = 000.500f;

        [SerializeField] private Image _image = null;

        [SerializeField] private Easing.Controller _easingController = new Easing.Controller(Easing.Shape.SINE, Easing.Extent.INOUT);

        public override void Stop()
        {
            _image.color = Color.clear;
            _timer.Reset();
        }

        public override void In(Action callback = null)
        {
            _image.color = Color.black;

            void Update() => _image.color = Color.black * new Color(1f, 1f, 1f, _easingController.Ease(ALPHA_MIN, ALPHA_MAX, _timer.InverseElapsed));
            callback += () => _image.color = Color.clear;

            _timer.Set(Update, callback);
        }

        public override void Out(Action callback = null)
        {
            _image.color = Color.clear;

            void Update() => _image.color = Color.black * new Color(1f, 1f, 1f, _easingController.Ease(ALPHA_MIN, ALPHA_MAX, _timer.Elapsed));
            callback += () => _image.color = Color.black;

            _timer.Set(Update, callback);
        }

        public override void SetIn()
        {
            _image.color = Color.clear;
        }

        public override void SetOut()
        {
            _image.color = Color.black;
        }
    }
}