using System;
using UnityEngine;

namespace VED.Utilities
{
    public abstract class View : MonoBehaviour
    {
        public abstract void Show(Action callback = null);
        public abstract void Hide(Action callback = null);
    }
}