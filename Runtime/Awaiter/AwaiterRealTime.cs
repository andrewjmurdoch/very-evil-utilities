using System;

namespace VED.Utilities
{
    [Serializable]
    public class AwaiterRealtime : Awaiter
    {
        public AwaiterRealtime(float timeout = 0) : base(timeout) { }

        public override void Reset()
        {
            TimeManager.Instance.RemoveAwaiter(this);

            _function = null;
            _callback = null;
            _awaiting = false;
            _timeout = _defaultTimeout;
            _timely = Timely;
        }

        public override void Set(Func<bool> function, Action callback, float timeout = 0)
        {
            Reset();

            _awaiting = true;
            _recursive = true;
            _function = function;
            _callback = callback;
            _timeState = TimeManager.Instance.GetTimeState();
            _timeout = timeout == 0 ? _defaultTimeout : timeout;

            TimeManager.Instance.AddAwaiter(this);
        }

        public override void Tick()
        {
            _time += UnityEngine.Time.unscaledDeltaTime;
            _timely = Timely;

            if (!_function.Invoke() && Timely)
            {
                return;
            }

            _recursive = false;
            _callback?.Invoke();

            if (!_recursive) Reset();
        }
    }
}