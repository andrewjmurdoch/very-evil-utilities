using System;
using UnityEngine;

namespace VED.Utilities
{
    [Serializable]
    public class Awaiter
    {
        public float Timeout { get { return _currentTimeout; } set { _currentTimeout = value; _defaultTimeout = value; } }
        [SerializeField] protected float _currentTimeout = 0;
        protected float _defaultTimeout = 0;

        public bool Timely => Timeout <= 0 || Elapsed < Timeout;
        [SerializeField] protected bool _debugTimely = false;

        public float Epoch { get; protected set; } = 1f;
        protected float _debugEpoch = 1f;

        public virtual float Elapsed => Mathf.Max(TimeManager.Time - Epoch, 0);
        [SerializeField] protected float _debugElapsed = 1f;

        public bool Awaiting => _awaiting;
        protected bool _awaiting = false;

        protected Func<bool> _function = null;
        protected Action _callback = null;

        public Awaiter(float timeout = 0)
        {
            _currentTimeout = timeout;
            _defaultTimeout = timeout;
        }

        public virtual void Reset()
        {
            TimeManager.Instance.RemoveAwaiter(this);

            _function = null;
            _callback = null;
            _awaiting = false;

            _currentTimeout = _defaultTimeout;
            Epoch = 0;

            UpdateDebug();
        }

        public virtual void Set(Func<bool> function, Action callback, float timeout = 0)
        {
            Reset();

            _function = function;
            _callback = callback;
            _awaiting = true;

            if (timeout != 0) _currentTimeout = timeout;
            Epoch = TimeManager.Time;

            TimeManager.Instance.AddAwaiter(this);
        }

        public virtual void Tick()
        {
            UpdateDebug();

            if (!_function.Invoke() && Timely)
            {
                return;
            }

            _callback?.Invoke();
            Reset();
        }

        protected void UpdateDebug()
        {
            _debugEpoch    = Epoch;
            _debugElapsed  = Elapsed;
            _debugTimely   = Timely;
        }
    }
}