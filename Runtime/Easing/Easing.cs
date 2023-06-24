using System;
using UnityEngine;

namespace VED.Utilities
{
    public sealed partial class Easing
    {
        public enum Extent
        {
            IN,
            OUT,
            INOUT
        }

        public enum Shape
        {
            LERP,
            SINE,
            QUAD,
            CUBIC,
            QUINT,
            EXPO,
            CIRC,
            BACK,
            ELASTIC,
            BOUNCE
        }

        public static float Ease(Shape shape, Extent extent, float a, float b, float t)
        {
            b -= a;
            return shape switch 
            {
                Shape.SINE    => EaseSine   (extent, a, b, t),
                Shape.QUAD    => EaseQuad   (extent, a, b, t),
                Shape.CUBIC   => EaseCubic  (extent, a, b, t),
                Shape.QUINT   => EaseQuint  (extent, a, b, t),
                Shape.EXPO    => EaseExpo   (extent, a, b, t),
                Shape.CIRC    => EaseCirc   (extent, a, b, t),
                Shape.BACK    => EaseBack   (extent, a, b, t),
                Shape.ELASTIC => EaseElastic(extent, a, b, t),
                Shape.BOUNCE  => EaseBounce (extent, a, b, t),
                Shape.LERP    => Mathf.Lerp(a, (b + a), t),
                _ => Mathf.Lerp(a, (b + a), t)
            };
        }

        public static Vector3 Ease(Shape shape, Extent extent, Vector3 a, Vector3 b, float t)
        {
            float x = (a.x == b.x) ? b.x : Ease(shape, extent, a.x, b.x, t);
            float y = (a.y == b.y) ? b.y : Ease(shape, extent, a.y, b.y, t);
            float z = (a.z == b.z) ? b.z : Ease(shape, extent, a.z, b.z, t);

            return new Vector3(x, y, z);
        }

        #region SINE
        static float EaseSine(Extent extent, float a, float b, float t)
        {
            switch (extent)
            {
                case Extent.IN: return EaseInSine(a, b, t);
                case Extent.OUT: return EaseOutSine(a, b, t);
                case Extent.INOUT: return EaseInOutSine(a, b, t);
                default: return EaseInSine(a, b, t);
            }
        }

        static float EaseInSine(float a, float b, float t)
        {
            return -b * (float)Math.Cos(t / 1f * (Math.PI / 2f)) + b + a;
        }

        static float EaseOutSine(float a, float b, float t)
        {
            return b * (float)Math.Sin(t / 1f * (Math.PI / 2f)) + a;
        }

        static float EaseInOutSine(float a, float b, float t)
        {
            return -b / 2f * ((float)Math.Cos(Math.PI * t / 1f) - 1f) + a;
        }
        #endregion

        #region QUAD
        static float EaseQuad(Extent extent, float a, float b, float t)
        {
            switch (extent)
            {
                case Extent.IN: return EaseInQuad(a, b, t);
                case Extent.OUT: return EaseOutQuad(a, b, t);
                case Extent.INOUT: return EaseInOutQuad(a, b, t);
                default: return EaseInQuad(a, b, t);
            }
        }

        static float EaseInQuad(float a, float b, float t)
        {
            return b * (t /= 1f) * t + a;
        }

        static float EaseOutQuad(float a, float b, float t)
        {
            return -b * (t /= 1f) * (t - 2f) + a;
        }

        static float EaseInOutQuad(float a, float b, float t)
        {
            if ((t /= 1f / 2f) < 1f)
            {
                return b / 2f * t * t + a;
            }

            return -b / 2f * ((--t) * (t - 2f) - 1f) + a;
        }
        #endregion

        #region CUBIC
        static float EaseCubic(Extent extent, float a, float b, float t)
        {
            switch (extent)
            {
                case Extent.IN: return EaseInCubic(a, b, t);
                case Extent.OUT: return EaseOutCubic(a, b, t);
                case Extent.INOUT: return EaseInOutCubic(a, b, t);
                default: return EaseInCubic(a, b, t);
            }
        }

        static float EaseInCubic(float a, float b, float t)
        {
            return b * (t /= 1f) * t * t + a;
        }

        static float EaseOutCubic(float a, float b, float t)
        {
            return b * ((t = t / 1f - 1f) * t * t + 1f) + a;
        }

        static float EaseInOutCubic(float a, float b, float t)
        {
            if ((t /= 1f / 2f) < 1f)
            {
                return b / 2f * t * t * t + a;
            }
            return b / 2f * ((t -= 2f) * t * t + 2f) + a;
        }
        #endregion

