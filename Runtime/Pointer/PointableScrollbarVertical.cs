using Gooey;
using UnityEngine;

namespace VED.Utilities
{
    [RequireComponent(typeof(Goo))]
    public class PointableScrollbarVertical : PointableButton
    {
        private const float MIN_THUMB_SCALE = 5f;

        [SerializeField] private PointableScrollVertical _pointableScrollVertical = null;
        [SerializeField] private Goo                     _gooScrollThumb          = null;

        private Goo _goo = null;

        public void OnValidate()
        {
            if (!_gooScrollThumb)
            {
                GameObject gameObjectGooScrollThumb = new GameObject("Scroll Thumb");
                gameObjectGooScrollThumb.transform.SetParent(transform);

                _gooScrollThumb = gameObjectGooScrollThumb.AddComponent<Goo>();
            }

            _gooScrollThumb.PositionAlignmentVertical = AlignmentVertical.CENTRE;
            _gooScrollThumb.PositionVertical.Type     = ValueType.PERCENTAGE;
            _gooScrollThumb.SizeVertical    .Type     = ValueType.PERCENTAGE;
        }

        public override void Init()
        {
            _goo = GetComponent<Goo>();

            base.Init();
        }

        public override void Tick()
        {
            if (!TickSize    ()) return;
            if (!TickPosition()) return;
        }

        private bool TickSize()
        {
            if (   !_goo
                || !_gooScrollThumb)
                return false;

            if (   !_pointableScrollVertical.GooScrollable
                || !_pointableScrollVertical.GooScrollable.GetReferenceSizeVertical(out Goo gooScrollableReference))
                return false;
            
            // scale thumb in accordance to total scrollable area
            float thumbScale = (1f - (_pointableScrollVertical.Total / gooScrollableReference.RectTransform.rect.height)) * 100f;
            _gooScrollThumb.SizeVertical.Float = Mathf.Max(thumbScale, MIN_THUMB_SCALE);
            _gooScrollThumb.Tick();

            return true;
        }

        private bool TickPosition()
        {
            if (   !_goo
                || !_gooScrollThumb)
                return false;

            if (   !_pointableScrollVertical.GooScrollable
                || !_pointableScrollVertical.GooScrollable.GetReferenceSizeVertical(out Goo gooScrollableReference))
                return false;
    
            // position thumb in accordance with normalized position / total
            float thumbPadding = ((_gooScrollThumb.Height / 2f) / _goo.Height) * 100f;
            _gooScrollThumb.PositionVertical.Float = -Mathf.Lerp(-50f + thumbPadding, 50f - thumbPadding, _pointableScrollVertical.Normal);

            return true;
        }

        public override void Press(Pointer pointer, Vector3 position)
        {
            base.Press(pointer, position);

            if (_gooScrollThumb.InBounds(position))
                return;
            
            position = Vector3.ProjectOnPlane(position, _goo.Forward);
            position = Vector3.ProjectOnPlane(position, _goo.Rightward );
            position = _goo.Centre + position;

            Vector3 bottom = _goo.Bottom + (_goo.Upward   * (_gooScrollThumb.Height / 2f));
            Vector3 top    = _goo.Top    + (_goo.Downward * (_gooScrollThumb.Height / 2f));

            Vector3 difference = position - bottom;

            float total = (top - bottom).magnitude;
            float amount = Vector3.Angle(difference.normalized, _goo.Upward) < 90f 
                ? (position - bottom).magnitude
                : 0f;

            float normal = Mathf.Clamp01(amount / total);

            _pointableScrollVertical.ScrollNormal(-normal);
        }

        public override void Drag(Pointer pointer, Drag drag)
        {
            base.Drag(pointer, drag);

            Vector3 from = drag.From;
            Vector3 to   = drag.To;

            from = Vector3.ProjectOnPlane(from, _goo.Forward  );
            from = Vector3.ProjectOnPlane(from, _goo.Rightward);
            from = _goo.Centre + from;

            to   = Vector3.ProjectOnPlane(to  , _goo.Forward  );
            to   = Vector3.ProjectOnPlane(to  , _goo.Rightward);
            to   = _goo.Centre + to;

            Vector3 difference = to - from;

            float sign = Vector3.Angle(difference, _goo.Upward) < 90f
                ?   1f
                : - 1f;

            Vector3 bottom = _goo.Bottom + (_goo.Upward   * (_gooScrollThumb.Height / 2f));
            Vector3 top    = _goo.Top    + (_goo.Downward * (_gooScrollThumb.Height / 2f));
            
            float total = (top - bottom).magnitude;
            float amount = Mathf.Clamp01(difference.magnitude / total);

            _pointableScrollVertical.ScrollNormal(_pointableScrollVertical.Normal + -(sign * amount));
        }
    }
}