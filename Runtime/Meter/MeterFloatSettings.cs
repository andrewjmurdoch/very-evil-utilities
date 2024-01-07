using System;
using UnityEngine;

namespace VED.Utilities
{
    [Serializable]
    public class MeterFloatSettings
    {
        [SerializeField] public float Minimum = 0;
        [SerializeField] public float Maximum = 0;
        [SerializeField] public float Initial = 0;
    }
}