using System;

namespace VED.Utilities
{
    [Serializable]
    public class TimerRealtime : Timer
    {
        public TimerRealtime(float duration = 0) : base(duration) { }

        public override void Reset()
        {
            TimeManager.Instance.RemoveTimer(this);

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
            _tick = tick;
            _callback = callback;
            _duration = (duration != 0) ? duration : _defaultDuration;

            TimeManager.Instance.AddTimer(this);
        }

        public override void Tick()
        {
            _time += UnityEngine.Time.unscaledDeltaTime;
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