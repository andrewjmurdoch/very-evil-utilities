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

        public Goo GooScrollThumb => _gooScrollThumb;

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

        public override void Tick()
        {
            if (!TickReferences()) return;
            if (!TickSize      ()) return;
            if (!TickPosition  ()) return;
        }

        private bool TickReferences()
        {
            if (   !_gooScrollThumb
                || !_gooScrollThumb.GetReferenceSizeHorizontal    (out Goo gooScrollThumbReferenceSize)
                || !_gooScrollThumb.GetReferencePositionHorizontal(out Goo gooScrollThumbReferencePosition)
                || gooScrollThumbReferenceSize != gooScrollThumbReferencePosition)
                return false;

            if (   !_pointableScrollHorizontal
                || !_pointableScrollHorizontal.GooScrollable
                || !_pointableScrollHorizontal.GooScrollable.GetReferenceSizeHorizontal(out Goo gooScrollableReference))
                return false;

            return true;
        }

        private bool TickSize()
        {
            _pointableScrollHorizontal.GooScrollable.GetReferenceSizeHorizontal(out Goo gooScrollableReference);

            // scale thumb in accordance to total scrollable area
            float thumbScale = Mathf.Clamp01(gooScrollableReference.Width / Mathf.Max(_pointableScrollHorizontal.GooScrollable.Width, Mathf.Epsilon)) * 100f;
            _gooScrollThumb.SizeHorizontal.Float = Mathf.Max(thumbScale, MIN_THUMB_SCALE);
            _gooScrollThumb.Tick();

            return true;
        }

        private bool TickPosition()
        {
            _gooScrollThumb.GetReferenceSizeHorizontal(out Goo gooScrollThumbReference);

            // position thumb in accordance with normalized position / total
            float thumbPadding = ((_gooScrollThumb.Width / 2f) / gooScrollThumbReference.Width) * 100f;
            _gooScrollThumb.PositionHorizontal.Float = Mathf.Lerp(-50f + thumbPadding, 50f - thumbPadding, _pointableScrollHorizontal.Normal);

            return true;
        }

        public override void Press(Pointer pointer, Vector3 position, bool repress = false) 
        {
            base.Press(pointer, position, repress);

            if (repress)
                return;

            if (_gooScrollThumb.InBounds(position))
                return;

            if (   !_gooScrollThumb
                || !_gooScrollThumb.GetReferencePositionHorizontal(out Goo gooScrollThumbReferencePosition))
                return;

            position = Vector3.ProjectOnPlane(position, gooScrollThumbReferencePosition.Forward);
            position = Vector3.ProjectOnPlane(position, gooScrollThumbReferencePosition.Upward );
            position = gooScrollThumbReferencePosition.Centre + position;

            Vector3 left  = gooScrollThumbReferencePosition.Left  + (gooScrollThumbReferencePosition.Rightward * (_gooScrollThumb.Width / 2f));
            Vector3 right = gooScrollThumbReferencePosition.Right + (gooScrollThumbReferencePosition.Leftward  * (_gooScrollThumb.Width / 2f));

            Vector3 difference = position - left;

            float total = (right - left).magnitude;
            float amount = Vector3.Angle(difference.normalized, gooScrollThumbReferencePosition.Rightward) < 90f 
                ? (position - left).magnitude
                : 0f;

            float normal = Mathf.Clamp01(amount / total);

            _pointableScrollHorizontal.ScrollNormal(normal);
        }

        public override void Drag(Pointer pointer, Drag drag)
        {
            base.Drag(pointer, drag);

            if (   !_gooScrollThumb
                || !_gooScrollThumb.GetReferencePositionHorizontal(out Goo gooScrollThumbReferencePosition))
                return;

            Vector3 from = drag.From;
            Vector3 to   = drag.To;

            from = Vector3.ProjectOnPlane(from, gooScrollThumbReferencePosition.Forward);
            from = Vector3.ProjectOnPlane(from, gooScrollThumbReferencePosition.Upward );
            from = gooScrollThumbReferencePosition.Centre + from;

            to   = Vector3.ProjectOnPlane(to  , gooScrollThumbReferencePosition.Forward);
            to   = Vector3.ProjectOnPlane(to  , gooScrollThumbReferencePosition.Upward );
            to   = gooScrollThumbReferencePosition.Centre + to;

            Vector3 difference = to - from;

            float sign = Vector3.Angle(difference, gooScrollThumbReferencePosition.Rightward) < 90f
                ?   1f
                : - 1f;

            Vector3 left  = gooScrollThumbReferencePosition.Left  + (gooScrollThumbReferencePosition.Rightward * (_gooScrollThumb.Width / 2f));
            Vector3 right = gooScrollThumbReferencePosition.Right + (gooScrollThumbReferencePosition.Leftward  * (_gooScrollThumb.Width / 2f));
            
            float total = (right - left).magnitude;
            float amount = Mathf.Clamp01(difference.magnitude / total);

            _pointableScrollHorizontal.ScrollNormal(_pointableScrollHorizontal.Normal + (sign * amount));
        }
    }
}