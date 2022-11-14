namespace VED.Utilities
{
    public abstract class State
    {
        public abstract void Enter();
        public abstract void Exit();
        public abstract void Tick();
        public abstract void FixedTick();
    }
}