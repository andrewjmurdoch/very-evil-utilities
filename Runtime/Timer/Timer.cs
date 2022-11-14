using System;
using UnityEngine;

namespace VED.Utilities
{
    [Serializable]
    public class Timer
    {
        public float Duration { get { return _currentDuration; } set { _currentDuration = value; _defaultDuration = value; } }
        [SerializeField, ReadOnly] protected float _currentDuration = 0;
        protected float _defaultDuration = 0;

        public float Epoch { get; protected set; } = 1f;
        protected float _debugEpoch = 1f;

        public virtual float Elapsed => Duration > 0f ? Mathf.Clamp01((TimeManager.Time - Epoch) / Duration) : 1f;
        [SerializeField, ReadOnly] protected float _debugElapsed = 1f;

        public virtual float InverseElapsed => 1f - Elapsed;
        protected float _debugInverseElapsed = 1f;

        public bool Complete => Elapsed >= 1f;
        protected bool _debugComplete = true;

        public float Time => Elapsed * Duration;
        [SerializeField, ReadOnly] protected float _debugTime = 1f;

        public float InverseTime => InverseElapsed * Duration;
        protected float _debugInverseTime = 0f;

        protected Action _tick = null;
        protected Action _callback = null;

        private bool _recursive = false;

        public Timer(float duration = 0)
        {
            _currentDuration = duration;
            _defaultDuration = duration;
            Epoch = TimeManager.Time - (Duration * 2f);
        }

        public virtual void Reset()
        {
            TimeManager.Instance.RemoveTimer(this);

            _tick = null;
            _callback = null;

            _currentDuration = _defaultDuration;
            Epoch = TimeManager.Time - (Duration * 2f);

            UpdateDebug();
        }

        public virtual void Set(Action update = null, Action callback = null, float duration = 0)
        {
            Reset();

            _recursive = true;

            _tick = update;
            _callback = callback;
            _currentDuration = (duration != 0) ? duration : _defaultDuration;
            Epoch = TimeManager.Time;

            TimeManager.Instance.AddTimer(this);
        }

        public void Tick()
        {
            UpdateDebug();

            if (!Complete)
            {
                _tick?.Invoke();
                return;
            }

            _recursive = false;

            _tick?.Invoke();
            _callback?.Invoke();

            if (!_recursive) Reset();
        }

        protected void UpdateDebug()
        {
            _debugEpoch          = Epoch;
            _debugElapsed        = Elapsed;
            _debugInverseElapsed = InverseElapsed;
            _debugComplete       = Complete;
            _debugTime           = Time;
            _debugInverseTime    = InverseTime;
        }
    }
}