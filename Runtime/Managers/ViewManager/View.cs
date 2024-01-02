using Nova;
using UnityEngine;

namespace VED.Utilities
{
    [RequireComponent(typeof(UIBlock2D))]
    public abstract class View : MonoBehaviour
    {
        public UIBlock2D UIBlock2D => _uiBlock2D;
        protected UIBlock2D _uiBlock2D = null;

        public virtual void Init()
        {
            int layer = transform.parent.gameObject.layer;

            _uiBlock2D = GetComponent<UIBlock2D>();
            _uiBlock2D.GameObjectLayer = layer;

            UIBlock[] uiBlocks = GetComponentsInChildren<UIBlock>();
            for (int i = 0; i < uiBlocks.Length; i++)
            {
                uiBlocks[i].GameObjectLayer = layer;
            }
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
    }
}