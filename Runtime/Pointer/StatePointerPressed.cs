using System.Collections.Generic;
using UnityEngine;

namespace VED.Utilities
{
    public class StatePointerPressed : StatePointer
    {
        // drag
        private const int   MAX_POSITION_COUNT        = 150;
        private const float DRAG_THRESHOLD_MAGNITUDE  = 000.005f;
    
        // swipe
        private const float SWIPE_THRESHOLD_MAGNITUDE = 000.100f;
        private const float SWIPE_THRESHOLD_ANGLE     = 030.000f;
        private const float SWIPE_THRESHOLD_DURATION  = 000.225f;
        private const float SWIPE_THRESHOLD_SPEED     = 001.150f;
        private const int   SWIPE_THRESHOLD_POSITIONS = 003;
        private const int   SWIPE_TOLERANCE_ANGLE     = 006;
    
        // circle
        private const int   CIRCLE_MIN_POSITION_COUNT = 006;
        private const float CIRCLE_MIN_CLOSE_ANGLE    = 330.000f;
    
        private Pointer _pointer = null;
    
        public struct PositionTime
        {
            public Vector3 Position;
            public float Time;
        
            public PositionTime(Vector3 position)
            {
                Position = position;
                Time = UnityEngine.Time.unscaledTime;
            }
        }
    
        public class Press
        {
            public Pointable                  Pointable;
            public bool                       Pointed;
            public Plane                      Plane;
            public LoopingArray<PositionTime> Positions;
            public Drag                       Drag;
            public Swipe                      Swipe;
    
            public Press(Point point, Vector3 direction)
            {
                Pointable = point.Pointable;
                Pointed = true;
    
                Plane = Pointable.Flat
                        ? new(Pointable.Transform.forward, Pointable.Transform.position)
                        : new(-direction                 , Pointable.Transform.position);
    
                Positions = new(MAX_POSITION_COUNT);
                Positions.Push(new(point.Position));
    
                Drag  = new(point.Position, point.Position);
                Swipe = new();
            }
        }
        private Press[] _pressed;
    
        public StatePointerPressed(Pointer pointer, List<Point> pressed)
        {
            _pointer = pointer;
            _pressed = new Press[pressed.Count];
    
            _pointer.GetRay(out Ray ray);
    
            for (int i = 0; i < pressed.Count; i++)
                _pressed[i] = new Press(pressed[i], ray.direction);
        }
    
        public override void Tick()
        {
            if (!TickDrag  ()) return;
            if (!TickCircle()) return;
            if (!TickPoint ()) return;
            if (!TickPress ()) return;
        }
    
        private bool TickDrag()
        {
            bool value = true;
    
            for (int i = 0; i < _pressed.Length; i++)
                value &= TickDrag(_pressed[i]);
    
            return value;
        }
    
        private bool TickDrag(Press pressed)
        {
            if (!_pointer.GetRay(out Ray ray) || !pressed.Plane.Raycast(ray, out float enter))
                return true;
    
    #if UNITY_EDITOR
            for (int i = 1; i < pressed.Positions.Count; i++)
            {
                Color color = Color.Lerp(Color.red, Color.yellow, i/(float)pressed.Positions.Count);
                Debug.DrawLine(pressed.Positions[i - 1].Position, pressed.Positions[i].Position, color, Time.deltaTime);
            }
    #endif
            
            Vector3 position = ray.origin + (ray.direction * enter);
            if ((position - pressed.Positions[^1].Position).magnitude < DRAG_THRESHOLD_MAGNITUDE)
                return true;
    
            pressed.Positions.Push(new PositionTime(position));
    
            pressed.Drag.From = pressed.Drag.To;
            pressed.Drag.To = position;
    
            pressed.Pointable.Drag(_pointer, pressed.Drag);
            return true;
        }
    
        private bool TickCircle()
        {
            bool value = true;
    
            for (int i = 0; i < _pressed.Length; i++)
                value &= TickCircle(_pressed[i]);
    
            return value;
        }
    
        private bool TickCircle(Press pressed)
        {
            if (!ValidateCircle(pressed, out Circle circle))
                return true;
    
    #if UNITY_EDITOR
            const int RESOLUTION = 40;
            const float ANGLE = 360 / (float)RESOLUTION;
            
            Vector3 up      = pressed.Pointable.Transform.up;
            Vector3 forward = circle.Normal;
    
            for (int i = 0; i < RESOLUTION; i++)
            {
                Vector3 directionStart = (Quaternion.AngleAxis( i      * ANGLE, forward) * up);
                Vector3 directionEnd   = (Quaternion.AngleAxis((i + 1) * ANGLE, forward) * up);
    
                Vector3 start = circle.Centre + (directionStart * circle.Radius);
                Vector3 end   = circle.Centre + (directionEnd   * circle.Radius);
    
                Debug.DrawLine(start, end, Color.yellow, 1f);
            }
    #endif
    
            pressed.Pointable.Circle(_pointer, circle);
    
            // reset positions to prevent finding same circle multiple times
            PositionTime position = pressed.Positions[^1];
            pressed.Positions.Clear();
            pressed.Positions.Push(position);
            return true;
        }
    
