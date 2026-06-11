using System.Collections.Generic;
using UnityEngine;

namespace VED.Utilities
{
    public struct Point
    {
        public Pointable Pointable;
        public Vector3   Position;
    
        public Point(Pointable pointable, Vector3 position)
        {
            Pointable = pointable;
            Position  = position;
        }
    }
    
    public struct Drag
    {
        public Vector3 From;
        public Vector3 To;
    
        public Drag(Vector3 from, Vector3 to)
        {
            From = from;
            To   = to;
        }
    }
    
    public struct Swipe
    {
        public Vector3 Position;
        public Vector3 Direction;
        public Vector3 Skew;
        public float   Magnitude;
        public float   Duration;
        public float   Speed;
    }
    
    public struct Circle
    {
        public Vector3 Normal;
        public Vector3 Centre;
        public float   Radius;
    
        public Circle(Vector3 normal, Vector3 centre, float radius)
        {
            Normal = normal;
            Centre = centre;
            Radius = radius;
        }
    }
    
    public abstract class Pointer
    {
        public List<Pointable> Pointables = null;
    
        public    StateManager<StatePointer> StateManager => _stateManager;
        protected StateManager<StatePointer> _stateManager = null;
    
        protected List<string> _disablers = null;
    
        public    List<Point>  Pointed => _pointed;
        protected List<Point> _pointed = null;
    
        public  bool  Enabled => _disablers.Count <= 0;
    
        public abstract bool GetRay(out Ray ray);
        public abstract bool GetPressed();
        public abstract bool GetScroll (out Vector2 scroll);
    
        public virtual void Init(List<Pointable> pointables = null)
        {
            Pointables = pointables != null 
                ? pointables 
                : new List<Pointable> { };
    
            _disablers = new();
            _pointed   = new();
    
            _stateManager = new();
            _stateManager.Push(new StatePointerUnpointed(this));
        }
        
        public virtual void Enable(string key)
        {
            _disablers.Remove(key);
    
            if (_disablers.Count > 0)
                return;
    
            if (_stateManager.TryPeek(out StatePointer statePointer))
                return;
    
            _stateManager.Push(new StatePointerUnpointed(this));
        }
    
        public virtual void Disable(string key)
        {
            if (_disablers.Contains(key))
                return;
    
            _disablers.Add(key);
    
            if (!_stateManager.TryPeek(out StatePointer statePointer))
                return;
    
            statePointer?.Disable();
        }
    
        public virtual void Tick()
        {
            if (!Enabled)
                return;
    
            _stateManager.Tick();
        }
    
        public bool GetPointed(out List<Point> pointed)
        {
            return GetPointed(Pointables, out pointed);
        }
    
        public bool GetPointed(List<Pointable> pointables, out List<Point> pointed)
        {
            pointed = _pointed;
    
            if (!GetRay(out Ray ray))
                return false;
    
            // find pointables ray will hit
            _pointed.Clear();
            foreach (Pointable pointable in pointables)
            {
                if (!pointable.Enabled)
                    continue;
    
                if (!pointable.Collider.Raycast(ray, out RaycastHit raycastHit, pointable.Range))
                    continue;
    
                Vector3 position = raycastHit.point;
    
                if (!pointable.GetFlat())
                {
                    _pointed.Add(new (pointable, position));
                    continue;
                }
                    
                // flat, create plane to find pointed position
                Plane plane = new (pointable.Transform.forward, pointable.Transform.position);
                if (!plane.Raycast(ray, out float enter))
                    continue;
                
                position = ray.origin + (ray.direction * enter);
                _pointed.Add(new (pointable, position));
            }
    
            // if hit 1 or less, don't consider blocking, return
            if (_pointed.Count <= 1)
                return _pointed.Count > 0;
    
            // at least 2 pointables, consider blocking
            bool blocking = false;
            float min = float.MaxValue;
            for (int i = 0; i < _pointed.Count; i++)
            {
                if (!_pointed[i].Pointable.Blocking)
                    continue;
    
                blocking = true;
    
                Vector3 difference = _pointed[i].Position - ray.origin;
                min = Mathf.Min(min, difference.magnitude);
            }
            
            // if no pointables block, return
            if (!blocking)
                return true;
    
            // remove all blocked pointables
            for (int i = _pointed.Count - 1; i >= 0; i--)
            {
                Vector3 difference = _pointed[i].Position - ray.origin;
                
                // pointable is in front of, or is, the blocking pointable
                if (difference.magnitude <= min)
                    continue;
    
                _pointed.RemoveAt(i);
            }
    
            return true;
        }
    }
}