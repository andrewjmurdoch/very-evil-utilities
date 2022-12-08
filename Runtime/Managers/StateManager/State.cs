using System;

namespace VED.Utilities
{
    public abstract class State
    {
        public virtual void Enter(Action callback) { callback?.Invoke(); }
        public virtual void Exit(Action callback) { callback?.Invoke(); }
        public virtual void Tick() { }
        public virtual void FixedTick() { }
    }
}