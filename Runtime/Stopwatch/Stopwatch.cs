using System;
using UnityEngine;

namespace VED.Utilities
{
    [Serializable]
    public class Stopwatch
    {
        public float Epoch { get; protected set; } = 0f;
        protected float _debugEpoch = 0f;

        public float End { get; protected set; } = 0f;
        protected float _debugEnd = 0f;

        public virtual float Elapsed => End - Epoch;
        [SerializeField, ReadOnly] protected float _debugElapsed = 0f;

        public bool Running { get; protected set; } = false;
        protected bool _debugRunning = false;

        public virtual void Start()
        {
            Running = true;
            Epoch = TimeManager.Time;
            End = TimeManager.Time;
            TimeManager.Instance.AddStopwatch(this);
            UpdateDebug();
        }

        public virtual void Stop()
        {
            Running = false;
            End = TimeManager.Time;
            TimeManager.Instance.RemoveStopwatch(this);
            UpdateDebug();
        }

        public virtual void Restart()
        {
            Epoch = TimeManager.Time;
            End = TimeManager.Time;
            UpdateDebug();
        }

        public virtual void Tick()
        {
            End = TimeManager.Time;
            UpdateDebug();
        }

        protected void UpdateDebug()
        {
            _debugEpoch   = Epoch;
            _debugEnd     = End;
            _debugElapsed = Elapsed;
            _debugRunning = Running;
        }
    }
}