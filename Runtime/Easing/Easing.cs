using System;
using UnityEngine;

namespace VED.Utilities
{
    public sealed class Easing
    {
        public enum EaseType
        {
            NONE,
            IN,
            OUT,
            INOUT
        }

        public enum LerpType
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

        [Serializable]
        public class Type
        {
            public LerpType LerpType;
            public EaseType EaseType;

            public Type(LerpType lerpType, EaseType easingType)
            {
                LerpType = lerpType;
                EaseType = easingType;
            }
        }

        public static float Ease(Type type, float a, float b, float t)
        {
            return Ease(type.LerpType, type.EaseType, a, b, t);
        }

        public static Vector3 Ease(Type type, Vector3 a, Vector3 b, float t)
        {
            return Ease(type.LerpType, type.EaseType, a, b, t);
        }

        public static float Ease(LerpType lerpType, EaseType easingType, float a, float b, float t)
        {
            b -= a;
            return lerpType switch 
            {
                LerpType.SINE    => EaseSine   (easingType, a, b, t),
                LerpType.QUAD    => EaseQuad   (easingType, a, b, t),
                LerpType.CUBIC   => EaseCubic  (easingType, a, b, t),
                LerpType.QUINT   => EaseQuint  (easingType, a, b, t),
                LerpType.EXPO    => EaseExpo   (easingType, a, b, t),
                LerpType.CIRC    => EaseCirc   (easingType, a, b, t),
                LerpType.BACK    => EaseBack   (easingType, a, b, t),
                LerpType.ELASTIC => EaseElastic(easingType, a, b, t),
                LerpType.BOUNCE  => EaseBounce (easingType, a, b, t),
                LerpType.LERP    => Mathf.Lerp(a, (b + a), t),
                _ => Mathf.Lerp(a, (b + a), t)
            };
        }

        public static Vector3 Ease(LerpType lerpType, EaseType easingType, Vector3 a, Vector3 b, float t)
        {
            float x = (a.x == b.x) ? b.x : Ease(lerpType, easingType, a.x, b.x, t);
            float y = (a.y == b.y) ? b.y : Ease(lerpType, easingType, a.y, b.y, t);
            float z = (a.z == b.z) ? b.z : Ease(lerpType, easingType, a.z, b.z, t);

            return new Vector3(x, y, z);
        }

        #region SINE
        static float EaseSine(EaseType easingType, float a, float b, float t, float lerpTotal = 1.0f)
        {
            switch (easingType)
            {
                case EaseType.IN: return EaseInSine(a, b, t, lerpTotal);
                case EaseType.OUT: return EaseOutSine(a, b, t, lerpTotal);
                case EaseType.INOUT: return EaseInOutSine(a, b, t, lerpTotal);
                default: return EaseInSine(a, b, t, lerpTotal);
            }
        }

        static float EaseInSine(float a, float b, float t, float lerpTotal = 1.0f)
        {
            return -b * (float)Math.Cos(t / lerpTotal * (Math.PI / 2)) + b + a;
        }

        static float EaseOutSine(float a, float b, float t, float lerpTotal = 1.0f)
        {
            return b * (float)Math.Sin(t / lerpTotal * (Math.PI / 2)) + a;
        }

        static float EaseInOutSine(float a, float b, float t, float lerpTotal = 1.0f)
        {
            return -b / 2 * ((float)Math.Cos(Math.PI * t / lerpTotal) - 1) + a;
        }
        #endregion

        #region QUAD
        static float EaseQuad(EaseType easingType, float a, float b, float t, float lerpTotal = 1.0f)
        {
            switch (easingType)
            {
                case EaseType.IN: return EaseInQuad(a, b, t, lerpTotal);
                case EaseType.OUT: return EaseOutQuad(a, b, t, lerpTotal);
                case EaseType.INOUT: return EaseInOutQuad(a, b, t, lerpTotal);
                default: return EaseInQuad(a, b, t, lerpTotal);
            }
        }

