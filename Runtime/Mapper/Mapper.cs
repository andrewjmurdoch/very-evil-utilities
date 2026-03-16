using System;
using System.Collections.Generic;
using UnityEngine;

namespace VED.Utilities
{
    [Serializable]
    public class Map<T, U>
    {
        [SerializeField] public T Key;
        [SerializeField] public U Value;
    
        public Map(T key, U value)
        {
            Key = key;
            Value = value;
        }
    }

    [Serializable]
    public class Mapper<T, U>
    {
        public List<Map<T, U>> Maps => _maps;
        [SerializeField] private List<Map<T, U>> _maps = new List<Map<T, U>>();
    
        private Dictionary<T, U>  Dictionary => _dictionary ??= InitDictionary();
        private Dictionary<T, U> _dictionary = null;
    
        private Dictionary<T, U> InitDictionary()
        {
            _dictionary = new Dictionary<T, U>();
            Validate();
            return _dictionary;
        }
    
        public void Init(List<Map<T, U>> maps)
        {
            _maps = maps;
            Validate();
        }
    
        public U this[T key]
        {
            get
            {
                if (Get(key, out U value)) return value;
                return default(U);
            }
    
            set
            {
                Set(key, value);
            }
        }
    
        public Optional<U> Get(T key)
        {
            return new Optional<U>(Get(key, out U value), value);
        }
    
        public bool Get(T key, out U value)
        {
            if (Dictionary.TryGetValue(key, out value))
                return true;

            Validate();
            return Dictionary.TryGetValue(key, out value);
        }
    
        public bool Set(T key, U value)
        {
            foreach (Map<T, U> map in _maps)
            {
                if (!map.Key.Equals(key)) continue;
                map.Value = value;
                Validate();
                return true;
            }
    
            return false;
        }
    
        public void Validate()
        {
            // remove duplicates
            for (int i = _maps.Count - 1; i >= 0; i--)
            {
                if (_maps[i].Key == null) continue;
    
                for (int j = i - 1; j >= 0; j--)
                {
                    if (_maps[j].Key == null) continue;
                    if (!_maps[i].Key.Equals(_maps[j].Key)) continue;
                    if (i == _maps.Count - 1 && j == i - 1) continue;
    
                    _maps.RemoveAt(i);
                    break;
                }
            }
    
            Dictionary.Clear();
            foreach (Map<T, U> map in _maps)
            {
                if (map.Key == null) continue;
                Dictionary.TryAdd(map.Key, map.Value);
            }
        }
    
        public bool Add(T key, U value)
        {
            if (Dictionary.ContainsKey(key)) return false;
    
            _maps.Add(new Map<T, U>(key, value));
            Validate();
    
            return true;
        }
    
        public bool Remove(T key)
        {
            if (!Dictionary.ContainsKey(key)) return false;
    
            for (int i = _maps.Count - 1; i >= 0; i--)
            {
                if (!_maps[i].Key.Equals(key)) continue;
    
                _maps.RemoveAt(i);
                Validate();
                return true;
            }
    
            return false;
        }
    
        public bool Contains(T key)
        {
            return Dictionary.ContainsKey(key);
        }
        public void Clear()
        {
            _maps.Clear();

            if (_dictionary != null)
                _dictionary.Clear();
            _dictionary = null;
        }
    }
}