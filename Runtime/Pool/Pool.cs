using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Pool;

namespace VED.Utilities
{
    public class Pool<T> where T : MonoBehaviour
    {
        private T _original = null;
        private Stack<T> _stack = new Stack<T>();
        private Transform _transform = null;

        private Action<T> _onCreate = null;
        private Action<T> _onDestroy = null;
        private Action<T> _onPool = null;
        private Action<T> _onUnpool = null;

        private const int INITIAL_SIZE = 30;

        public Pool(Transform parent, T original, int size = INITIAL_SIZE, Action<T> onCreate = null, Action<T> onDestroy = null, Action<T> onPool = null, Action<T> onUnpool = null)
        {
            _original = original;
            
            // create transform to hold pooled objects
            _transform = new GameObject(typeof(T).ToString() + " Pool").transform;
            _transform.SetParent(parent);
            _transform.gameObject.SetActive(false);

            _onCreate  = onCreate;
            _onDestroy = onDestroy;
            _onPool    = onPool;
            _onUnpool  = onUnpool;

            Resize(size);
        }

        public void Destroy()
        {
            int count = _stack.Count;
            for (int i = 0; i < count; i++)
            {
                T instance = _stack.Pop();
                _onDestroy?.Invoke(instance);
                UnityEngine.Object.Destroy(instance);
            }

            _stack.Clear();
        }

        public T Pop()
        {
            Resize();
            T instance = _stack.Pop();
            instance.transform.SetParent(null);
            _onUnpool?.Invoke(instance);
            return instance;
        }

        public void Push(T instance)
        {
            if (_stack.Contains(instance)) return;

            _onPool?.Invoke(instance);
            instance.transform.SetParent(_transform);
            _stack.Push(instance);
        }

        public void Resize(int size = 1)
        {
            if (_stack.Count > 0) return;

            size = Mathf.Max(1, size);
            for (int i = 0; i < size; i++)
            {
                T instance = UnityEngine.Object.Instantiate(_original);
                _onCreate?.Invoke(instance);
                Push(instance);
            }
        }
    }
}