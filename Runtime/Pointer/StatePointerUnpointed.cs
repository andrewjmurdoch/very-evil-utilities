using System.Collections.Generic;

namespace VED.Utilities
{
    public class StatePointerUnpointed : StatePointer
    {
        private Pointer _pointer = null;
    
        public StatePointerUnpointed(Pointer pointer)
        {
            _pointer = pointer;
        }
    
        public override void Tick()
        {
            if (!TickPress()) return;
            if (!TickPoint()) return;
        }
    
        private bool TickPress()
        {
            // pressing, can't point
            if (_pointer.GetPressed())
                return false;
    
            return true;
        }
    
        private bool TickPoint()
        {
            if (!_pointer.GetPointed(out List<Point> pointed))
                return true;
    
            for (int i = 0; i < pointed.Count; i++)
                pointed[i].Pointable.Point(_pointer, pointed[i].Position);
    
            _pointer.StateManager.Pop();
            _pointer.StateManager.Push(new StatePointerPointed(_pointer, pointed));
            return false;
        }
    }
}