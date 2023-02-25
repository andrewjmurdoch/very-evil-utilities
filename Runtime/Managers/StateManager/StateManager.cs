using System;
using System.Collections.Generic;

namespace VED.Utilities
{
    public class StateManager
    {
        public IReadOnlyCollection<State> Stack => _stack;
        private Stack<State> _stack = new Stack<State>();

        public Action OnPush = null;
        public Action<State> OnPop  = null;

        public void Push(State state, Action callback = null)
        {
            state.Enter(() =>
            {
                _stack.Push(state);
                OnPush?.Invoke();
                callback?.Invoke();
            });
        }

        public void Pop(Action<State> callback = null)
        {
            if (_stack.TryPop(out State result))
            {
                result.Exit(() =>
                {
                    OnPop?.Invoke(result);
                    callback?.Invoke(result);
                });
            }
        }

        public bool TryPeek(out State result)
        {
            result = null;
            if (_stack.TryPeek(out result))
            {
                return true; 
            }
            return false;
        }

        public void Tick()
        {
            if (_stack.TryPeek(out State result)) result.Tick();
        }

        public void FixedTick()
        {
            if (_stack.TryPeek(out State result)) result.FixedTick();
        }
    }
}