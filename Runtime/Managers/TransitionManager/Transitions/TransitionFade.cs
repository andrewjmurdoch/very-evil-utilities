using System;
using UnityEngine;

namespace VED.Utilities
{
    public class TransitionFade : Transition
    {
        private TimerRealtime _timer = null;

        private const float ALPHA_MIN = 000.000f;
        private const float ALPHA_MAX = 001.000f;
        private const float DURATION  = 000.500f;

        [SerializeField] private Easing.Controller _easingController = new Easing.Controller(Easing.Shape.SINE, Easing.Extent.INOUT);

        protected override void Awake()
        {
            base.Awake();
            _timer = new TimerRealtime(DURATION);
        }

        public override void Stop()
        {
            _uiBlock2D.Color = Color.clear;
            _timer.Reset();
        }

        public override void In(Action callback = null)
        {
            _uiBlock2D.Color = Color.black;
            void Update() => _uiBlock2D.Color = Color.black * new Color(1f, 1f, 1f, _easingController.Ease(ALPHA_MIN, ALPHA_MAX, _timer.InverseElapsed));
            callback += () => _uiBlock2D.Color = Color.clear;
            _timer.Set(Update, callback);
        }

        public override void Out(Action callback = null)
        {
            _uiBlock2D.Color = Color.clear;
            void Update() => _uiBlock2D.Color = Color.black * new Color(1f, 1f, 1f, _easingController.Ease(ALPHA_MIN, ALPHA_MAX, _timer.Elapsed));
            callback += () => _uiBlock2D.Color = Color.black;
            _timer.Set(Update, callback);
        }
    }
}