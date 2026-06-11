using System;
using UnityEngine;
using UnityEngine.UI;

namespace VED.Utilities
{
    [Serializable]
    public abstract class PointableButtonAnimation<T, U>
    {
        public static readonly float LERP_DECAY_CONSTANT = 42f;

        [SerializeField] public T Component;
        [SerializeField] public U Unpointed;
        [SerializeField] public U Pointed;
        [SerializeField] public U Pressed;
        [SerializeField] public U Disabled;

        public U Get(PointableButton.State state)
        {
            switch (state)
            {
                case PointableButton.State.UNPOINTED: return Unpointed;
                case PointableButton.State.POINTED  : return Pointed;
                case PointableButton.State.PRESSED  : return Pressed;
                case PointableButton.State.DISABLED : return Disabled;
                            default : return Unpointed;
            }
        }

        public abstract void Tick(PointableButton.State state);
    }

    [Serializable]
    public class PointableButtonAnimationGraphicColor : PointableButtonAnimation<Graphic, Color>
    {
        public override void Tick(PointableButton.State state)
        {
            Color color = Easing.Lerp(Component.color, Get(state), Time.deltaTime, LERP_DECAY_CONSTANT);
            color.a = Component.color.a;
            Component.color = color;
        }
    }

    [Serializable]
    public class PointableButtonAnimationGraphicAlpha : PointableButtonAnimation<Graphic, float>
    {
        public override void Tick(PointableButton.State state)
        {
            Color color = Component.color;
            color.a = Easing.Lerp(color.a, Get(state), Time.deltaTime, LERP_DECAY_CONSTANT);
            Component.color = color;
        }
    }

    [Serializable]
    public class PointableButtonAnimationTransformScale : PointableButtonAnimation<Transform, float>
    {
        public override void Tick(PointableButton.State state)
        {
            Component.localScale = Easing.Lerp(Component.localScale, Vector3.one * Get(state), Time.deltaTime, LERP_DECAY_CONSTANT);
        }
    }

    [Serializable]
    public class PointableButtonAnimationTransformZPosition : PointableButtonAnimation<Transform, float>
    {
        public override void Tick(PointableButton.State state)
        {
            Vector3 localPosition = Component.localPosition;
            localPosition.z = Easing.Lerp(localPosition.z, Get(state), Time.deltaTime, LERP_DECAY_CONSTANT);
            Component.localPosition = localPosition;
        }
    }
}
