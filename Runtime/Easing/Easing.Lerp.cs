using UnityEngine;

namespace VED.Utilities
{
    public sealed partial class Easing
    {
        public static float Lerp(float a, float b, float deltaTime, float decayConstant)
        {
            return b + (a - b) * Mathf.Exp(-decayConstant * deltaTime);
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, float deltaTime, float decayConstant)
        {
            return b + (a - b) * Mathf.Exp(-decayConstant * deltaTime);
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, float deltaTime, float decayConstant)
        {
            return b + (a - b) * Mathf.Exp(-decayConstant * deltaTime);
        }
    }
}