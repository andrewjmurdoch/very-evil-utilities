using Nova;
using System;
using UnityEngine;

namespace VED.Utilities
{
    [RequireComponent(typeof(UIBlock2D))]
    public abstract class Transition : MonoBehaviour
    {
        public const float DEFAULT_DURATION = 1f;

        public abstract void Stop();
        public abstract void In(Action callback = null, float duration = DEFAULT_DURATION);
        public abstract void Out(Action callback = null, float duration = DEFAULT_DURATION);

        public UIBlock2D UIBlock2D => _uiBlock2D;
        protected UIBlock2D _uiBlock2D = null;

        protected virtual void Awake()
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