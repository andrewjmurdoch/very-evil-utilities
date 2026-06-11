using System.Collections.Generic;
using UnityEngine;

namespace VED.Utilities
{
    public class StatePointerPointed : StatePointer 
    {
        private Pointer _pointer = null;
    
        private List<Point> _pointed = null;
        private List<Point> _added   = null;
        private List<Point> _removed = null;
    
        public StatePointerPointed(Pointer pointer, List<Point> pointed)
        {
            _pointer = pointer;
            _pointed = new(pointed);
            _added   = new();
            _removed = new();
        }
    
        public override void Tick()
        {
            if (!TickPoint ()) return;
            if (!TickScroll()) return;
            if (!TickPress ()) return;
        }
    
        private bool TickPoint()
        {
            if (!_pointer.GetPointed(out List<Point> pointed))
            {
                for (int i = 0; i < _pointed.Count; i++)
                    _pointed[i].Pointable.Unpoint(_pointer);
    
                _pointer.StateManager.Pop();
                _pointer.StateManager.Push(new StatePointerUnpointed(_pointer));
                return false;
            }
    
            if (ValidatePointed(pointed, out List<Point> added, out List<Point> removed))
                return true;
    
            for (int i = 0; i < added.Count; i++)
                added[i].Pointable.Point(_pointer, added[i].Position);
            
            for (int i = 0; i < removed.Count; i++)
                removed[i].Pointable.Unpoint(_pointer);
    
            return true;
        }
    
        private bool TickScroll()
        {
            if (!_pointer.GetScroll(out Vector2 scroll))
                return true;
    
            for (int i = 0; i < _pointed.Count; i++)
                _pointed[i].Pointable.Scroll(_pointer, scroll);
    
            return true;
        }
    
        private bool TickPress()
        {
            if (!_pointer.GetPressed())
                return true;
    
            for (int i = 0; i < _pointed.Count; i++)
                _pointed[i].Pointable.Press(_pointer, _pointed[i].Position);
    
            _pointer.StateManager.Pop();
            _pointer.StateManager.Push(new StatePointerPressed(_pointer, _pointed));
            return false;
        }
    
        private bool ValidatePointed(List<Point> pointed, out List<Point> added, out List<Point> removed)
        {
            // find removed
            _removed.Clear();
            for (int i = 0; i < _pointed.Count; i++)
            {
                bool found = false;
                for (int j = 0; j < pointed.Count; j++)
                {
                    if (_pointed[i].Pointable != pointed[j].Pointable)
                        continue;
    
                    found = true;
                    break;
                }
    
                if (!found)
                    _removed.Add(_pointed[i]);
            }
    
            // find added
            _added.Clear();
            for (int i = 0; i < pointed.Count; i++)
            {
                bool found = false;
                for (int j = 0; j < _pointed.Count; j++)
                {
                    if (pointed[i].Pointable != _pointed[j].Pointable)
                        continue;
    
                    found = true;
                    break;
                }
    
                if (!found)
                    _added.Add(pointed[i]);
            }
    
            _pointed = new(pointed);
            added    = _added;
            removed  = _removed;
    
            return _added.Count <= 0 && _removed.Count <= 0;
        }
    
        public override void Disable()
        {
            for (int i = 0; i < _pointed.Count; i++)
                _pointed[i].Pointable.Unpoint(_pointer);
        }
    }
}