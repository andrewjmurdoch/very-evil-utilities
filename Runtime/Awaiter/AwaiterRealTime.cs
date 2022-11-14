using System;
using UnityEngine;

namespace VED.Utilities
{
    public class AwaiterRealTime : Awaiter
    {
        public override float Elapsed => Mathf.Max(TimeManager.RealTime - Epoch, 0);

        public AwaiterRealTime(float timeout = 0) : base(timeout) { }

        public override void Set(Func<bool> function, Action callback, float timeout = 0)
        {
            Reset();

            _function = function;
            _callback = callback;
            _awaiting = true;

            if (timeout != 0) _currentTimeout = timeout;
            Epoch = TimeManager.RealTime;

            TimeManager.Instance.AddAwaiter(this);
        }
    }
}