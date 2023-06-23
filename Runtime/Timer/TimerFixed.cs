using System;
using UnityEngine;

namespace VED.Utilities
{
    [Serializable]
    public class TimerFixed : Timer
    {
        public TimerFixed(float duration = 0)
        {
            _time = duration;
            _duration = duration;
            _defaultDuration = duration;
        }

        public override void Reset()
        {
            TimeManager.Instance.RemoveTimerFixed(this, _timeStateIndex);

            _time = _defaultDuration;
            _tick = null;
            _callback = null;
            _duration = _defaultDuration;
            _elapsed = Elapsed;
            _complete = Complete;
        }

        public override void Set(Action tick = null, Action callback = null, float duration = 0)
        {
            Reset();

            _time = 0f;
            _recursive = true;
            _timeStateIndex = TimeManager.Instance.TimeStates.Count - 1;
            _tick = tick;
            _callback = callback;
            _duration = (duration != 0) ? duration : _defaultDuration;

            TimeManager.Instance.AddTimerFixed(this);
        }

        public override void Tick()
        {
            _time += UnityEngine.Time.fixedDeltaTime;
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