        #region Quart 
        static float EaseQuart(Extent extent, float a, float b, float t)
        {
            switch (extent)
            {
                case Extent.IN: return EaseInQuart(a, b, t);
                case Extent.OUT: return EaseOutQuart(a, b, t);
                case Extent.INOUT: return EaseInOutQuart(a, b, t);
                default: return EaseInQuart(a, b, t);
            }
        }

        static float EaseInQuart(float a, float b, float t)
        {
            return b * (t /= 1f) * t * t * t + a;
        }

        static float EaseOutQuart(float a, float b, float t)
        {
            return -b * ((t = t / 1f - 1f) * t * t * t - 1f) + a;
        }

        static float EaseInOutQuart(float a, float b, float t)
        {
            if ((t /= 1f / 2f) < 1f)
            {
                return b / 2f * t * t * t * t + a;
            }
            return -b / 2f * ((t -= 2f) * t * t * t - 2f) + a;
        }
        #endregion

        #region QUINT 
        static float EaseQuint(Extent extent, float a, float b, float t)
        {
            switch (extent)
            {
                case Extent.IN: return EaseInQuint(a, b, t);
                case Extent.OUT: return EaseOutQuint(a, b, t);
                case Extent.INOUT: return EaseInOutQuint(a, b, t);
                default: return EaseInQuint(a, b, t);
            }
        }

        static float EaseInQuint(float a, float b, float t)
        {
            return b * (t /= 1f) * t * t * t * t + a;
        }

        static float EaseOutQuint(float a, float b, float t)
        {
            return b * ((t = t / 1f - 1f) * t * t * t * t + 1f) + a;
        }

        static float EaseInOutQuint(float a, float b, float t)
        {
            if ((t /= 1f / 2f) < 1f)
            {
                return b / 2f * t * t * t * t * t + a;
            }
            return b / 2f*  ((t -= 2f) * t * t * t * t + 2f) + a;
        }
        #endregion

        #region EXPO  
        static float EaseExpo(Extent extent, float a, float b, float t)
        {
            switch (extent)
            {
                case Extent.IN: return EaseInExpo(a, b, t);
                case Extent.OUT: return EaseOutExpo(a, b, t);
                case Extent.INOUT: return EaseInOutExpo(a, b, t);
                default: return EaseInExpo(a, b, t);
            }
        }

        static float EaseInExpo(float a, float b, float t)
        {
            if (t == 0)
            {
                return a;
            }
            else
            {
                return b * (float)Math.Pow(2f, 10f * (t / 1f - 1f)) + a;
            }
        }

        static float EaseOutExpo(float a, float b, float t)
        {
            if (t == 1)
            {
                return a + b;
            }
            else
            {
                return b * (-(float)Math.Pow(2f, -10f * t / 1f) + 1f) + a;
            }
        }

        static float EaseInOutExpo(float a, float b, float t)
        {
            if (t == 0f)
            {
                return a;
            }
            else if (t == 1f)
            {
                return a + b;
            }
            if ((t /= 1f / 2f) < 1f)
            {
                return b / 2f * (float)Math.Pow(2f, 10f * (t - 1f)) + a;
            }
            return b / 2f * (-(float)Math.Pow(2f, -10f * --t) + 2f) + a;
        }
        #endregion

        #region CIRC 
        static float EaseCirc(Extent extent, float a, float b, float t)
        {
            switch (extent)
            {
                case Extent.IN: return EaseInCirc(a, b, t);
                case Extent.OUT: return EaseOutCirc(a, b, t);
                case Extent.INOUT: return EaseInOutCirc(a, b, t);
                default: return EaseInCirc(a, b, t);
            }
        }

        static float EaseInCirc(float a, float b, float t)
        {
            return -b * ((float)Math.Sqrt(1f - (t /= 1f) * t) - 1f) + a;
        }

        static float EaseOutCirc(float a, float b, float t)
        {
            return b * (float)Math.Sqrt(1f - (t = t / 1f - 1f) * t) + a;
        }

        static float EaseInOutCirc(float a, float b, float t)
        {
            if ((t /= 1f / 2f) < 1f)
            {
                return -b / 2f * ((float)Math.Sqrt(1f - t * t) - 1f) + a;
            }
            return b / 2f * ((float)Math.Sqrt(1f - (t -= 2f) * t) + 1f) + a;
        }
        #endregion

        #region BACK 
        static float EaseBack(Extent extent, float a, float b, float t)
        {
            switch (extent)
            {
                case Extent.IN: return EaseInBack(a, b, t);
                case Extent.OUT: return EaseOutBack(a, b, t);
                case Extent.INOUT: return EaseInOutBack(a, b, t);
                default: return EaseInBack(a, b, t);
            }
        }

