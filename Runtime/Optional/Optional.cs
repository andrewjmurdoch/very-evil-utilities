using System;
using UnityEngine;

namespace VED.Utilities
{
    [Serializable]
    public struct Optional<T>
    {
        [SerializeField] public bool Enabled;
        [SerializeField] public T Value;

        public Optional(bool enabled, T value)
        {
            Enabled = enabled;
            Value = value;
        }
    }
}
