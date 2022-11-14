using System;
using UnityEngine;

namespace VED.Utilities
{
    [Serializable]
    public struct Optional<T>
    {
        [SerializeField] public bool Enabled;
        [SerializeField] public T Value;

        public Optional(T value, bool enabled = false)
        {
            Enabled = enabled;
            Value = value;
        }
    }
}
