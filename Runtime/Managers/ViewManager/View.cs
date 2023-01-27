using Nova;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace VED.Utilities
{
    [RequireComponent(typeof(UIBlock2D))]
    public abstract class View : MonoBehaviour
    {
        public abstract void Show(Action callback = null);
        public abstract void Hide(Action callback = null);

        public UIBlock2D UIBlock2D => _uiBlock2D;
        protected UIBlock2D _uiBlock2D = null;

        public virtual void Awake()
        {
            int layer = LayerMask.NameToLayer("UI");

            _uiBlock2D = GetComponent<UIBlock2D>();
            _uiBlock2D.GameObjectLayer = layer;

            UIBlock[] uiBlocks = GetComponentsInChildren<UIBlock>();
            for (int i = 0; i < uiBlocks.Length; i++)
            {
                uiBlocks[i].GameObjectLayer = layer;
            }
        }
    }
}