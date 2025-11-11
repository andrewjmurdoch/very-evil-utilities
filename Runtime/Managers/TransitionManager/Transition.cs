using Gooey;
using System;
using UnityEngine;

namespace VED.Utilities
{
    [RequireComponent(typeof(Goo))]
    public abstract class Transition : MonoBehaviour
    {
        public abstract void Stop();
        public abstract void In(Action callback = null);
        public abstract void Out(Action callback = null);
        public abstract void SetIn();
        public abstract void SetOut();

        public Goo Goo => _goo;
        protected Goo _goo = null;

        protected virtual void Awake()
        {
            int layer = transform.parent.gameObject.layer;

            _goo = GetComponent<Goo>();
            gameObject.layer = layer;

            Goo[] goo = GetComponentsInChildren<Goo>();
            for (int i = 0; i < goo.Length; i++)
            {
                goo[i].gameObject.layer = layer;
            }
        }
    }
}