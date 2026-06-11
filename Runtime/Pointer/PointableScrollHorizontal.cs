using Gooey;
using UnityEngine;

namespace VED.Utilities
{
    public class PointableScrollHorizontal : Pointable
    {
        protected enum Behaviour
        {
            SPRING,
            CLAMP
        }
    
        [Space(10)]
        [SerializeField] private Goo _gooScrollable  = null;
        [SerializeField] private Goo _gooScrollThumb = null;
        [SerializeField] private Goo _gooScrollTrack = null;
    
        [Space(10)]
        [SerializeField] private Behaviour _behaviour   = Behaviour.SPRING;
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
    
        private const float MIN_THUMB_SCALE = 5f;
        private const float LIMIT_PERCENT   = 5f;
    
        public void OnValidate()
        {
            if (!_gooScrollable)
                return;

            _gooScrollable.PositionAlignmentHorizontal = AlignmentHorizontal.LEFT;
            _gooScrollable.PositionHorizontal.Type     = ValueType.VALUE;
            _gooScrollable.PivotHorizontal             = -1f;
            
            if (!_gooScrollThumb)
                return;
    
            _gooScrollThumb.PositionAlignmentHorizontal = AlignmentHorizontal.CENTRE;
            _gooScrollThumb.PositionHorizontal.Type     = ValueType.PERCENTAGE;
            _gooScrollThumb.SizeHorizontal    .Type     = ValueType.PERCENTAGE;
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
            if (!TickScrollbar ()) return;
            if (!TickScroll    ()) return;
        }
    
        private bool TickReferences()
        {
            if (!_gooScrollable)
                return false;

            _scaler = _gooScrollable.RectTransform.lossyScale.x;
            _scaler = _scaler <= 0f ? 1f : _scaler;

            return true;
        }
    
        private bool TickTotal()
        {
            if (!_gooScrollable.GetReferenceSizeHorizontal(out Goo gooScrollableReference))
                return false;
    
            _total = Mathf.Max(0f, (_gooScrollable.RectTransform.rect.width) - gooScrollableReference.RectTransform.rect.width);
            _limit = (gooScrollableReference.RectTransform.rect.width / 100f) * LIMIT_PERCENT;
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
    
        private bool TickScrollbar()
        {
            if (!_gooScrollable.GetReferenceSizeHorizontal(out Goo gooScrollableReference))
                return true;
    
            if (   _gooScrollThumb == null
                || _gooScrollTrack == null)
                return true;
            
            // scale thumb in accordance to total scrollable area
            float thumbScale = (1f - (_total / gooScrollableReference.RectTransform.rect.width)) * 100f;
            _gooScrollThumb.SizeHorizontal.Float = Mathf.Max(thumbScale, MIN_THUMB_SCALE);
            _gooScrollThumb.Tick();
    
            // position thumb in accordance with normalized position / total
            float thumbPadding = ((_gooScrollThumb.Width / 2f) / _gooScrollTrack.Width) * 100f;
            _gooScrollThumb.PositionHorizontal.Float = Mathf.Lerp(-50f + thumbPadding, 50f - thumbPadding, _normal);
    
            return true;
        }
    
        private bool TickScroll()
        {
            // position scrollable
            _gooScrollable.PositionHorizontal.Float = -_position;
    
            return true;
        }
    
        private void ScrollPosition(float amount)
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
    
        private void ScrollVelocity(float amount)
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
    
            ScrollPosition(_position - position);
        }
    
        public override void Press(Pointer pointer, Vector3 position)
        {
            base.Press(pointer, position);
    
            _velocity = 0f;
        }
    
        public override void Drag(Pointer pointer, Drag drag) 
        {
            Vector3 difference = (drag.To - drag.From) / _scaler;
            Vector3 projected  = Vector3.Project(difference, _gooScrollable.Rightward);
    
            bool  rightward  = Vector3.Angle(projected.normalized, _gooScrollable.Rightward) <= 90f;
            float multiplier = rightward ? -1f : 1f;
    
            ScrollPosition(projected.magnitude * multiplier);
        }
    
        public override void Swipe(Pointer pointer, Swipe swipe)
        {
            Vector3 projected = Vector3.Project(swipe.Direction, _gooScrollable.Rightward);
    
            bool rightward = Vector3.Angle(projected.normalized, _gooScrollable.Rightward) <= 90f;
            float multiplier = rightward ? -1f : 1f;
    
            ScrollVelocity(multiplier * (swipe.Speed / _scaler));
        }
    
        public override void Scroll(Pointer pointer, Vector2 scroll)
        {
            ScrollVelocity((scroll.x * _scrollStrength) / _scaler);
        }
    }
}