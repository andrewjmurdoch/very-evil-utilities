using System;
using System.Collections.Generic;
using UnityEngine;

namespace VED.Utilities
{
    public class Pointable : MonoBehaviour
    {
        protected const float RANGE = 20f;

        private Transform       _transform = null;
        private bool            _enabled   = true;
        private bool            _flat      = true;
        private List<Collider > _c         = null;
        
        [SerializeField] private Optional<List<Collider>> _colliders = new(false, null);
        [SerializeField] private bool                     _blocking  = false;
        
        [SerializeField, ReadOnly] public Dictionary<Pointer, Vector3> Pointers = new();
        [SerializeField, ReadOnly] public Dictionary<Pointer, Vector3> Pressers = new();
    
        public Transform      Transform => _transform ??= transform;
        public List<Collider> Colliders
        {
            get
            {
                if (_colliders.Enabled && _colliders.Value != null)
                    return _colliders.Value;

                return _c ??= GetColliders();
            }

            set
            {
                _c = value;
                _colliders.Enabled = false;
            }
        }
        public virtual float Range => RANGE;
    
        public bool Pointed  => Pointers.Count > 0;
        public bool Pressed  => Pressers.Count > 0;
        public bool Enabled  => _enabled;
        public bool Blocking => _blocking;
        public bool Flat     => _flat;
    
        public virtual void Init()
        {
            Pointers.Clear();
            Pressers.Clear();

            _flat = GetFlat();
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
    
        private bool GetFlat()
        {
            // only box colliders can be flat
            foreach (Collider collider in Colliders)
                if (collider is not BoxCollider boxCollider)
                    return false;

            List<Vector3> positions = new();

            foreach (Collider collider in Colliders)
            {
                BoxCollider boxCollider = collider as BoxCollider;

                positions.Add(Transform.worldToLocalMatrix * (boxCollider.transform.position + (boxCollider.center + boxCollider.transform.rotation * new Vector3(-boxCollider.size.x * boxCollider.transform.lossyScale.x, -boxCollider.size.y * boxCollider.transform.lossyScale.y, -boxCollider.size.z * boxCollider.transform.lossyScale.z) * 0.5f)));
                positions.Add(Transform.worldToLocalMatrix * (boxCollider.transform.position + (boxCollider.center + boxCollider.transform.rotation * new Vector3( boxCollider.size.x * boxCollider.transform.lossyScale.x, -boxCollider.size.y * boxCollider.transform.lossyScale.y, -boxCollider.size.z * boxCollider.transform.lossyScale.z) * 0.5f)));
                positions.Add(Transform.worldToLocalMatrix * (boxCollider.transform.position + (boxCollider.center + boxCollider.transform.rotation * new Vector3( boxCollider.size.x * boxCollider.transform.lossyScale.x, -boxCollider.size.y * boxCollider.transform.lossyScale.y,  boxCollider.size.z * boxCollider.transform.lossyScale.z) * 0.5f)));
                positions.Add(Transform.worldToLocalMatrix * (boxCollider.transform.position + (boxCollider.center + boxCollider.transform.rotation * new Vector3(-boxCollider.size.x * boxCollider.transform.lossyScale.x, -boxCollider.size.y * boxCollider.transform.lossyScale.y,  boxCollider.size.z * boxCollider.transform.lossyScale.z) * 0.5f)));
                positions.Add(Transform.worldToLocalMatrix * (boxCollider.transform.position + (boxCollider.center + boxCollider.transform.rotation * new Vector3(-boxCollider.size.x * boxCollider.transform.lossyScale.x,  boxCollider.size.y * boxCollider.transform.lossyScale.y, -boxCollider.size.z * boxCollider.transform.lossyScale.z) * 0.5f)));
                positions.Add(Transform.worldToLocalMatrix * (boxCollider.transform.position + (boxCollider.center + boxCollider.transform.rotation * new Vector3( boxCollider.size.x * boxCollider.transform.lossyScale.x,  boxCollider.size.y * boxCollider.transform.lossyScale.y, -boxCollider.size.z * boxCollider.transform.lossyScale.z) * 0.5f)));
                positions.Add(Transform.worldToLocalMatrix * (boxCollider.transform.position + (boxCollider.center + boxCollider.transform.rotation * new Vector3( boxCollider.size.x * boxCollider.transform.lossyScale.x,  boxCollider.size.y * boxCollider.transform.lossyScale.y,  boxCollider.size.z * boxCollider.transform.lossyScale.z) * 0.5f)));
                positions.Add(Transform.worldToLocalMatrix * (boxCollider.transform.position + (boxCollider.center + boxCollider.transform.rotation * new Vector3(-boxCollider.size.x * boxCollider.transform.lossyScale.x,  boxCollider.size.y * boxCollider.transform.lossyScale.y,  boxCollider.size.z * boxCollider.transform.lossyScale.z) * 0.5f)));
            }

            if (positions.Count <= 0)
                return false;

            Bounds bounds = GeometryUtility.CalculateBounds(positions.ToArray(), Matrix4x4.identity);
            Matrix4x4 matrixScale = Matrix4x4.Scale(Transform.lossyScale);
            Vector3 size = matrixScale * bounds.size;

            // flatness is determined by the x and y axis of bounding box together being THRESHOLD_SIZE times the size of the z axis
            const float THRESHOLD_SIZE = 27.5f;

            return (size.x + size.y) / Mathf.Max(size.z, float.Epsilon) >= THRESHOLD_SIZE;
        }
        
        private List<Collider> GetColliders()
        {
            Type[] types = new Type[]
                {
                    typeof(BoxCollider    ),
                    typeof(SphereCollider ),
                    typeof(CapsuleCollider),
                    typeof(MeshCollider   )
            };

            List<Collider> colliders = new();
    
            foreach (Type type in types)
                foreach (Component component in gameObject.GetComponents(type))
                    colliders.Add(component as Collider);

            return colliders;;
        }
    }
}