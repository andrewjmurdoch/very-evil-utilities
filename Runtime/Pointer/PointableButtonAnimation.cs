using System;
using UnityEngine;
using UnityEngine.UI;

namespace VED.Utilities
{
    [Serializable]
    public abstract class PointableButtonAnimation<T, U>
    {
        public static readonly float LERP_DECAY_CONSTANT = 42f;

        [SerializeField] protected T _component;
        [SerializeField] protected U _unpointed;
        [SerializeField] protected U _pointed;
        [SerializeField] protected U _pressed;
        [SerializeField] protected U _disabled;

        public U Get(PointableButton.State state)
        {
            switch (state)
            {
                case PointableButton.State.UNPOINTED: return _unpointed;
                case PointableButton.State.POINTED  : return _pointed;
                case PointableButton.State.PRESSED  : return _pressed;
                case PointableButton.State.DISABLED : return _disabled;
                default                             : return _unpointed;
            }
        }

        public abstract void Tick(PointableButton.State state);
    }

    [Serializable]
    public class PointableButtonAnimationGraphicColor : PointableButtonAnimation<Graphic, Color>
    {
        public override void Tick(PointableButton.State state)
        {
            Color color = Easing.Lerp(_component.color, Get(state), Time.deltaTime, LERP_DECAY_CONSTANT);
            color.a = _component.color.a;
            _component.color = color;
        }
    }

    [Serializable]
    public class PointableButtonAnimationGraphicAlpha : PointableButtonAnimation<Graphic, float>
    {
        public override void Tick(PointableButton.State state)
        {
            Color color = _component.color;
            color.a = Easing.Lerp(color.a, Get(state), Time.deltaTime, LERP_DECAY_CONSTANT);
            _component.color = color;
        }
    }

    [Serializable]
    public class PointableButtonAnimationTransformScale : PointableButtonAnimation<Transform, float>
    {
        public override void Tick(PointableButton.State state)
        {
            _component.localScale = Easing.Lerp(_component.localScale, Vector3.one * Get(state), Time.deltaTime, LERP_DECAY_CONSTANT);
        }
    }

    [Serializable]
    public class PointableButtonAnimationTransformZPosition : PointableButtonAnimation<Transform, float>
    {
        public override void Tick(PointableButton.State state)
        {
            Vector3 localPosition = _component.localPosition;
            localPosition.z = Easing.Lerp(localPosition.z, Get(state), Time.deltaTime, LERP_DECAY_CONSTANT);
            _component.localPosition = localPosition;
        }
    }
}
