using System;
using System.Collections.Generic;
using UnityEngine;

namespace VED.Utilities
{
    public class TimeManager : Singleton<TimeManager>
    {
        public static float TimeScale 
        {
            get { return Instance._timeScale; } 
            set { Instance.SetTimeScale(value); }
        }
        [SerializeField] private float _timeScale = 1f;

        public static Action<float> OnTimeScaleChange;

        public static float DeltaTime => UnityEngine.Time.deltaTime * TimeScale;

        public static float FixedDeltaTime => UnityEngine.Time.fixedDeltaTime * TimeScale;

        public static float RealTime => UnityEngine.Time.unscaledTime;

        public static float Time => Instance._time;
        private float _time = 0f;

        private List<Timer> _timers = new List<Timer>();
        private List<Timer> _timersAdded = new List<Timer>();
        private List<Timer> _timersRemoved = new List<Timer>();

        private List<Awaiter> _awaiters = new List<Awaiter>();
        private List<Awaiter> _awaitersAdded = new List<Awaiter>();
        private List<Awaiter> _awaitersRemoved = new List<Awaiter>();

        private List<Stopwatch> _stopwatches = new List<Stopwatch>();
        private List<Stopwatch> _stopwatchesAdded = new List<Stopwatch>();
        private List<Stopwatch> _stopwatchesRemoved = new List<Stopwatch>();

        private void SetTimeScale(float value)
        {
            _timeScale = Mathf.Max(0f, value);
            OnTimeScaleChange?.Invoke(_timeScale);
        }

        public void Tick()
        {
            _time += UnityEngine.Time.deltaTime * _timeScale;

            TickTimers();
            TickAwaiters();
            TickStopwatches();
        }

        private void TickTimers()
        {
            _timersRemoved.ForEach(t => _timers.Remove(t));
            _timersRemoved.Clear();
            _timersAdded.ForEach(t => _timers.Add(t));
            _timersAdded.Clear();
            _timers.ForEach(t => t.Tick());
        }

        private void TickAwaiters()
        {
            _awaitersRemoved.ForEach(a => _awaiters.Remove(a));
            _awaitersRemoved.Clear();
            _awaitersAdded.ForEach(a => _awaiters.Add(a));
            _awaitersAdded.Clear();
            _awaiters.ForEach(a => a.Tick());
        }

        private void TickStopwatches()
        {
            _stopwatchesRemoved.ForEach(s => _stopwatches.Remove(s));
            _stopwatchesRemoved.Clear();
            _stopwatchesAdded.ForEach(s => _stopwatches.Add(s));
            _stopwatchesAdded.Clear();
            _stopwatches.ForEach(s => s.Tick());
        }

        public void AddTimer(Timer timer)
        {
            _timersAdded.Add(timer);
        }

        public void RemoveTimer(Timer timer)
        {
            _timersRemoved.Add(timer);
        }

        public void AddAwaiter(Awaiter awaiter)
        {
            _awaitersAdded.Add(awaiter);
        }

        public void RemoveAwaiter(Awaiter awaiter)
        {
            _awaitersRemoved.Add(awaiter);
        }
        public void AddStopwatch(Stopwatch stopwatch)
        {
            _stopwatchesAdded.Add(stopwatch);
        }

        public void RemoveStopwatch(Stopwatch stopwatch)
        {
            _stopwatchesRemoved.Add(stopwatch);
        }
    }
}