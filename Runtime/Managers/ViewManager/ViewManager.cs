using Nova;
using System;
using System.Collections.Generic;
using UnityEngine;

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

        public Dictionary<string, UIBlock2D> ViewLayers => _viewLayers;
        private Dictionary<string, UIBlock2D> _viewLayers = new Dictionary<string, UIBlock2D>();

        public UIBlock2D UIBlock2D => _uiBlock2D;
        private UIBlock2D _uiBlock2D = null;

        public Camera Camera => _camera;
        private Camera _camera = null;

        private ScreenSpace _screenSpace = null;

        protected override void Awake()
        {
            base.Awake();

            _camera = new GameObject("View Camera").AddComponent<Camera>();
            _camera.transform.SetParent(transform);
            _camera.transform.position = new Vector3(0f, 0f, -10f);
            _camera.backgroundColor = Color.black;
            _camera.orthographic = true;
            _camera.cullingMask = 1 << LayerMask.NameToLayer("UI");
            _camera.clearFlags = CameraClearFlags.Nothing;
            _camera.depth = 100;

            _uiBlock2D = new GameObject("View UIBlock2D").AddComponent<UIBlock2D>();
            _uiBlock2D.transform.SetParent(transform);
            _uiBlock2D.BodyEnabled = false;
            _uiBlock2D.GameObjectLayer = LayerMask.NameToLayer("UI");

            _screenSpace = _uiBlock2D.gameObject.AddComponent<ScreenSpace>();
            _screenSpace.TargetCamera = _camera;
            _screenSpace.Mode = ScreenSpace.FillMode.MatchCameraResolution;
        }

        private void InitViewMapper(ViewMapper viewMapper)
        {
            _viewMapper = viewMapper;

            int count = 0;
            for (int i = 0; i < _viewMapper.ViewLayers.Count; i++)
            {
                ViewLayer viewLayer = _viewMapper.ViewLayers[i];

                if (i > 0) count += _viewMapper.ViewLayers[i - 1].Views.Count + 1;

                UIBlock2D uiBlock2D = new GameObject(viewLayer.ID).AddComponent<UIBlock2D>();
                Transform uiBlock2DTransform = uiBlock2D.transform;

                uiBlock2DTransform.SetParent(_uiBlock2D.transform);
                uiBlock2DTransform.SetAsLastSibling();
                uiBlock2D.ZIndex = (short)count;
                uiBlock2D.BodyEnabled = false;
                uiBlock2D.Size.Percent = new Vector3(1f, 1f, 1f);
                uiBlock2D.GameObjectLayer = LayerMask.NameToLayer("UI");

                _viewLayers.Add(viewLayer.ID, uiBlock2D);
            }
        }

        private void DeinitViewMapper()
        {
            // destroy all current view layers
            for (int i = 0; i < _uiBlock2D.transform.childCount; i++)
            {
                Destroy(_uiBlock2D.transform.GetChild(0).gameObject);
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

            T instance = Instantiate((T)viewData.Prefab, _viewLayers[viewData.LayerID].transform);
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
            ViewLayer viewLayer = null;
            int count = 1;
            for (int i = 0; i < _viewMapper.ViewLayers.Count; i++)
            {
                if (_viewMapper.ViewLayers[i].ID == id)
                {
                    viewLayer = _viewMapper.ViewLayers[i];
                    break;
                }

                count += _viewMapper.ViewLayers[i].Views.Count + 1;
            }

            for (int i = 0; i < viewLayer.Views.Count; i++)
            { 
                Type type = viewLayer.Views[i].GetType();

                if (_views.TryGetValue(type, out View view))
                {
                    Transform viewTransform = view.transform;
                    viewTransform.SetAsLastSibling();
                    view.UIBlock2D.ZIndex = (short)(count + i);
                }
            }
        }
    }
}