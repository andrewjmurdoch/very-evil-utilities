using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VED.Utilities
{
    public class ViewManager : SingletonMonoBehaviour<ViewManager>
    {
        public ViewMapper ViewMapper
        {
            get 
            { 
                return _viewMapper; 
            }
            set
            {
                if (_viewMapper != null) DeinitViewMapper();
                if (value != null) InitViewMapper(value);
            }
        }
        private ViewMapper _viewMapper = null;

        private Dictionary<Type, View> _views = new Dictionary<Type, View>();

        public Dictionary<string, Transform> ViewLayers => _viewLayers;
        private Dictionary<string, Transform> _viewLayers = new Dictionary<string, Transform>();

        private Canvas _canvas = null;

        protected override void Awake()
        {
            base.Awake();

            _canvas = new GameObject("View Canvas").AddComponent<Canvas>();
            _canvas.transform.SetParent(transform);
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.gameObject.AddComponent<GraphicRaycaster>();
        }

        private void InitViewMapper(ViewMapper viewMapper)
        { 
            _viewMapper = viewMapper;

            foreach (ViewLayer viewLayer in _viewMapper.ViewLayers)
            {
                RectTransform rectTransform = new GameObject(viewLayer.ID).AddComponent<RectTransform>();
                rectTransform.SetParent(_canvas.transform);
                rectTransform.SetAsLastSibling();
                rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                rectTransform.pivot     = new Vector2(0.5f, 0.5f);
                rectTransform.offsetMin = new Vector2(0.0f, 0.0f);
                rectTransform.offsetMax = new Vector2(0.0f, 0.0f);

                _viewLayers.Add(viewLayer.ID, rectTransform);
            }
        }

        private void DeinitViewMapper()
        {
            // destroy all current view layers
            for (int i = 0; i < _canvas.transform.childCount; i++)
            {
                Destroy(_canvas.transform.GetChild(0).gameObject);
            }
            _viewLayers.Clear();
            _views.Clear();
        }

        public T GetView<T>() where T : View
        {
            Type type = typeof(T);

            if (_views.ContainsKey(type)) return (T)_views[type];

            return LoadView<T>();
        }

        public T LoadView<T>() where T : View
        {
            if (_viewMapper == null)
            {
                Debug.LogError("Attempted to load view before assigning ViewMapper");
            }

            Type type = typeof(T);

            ViewMapper.ViewData viewData = _viewMapper[type];
            if (viewData == null)
            {
                Debug.LogError("View of type " + type + " cannot be found in ViewMapper");
                return null;
            }

            T instance = Instantiate((T)viewData.Prefab, _viewLayers[viewData.LayerID]);
            _views.Add(type, instance);

            OrderViewLayer(viewData.LayerID);

            return instance;
        }

        public void UnloadView<T>() where T : View
        {
            Type type = typeof(T);

            if (!_views.ContainsKey(type)) return;

            T instance = (T)_views[type];
            _views.Remove(type);
            Destroy(instance);
        }

        private void OrderViewLayer(string id)
        {
            ViewLayer viewLayer = _viewMapper.ViewLayers.Find(l => l.ID == id);
            Transform viewLayerTransform = _viewLayers[id];

            foreach (View view in viewLayer.Views)
            {
                Type type = view.GetType();

                if (_views.TryGetValue(type, out View value))
                {
                    value.transform.SetAsLastSibling();
                }
            }
        }
    }
}