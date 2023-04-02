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
            public enum Action
            {
                ADD,
                REMOVE
            }

            public class TimeStateAction<T>
            {
                public TimeStateAction(T value, Action action)
                {
                    Value = value;
                    Action = action;
                }

                public T Value;
                public Action Action = Action.ADD;
            }

            public List<Timer> Timers = new List<Timer>();
            public List<TimeStateAction<Timer>> TimeStateActionsTimer = new List<TimeStateAction<Timer>>();

            public List<Awaiter> Awaiters = new List<Awaiter>();
            public List<TimeStateAction<Awaiter>> TimeStateActionsAwaiter = new List<TimeStateAction<Awaiter>>();

            public List<Stopwatch> Stopwatches = new List<Stopwatch>();
            public List<TimeStateAction<Stopwatch>> TimeStateActionsStopwatch = new List<TimeStateAction<Stopwatch>>();
        }

        private void OnPush(State state)
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
            for (int i = 0; i < timeState.TimeStateActionsTimer.Count; i++)
            {
                if (timeState.TimeStateActionsTimer[i].Action == TimeState.Action.ADD)
                {
                    timeState.Timers.Add(timeState.TimeStateActionsTimer[i].Value);
                    continue;
                }

                if (timeState.TimeStateActionsTimer[i].Action == TimeState.Action.REMOVE)
                {
                    timeState.Timers.Remove(timeState.TimeStateActionsTimer[i].Value);
                    continue;
                }
            }
            timeState.TimeStateActionsTimer.Clear();
            timeState.Timers.ForEach(t => t.Tick());
        }

        private void TickAwaiters(TimeState timeState)
        {
            for (int i = 0; i < timeState.TimeStateActionsAwaiter.Count; i++)
            {
                if (timeState.TimeStateActionsAwaiter[i].Action == TimeState.Action.ADD)
                {
                    timeState.Awaiters.Add(timeState.TimeStateActionsAwaiter[i].Value);
                    continue;
                }

                if (timeState.TimeStateActionsAwaiter[i].Action == TimeState.Action.REMOVE)
                {
                    timeState.Awaiters.Remove(timeState.TimeStateActionsAwaiter[i].Value);
                    continue;
                }
            }
            timeState.TimeStateActionsAwaiter.Clear();
            timeState.Awaiters.ForEach(a => a.Tick());
        }

        private void TickStopwatches(TimeState timeState)
        {
            for (int i = 0; i < timeState.TimeStateActionsStopwatch.Count; i++)
            {
                if (timeState.TimeStateActionsStopwatch[i].Action == TimeState.Action.ADD)
                {
                    timeState.Stopwatches.Add(timeState.TimeStateActionsStopwatch[i].Value);
                    continue;
                }

                if (timeState.TimeStateActionsStopwatch[i].Action == TimeState.Action.REMOVE)
                {
                    timeState.Stopwatches.Remove(timeState.TimeStateActionsStopwatch[i].Value);
                    continue;
                }
            }
            timeState.TimeStateActionsStopwatch.Clear();
            timeState.Stopwatches.ForEach(s => s.Tick());
        }

        #region Add/Remove
        public void AddTimer(Timer timer)
        {
            _timeStates[^1].TimeStateActionsTimer.Add(new TimeState.TimeStateAction<Timer>(timer, TimeState.Action.ADD));
        }

        public void AddTimer(TimerRealtime timer)
        {
            _realTimeState.TimeStateActionsTimer.Add(new TimeState.TimeStateAction<Timer>(timer, TimeState.Action.ADD));
        }

        public void RemoveTimer(Timer timer, int timeStateIndex)
        {
            if (timeStateIndex >= _timeStates.Count) return;
            _timeStates[timeStateIndex].TimeStateActionsTimer.Add(new TimeState.TimeStateAction<Timer>(timer, TimeState.Action.REMOVE));
        }

        public void RemoveTimer(TimerRealtime timer)
        {
            _realTimeState.TimeStateActionsTimer.Add(new TimeState.TimeStateAction<Timer>(timer, TimeState.Action.REMOVE));
        }

        public void AddAwaiter(Awaiter awaiter)
        {
            _timeStates[^1].TimeStateActionsAwaiter.Add(new TimeState.TimeStateAction<Awaiter>(awaiter, TimeState.Action.ADD));
        }

        public void AddAwaiter(AwaiterRealtime awaiter)
        {
            _realTimeState.TimeStateActionsAwaiter.Add(new TimeState.TimeStateAction<Awaiter>(awaiter, TimeState.Action.ADD));
        }

        public void RemoveAwaiter(Awaiter awaiter, int timeStateIndex)
        {
            if (timeStateIndex >= _timeStates.Count) return;
            _timeStates[timeStateIndex].TimeStateActionsAwaiter.Add(new TimeState.TimeStateAction<Awaiter>(awaiter, TimeState.Action.REMOVE));
        }

        public void RemoveAwaiter(AwaiterRealtime awaiter)
        {
            _realTimeState.TimeStateActionsAwaiter.Add(new TimeState.TimeStateAction<Awaiter>(awaiter, TimeState.Action.REMOVE));
        }

        public void AddStopwatch(Stopwatch stopwatch)
        {
            _timeStates[^1].TimeStateActionsStopwatch.Add(new TimeState.TimeStateAction<Stopwatch>(stopwatch, TimeState.Action.ADD));
        }

        public void AddStopwatch(StopwatchRealtime stopwatch)
        {
            _realTimeState.TimeStateActionsStopwatch.Add(new TimeState.TimeStateAction<Stopwatch>(stopwatch, TimeState.Action.ADD));
        }

        public void RemoveStopwatch(Stopwatch stopwatch, int timeStateIndex)
        {
            if (timeStateIndex >= _timeStates.Count) return;
            _timeStates[timeStateIndex].TimeStateActionsStopwatch.Add(new TimeState.TimeStateAction<Stopwatch>(stopwatch, TimeState.Action.REMOVE));
        }

        public void RemoveStopwatch(StopwatchRealtime stopwatch)
        {
            _realTimeState.TimeStateActionsStopwatch.Add(new TimeState.TimeStateAction<Stopwatch>(stopwatch, TimeState.Action.REMOVE));
        }
        #endregion
    }
}