using System;
using System.Collections.Generic;

namespace VED.Utilities
{
    public class StateManager
    {
        public IReadOnlyCollection<State> Stack => _stack;
        private Stack<State> _stack = new Stack<State>();

        public Action<State> OnPush = null;
        public Action<State> OnPop  = null;

        public void Push(State state)
        {
            _stack.Push(state);
            OnPush?.Invoke(state);
        }

        public State Pop()
        {
            if (_stack.TryPop(out State result))
            {
                OnPop?.Invoke(result);
                return result;
            }
            return null;
        }

        public bool TryPeek(out State result)
        {
            if (_stack.TryPeek(out result))
            {
                return true; 
            }
            return false;
        }

        public void Tick()
        {
            if (_stack.TryPeek(out State result))
                result.Tick();
        }

        public void LateTick()
        {
            if (_stack.TryPeek(out State result))
                result.LateTick();
        }

        public void FixedTick()
        {
            if (_stack.TryPeek(out State result))
                result.FixedTick();
        }
    }

    public class StateManager<T> where T : State
    {
        public IReadOnlyCollection<T> Stack => _stack;
        private Stack<T> _stack = new Stack<T>();

        public Action<T> OnPush = null;
        public Action<T> OnPop = null;

        public void Push(T state)
        {
            _stack.Push(state);
            OnPush?.Invoke(state);
        }

        public T Pop()
        {
            if (_stack.TryPop(out T result))
            {
                OnPop?.Invoke(result);
                return result;
            }
            return null;
        }

        public bool TryPeek(out T result)
        {
            if (_stack.TryPeek(out result))
                return true;
            
            return false;
        }

        public void Tick()
        {
            if (_stack.TryPeek(out T result))
                result.Tick();
        }

        public void LateTick()
        {
            if (_stack.TryPeek(out T result))
                result.LateTick();
        }

        public void FixedTick()
        {
            if (_stack.TryPeek(out T result))
                result.FixedTick();
        }
    }
}