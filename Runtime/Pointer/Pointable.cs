using System;
using System.Collections.Generic;
using UnityEngine;

namespace VED.Utilities
{ 
    [RequireComponent(typeof(Collider))]
    public class Pointable : MonoBehaviour
    {
        protected const float RANGE = 10f;
    
        private Transform _transform = null;
        private bool      _enabled   = true;
        private Collider  _c         = null;
        
        [SerializeField] private Optional<Collider> _collider = new(false, null);
        [SerializeField] private bool               _blocking = false;
        
        [SerializeField, ReadOnly] public Dictionary<Pointer, Vector3> Pointers = new();
        [SerializeField, ReadOnly] public Dictionary<Pointer, Vector3> Pressers = new();
    
        public Transform Transform => _transform ??= transform;
        public Collider  Collider  => _c         ??= (_collider.Enabled && _collider.Value != null) 
                                                     ? _collider.Value
                                                     : GetCollider();
        public virtual float Range => RANGE;
    
        public bool Pointed  => Pointers.Count > 0;
        public bool Pressed  => Pressers.Count > 0;
        public bool Enabled  => _enabled;
        public bool Blocking => _blocking;
    
    
        public virtual void Init()
        {
            Pointers.Clear();
            Pressers.Clear();
        }
    
        public virtual void Tick() { }
    
        public virtual void Point(Pointer pointer, Vector3 position)
        {
            if (Pointers.ContainsKey(pointer))
            {
                Pointers[pointer] = position;
                return;
            }
    
            Pointers.Add(pointer, position);
        }
    
        public virtual void Unpoint(Pointer pointer)
        {
            if (!Pointers.ContainsKey(pointer))
                return;
    
            Pointers.Remove(pointer);
        }
    
        public virtual void Press(Pointer pointer, Vector3 position)
        {
            if (Pressers.ContainsKey(pointer))
            {
                Pressers[pointer] = position;
                return;
            }
    
            Pressers.Add(pointer, position);
        }
    
        public virtual void Unpress(Pointer pointer, bool cancel = false)
        {
            if (!Pressers.ContainsKey(pointer))
                return;
    
            Pressers.Remove(pointer);
        }
    
        public virtual void Enable()
        {
            _enabled = true;
        }
    
        public virtual void Disable()
        {
            _enabled = false;
        }
    
        public virtual void Drag  (Pointer pointer, Drag    drag  ) { }
        public virtual void Swipe (Pointer pointer, Swipe   swipe ) { }
        public virtual void Circle(Pointer pointer, Circle  circle) { }
        public virtual void Scroll(Pointer pointer, Vector2 scroll) { }
    
        public bool GetFlat()
        {
            // only box colliders can be flat
            if (Collider is not BoxCollider boxCollider)
                return false;
            
            // flatness is determined by 2 axis together being 100 times the size of another
            const float THRESHOLD = 100f;
    
            if (((boxCollider.size.x + boxCollider.size.y) / boxCollider.size.z) >= THRESHOLD
             || ((boxCollider.size.x + boxCollider.size.z) / boxCollider.size.y) >= THRESHOLD
             || ((boxCollider.size.y + boxCollider.size.z) / boxCollider.size.x) >= THRESHOLD)
                return true;
    
            return false;
        }
        
        private Collider GetCollider()
        {
            Type[] types = new Type[]
                {
                    typeof(BoxCollider    ),
                    typeof(SphereCollider ),
                    typeof(CapsuleCollider),
                    typeof(MeshCollider   )
            };
    
            foreach (Type type in types)
            {
                Component[] components = gameObject.GetComponents(type);
                
                if (components.Length <= 0)
                    continue;
    
                return components[0] as Collider;
            }
    
            return null;
        }
    }
}