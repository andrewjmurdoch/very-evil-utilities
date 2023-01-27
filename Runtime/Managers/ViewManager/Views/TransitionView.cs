using Nova;
using System;
using UnityEngine;

namespace VED.Utilities
{
    public class TransitionView : View
    {
        public override void Show(Action callback = null)
        {
            _uiBlock2D.Visible = true;
            callback?.Invoke();
        }

        public override void Hide(Action callback = null)
        {
            _uiBlock2D.Visible = false;
            callback?.Invoke();
        }
    }
}