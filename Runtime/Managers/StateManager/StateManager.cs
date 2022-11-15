using System.Collections.Generic;

namespace VED.Utilities
{
    public class StateManager
    {
        private Stack<State> _stateStack = new Stack<State>();

        public void Push(State state)
        {
            _stateStack.Push(state);
        }

        public State Pop()
        {
            if (_stateStack.TryPop(out State result)) return result;
            return null;
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