using System;
using UnityEngine;

namespace VED.Utilities
{
    [Serializable]
    public class Awaiter
    {
        public virtual float Time => _time;
        [SerializeField, ReadOnly] protected float _time = 0f;

        public float Timeout { get { return _timeout; } set { _timeout = value; _defaultTimeout = value; } }
        [SerializeField, ReadOnly] protected float _timeout = 0;
        protected float _defaultTimeout = 0;

        public bool Timely => _timeout <= 0 || _time < _timeout;
        [SerializeField, ReadOnly] protected bool _timely = false;

        public bool Awaiting => _awaiting;
        [SerializeField, ReadOnly] protected bool _awaiting = false;

        protected Func<bool> _function = null;
        protected Action _callback = null;
        protected bool _recursive = false;
        protected int _timeStateIndex = 0;

        public Awaiter(float timeout = 0)
        {
            _timeout = timeout;
            _defaultTimeout = timeout;
        }

        public virtual void Reset()
        {
            TimeManager.Instance.RemoveAwaiter(this, _timeStateIndex);

            _function = null;
            _callback = null;
            _awaiting = false;
            _timeout  = _defaultTimeout;
            _timely   = Timely;
        }

        public virtual void Set(Func<bool> function, Action callback, float timeout = 0)
        {
            Reset();

            _awaiting  = true;
            _recursive = true;
            _function  = function;
            _callback  = callback;
            _timeStateIndex = TimeManager.Instance.TimeStates.Count - 1;
            _timeout   = timeout == 0 ? _defaultTimeout : timeout;

            TimeManager.Instance.AddAwaiter(this);
        }

        public virtual void Tick()
        {
            _time += UnityEngine.Time.deltaTime;
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