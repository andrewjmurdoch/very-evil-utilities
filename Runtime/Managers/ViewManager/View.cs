using Gooey;
using UnityEngine;

namespace VED.Utilities
{
    [RequireComponent(typeof(Goo))]
    public abstract class View : MonoBehaviour
    {
        protected Goo _goo = null;

        public Goo Goo => _goo;

        public virtual void Init()
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

        public virtual void Hide()
        {
            gameObject.SetActive(false);
            ViewManager.Instance.Hidden(this);
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
            ViewManager.Instance.Shown(this);
        }
    }
}