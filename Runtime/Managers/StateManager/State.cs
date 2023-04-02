namespace VED.Utilities
{
    public abstract class State
    {
        public virtual void Tick() { }
        public virtual void LateTick() { }
        public virtual void FixedTick() { }
    }
}