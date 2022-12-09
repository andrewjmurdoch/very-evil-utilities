using System.Collections.Generic;

namespace VED.Utilities
{
    public class TimeManager : Singleton<TimeManager>
    {
        public TimeManager() : base()
        {
            GameManager.Instance.StateManager.OnPop += OnPop;
        }

        private Dictionary<State, TimeState> _timeStates = new Dictionary<State, TimeState>();
        private TimeState _defaultTimeState = new TimeState();
        private TimeState _realTimeState = new TimeState();

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

        public void OnPop(State state)
        {
            if (_timeStates.ContainsKey(state))
            {
                _timeStates.Remove(state);
            }
        }

        public void Tick()
        {
            // update realtime 
            TickTimers(_realTimeState);
            TickAwaiters(_realTimeState);
            TickStopwatches(_realTimeState);

            // update scaled time
            TimeState timeState = GetTimeState();
            TickTimers(timeState);
            TickAwaiters(timeState);
            TickStopwatches(timeState);
        }

        public TimeState GetTimeState()
        {
            if (GameManager.Instance.StateManager.TryPeek(out State result))
            {
                if (_timeStates.ContainsKey(result))
                {
                    return _timeStates[result];
                }
                _timeStates.Add(result, new TimeState());
            };
            return _defaultTimeState;
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
            TimeState timeState = GetTimeState();
            timeState.TimersAdded.Add(timer);
        }

        public void AddTimer(TimerRealtime timer)
        {
            _realTimeState.TimersAdded.Add(timer);
        }

        public void RemoveTimer(Timer timer, TimeState timeState)
        {
            if (timeState == null) return;
            timeState.TimersRemoved.Add(timer);
        }

        public void RemoveTimer(TimerRealtime timer)
        {
            _realTimeState.TimersRemoved.Add(timer);
        }

        public void AddAwaiter(Awaiter awaiter)
        {
            TimeState timeState = GetTimeState();
            timeState.AwaitersAdded.Add(awaiter);
        }

        public void AddAwaiter(AwaiterRealtime awaiter)
        {
            _realTimeState.AwaitersAdded.Add(awaiter);
        }

        public void RemoveAwaiter(Awaiter awaiter, TimeState timeState)
        {
            if (timeState == null) return;
            timeState.AwaitersRemoved.Add(awaiter);
        }

        public void RemoveAwaiter(AwaiterRealtime awaiter)
        {
            _realTimeState.AwaitersRemoved.Add(awaiter);
        }

        public void AddStopwatch(Stopwatch stopwatch)
        {
            TimeState timeState = GetTimeState();
            timeState.StopwatchesAdded.Add(stopwatch);
        }

        public void AddStopwatch(StopwatchRealtime stopwatch)
        {
            _realTimeState.StopwatchesAdded.Add(stopwatch);
        }

        public void RemoveStopwatch(Stopwatch stopwatch, TimeState timeState)
        {
            if (timeState == null) return;
            timeState.StopwatchesRemoved.Add(stopwatch);
        }

        public void RemoveStopwatch(StopwatchRealtime stopwatch)
        {
            _realTimeState.StopwatchesRemoved.Add(stopwatch);
        }
        #endregion
    }
}