using System;
using System.Collections.Generic;

namespace VED.Utilities
{
    public class StateManager
    {
        private Stack<State> _stateStack = new Stack<State>();

        public void Push(State state, Action callback = null)
        {
            state.Enter(() =>
            {
                _stateStack.Push(state);
                callback?.Invoke();
            });
        }

        public void Pop(Action<State> callback = null)
        {
            if (_stateStack.TryPop(out State result))
            {
                result.Exit(() =>
                {
                    callback?.Invoke(result);
                });
            }
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