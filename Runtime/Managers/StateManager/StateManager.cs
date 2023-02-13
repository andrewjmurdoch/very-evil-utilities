using System;
using System.Collections.Generic;

namespace VED.Utilities
{
    public class StateManager
    {
        private Stack<State> _stateStack = new Stack<State>();

        public Action OnPush = null;
        public Action<State> OnPop  = null;

        public void Push(State state, Action callback = null)
        {
            state.Enter(() =>
            {
                _stateStack.Push(state);
                callback?.Invoke();
                OnPush?.Invoke();
            });
        }

        public void Pop(Action<State> callback = null)
        {
            if (_stateStack.TryPop(out State result))
            {
                result.Exit(() =>
                {
                    callback?.Invoke(result);
                    OnPop?.Invoke(result);

                    if (_stateStack.TryPeek(out result))
                    {
                        result.Enter(null);
                    }
                });
            }
        }

        public bool TryPeek(out State result)
        {
            result = null;
            if (_stateStack.TryPeek(out result))
            {
                return true; 
            }
            return false;
        }

        public void Tick()
        {
            if (_stateStack.TryPeek(out State result)) result.Tick();
        }

        public void FixedTick()
        {
            if (_stateStack.TryPeek(out State result)) result.FixedTick();
        }
    }
}