        static float EaseInQuad(float a, float b, float t, float lerpTotal = 1.0f)
        {
            return b * (t /= lerpTotal) * t + a;
        }

        static float EaseOutQuad(float a, float b, float t, float lerpTotal = 1.0f)
        {
            return -b * (t /= lerpTotal) * (t - 2) + a;
        }

        static float EaseInOutQuad(float a, float b, float t, float lerpTotal = 1.0f)
        {
            if ((t /= lerpTotal / 2) < 1)
            {
                return b / 2 * t * t + a;
            }

            return -b / 2 * ((--t) * (t - 2) - 1) + a;
        }
        #endregion

        #region CUBIC
        static float EaseCubic(EaseType easingType, float a, float b, float t, float lerpTotal = 1.0f)
        {
            switch (easingType)
            {
                case EaseType.IN: return EaseInCubic(a, b, t, lerpTotal);
                case EaseType.OUT: return EaseOutCubic(a, b, t, lerpTotal);
                case EaseType.INOUT: return EaseInOutCubic(a, b, t, lerpTotal);
                default: return EaseInCubic(a, b, t, lerpTotal);
            }
        }

        static float EaseInCubic(float a, float b, float t, float lerpTotal = 1.0f)
        {
            return b * (t /= lerpTotal) * t * t + a;
        }

        static float EaseOutCubic(float a, float b, float t, float lerpTotal = 1.0f)
        {
            return b * ((t = t / lerpTotal - 1) * t * t + 1) + a;
        }

        static float EaseInOutCubic(float a, float b, float t, float lerpTotal = 1.0f)
        {
            if ((t /= lerpTotal / 2) < 1)
            {
                return b / 2 * t * t * t + a;
            }
            return b / 2 * ((t -= 2) * t * t + 2) + a;
        }
        #endregion

        #region Quart 
        static float EaseQuart(EaseType easingType, float a, float b, float t, float lerpTotal = 1.0f)
        {
            switch (easingType)
            {
                case EaseType.IN: return EaseInQuart(a, b, t, lerpTotal);
                case EaseType.OUT: return EaseOutQuart(a, b, t, lerpTotal);
                case EaseType.INOUT: return EaseInOutQuart(a, b, t, lerpTotal);
                default: return EaseInQuart(a, b, t, lerpTotal);
            }
        }

        static float EaseInQuart(float a, float b, float t, float lerpTotal = 1.0f)
        {
            return b * (t /= lerpTotal) * t * t * t + a;
        }

        static float EaseOutQuart(float a, float b, float t, float lerpTotal = 1.0f)
        {
            return -b * ((t = t / lerpTotal - 1) * t * t * t - 1) + a;
        }

        static float EaseInOutQuart(float a, float b, float t, float lerpTotal = 1.0f)
        {
            if ((t /= lerpTotal / 2) < 1)
            {
                return b / 2 * t * t * t * t + a;
            }
            return -b / 2 * ((t -= 2) * t * t * t - 2) + a;
        }
        #endregion

        #region QUINT 
        static float EaseQuint(EaseType easingType, float a, float b, float t, float lerpTotal = 1.0f)
        {
            switch (easingType)
            {
                case EaseType.IN: return EaseInQuint(a, b, t, lerpTotal);
                case EaseType.OUT: return EaseOutQuint(a, b, t, lerpTotal);
                case EaseType.INOUT: return EaseInOutQuint(a, b, t, lerpTotal);
                default: return EaseInQuint(a, b, t, lerpTotal);
            }
        }

        static float EaseInQuint(float a, float b, float t, float lerpTotal = 1.0f)
        {
            return b * (t /= lerpTotal) * t * t * t * t + a;
        }

        static float EaseOutQuint(float a, float b, float t, float lerpTotal = 1.0f)
        {
            return b * ((t = t / lerpTotal - 1) * t * t * t * t + 1) + a;
        }

        static float EaseInOutQuint(float a, float b, float t, float lerpTotal = 1.0f)
        {
            if ((t /= lerpTotal / 2) < 1)
            {
                return b / 2 * t * t * t * t * t + a;
            }
            return b / 2 * ((t -= 2) * t * t * t * t + 2) + a;
        }
        #endregion

