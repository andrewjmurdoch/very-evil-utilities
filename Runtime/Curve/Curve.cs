using System;
using UnityEngine;

namespace VED.Utilities
{
    [Serializable]
    public class Curve
    {
        [SerializeField] public float Width  = 1f;
        [SerializeField] public float Height = 1f;
        [SerializeField] public AnimationCurve AnimationCurve = new AnimationCurve();

        public float this[float value]
        {
            get
            {
                value /= Width;

                if (value == float.NaN
                    || value == float.PositiveInfinity
                    || value == float.NegativeInfinity)
                    value = Width;

                return AnimationCurve.Evaluate(value) * Height;
            }
        }

        public Curve() { }

        public Curve(Curve curve)
        {
            Width  = curve.Width;
            Height = curve.Height;
            AnimationCurve = new(curve.AnimationCurve.keys);
        }

        public Curve(Keyframe[] keyframes)
        {
            AnimationCurve = new AnimationCurve(keyframes);
        }

        public float NormalizeLinear(float value, float resolution = 100f)
        {
            float nearest = 0f;
            float min = float.MaxValue;
            for (float i = 0f; i <= resolution; i++)
            {
                float evaluation = this[i * (1f / resolution) * Width];

                if (Mathf.Abs(evaluation - value) >= min) continue;

                min = Mathf.Abs(evaluation - value);
                nearest = i * (1f / resolution);
            }
            return nearest;
        }

        public float NormalizeBinary(float value, int resolution = 10)
        {
            bool inverted = this[0f] > this[Width];

            float mid = 0.5f;
            for (int i = 0; i < resolution; i++)
            {
                float evaluation = this[mid * Width];

                if (value > evaluation) mid += inverted ? -(mid / 2f) : (mid / 2f);
                else if (value < evaluation) mid -= inverted ? (mid / 2f) : -(mid / 2f);
            }
            return mid;
        }
    }
}