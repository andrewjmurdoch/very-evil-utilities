using System;
using UnityEngine;

namespace VED.Utilities
{
    [Serializable]
    public class MeterSettings
    {
        [SerializeField] public int Minimum = 0;
        [SerializeField] public int Maximum = 0;
        [SerializeField] public int Initial = 0;
    }
}