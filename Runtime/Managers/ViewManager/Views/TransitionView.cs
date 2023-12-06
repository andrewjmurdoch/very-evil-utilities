using Nova;
using System;
using UnityEngine;

namespace VED.Utilities
{
    public class TransitionView : View
    {
        public override void Init()
        {
            int layer = LayerMask.NameToLayer("Transition");

            _uiBlock2D = GetComponent<UIBlock2D>();
            _uiBlock2D.GameObjectLayer = layer;

            UIBlock[] uiBlocks = GetComponentsInChildren<UIBlock>();
            for (int i = 0; i < uiBlocks.Length; i++)
            {
                uiBlocks[i].GameObjectLayer = layer;
            }
        }

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