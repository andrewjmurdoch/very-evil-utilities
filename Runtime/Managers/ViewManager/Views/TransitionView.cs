using Nova;
using System;
using UnityEngine;

namespace VED.Utilities
{
    public class TransitionView : View
    {
        public override void Show()
        {
            _uiBlock2D.Visible = true;
        }

        public override void Hide()
        {
            _uiBlock2D.Visible = false;
        }
    }
}