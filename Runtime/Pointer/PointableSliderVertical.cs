using Gooey;
using System;
using UnityEngine;

namespace VED.Utilities
{
    public class PointableSliderVertical : PointableButton
    {
        [SerializeField] private Goo    _gooSlideHandle = null;
        [SerializeField] private Goo    _gooSlideLevel  = null;

        [Space(10)]
        [SerializeField] private float  _value   = 0f;
        [SerializeField] private float  _minimum = 0f;
        [SerializeField] private float  _maximum = 1f;
    
        public float Value  => _value;
        public float Normal => Mathf.InverseLerp(_minimum, _maximum, _value);
        
        public Action OnSlide = null;

        public void OnValidate()
        {
            if (!_gooSlideHandle)
            {
                GameObject gameObjectGooSlideHandle = new GameObject("Slide Handle");
                gameObjectGooSlideHandle.transform.SetParent(transform);

                _gooSlideHandle = gameObjectGooSlideHandle.AddComponent<Goo>();
            }
            
            _gooSlideHandle.PositionAlignmentVertical = AlignmentVertical.BOTTOM;
            _gooSlideHandle.PositionVertical = new Value(Gooey.ValueType.PERCENTAGE, (float)Normal * 100f);

            if (_gooSlideLevel)
            {
                _gooSlideLevel.PositionAlignmentVertical = AlignmentVertical.BOTTOM;
                _gooSlideLevel.PivotVertical = -1f;
                _gooSlideLevel.SizeVertical = new Value(Gooey.ValueType.PERCENTAGE, (float)Normal * 100f);
            }
        }

        public override void Init()
        {
            Init(0f, 0f, 1f);
        }

        public void Init(float value, float minimum, float maximum)
        {
            _minimum = minimum;
            _maximum = maximum;
            _value = Mathf.Clamp(value, _minimum, _maximum);

            base.Init();
        }
    
        public override void Press(Pointer pointer, Vector3 position, bool repress = false) 
        {
            base.Press(pointer, position, repress);

            if (!_gooSlideHandle)
                return;
            
            _value = Mathf.Lerp(_minimum, _maximum, GetNormal(position));
            _gooSlideHandle.PositionVertical.Float = (float)Normal * 100f;
            
            if (_gooSlideLevel)
                _gooSlideLevel.SizeVertical.Float = (float)Normal * 100f;
    
            OnSlide?.Invoke();
        }
    
        public override void Drag(Pointer pointer, Drag drag)
        {
            if (!_gooSlideHandle)
                return;

            _value = Mathf.Lerp(_minimum, _maximum, GetNormal(drag.To));
            _gooSlideHandle.PositionVertical.Float = (float)Normal * 100f;
    
            if (_gooSlideLevel)
                _gooSlideLevel.SizeVertical.Float = (float)Normal * 100f;
            
            OnSlide?.Invoke();
        }
    
        private float GetNormal(Vector3 position)
        {
            if (   !_gooSlideHandle
                || !_gooSlideHandle.GetReferencePositionVertical(out Goo gooSlideHandeReference))
                return 0f;

            Vector3 difference = position - gooSlideHandeReference.Bottom;
            difference = Vector3.ProjectOnPlane(difference, gooSlideHandeReference.Forward  );
            difference = Vector3.ProjectOnPlane(difference, gooSlideHandeReference.Rightward);
    
            Vector3 total = gooSlideHandeReference.Top - gooSlideHandeReference.Bottom;
    
            if (Vector3.Angle(total, difference) > 90f)
                return 0f;
    
            float maximum = total.magnitude > 0 
                ? total.magnitude 
                : 1f;

            return Mathf.Clamp01(difference.magnitude / maximum);
        }
    
        public void SetNormal(float normal)
        {
            if (!_gooSlideHandle)
                return;

            _value = Mathf.Lerp(_minimum, _maximum, normal);
            _gooSlideHandle.PositionVertical.Float = (float)Normal * 100f;
            
            if (_gooSlideLevel)
                _gooSlideLevel.SizeVertical.Float = (float)Normal * 100f;
        }
    
        public void SetValue(float value)
        {
            if (!_gooSlideHandle)
                return;

            _value = Mathf.Clamp(value, _minimum, _maximum);
            _gooSlideHandle.PositionVertical.Float = (float)Normal * 100f;
            
            if (_gooSlideLevel)
                _gooSlideLevel.SizeVertical.Float = (float)Normal * 100f;
        }
    }
}