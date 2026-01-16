using System;

namespace VED.Utilities
{
    [Serializable]
    public class AwaiterRealtime : Awaiter
    {
        public AwaiterRealtime(float timeout = 0f) : base(timeout) { }

        public override void Reset()
        {
            TimeManager.Instance.RemoveAwaiter(this);

            _function = null;
            _callback = null;
            _awaiting = false;
            _timeout  = _defaultTimeout;
            _timely   = Timely;
            _time     = 0f;
        }

        public override void Set(Func<bool> function, Action callback, float timeout = 0f)
        {
            Reset();

            _awaiting  = true;
            _recursive = true;
            _function  = function;
            _callback  = callback;
            _timeout   = timeout == 0f ? _defaultTimeout : timeout;

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