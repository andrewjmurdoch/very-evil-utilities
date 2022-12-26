using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace VED.Utilities
{
    public class ViewManager : SingletonMonoBehaviour<ViewManager>
    {
        public ViewMapper ViewMapper => _viewMapper;
        private ViewMapper _viewMapper = null;

        public ViewMapper CustomViewMapper
        {
            get => _customViewMapper;
            set => _customViewMapper = value;
        }
        private ViewMapper _customViewMapper = null;

        private Dictionary<Type, View> _views = new Dictionary<Type, View>();

        public Dictionary<View.Layers, Transform> Layers => _layers;
        private Dictionary<View.Layers, Transform> _layers = new Dictionary<View.Layers, Transform>();

        private Canvas _canvas = null;

        protected override void Awake()
        {
            base.Awake();

            AsyncOperationHandle<ViewMapper> viewMapperHandle = Addressables.LoadAssetAsync<ViewMapper>(ViewMapper.PATH);
            _viewMapper = viewMapperHandle.WaitForCompletion();

            _canvas = new GameObject("View Canvas").AddComponent<Canvas>();
            _canvas.transform.SetParent(transform);
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.gameObject.AddComponent<GraphicRaycaster>();

            foreach (string name in Enum.GetNames(typeof(View.Layers)))
            {
                RectTransform rectTransform = new GameObject(name).AddComponent<RectTransform>();
                rectTransform.SetParent(_canvas.transform);
                rectTransform.SetAsLastSibling();
                rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                rectTransform.pivot     = new Vector2(0.5f, 0.5f);
                rectTransform.offsetMin = new Vector2(0.0f, 0.0f);
                rectTransform.offsetMax = new Vector2(0.0f, 0.0f);

                _layers.Add(Enum.Parse<View.Layers>(name), rectTransform);
            }
        }

        public T GetView<T>() where T : View
        {
            Type type = typeof(T);

            if (_views.ContainsKey(type)) return (T)_views[type];

            return LoadView<T>();
        }

        public T LoadView<T>() where T : View
        {
            Type type = typeof(T);

            View original = _viewMapper[type];
            original ??= _customViewMapper?[type];
            if (original == null)
            {
                Debug.LogError("View of type " + type + " cannot be found in ViewMapper");
                return null;
            }

            T instance = Instantiate((T)original, _layers[original.Layer]);
            _views.Add(type, instance);

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
    }
}