        #region EXPO  
        static float EaseExpo(EaseType easingType, float a, float b, float t, float lerpTotal = 1.0f)
        {
            switch (easingType)
            {
                case EaseType.IN: return EaseInExpo(a, b, t, lerpTotal);
                case EaseType.OUT: return EaseOutExpo(a, b, t, lerpTotal);
                case EaseType.INOUT: return EaseInOutExpo(a, b, t, lerpTotal);
                default: return EaseInExpo(a, b, t, lerpTotal);
            }
        }

        static float EaseInExpo(float a, float b, float t, float lerpTotal = 1.0f)
        {
            if (t == 0)
            {
                return a;
            }
            else
            {
                return b * (float)Math.Pow(2, 10 * (t / lerpTotal - 1)) + a;
            }
        }

        static float EaseOutExpo(float a, float b, float t, float lerpTotal = 1.0f)
        {
            if (t == lerpTotal)
            {
                return a + b;
            }
            else
            {
                return b * (-(float)Math.Pow(2, -10 * t / lerpTotal) + 1) + a;
            }
        }

        static float EaseInOutExpo(float a, float b, float t, float lerpTotal = 1.0f)
        {
            if (t == 0)
            {
                return a;
            }
            else if (t == lerpTotal)
            {
                return a + b;
            }
            if ((t /= lerpTotal / 2) < 1)
            {
                return b / 2 * (float)Math.Pow(2, 10 * (t - 1)) + a;
            }
            return b / 2 * (-(float)Math.Pow(2, -10 * --t) + 2) + a;
        }
        #endregion

        #region CIRC 
        static float EaseCirc(EaseType easingType, float a, float b, float t, float lerpTotal = 1.0f)
        {
            switch (easingType)
            {
                case EaseType.IN: return EaseInCirc(a, b, t, lerpTotal);
                case EaseType.OUT: return EaseOutCirc(a, b, t, lerpTotal);
                case EaseType.INOUT: return EaseInOutCirc(a, b, t, lerpTotal);
                default: return EaseInCirc(a, b, t, lerpTotal);
            }
        }

        static float EaseInCirc(float a, float b, float t, float lerpTotal = 1.0f)
        {
            return -b * ((float)Math.Sqrt(1 - (t /= lerpTotal) * t) - 1) + a;
        }

        static float EaseOutCirc(float a, float b, float t, float lerpTotal = 1.0f)
        {
            return b * (float)Math.Sqrt(1 - (t = t / lerpTotal - 1) * t) + a;
        }

        static float EaseInOutCirc(float a, float b, float t, float lerpTotal = 1.0f)
        {
            if ((t /= lerpTotal / 2) < 1)
            {
                return -b / 2 * ((float)Math.Sqrt(1 - t * t) - 1) + a;
            }
            return b / 2 * ((float)Math.Sqrt(1 - (t -= 2) * t) + 1) + a;
        }
        #endregion

        #region BACK 
        static float EaseBack(EaseType easingType, float a, float b, float t, float lerpTotal = 1.0f)
        {
            switch (easingType)
            {
                case EaseType.IN: return EaseInBack(a, b, t, lerpTotal);
                case EaseType.OUT: return EaseOutBack(a, b, t, lerpTotal);
                case EaseType.INOUT: return EaseInOutBack(a, b, t, lerpTotal);
                default: return EaseInBack(a, b, t, lerpTotal);
            }
        }

        static float EaseInBack(float a, float b, float t, float lerpTotal = 1.0f, float s = 1.70158f)
        {
            return b * (t /= lerpTotal) * t * ((s + 1) * t - s) + a;
        }

        static float EaseOutBack(float a, float b, float t, float lerpTotal = 1.0f, float s = 1.70158f)
        {
            return b * ((t = t / lerpTotal - 1) * t * ((s + 1) * t + s) + 1) + a;
        }

