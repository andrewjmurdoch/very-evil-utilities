using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VED.Utilities
{
    public class PointerMouse : Pointer
    {
        private Camera  _camera = null;
        private Vector2 _scroll = Vector2.zero;
    
        private const float THRESHOLD_SCROLL = 000.010f;
    
        public void Init(Camera camera, List<Pointable> pointables = null)
        {
            _camera = camera;

            base.Init(pointables);
        }
    
        public override void Tick()
        {
            if (!TickScroll()) return;
    
            base.Tick();
        }
    
        private bool TickScroll()
        {
            _scroll = Mouse.current.scroll.value;
            return true;
        }
    
        public override bool GetPressed()
        {
            return Mouse.current.leftButton.isPressed;
        }
    
        public override bool GetRay(out Ray ray)
        {
            ray = _camera.ScreenPointToRay(Mouse.current.position.value);
            return Mouse.current != null;
        }
    
        public override bool GetScroll(out Vector2 scroll)
        {
            scroll = _scroll;
            return _scroll.magnitude >= THRESHOLD_SCROLL;
        }
    }
}