using System;
using UnityEngine;

namespace VED.Utilities
{
    [Serializable]
    public class Stopwatch
    {
        public virtual float Time => _time;
        [SerializeField, ReadOnly] protected float _time = 0f;

        public bool Running => _running;
        [SerializeField, ReadOnly] protected bool _running = false;

        protected TimeManager.TimeState _timeState = null;

        public virtual void Start()
        {
            _running = true;
            _timeState = TimeManager.Instance.GetTimeState();
            TimeManager.Instance.AddStopwatch(this);
        }

        public virtual void Stop()
        {
            _running = false;
            TimeManager.Instance.RemoveStopwatch(this, _timeState);
        }

        public virtual void Restart()
        {
            _time = 0f;
        }

        public virtual void Tick()
        {
            _time += UnityEngine.Time.deltaTime;
        }
    }
}