        static float EaseInOutBack(float a, float b, float t, float lerpTotal = 1.0f, float s = 1.70158f)
        {
            if ((t /= lerpTotal / 2) < 1)
            {
                return b / 2 * (t * t * (((s *= (1.525f)) + 1) * t - s)) + a;
            }
            return b / 2 * ((t -= 2) * t * (((s *= (1.525f)) + 1) * t + s) + 2) + a;
        }
        #endregion

        #region ELASTIC 
        static float EaseElastic(EaseType easingType, float a, float b, float t, float lerpTotal = 1.0f)
        {
            switch (easingType)
            {
                case EaseType.IN: return EaseInElastic(a, b, t, lerpTotal);
                case EaseType.OUT: return EaseOutElastic(a, b, t, lerpTotal);
                case EaseType.INOUT: return EaseInOutElastic(a, b, t, lerpTotal);
                default: return EaseInElastic(a, b, t, lerpTotal);
            }
        }

        static float EaseInElastic(float a, float b, float t, float lerpTotal = 1.0f)
        {
            if (t == 0)
            {
                return a;
            }
            if ((t /= lerpTotal) == 1)
            {
                return a + b;
            }
            float p = lerpTotal * .3f;
            float i = b;
            float s = p / 4;
            float postFix = i * (float)Math.Pow(2, 10 * (t -= 1));

            return -(postFix * (float)Math.Sin((t * lerpTotal - s) * (2 * Math.PI) / p)) + a;
        }

        static float EaseOutElastic(float a, float b, float t, float lerpTotal = 1.0f)
        {
            if (t == 0)
            {
                return a;
            }
            if ((t /= lerpTotal) == 1)
            {
                return a + b;
            }
            float p = lerpTotal * .3f;
            float i = b;
            float s = p / 4;
            return (i * (float)Math.Pow(2, -10 * t) * (float)Math.Sin((t * lerpTotal - s) * (2 * (float)Math.PI) / p) + b + a);
        }

        static float EaseInOutElastic(float a, float b, float t, float lerpTotal = 1.0f)
        {
            if (t == 0)
            {
                return a;
            }
            if ((t /= lerpTotal / 2) == 2)
            {
                return a + b;

            }

            float p = lerpTotal * (.3f * 1.5f);
            float i = b;
            float s = p / 4;

            if (t < 1)
            {
                return -.5f * (i * (float)Math.Pow(2, 10 * (t -= 1)) * (float)Math.Sin((t * lerpTotal - s) * (2 * (float)Math.PI) / p)) + a;
            }
            return i * (float)Math.Pow(2, -10 * (t -= 1)) * (float)Math.Sin((t * lerpTotal - s) * (2 * (float)Math.PI) / p) * .5f + b + a;
        }
        #endregion

        #region BOUNCE  
        static float EaseBounce(EaseType easingType, float a, float b, float t, float lerpTotal = 1.0f)
        {
            switch (easingType)
            {
                case EaseType.IN: return EaseInBounce(a, b, t, lerpTotal);
                case EaseType.OUT: return EaseOutBounce(a, b, t, lerpTotal);
                case EaseType.INOUT: return EaseInOutBounce(a, b, t, lerpTotal);
                default: return EaseInBounce(a, b, t, lerpTotal);
            }
        }

        static float EaseInBounce(float a, float b, float t, float lerpTotal = 1.0f)
        {
            b += a;
            a -= b;
            return EaseOutBounce(b, a, lerpTotal - t);
        }

        static float EaseOutBounce(float a, float b, float t, float lerpTotal = 1.0f)
        {
            if ((t /= lerpTotal) < (1 / 2.75f))
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

        static float EaseInOutBounce(float a, float b, float t, float lerpTotal = 1.0f)
        {
            if (t < lerpTotal / 2)
            {
                return EaseInBounce(0, b, t * 2, lerpTotal) * .5f + a;
            }
            else
            {
                return EaseOutBounce(0, b, t * 2 - lerpTotal, lerpTotal) * .5f + b * .5f + a;
            }
        }
        #endregion
    }
}