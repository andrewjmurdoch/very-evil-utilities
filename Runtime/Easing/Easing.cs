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
            return -b * (float)Math.Cos(t / 1 * (Math.PI / 2)) + b + a;
        }

        static float EaseOutSine(float a, float b, float t)
        {
            return b * (float)Math.Sin(t / 1 * (Math.PI / 2)) + a;
        }

        static float EaseInOutSine(float a, float b, float t)
        {
            return -b / 2 * ((float)Math.Cos(Math.PI * t / 1) - 1) + a;
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
            return b * (t /= 1) * t + a;
        }

        static float EaseOutQuad(float a, float b, float t)
        {
            return -b * (t /= 1) * (t - 2) + a;
        }

        static float EaseInOutQuad(float a, float b, float t)
        {
            if ((t /= 1 / 2) < 1)
            {
                return b / 2 * t * t + a;
            }

            return -b / 2 * ((--t) * (t - 2) - 1) + a;
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
            return b * (t /= 1) * t * t + a;
        }

        static float EaseOutCubic(float a, float b, float t)
        {
            return b * ((t = t / 1 - 1) * t * t + 1) + a;
        }

        static float EaseInOutCubic(float a, float b, float t)
        {
            if ((t /= 1 / 2) < 1)
            {
                return b / 2 * t * t * t + a;
            }
            return b / 2 * ((t -= 2) * t * t + 2) + a;
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
            return b * (t /= 1) * t * t * t + a;
        }

        static float EaseOutQuart(float a, float b, float t)
        {
            return -b * ((t = t / 1 - 1) * t * t * t - 1) + a;
        }

        static float EaseInOutQuart(float a, float b, float t)
        {
            if ((t /= 1 / 2) < 1)
            {
                return b / 2 * t * t * t * t + a;
            }
            return -b / 2 * ((t -= 2) * t * t * t - 2) + a;
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
            return b * (t /= 1) * t * t * t * t + a;
        }

        static float EaseOutQuint(float a, float b, float t)
        {
            return b * ((t = t / 1 - 1) * t * t * t * t + 1) + a;
        }

        static float EaseInOutQuint(float a, float b, float t)
        {
            if ((t /= 1 / 2) < 1)
            {
                return b / 2 * t * t * t * t * t + a;
            }
            return b / 2 * ((t -= 2) * t * t * t * t + 2) + a;
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
                return b * (float)Math.Pow(2, 10 * (t / 1 - 1)) + a;
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
                return b * (-(float)Math.Pow(2, -10 * t / 1) + 1) + a;
            }
        }

        static float EaseInOutExpo(float a, float b, float t)
        {
            if (t == 0)
            {
                return a;
            }
            else if (t == 1)
            {
                return a + b;
            }
            if ((t /= 1 / 2) < 1)
            {
                return b / 2 * (float)Math.Pow(2, 10 * (t - 1)) + a;
            }
            return b / 2 * (-(float)Math.Pow(2, -10 * --t) + 2) + a;
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
            return -b * ((float)Math.Sqrt(1 - (t /= 1) * t) - 1) + a;
        }

        static float EaseOutCirc(float a, float b, float t)
        {
            return b * (float)Math.Sqrt(1 - (t = t / 1 - 1) * t) + a;
        }

        static float EaseInOutCirc(float a, float b, float t)
        {
            if ((t /= 1 / 2) < 1)
            {
                return -b / 2 * ((float)Math.Sqrt(1 - t * t) - 1) + a;
            }
            return b / 2 * ((float)Math.Sqrt(1 - (t -= 2) * t) + 1) + a;
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
            return b * (t /= 1) * t * ((s + 1) * t - s) + a;
        }

        static float EaseOutBack(float a, float b, float t, float s = 1.70158f)
        {
            return b * ((t = t / 1 - 1) * t * ((s + 1) * t + s) + 1) + a;
        }

        static float EaseInOutBack(float a, float b, float t, float s = 1.70158f)
        {
            if ((t /= 1 / 2) < 1)
            {
                return b / 2 * (t * t * (((s *= (1.525f)) + 1) * t - s)) + a;
            }
            return b / 2 * ((t -= 2) * t * (((s *= (1.525f)) + 1) * t + s) + 2) + a;
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
            if (t == 0)
            {
                return a;
            }
            if ((t /= 1) == 1)
            {
                return a + b;
            }
            float p = 1 * .3f;
            float i = b;
            float s = p / 4;
            float postFix = i * (float)Math.Pow(2, 10 * (t -= 1));

            return -(postFix * (float)Math.Sin((t * 1 - s) * (2 * Math.PI) / p)) + a;
        }

        static float EaseOutElastic(float a, float b, float t)
        {
            if (t == 0)
            {
                return a;
            }
            if ((t /= 1) == 1)
            {
                return a + b;
            }
            float p = 1 * .3f;
            float i = b;
            float s = p / 4;
            return (i * (float)Math.Pow(2, -10 * t) * (float)Math.Sin((t * 1 - s) * (2 * (float)Math.PI) / p) + b + a);
        }

        static float EaseInOutElastic(float a, float b, float t)
        {
            if (t == 0)
            {
                return a;
            }
            if ((t /= 1 / 2) == 2)
            {
                return a + b;

            }

            float p = 1 * (.3f * 1.5f);
            float i = b;
            float s = p / 4;

            if (t < 1)
            {
                return -.5f * (i * (float)Math.Pow(2, 10 * (t -= 1)) * (float)Math.Sin((t * 1 - s) * (2 * (float)Math.PI) / p)) + a;
            }
            return i * (float)Math.Pow(2, -10 * (t -= 1)) * (float)Math.Sin((t * 1 - s) * (2 * (float)Math.PI) / p) * .5f + b + a;
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
            return EaseOutBounce(b, a, 1 - t);
        }

        static float EaseOutBounce(float a, float b, float t)
        {
            if ((t /= 1) < (1 / 2.75f))
            {
                return b * (7.5625f * t * t) + a;
            }
            else if (t < (2 / 2.75f))
            {
                return b * (7.5625f * (t -= (1.5f / 2.75f)) * t + .75f) + a;
            }
            else if (t < (2.5 / 2.75))
            {
                return b * (7.5625f * (t -= (2.25f / 2.75f)) * t + .9375f) + a;
            }
            else
            {
                return b * (7.5625f * (t -= (2.625f / 2.75f)) * t + .984375f) + a;
            }
        }

        static float EaseInOutBounce(float a, float b, float t)
        {
            if (t < 1 / 2)
            {
                return EaseInBounce(0, b, t * 2) * .5f + a;
            }
            else
            {
                return EaseOutBounce(0, b, t * 2 - 1) * .5f + b * .5f + a;
            }
        }
        #endregion
    }
}