        private bool TickPoint()
        {
            _pointer.GetPointed(out List<Point> pointed);
    
            bool value = true;
    
            for (int i = 0; i < _pressed.Length; i++)
                value &= TickPoint(pointed, _pressed[i]);
    
            return value;
        }
    
        private bool TickPoint(List<Point> pointed, Press pressed)
        {
            // find pointed or not
            Point point = new Point();
            bool pressedPointed = false;
    
            for (int i = 0; i < pointed.Count; i++)
            {
                if (pointed[i].Pointable != pressed.Pointable)
                    continue;
    
                pressedPointed = true;
                point = pointed[i];
                break;
            }
    
            // pointed -> unpointed
            if (pressed.Pointed && !pressedPointed)
            {
                pressed.Pointed = false;
                pressed.Pointable.Unpress(_pointer, true);
                pressed.Pointable.Unpoint(_pointer);
                return true;
            }
    
            // unpointed -> pointed
            if (!pressed.Pointed && pressedPointed)
            {
                pressed.Pointed = true;
                pressed.Pointable.Point(_pointer, point.Position);
                pressed.Pointable.Press(_pointer, point.Position, true);
                return true;
            }
    
            return true;
        }
    
        private bool TickPress()
        {
            if (_pointer.GetPressed())
                return true;
            
            for (int i = 0; i < _pressed.Length; i++)
                TickSwipe(_pressed[i]);
    
            if (!_pointer.GetPointed(out List<Point> pointed))
            {
                _pointer.StateManager.Pop();
                _pointer.StateManager.Push(new StatePointerUnpointed(_pointer));
                return false;
            }
    
            for (int i = 0; i < _pressed.Length; i++)
            {
                if (!_pressed[i].Pointed)
                    continue;
    
                _pressed[i].Pointable.Unpress(_pointer);
            }
    
            _pointer.StateManager.Pop();
            _pointer.StateManager.Push(new StatePointerPointed(_pointer, pointed));
            return false;
        }
    
        private void TickSwipe(Press pressed)
        {
            if (!ValidateSwipe(pressed))
                return;
            
    #if UNITY_EDITOR
            Vector3 end = pressed.Swipe.Position + (pressed.Swipe.Direction * pressed.Swipe.Magnitude);
            Vector3 skew = end + (pressed.Swipe.Skew * (pressed.Swipe.Magnitude / 4f));
    
            Debug.DrawLine(pressed.Swipe.Position, end, Color.green, 3f);
            Debug.DrawLine(end, skew, Color.magenta, 3f);
            Debug.DrawLine(pressed.Swipe.Position, pressed.Swipe.Position + (pressed.Swipe.Direction * SWIPE_THRESHOLD_MAGNITUDE), Color.red, 3f);
    #endif
    
            pressed.Pointable.Swipe(_pointer, pressed.Swipe);
        }
    
        private bool ValidateSwipe(Press pressed)
        {
            pressed.Swipe = new Swipe();
           
            if (pressed.Positions.Count < SWIPE_THRESHOLD_POSITIONS)
                return false;
    
            // loop over each position and determine if press has been 1 continuous swipe
            // or if press has a harsh angle in it, and so swipe can only be validated after such
            int count = 1;
            Vector2 previous        = pressed.Positions[^1].Position - pressed.Positions[^2].Position;
            pressed.Swipe.Duration  = pressed.Positions[^1].Time     - pressed.Positions[^2].Time;
            
            pressed.Swipe.Magnitude = 0;
            pressed.Swipe.Speed = previous.magnitude / pressed.Swipe .Duration;
            float t = 0.25f;
            for (int i = 2; i < pressed.Positions.Count; i++)
            {
                Vector2 current = pressed.Positions[^i].Position - pressed.Positions[^(i + 1)].Position;
    
                // early exit if there is a distinct break in a swipe
                if (i >= SWIPE_TOLERANCE_ANGLE && Vector2.Angle(current, previous) > SWIPE_THRESHOLD_ANGLE)
                {
                    pressed.Swipe.Position = pressed.Positions[^(count + 1)].Position;
                    ValidateSwipeDirection(pressed, count);
                    ValidateSwipeSkew     (pressed, count);
    
                    return pressed.Swipe.Magnitude >= SWIPE_THRESHOLD_MAGNITUDE
                        && pressed.Swipe.Speed     >= SWIPE_THRESHOLD_SPEED
                        && pressed.Swipe.Duration  <  SWIPE_THRESHOLD_DURATION;
                }
    
                pressed.Swipe.Magnitude += current.magnitude;
                pressed.Swipe.Duration = pressed.Positions[^1].Time - pressed.Positions[^i].Time;
    
                float s = current.magnitude / (pressed.Positions[^i].Time - pressed.Positions[^(i + 1)].Time);
                pressed.Swipe.Speed = Mathf.Lerp(pressed.Swipe.Speed, s, t);
                t /= 4f;
    
                count++;
                previous = current;
            }
    
            pressed.Swipe.Position = pressed.Positions[^(count + 1)].Position;
            ValidateSwipeDirection(pressed, count);
            ValidateSwipeSkew     (pressed, count);
    
            return pressed.Swipe.Magnitude >= SWIPE_THRESHOLD_MAGNITUDE
                && pressed.Swipe.Speed     >= SWIPE_THRESHOLD_SPEED
                && pressed.Swipe.Duration  <  SWIPE_THRESHOLD_DURATION;
        }
    