        static float EaseInBack(float a, float b, float t, float s = 1.70158f)
        {
            return b * (t /= 1f) * t * ((s + 1f) * t - s) + a;
        }

        static float EaseOutBack(float a, float b, float t, float s = 1.70158f)
        {
            return b * ((t = t / 1f - 1f) * t * ((s + 1f) * t + s) + 1f) + a;
        }

        static float EaseInOutBack(float a, float b, float t, float s = 1.70158f)
        {
            if ((t /= 1f / 2f) < 1f)
            {
                return b / 2f * (t * t * (((s *= (1.525f)) + 1f) * t - s)) + a;
            }
            return b / 2f * ((t -= 2f) * t * (((s *= (1.525f)) + 1f) * t + s) + 2f) + a;
        }
        #endregion

        #region ELASTIC 
        static float EaseElastic(Extent extent, float a, float b, float t)
        {
            switch (extent)
            {
                case Extent.IN: return EaseInElastic(a, b, t);
                case Extent.OUT: return EaseOutElastic(a, b, t);
                case Extent.INOUT: return EaseInOutElastic(a, b, t);
                default: return EaseInElastic(a, b, t);
            }
        }

        static float EaseInElastic(float a, float b, float t)
        {
            if (t == 0f)
            {
                return a;
            }
            if ((t /= 1f) == 1f)
            {
                return a + b;
            }
            float p = 1f * 0.3f;
            float i = b;
            float s = p / 4f;
            float postFix = i * (float)Math.Pow(2f, 10f * (t -= 1f));

            return -(postFix * (float)Math.Sin((t * 1f - s) * (2f * Math.PI) / p)) + a;
        }

        static float EaseOutElastic(float a, float b, float t)
        {
            if (t == 0f)
            {
                return a;
            }
            if ((t /= 1f) == 1f)
            {
                return a + b;
            }
            float p = 1f * 0.3f;
            float i = b;
            float s = p / 4f;
            return (i * (float)Math.Pow(2f, -10f * t) * (float)Math.Sin((t * 1f - s) * (2f * (float)Math.PI) / p) + b + a);
        }

        static float EaseInOutElastic(float a, float b, float t)
        {
            if (t == 0f)
            {
                return a;
            }
            if ((t /= 1f / 2f) == 2f)
            {
                return a + b;

            }

            float p = 1f * (0.3f * 1.5f);
            float i = b;
            float s = p / 4f;

            if (t < 1)
            {
                return -.5f * (i * (float)Math.Pow(2f, 10f * (t -= 1f)) * (float)Math.Sin((t * 1f - s) * (2f * (float)Math.PI) / p)) + a;
            }
            return i * (float)Math.Pow(2f, -10f * (t -= 1f)) * (float)Math.Sin((t * 1f - s) * (2f * (float)Math.PI) / p) * 0.5f + b + a;
        }
        #endregion

        #region BOUNCE  
        static float EaseBounce(Extent extent, float a, float b, float t)
        {
            switch (extent)
            {
                case Extent.IN: return EaseInBounce(a, b, t);
                case Extent.OUT: return EaseOutBounce(a, b, t);
                case Extent.INOUT: return EaseInOutBounce(a, b, t);
                default: return EaseInBounce(a, b, t);
            }
        }

        static float EaseInBounce(float a, float b, float t)
        {
            b += a;
            a -= b;
            return EaseOutBounce(b, a, 1f - t);
        }

        static float EaseOutBounce(float a, float b, float t)
        {
            if ((t /= 1f) < (1f / 2.75f))
            {
                return b * (7.5625f * t * t) + a;
            }
            else if (t < (2f / 2.75f))
            {
                return b * (7.5625f * (t -= (1.5f / 2.75f)) * t + 0.75f) + a;
            }
            else if (t < (2.5 / 2.75))
            {
                return b * (7.5625f * (t -= (2.25f / 2.75f)) * t + 0.9375f) + a;
            }
            else
            {
                return b * (7.5625f * (t -= (2.625f / 2.75f)) * t + 0.984375f) + a;
            }
        }

        static float EaseInOutBounce(float a, float b, float t)
        {
            if (t < 1f / 2f)
            {
                return EaseInBounce(0f, b, t * 2f) * 0.5f + a;
            }
            else
            {
                return EaseOutBounce(0f, b, t * 2f - 1) * 0.5f + b * 0.5f + a;
            }
        }
        #endregion
    }
}