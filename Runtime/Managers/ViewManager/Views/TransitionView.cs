using System;
using UnityEngine;

namespace VED.Utilities
{
    public class TransitionView : View
    {
        public Canvas Canvas => _canvas;
        private Canvas _canvas = null;

        private void Awake()
        {
            _canvas = transform.Find("Canvas").GetComponent<Canvas>();
        }

        public override void Show(Action callback = null)
        {
            _canvas.enabled = true;
            callback?.Invoke();
        }

        public override void Hide(Action callback = null)
        {
            _canvas.enabled = false;
            callback?.Invoke();
        }
    }
}