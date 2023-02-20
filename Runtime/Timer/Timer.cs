using System;
using UnityEngine;

namespace VED.Utilities
{
    [Serializable]
    public class Timer
    {
        public virtual float Time => _time;
        [SerializeField, ReadOnly] protected float _time = 0f;

        public float Duration { get { return _duration; } set { _duration = value; _defaultDuration = value; } }
        [SerializeField, ReadOnly] protected float _duration = 0;
        protected float _defaultDuration = 0;

        public virtual float Elapsed => Duration > 0f ? Mathf.Clamp01(Time / Duration) : 1f;
        [SerializeField, ReadOnly] protected float _elapsed = 1f;

        public bool Complete => Elapsed >= 1f;
        [SerializeField, ReadOnly] protected bool _complete = true;

        public virtual float InverseElapsed => 1f - Elapsed;

        public float InverseTime => InverseElapsed * Duration;

        protected Action _tick = null;
        protected Action _callback = null;

        protected bool _recursive = false;
        protected int _timeStateIndex = 0;

        public Timer(float duration = 0)
        {
            _time = duration;
            _duration = duration;
            _defaultDuration = duration;
        }

        public virtual void Reset()
        {
            TimeManager.Instance.RemoveTimer(this, _timeStateIndex);

            _time = _defaultDuration;
            _tick = null;
            _callback = null;
            _duration = _defaultDuration;
            _elapsed = Elapsed;
            _complete = Complete;
        }

        public virtual void Set(Action tick = null, Action callback = null, float duration = 0)
        {
            Reset();

            _time = 0f;
            _recursive = true;
            _timeStateIndex = TimeManager.Instance.TimeStates.Count - 1;
            _tick = tick;
            _callback = callback;
            _duration = (duration != 0) ? duration : _defaultDuration;

            TimeManager.Instance.AddTimer(this);
        }

        public virtual void Tick()
        {
            _time += UnityEngine.Time.deltaTime;
            _elapsed = Elapsed;
            _complete = Complete;

            if (!Complete)
            {
                _tick?.Invoke();
                return;
            }

            _recursive = false;
            _tick?.Invoke();
            _callback?.Invoke();

            // if timer has set itself within tick or callback invocations, it is recursive and shouldn't reset here
            if (!_recursive) Reset();
        }
    }
}