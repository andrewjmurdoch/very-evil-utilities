using System;
using UnityEngine;

namespace VED.Utilities
{
    public class TimerRealTime : Timer
    {
        public override float Elapsed { get { return Duration > 0f ? Mathf.Clamp01((TimeManager.RealTime - Epoch) / Duration) : 1f; } }

        public override float InverseElapsed { get { return Duration > 0f ? (1f - Mathf.Clamp01((TimeManager.RealTime - Epoch) / Duration)) : 0f; } }

        public TimerRealTime(float duration = 1)
        {
            _currentDuration = duration;
            _defaultDuration = duration;
            Epoch = TimeManager.RealTime - (Duration * 2f);
        }

        public override void Reset()
        {
            TimeManager.Instance.RemoveTimer(this);

            _tick = null;
            _callback = null;

            _currentDuration = _defaultDuration;
            Epoch = TimeManager.RealTime - (Duration * 2f);

            UpdateDebug();
        }

        public override void Set(Action update = null, Action callback = null, float duration = 0)
        {
            Reset();

            _tick = update;
            _callback = callback;
            _currentDuration = (duration != 0) ? duration : _defaultDuration;
            Epoch = TimeManager.RealTime;

            TimeManager.Instance.AddTimer(this);
        }
    }
}