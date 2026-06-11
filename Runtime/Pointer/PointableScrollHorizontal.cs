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
    
        [Space(10)]
        [SerializeField] private Goo _gooScrollThumb = null;
        [SerializeField] private Goo _gooScrollTrack = null;
    
        [Space(10)]
        [SerializeField] private Behaviour _behaviour = Behaviour.SPRING;
    
        [Space(10)]
        [SerializeField] private float _position     = 000.000f;
        [SerializeField] private float _velocity     = 000.000f;
        [SerializeField] private float _acceleration = 010.000f;
        [SerializeField] private float _deceleration = 000.700f;
        [SerializeField] private float _friction     = 000.650f;
        [SerializeField] private float _velocityMax  = 020.000f;
    
        [SerializeField, ReadOnly] private float _normal = 000.000f;
        [SerializeField, ReadOnly] private float _total  = 000.000f;
        [SerializeField, ReadOnly] private float _limit  = 000.000f;
    
        private const float MIN_THUMB_SCALE = 5f;
        private const float LIMIT_PERCENT   = 5f;
    
        public void OnValidate()
        {
            _gooScrollable.PositionAlignmentHorizontal = AlignmentHorizontal.LEFT;
            _gooScrollable.PositionHorizontal.Type     = ValueType.VALUE;
            
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
            return _gooScrollable != null;
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
    
            // don't apply friciton while returning from out of bounds
            if (_position < 0f || _position > _total)
                return true;
    
            _velocity -= Mathf.Sign(_velocity) * Mathf.Min(_friction * Time.deltaTime, Mathf.Abs(_velocity));
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
            float amount;
    
            float difference = Mathf.Abs(_position);
            float normal = Mathf.InverseLerp(0f, _limit, difference);
    
            // decel if moving away
            if (_velocity < 0f)
                _velocity += Mathf.Abs(_velocity) * normal * Time.deltaTime;
    
            // accel to return
            _velocity += normal * _acceleration * Time.deltaTime;
    
            // decel returning
            normal = 1f - normal;
            amount = Mathf.Abs(_velocity) * normal * _deceleration;
            _velocity -= Mathf.Min(amount * Time.deltaTime, Mathf.Abs(_velocity));
    
            // prevent overshoot
            _velocity = Mathf.Min(difference / Time.deltaTime, _velocity);
        }
    
        private void TickBoundsMax()
        {
            float amount;
    
            float difference = Mathf.Abs(_position - _total);
            float normal = Mathf.InverseLerp(0f, _limit, difference);
    
            // decel if moving away
            if (_velocity > 0f)
                _velocity -= Mathf.Abs(_velocity) * normal * Time.deltaTime;
    
            // accel to return
            _velocity -= normal * _acceleration * Time.deltaTime;
    
            // decel returning
            normal = 1f - normal;
            amount = Mathf.Abs(_velocity) * normal * _deceleration;
            _velocity += Mathf.Min(amount * Time.deltaTime, Mathf.Abs(_velocity));
    
            // prevent overshoot
            _velocity = Mathf.Max(-(difference / Time.deltaTime), _velocity);
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
    
            float min = sign == Mathf.Sign(_velocity)
                ? _velocityMax - Mathf.Abs(_velocity)
                : _velocityMax + Mathf.Abs(_velocity);
    
            _velocity += sign * Mathf.Min(Mathf.Abs(amount) * Time.deltaTime, min);
        }
    
        private void ScrollNormal(float normal)
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
            Vector3 difference = drag.To - drag.From;
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
    
            ScrollVelocity(multiplier * swipe.Speed);
        }
    
        public override void Scroll(Pointer pointer, Vector2 scroll)
        {
            const float SCROLL_VELOCITY = 10f;
            ScrollVelocity(scroll.x * SCROLL_VELOCITY);
        }
    }
}