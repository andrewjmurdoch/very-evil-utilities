using System;

namespace VED.Utilities
{
    [Serializable]
    public class StopwatchRealtime : Stopwatch
    {
        public override void Start()
        {
            _running = true;
            TimeManager.Instance.AddStopwatch(this);
        }

        public override void Stop()
        {
            _running = false;
            TimeManager.Instance.RemoveStopwatch(this);
        }

        public override void Tick()
        {
            _time += UnityEngine.Time.unscaledDeltaTime;
        }
    }
}