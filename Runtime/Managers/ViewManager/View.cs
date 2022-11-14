using System;
using UnityEngine;

namespace VED.Utilities
{
    public abstract class View : MonoBehaviour
    {
        public enum Layers
        {
            TRANSITIONS,
            HUD,
            MAIN,
        }

        public Layers Layer => _layer;
        [SerializeField] private Layers _layer = Layers.MAIN;

        public abstract void Show(Action callback = null);
        public abstract void Hide(Action callback = null);
    }
}