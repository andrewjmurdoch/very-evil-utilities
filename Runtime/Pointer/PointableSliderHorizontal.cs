using Gooey;
using System;
using UnityEngine;

namespace VED.Utilities
{
    public class PointableSliderHorizontal : PointableButton
    {
        [SerializeField] private Goo    _gooSlideTrack  = null;
        [SerializeField] private Goo    _gooSlideHandle = null;
        [SerializeField] private Goo    _gooSlideLevel  = null;
        [SerializeField] private float  _value          = 0f;
        [SerializeField] private float  _minimum        = 0f;
        [SerializeField] private float  _maximum        = 1f;
    
        public Goo   GooSlideTrack => _gooSlideTrack;
        public float Value         => _value;
        public float Normal        => Mathf.InverseLerp(_minimum, _maximum, _value);
        
        public Action OnSlide = null;
    
        public void Init(float value, float minimum = 0f, float maximum = 1f)
        {
            _minimum = minimum;
            _maximum = maximum;
            _value = Mathf.Clamp(value, _minimum, _maximum);
    
            _gooSlideHandle.PositionAlignmentHorizontal = AlignmentHorizontal.LEFT;
            _gooSlideHandle.PositionHorizontal = new Value(Gooey.ValueType.PERCENTAGE, (float)Normal * 100f);

            if (_gooSlideLevel)
            {
                _gooSlideLevel.PositionAlignmentHorizontal = AlignmentHorizontal.LEFT;
                _gooSlideLevel.PivotHorizontal = -1f;
                _gooSlideLevel.SizeHorizontal = new Value(Gooey.ValueType.PERCENTAGE, (float)Normal * 100f);
            }

            base.Init();
        }
    
        public override void Press(Pointer pointer, Vector3 position)
        {
            base.Press(pointer, position);
            
            _value = Mathf.Lerp(_minimum, _maximum, GetNormal(position));
            _gooSlideHandle.PositionHorizontal.Float = (float)Normal * 100f;
            
            if (_gooSlideLevel)
                _gooSlideLevel.SizeHorizontal.Float = (float)Normal * 100f;
    
            OnSlide?.Invoke();
        }
    
        public override void Drag(Pointer pointer, Drag drag)
        {
            base.Drag(pointer, drag);
    
            _value = Mathf.Lerp(_minimum, _maximum, GetNormal(drag.To));
            _gooSlideHandle.PositionHorizontal.Float = (float)Normal * 100f;
    
            if (_gooSlideLevel)
                _gooSlideLevel.SizeHorizontal.Float = (float)Normal * 100f;
            
            OnSlide?.Invoke();
        }
    
        private float GetNormal(Vector3 position)
        {
            Vector3 differenceLeftToPosition = position - _gooSlideTrack.Left;
            differenceLeftToPosition = Vector3.ProjectOnPlane(differenceLeftToPosition, _gooSlideTrack.Forward);
            differenceLeftToPosition = Vector3.ProjectOnPlane(differenceLeftToPosition, _gooSlideTrack.Upward );
    
            Vector3 differenceLeftToRight    = _gooSlideTrack.Right - _gooSlideTrack.Left;
    
            if (Vector3.Angle(differenceLeftToRight, differenceLeftToPosition) > 90f)
                return 0f;
    
            float maximum = differenceLeftToRight.magnitude > 0 ? differenceLeftToRight.magnitude : 1f;
            return Mathf.Clamp(differenceLeftToPosition.magnitude / maximum, 0f, 1f);
        }
    
        public void SetNormal(float normal)
        {
            _value = Mathf.Lerp(_minimum, _maximum, normal);
            _gooSlideHandle.PositionHorizontal.Float = (float)Normal * 100f;
            
            if (_gooSlideLevel)
                _gooSlideLevel.SizeHorizontal.Float = (float)Normal * 100f;
        }
    
        public void SetValue(float value)
        {
            _value = Mathf.Clamp(value, _minimum, _maximum);
            _gooSlideHandle.PositionHorizontal.Float = (float)Normal * 100f;
            
            if (_gooSlideLevel)
                _gooSlideLevel.SizeHorizontal.Float = (float)Normal * 100f;
        }
    }
}