        private void ValidateSwipeDirection(Press pressed, int count)
        {
            // get direction, from start of swipe towards end, lerp towards next direction by [0.5f -> 0.0f]
            pressed.Swipe.Direction = (pressed.Positions[^count].Position - pressed.Positions[^(count + 1)].Position).normalized;
            float t = 0.5f;
            for (int i = count; i >= 2; i--)
            {
                Vector2 influence = (pressed.Positions[^(i - 1)].Position - pressed.Positions[^i].Position).normalized;
                pressed.Swipe.Direction = Vector2.Lerp(pressed.Swipe.Direction, influence, t);
                t /= 2f;
            }
        }
    
        private void ValidateSwipeSkew(Press pressed, int count)
        {
            // get skew, from end of swipe towards start, lerp towards previous direction by [0.5f -> 0.0f]
            pressed.Swipe.Skew = (pressed.Positions[^1].Position - pressed.Positions[^2].Position).normalized;
            float t = 0.5f;
            for (int i = 2; i <= count; i++)
            {
                Vector2 influence = (pressed.Positions[^i].Position - pressed.Positions[^(i + 1)].Position).normalized;
                pressed.Swipe.Skew = Vector2.Lerp(pressed.Swipe.Skew, influence, t);
                t /= 2f;
            }
        }
    
        private bool ValidateCircle(Press pressed, out Circle circle)
        {
            circle = new Circle(pressed.Plane.normal, Vector3.zero, 0f);
    
            // find if there are fewer than min positions, and therefore cannot calculate circle
            if (pressed.Positions.Count <= CIRCLE_MIN_POSITION_COUNT) 
                return false;
    
            // check if there is a full rotation within all points
            if (!ValidateCircleRotation(pressed, out int count))
                return false;
            
            // find average position, centre of circle
            for (int i = 1; i <= count; i++)
            {
                circle.Centre += pressed.Positions[^i].Position;
            }
            circle.Centre /= count;
    
            // find average distance from centre, radius of circle
            circle.Radius = 0;
            for (int i = 1; i <= count; i++)
            {
                circle.Radius += (pressed.Positions[^i].Position - circle.Centre).magnitude;
            }
            circle.Radius /= count;
    
            // ensure angles between each point progress either clockwise or anticlockwise
            Vector3 initial  = circle.Centre - pressed.Positions[^(count + 1)].Position;
            Vector3 previous = initial;
            Vector3 next     = circle.Centre - pressed.Positions[^count].Position;
            float angle      = Vector3.SignedAngle(previous, next, circle.Normal);
            float sign       = Mathf.Sign(angle);
            float totalAngle = Mathf.Abs(angle);
            for (int i = (count - 1); i >= 1; i--)
            {
                previous = next;
                next = circle.Centre - pressed.Positions[^i].Position;
    
                // validate whether angle sign is congruent
                angle = Vector3.SignedAngle(previous, next, circle.Normal);
                if (Mathf.Abs(angle) > 0 && Mathf.Sign(angle) != sign)
                    return false;
    
                // validate whether a full angle has been reached & start and end points are near enough each other
                totalAngle += Mathf.Abs(angle);
                if (totalAngle >= CIRCLE_MIN_CLOSE_ANGLE && (pressed.Positions[i].Position - pressed.Positions[0].Position).magnitude <= circle.Radius) 
                    return true;
            }
    
            return false;
        }
    
        private bool ValidateCircleRotation(Press pressed, out int index)
        {
            index = 1;
    
            // ensure angle totals over 360 degrees
            float totalAngle = 0f;
    
            for (index = 1; index < (pressed.Positions.Count - 2); index++)
            {
                Vector2 current  = pressed.Positions[^index      ].Position - pressed.Positions[^(index + 1)].Position;
                Vector2 previous = pressed.Positions[^(index + 1)].Position - pressed.Positions[^(index + 2)].Position;
    
                totalAngle += Vector2.SignedAngle(previous, current);
    
                if (Mathf.Abs(totalAngle) >= CIRCLE_MIN_CLOSE_ANGLE)
                    return true;
            }
    
            return false;
        }
    
        public override void Disable()
        {
            for (int i = 0; i < _pressed.Length; i++)
            {
                if (!_pressed[i].Pointed)
                    continue;
    
                _pressed[i].Pointable.Unpress(_pointer, true);
                _pressed[i].Pointable.Unpoint(_pointer);
            }
        }
    }
}