using Gooey;
using UnityEngine;

namespace VED.Utilities
{
    public class PointableScrollVertical : Pointable
    {
        protected enum Behaviour
        {
            SPRING,
            CLAMP
        }

        private const float LIMIT_PERCENT   = 5f;
    
        [Space(10)]
        [SerializeField] private Goo _gooScrollable  = null;
    
        [Space(10)]
        [SerializeField] private Behaviour _behaviour = Behaviour.SPRING;
        [SerializeField] private Curve     _curveSpring = new();
    
        [Space(10)]
        [SerializeField] private float _friction       = 010.000f;
        [SerializeField] private float _velocityMax    = 020.000f;
        [SerializeField] private float _springStrength = 010.000f;
        [SerializeField] private float _scrollStrength = 010.000f;
    
        private float _position = 000.000f;
        private float _velocity = 000.000f;
        private float _normal   = 000.000f;
        private float _total    = 000.000f;
        private float _limit    = 000.000f;
        private float _scaler   = 001.000f;
    
        public Goo   GooScrollable => _gooScrollable;
        public float Total         => _total;
        public float Normal        => _normal;
    
        private void OnValidate()
        {
            if (!_gooScrollable)
           {
                GameObject gameObjectGooScrollable = new GameObject("Scrollable");
                gameObjectGooScrollable.transform.SetParent(transform);

                _gooScrollable = gameObjectGooScrollable.AddComponent<Goo>();
            }

            _gooScrollable.PositionAlignmentVertical = AlignmentVertical.TOP;
            _gooScrollable.PositionVertical.Type     = ValueType.VALUE;
            _gooScrollable.PivotVertical             = 1f;
        }
    
        public override void Tick()
        {
            if (!TickReferences()) return;
            if (!TickTotal     ()) return;
            if (!TickFriction  ()) return;
            if (!TickBounds    ()) return;
            if (!TickVelocity  ()) return;
            if (!TickPosition  ()) return;
            if (!TickNormal    ()) return;
            if (!TickScroll    ()) return;
        }
    
        private bool TickReferences()
        {
            if (!_gooScrollable)
                return false;

            _scaler = _gooScrollable.RectTransform.lossyScale.y;
            _scaler = _scaler <= 0f ? 1f : _scaler;

            return true;
        }
    
        private bool TickTotal()
        {
            if (!_gooScrollable.GetReferenceSizeVertical(out Goo gooScrollableReference))
                return false;
    
            _total = Mathf.Max(0f, (_gooScrollable.RectTransform.rect.height) - gooScrollableReference.RectTransform.rect.height);
            _limit = (gooScrollableReference.RectTransform.rect.height / 100f) * LIMIT_PERCENT;
            return true;
        }
    
        private bool TickFriction()
        {
            if (Mathf.Abs(_velocity) <= 0)
                return true;

            _velocity -= Mathf.Sign(_velocity) * Mathf.Min((_friction / _scaler) * Time.deltaTime, Mathf.Abs(_velocity));
            return true;
        }
    
        private bool TickBounds()
        {
            if (_position >= 0f && _position <= _total)
                return true;
    
            if (_position < 0f)
            {
                TickBoundsMin();
                return true;
            }
    
            if (_position > _total)
            {
                TickBoundsMax();
                return true;
            }
    
            return true;
        }
    
        private void TickBoundsMin()
        {
            float difference = Mathf.Abs(_position);
            float normal = _curveSpring[Mathf.InverseLerp(0f, _limit, difference)];
    
            _velocity = Mathf.Lerp(_velocity, 0f, normal);

            _position = Easing.Lerp(_position, 0f, _springStrength);
        }
    
        private void TickBoundsMax()
        {
            float difference = Mathf.Abs(_position - _total);
            float normal = _curveSpring[Mathf.InverseLerp(0f, _limit, difference)];

            _velocity = Mathf.Lerp(_velocity, 0f, normal);

            _position = Easing.Lerp(_position, _total, _springStrength);
        }
    
        private bool TickVelocity()
        {
            // apply velocity to position
            _position += _velocity * Time.deltaTime;
    
            // reset velocity if exceeding limit
            float min = 0f;
            float max = _total;
    
            if (_behaviour == Behaviour.SPRING)
            {
                min = -_limit;
                max = _total + _limit;
            }
    
            if (_velocity > 0f && _position >= max)
                _velocity = 0f;
    
            if (_velocity < 0f && _position <= min)
                _velocity = 0f;
    
            return true;
        }
    
