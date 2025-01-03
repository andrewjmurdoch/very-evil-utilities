using System;
using System.Collections.Generic;
using UnityEngine;
using VED.Utilities;

namespace VED
{
    [Serializable]
    public class Mapper<T, U>
    {
        [Serializable]
        public class Map
        {
            [SerializeField] public T Key;
            [SerializeField] public U Value;
    
            public Map(T key, U value)
            {
                Key = key;
                Value = value;
            }
        }
    
        public List<Map> Maps => _maps;
        [SerializeField] private List<Map> _maps = new List<Map>();
    
        private Dictionary<T, U> Dictionary => _dictionary ??= InitDictionary();
        private Dictionary<T, U> _dictionary = null;
    
        private Dictionary<T, U> InitDictionary()
        {
            _dictionary = new Dictionary<T, U>();
            Validate();
            return _dictionary;
        }
    
        public void Init(List<Map> maps)
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
            return new Optional<U>(Dictionary.TryGetValue(key, out U value), value);
        }
    
        public bool Get(T key, out U value)
        {
            return Dictionary.TryGetValue(key, out value);
        }
    
        public bool Set(T key, U value)
        {
            foreach (Map map in _maps)
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
            foreach (Map map in _maps)
            {
                if (map.Key == null) continue;
                Dictionary.TryAdd(map.Key, map.Value);
            }
        }
    
        public bool Add(T key, U value)
        {
            if (Dictionary.ContainsKey(key)) return false;
    
            _maps.Add(new Map(key, value));
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
    }
}