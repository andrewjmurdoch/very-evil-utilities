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

        public override void Tick()
        {
            if (!TickReferences()) return;
            if (!TickSize      ()) return;
            if (!TickPosition  ()) return;
        }

        private bool TickReferences()
        {
            if (   !_gooScrollThumb
                || !_gooScrollThumb.GetReferenceSizeVertical    (out Goo gooScrollThumbReferenceSize    )
                || !_gooScrollThumb.GetReferencePositionVertical(out Goo gooScrollThumbReferencePosition)
                || gooScrollThumbReferenceSize != gooScrollThumbReferencePosition)
                return false;

            if (   !_pointableScrollVertical
                || !_pointableScrollVertical.GooScrollable
                || !_pointableScrollVertical.GooScrollable.GetReferenceSizeVertical(out Goo gooScrollableReference))
                return false;

            return true;
        }

        private bool TickSize()
        {
            _pointableScrollVertical.GooScrollable.GetReferenceSizeVertical(out Goo gooScrollableReference);
            
            // scale thumb in accordance to total scrollable area
            float thumbScale = (1f - (_pointableScrollVertical.Total / gooScrollableReference.RectTransform.rect.height)) * 100f;
            _gooScrollThumb.SizeVertical.Float = Mathf.Max(thumbScale, MIN_THUMB_SCALE);
            _gooScrollThumb.Tick();

            return true;
        }

        private bool TickPosition()
        {
            _gooScrollThumb.GetReferenceSizeVertical(out Goo gooScrollThumbReference);
    
            // position thumb in accordance with normalized position / total
            float thumbPadding = ((_gooScrollThumb.Height / 2f) / gooScrollThumbReference.Height) * 100f;
            _gooScrollThumb.PositionVertical.Float = -Mathf.Lerp(-50f + thumbPadding, 50f - thumbPadding, _pointableScrollVertical.Normal);

            return true;
        }

        public override void Press(Pointer pointer, Vector3 position)
        {
            base.Press(pointer, position);

            if (_gooScrollThumb.InBounds(position))
                return;

            if (   !_gooScrollThumb
                || !_gooScrollThumb.GetReferencePositionVertical(out Goo gooScrollThumbReferencePosition))
                return;

            position = Vector3.ProjectOnPlane(position, gooScrollThumbReferencePosition.Forward  );
            position = Vector3.ProjectOnPlane(position, gooScrollThumbReferencePosition.Rightward);
            position = gooScrollThumbReferencePosition.Centre + position;

            Vector3 bottom = gooScrollThumbReferencePosition.Bottom + (gooScrollThumbReferencePosition.Upward   * (_gooScrollThumb.Height / 2f));
            Vector3 top    = gooScrollThumbReferencePosition.Top    + (gooScrollThumbReferencePosition.Downward * (_gooScrollThumb.Height / 2f));

            Vector3 difference = position - bottom;

            float total = (top - bottom).magnitude;
            float amount = Vector3.Angle(difference.normalized, gooScrollThumbReferencePosition.Upward) < 90f 
                ? (position - bottom).magnitude
                : 0f;

            float normal = Mathf.Clamp01(amount / total);

            _pointableScrollVertical.ScrollNormal(1f - normal);
        }

        public override void Drag(Pointer pointer, Drag drag)
        {
            base.Drag(pointer, drag);

            if (   !_gooScrollThumb
                || !_gooScrollThumb.GetReferencePositionVertical(out Goo gooScrollThumbReferencePosition))
                return;

            Vector3 from = drag.From;
            Vector3 to   = drag.To;

            from = Vector3.ProjectOnPlane(from, gooScrollThumbReferencePosition.Forward  );
            from = Vector3.ProjectOnPlane(from, gooScrollThumbReferencePosition.Rightward);
            from = gooScrollThumbReferencePosition.Centre + from;

            to   = Vector3.ProjectOnPlane(to  , gooScrollThumbReferencePosition.Forward  );
            to   = Vector3.ProjectOnPlane(to  , gooScrollThumbReferencePosition.Rightward);
            to   = gooScrollThumbReferencePosition.Centre + to;

            Vector3 difference = to - from;

            float sign = Vector3.Angle(difference, gooScrollThumbReferencePosition.Upward) < 90f
                ?   1f
                : - 1f;

            Vector3 bottom = gooScrollThumbReferencePosition.Bottom + (gooScrollThumbReferencePosition.Upward   * (_gooScrollThumb.Height / 2f));
            Vector3 top    = gooScrollThumbReferencePosition.Top    + (gooScrollThumbReferencePosition.Downward * (_gooScrollThumb.Height / 2f));
            
            float total = (top - bottom).magnitude;
            float amount = Mathf.Clamp01(difference.magnitude / total);

            _pointableScrollVertical.ScrollNormal(_pointableScrollVertical.Normal + -(sign * amount));
        }
    }
}