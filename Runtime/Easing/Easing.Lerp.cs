using UnityEngine;

namespace VED.Utilities
{
    public sealed partial class Easing
    {
        public static float Lerp(float a, float b, float deltaTime, float decayConstant)
        {
            return b + (a - b) * Mathf.Exp(-decayConstant * deltaTime);
        }
    }
}