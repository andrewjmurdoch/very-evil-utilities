using Gooey;
using UnityEngine;

namespace VED.Utilities
{
    [RequireComponent(typeof(Goo))]
    public class PointableScrollbarHorizontal : PointableButton
    {
        private const float MIN_THUMB_SCALE = 5f;

        [SerializeField] private PointableScrollHorizontal _pointableScrollHorizontal = null;
        [SerializeField] private Goo                       _gooScrollThumb            = null;

        private Goo _goo = null;

        public void OnValidate()
        {
            if (!_gooScrollThumb)
            {
                GameObject gameObjectGooScrollThumb = new GameObject("Scroll Thumb");
                gameObjectGooScrollThumb.transform.SetParent(transform);

                _gooScrollThumb = gameObjectGooScrollThumb.AddComponent<Goo>();
            }

            _gooScrollThumb.PositionAlignmentHorizontal = AlignmentHorizontal.CENTRE;
            _gooScrollThumb.PositionHorizontal.Type     = ValueType.PERCENTAGE;
            _gooScrollThumb.SizeHorizontal    .Type     = ValueType.PERCENTAGE;
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

            if (   !_pointableScrollHorizontal.GooScrollable
                || !_pointableScrollHorizontal.GooScrollable.GetReferenceSizeHorizontal(out Goo gooScrollableReference))
                return false;
            
            // scale thumb in accordance to total scrollable area
            float thumbScale = (1f - (_pointableScrollHorizontal.Total / gooScrollableReference.RectTransform.rect.width)) * 100f;
            _gooScrollThumb.SizeHorizontal.Float = Mathf.Max(thumbScale, MIN_THUMB_SCALE);
            _gooScrollThumb.Tick();

            return true;
        }

        private bool TickPosition()
        {
            if (   !_goo
                || !_gooScrollThumb)
                return false;

            if (   !_pointableScrollHorizontal.GooScrollable
                || !_pointableScrollHorizontal.GooScrollable.GetReferenceSizeHorizontal(out Goo gooScrollableReference))
                return false;
    
            // position thumb in accordance with normalized position / total
            float thumbPadding = ((_gooScrollThumb.Width / 2f) / _goo.Width) * 100f;
            _gooScrollThumb.PositionHorizontal.Float = Mathf.Lerp(-50f + thumbPadding, 50f - thumbPadding, _pointableScrollHorizontal.Normal);

            return true;
        }

        public override void Press(Pointer pointer, Vector3 position)
        {
            base.Press(pointer, position);

            if (_gooScrollThumb.InBounds(position))
                return;
            
            position = Vector3.ProjectOnPlane(position, _goo.Forward);
            position = Vector3.ProjectOnPlane(position, _goo.Upward );
            position = _goo.Centre + position;

            Vector3 left  = _goo.Left  + (_goo.Rightward * (_gooScrollThumb.Width / 2f));
            Vector3 right = _goo.Right + (_goo.Leftward  * (_gooScrollThumb.Width / 2f));

            Vector3 difference = position - left;

            float total = (right - left).magnitude;
            float amount = Vector3.Angle(difference.normalized, _goo.Rightward) < 90f 
                ? (position - left).magnitude
                : 0f;

            float normal = Mathf.Clamp01(amount / total);

            _pointableScrollHorizontal.ScrollNormal(normal);
        }

        public override void Drag(Pointer pointer, Drag drag)
        {
            base.Drag(pointer, drag);

            Vector3 from = drag.From;
            Vector3 to   = drag.To;

            from = Vector3.ProjectOnPlane(from, _goo.Forward);
            from = Vector3.ProjectOnPlane(from, _goo.Upward );
            from = _goo.Centre + from;

            to   = Vector3.ProjectOnPlane(to  , _goo.Forward);
            to   = Vector3.ProjectOnPlane(to  , _goo.Upward );
            to   = _goo.Centre + to;

            Vector3 difference = to - from;

            float sign = Vector3.Angle(difference, _goo.Rightward) < 90f
                ?   1f
                : - 1f;

            Vector3 left  = _goo.Left  + (_goo.Rightward * (_gooScrollThumb.Width / 2f));
            Vector3 right = _goo.Right + (_goo.Leftward  * (_gooScrollThumb.Width / 2f));
            
            float total = (right - left).magnitude;
            float amount = Mathf.Clamp01(difference.magnitude / total);

            _pointableScrollHorizontal.ScrollNormal(_pointableScrollHorizontal.Normal + (sign * amount));
        }
    }
}