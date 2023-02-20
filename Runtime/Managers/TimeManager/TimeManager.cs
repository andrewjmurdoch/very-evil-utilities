using System.Collections.Generic;

namespace VED.Utilities
{
    public class TimeManager : Singleton<TimeManager>
    {
        public TimeManager() : base()
        {
            _defaultTimeState = new TimeState();
            _realTimeState = new TimeState();

            _timeStates = new List<TimeState> { _defaultTimeState };
            for (int i = 0; i < GameManager.Instance.StateManager.Stack.Count; i++)
            {
                _timeStates.Add(new TimeState());
            }

            GameManager.Instance.StateManager.OnPush += OnPush;
            GameManager.Instance.StateManager.OnPop += OnPop;
        }

        public IReadOnlyList<TimeState> TimeStates => _timeStates;
        private List<TimeState> _timeStates = null;
        private TimeState _defaultTimeState = null;
        private TimeState _realTimeState = null;

        public class TimeState
        {
            public List<Timer> Timers = new List<Timer>();
            public List<Timer> TimersAdded = new List<Timer>();
            public List<Timer> TimersRemoved = new List<Timer>();

            public List<Awaiter> Awaiters = new List<Awaiter>();
            public List<Awaiter> AwaitersAdded = new List<Awaiter>();
            public List<Awaiter> AwaitersRemoved = new List<Awaiter>();

            public List<Stopwatch> Stopwatches = new List<Stopwatch>();
            public List<Stopwatch> StopwatchesAdded = new List<Stopwatch>();
            public List<Stopwatch> StopwatchesRemoved = new List<Stopwatch>();
        }

        private void OnPush()
        {
            _timeStates.Add(new TimeState());
        }

        private void OnPop(State state)
        {
            _timeStates.Remove(_timeStates[^1]);
        }

        public void Tick()
        {
            // update realtime 
            TickTimers(_realTimeState);
            TickAwaiters(_realTimeState);
            TickStopwatches(_realTimeState);

            // update scaled time
            TimeState timeState = _timeStates[^1];
            TickTimers(timeState);
            TickAwaiters(timeState);
            TickStopwatches(timeState);
        }

        private void TickTimers(TimeState timeState)
        {
            timeState.TimersRemoved.ForEach(t => timeState.Timers.Remove(t));
            timeState.TimersRemoved.Clear();
            timeState.TimersAdded.ForEach(t => timeState.Timers.Add(t));
            timeState.TimersAdded.Clear();
            timeState.Timers.ForEach(t => t.Tick());
        }

        private void TickAwaiters(TimeState timeState)
        {
            timeState.AwaitersRemoved.ForEach(a => timeState.Awaiters.Remove(a));
            timeState.AwaitersRemoved.Clear();
            timeState.AwaitersAdded.ForEach(a => timeState.Awaiters.Add(a));
            timeState.AwaitersAdded.Clear();
            timeState.Awaiters.ForEach(a => a.Tick());
        }

        private void TickStopwatches(TimeState timeState)
        {
            timeState.StopwatchesRemoved.ForEach(s => timeState.Stopwatches.Remove(s));
            timeState.StopwatchesRemoved.Clear();
            timeState.StopwatchesAdded.ForEach(s => timeState.Stopwatches.Add(s));
            timeState.StopwatchesAdded.Clear();
            timeState.Stopwatches.ForEach(s => s.Tick());
        }

        #region Add/Remove
        public void AddTimer(Timer timer)
        {
            _timeStates[^1].TimersAdded.Add(timer);
        }

        public void AddTimer(TimerRealtime timer)
        {
            _realTimeState.TimersAdded.Add(timer);
        }

        public void RemoveTimer(Timer timer, int timeStateIndex)
        {
            if (timeStateIndex >= _timeStates.Count) return;
            _timeStates[timeStateIndex].TimersRemoved.Add(timer);
        }

        public void RemoveTimer(TimerRealtime timer)
        {
            _realTimeState.TimersRemoved.Add(timer);
        }

        public void AddAwaiter(Awaiter awaiter)
        {
            _timeStates[^1].AwaitersAdded.Add(awaiter);
        }

        public void AddAwaiter(AwaiterRealtime awaiter)
        {
            _realTimeState.AwaitersAdded.Add(awaiter);
        }

        public void RemoveAwaiter(Awaiter awaiter, int timeStateIndex)
        {
            if (timeStateIndex >= _timeStates.Count) return;
            _timeStates[timeStateIndex].AwaitersRemoved.Add(awaiter);
        }

        public void RemoveAwaiter(AwaiterRealtime awaiter)
        {
            _realTimeState.AwaitersRemoved.Add(awaiter);
        }

        public void AddStopwatch(Stopwatch stopwatch)
        {
            _timeStates[^1].StopwatchesAdded.Add(stopwatch);
        }

        public void AddStopwatch(StopwatchRealtime stopwatch)
        {
            _realTimeState.StopwatchesAdded.Add(stopwatch);
        }

        public void RemoveStopwatch(Stopwatch stopwatch, int timeStateIndex)
        {
            if (timeStateIndex >= _timeStates.Count) return;
            _timeStates[timeStateIndex].StopwatchesRemoved.Add(stopwatch);
        }

        public void RemoveStopwatch(StopwatchRealtime stopwatch)
        {
            _realTimeState.StopwatchesRemoved.Add(stopwatch);
        }
        #endregion
    }
}