        private bool TickPosition()
        {
            _position = Mathf.Clamp(_position, -_limit, _total + _limit);
    
            if (_behaviour == Behaviour.CLAMP)
                _position = Mathf.Clamp(_position, 0f, _total);
    
            return true;
        }
    
        private bool TickNormal()
        {
            _normal = Mathf.InverseLerp(0f, _total, _position);
            return true;
        }

        private bool TickScroll()
        {
            // position scrollable
            _gooScrollable.PositionVertical.Float = -_position;
    
            return true;
        }
    
        public void ScrollPosition(float amount)
        {
            float normal = 1f;
            float sign = Mathf.Sign(amount);
    
            // prevent drag beneath limit
            if (_position < 0f && sign < 0f)
            {
                float difference = Mathf.Abs(_position);
                normal = 1f - Mathf.InverseLerp(0, _limit, difference);
            }
    
            // prevent drag above limit
            if (_position > _total && sign > 0f)
            {
                float difference = Mathf.Abs(_position - _total);
                normal = 1f - Mathf.InverseLerp(0, _limit, difference);
            }
            
            _position += (amount * normal);
            _position = Mathf.Clamp(_position, -_limit, _total + _limit);
    
            // being scrolled by position, so stop all velocity
            _velocity = 0f;
        }
    
        public void ScrollVelocity(float amount)
        {
            float sign = Mathf.Sign(amount);
            float max;

            if (_position < 0f)
            {
                float difference = Mathf.Abs(_position);
                float normal = _curveSpring[1f - Mathf.InverseLerp(0f, _limit, difference)];

                max = sign == Mathf.Sign(_velocity)
                ? ((_velocityMax * normal) / _scaler) - Mathf.Abs(_velocity)
                : ((_velocityMax * normal) / _scaler) + Mathf.Abs(_velocity);
    
                _velocity += sign * Mathf.Min(Mathf.Abs(amount * Time.deltaTime), max);
            }
    
            if (_position > _total)
            {
                float difference = Mathf.Abs(_position - _total);
                float normal = _curveSpring[1f - Mathf.InverseLerp(0f, _limit, difference)];

                max = sign == Mathf.Sign(_velocity)
                ? ((_velocityMax * normal) / _scaler) - Mathf.Abs(_velocity)
                : ((_velocityMax * normal) / _scaler) + Mathf.Abs(_velocity);
    
                _velocity += sign * Mathf.Min(Mathf.Abs(amount * Time.deltaTime), max);
            }

            max = sign == Mathf.Sign(_velocity)
            ? (_velocityMax / _scaler) - Mathf.Abs(_velocity)
            : (_velocityMax / _scaler) + Mathf.Abs(_velocity);
    
            _velocity += sign * Mathf.Min(Mathf.Abs(amount * Time.deltaTime), max);
        }
    
        public void ScrollNormal(float normal)
        {
            float position = Mathf.Clamp01(normal) * _total;
    
            ScrollPosition(position - _position);
        }
    
        public override void Press(Pointer pointer, Vector3 position)
        {
            base.Press(pointer, position);
    
            _velocity = 0f;
        }
    
        public override void Drag(Pointer pointer, Drag drag) 
        {
            Vector3 difference = (drag.To - drag.From) / _gooScrollable.RectTransform.lossyScale.y;
            Vector3 projected  = Vector3.Project(difference, _gooScrollable.Downward);
    
            bool  downward  = Vector3.Angle(projected.normalized, _gooScrollable.Downward) <= 90f;
            float multiplier = downward ? -1f : 1f;
    
            ScrollPosition(projected.magnitude * multiplier);
        }
    
        public override void Swipe(Pointer pointer, Swipe swipe)
        {
            Vector3 projected = Vector3.Project(swipe.Direction, _gooScrollable.Downward);
    
            bool downward = Vector3.Angle(projected.normalized, _gooScrollable.Downward) <= 90f;
            float multiplier = downward ? -1f : 1f;
    
            ScrollVelocity(multiplier * (swipe.Speed / _scaler));
        }
    
        public override void Scroll(Pointer pointer, Vector2 scroll)
        {
            ScrollVelocity((scroll.y * _scrollStrength) / _scaler);
        }
    }
}