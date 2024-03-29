using System;
using System.Collections.Generic;
using UnityEngine;

namespace VED.Utilities
{
    public class Pool<T> where T : MonoBehaviour
    {
        private T _original = null;
        private Transform _transform = null;

        public Stack<T> Inactive => _inactive;
        private Stack<T> _inactive = new Stack<T>();

        public List<T> Active => _active;
        private List<T> _active = new List<T>();

        public Action<T> Initialize = null;
        public Action<T> Deinitialize = null;

        private const int INITIAL_SIZE = 30;

        public Pool(T original, Transform parent = null, int size = INITIAL_SIZE, Action<T> initialize = null, Action<T> deinitialize = null)
        {
            Init(original, parent, size, initialize, deinitialize);
        }

        public Pool<T> Init(T original, Transform parent = null, int size = INITIAL_SIZE, Action<T> initialize = null, Action<T> deinitialize = null)
        {
            _original = original;

            // create transform to hold pooled objects
            _transform = new GameObject(typeof(T).ToString() + " Pool").transform;
            _transform.SetParent(parent);
            _transform.gameObject.SetActive(false);

            Initialize = initialize;
            Deinitialize = deinitialize;

            Extend(size);

            return this;
        }

        public void Deinit()
        {
            int count = _inactive.Count;
            for (int i = 0; i < count; i++)
            {
                T instance = _inactive.Pop();
                Deinitialize?.Invoke(instance);
                UnityEngine.Object.Destroy(instance);
            }

            _inactive.Clear();
            _active.Clear();
            UnityEngine.Object.Destroy(_transform.gameObject);
        }

        public T Pop()
        {
            T instance;

            if (_inactive.Count <= 0)
            {
                instance = UnityEngine.Object.Instantiate(_original);
                Initialize?.Invoke(instance);
                _active.Add(instance);
                return instance;
            }

            instance = _inactive.Pop();
            _active.Add(instance);
            instance.transform.SetParent(null);
            return instance;
        }

        public void Push(T instance)
        {
            if (_inactive.Contains(instance)) return;
            if (!_active.Contains(instance))
            {
                UnityEngine.Object.Destroy(instance.gameObject);
                return;
            }

            _active.Remove(instance);
            _inactive.Push(instance);
            instance.transform.SetParent(_transform);
        }

        public void Extend(int size)
        {
            for (int i = 0; i < size; i++)
            {
                T instance = UnityEngine.Object.Instantiate(_original);
                Initialize?.Invoke(instance);
                _inactive.Push(instance);
                instance.transform.SetParent(_transform);
            }
        }
    }
}