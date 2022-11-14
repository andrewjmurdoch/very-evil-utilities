using System;
using UnityEngine;

namespace VED.Utilities
{
    public abstract class Transition : MonoBehaviour
    {
        public const float DEFAULT_DURATION = 1f;

        public abstract void Stop();
        public abstract void In(Action callback = null, float duration = DEFAULT_DURATION);
        public abstract void Out(Action callback = null, float duration = DEFAULT_DURATION);
    }
}