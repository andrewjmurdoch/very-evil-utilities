using Gooey;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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

        public Dictionary<string, Goo> ViewLayers => _viewLayers;
        private Dictionary<string, Goo> _viewLayers = new Dictionary<string, Goo>();

        public Goo Goo => _goo;
        private Goo _goo = null;

        public Camera Camera => _camera;
        private Camera _camera = null;

        private Canvas _canvas = null;

        public string Layer
        {
            get => _layer;

            set
            {
                _layer = value;

                _camera.cullingMask = 1 << LayerMask.NameToLayer(_layer);
                _goo.gameObject.layer = LayerMask.NameToLayer(_layer);

                View[] views = _views.Values.ToArray();
                for (int i = 0; i < views.Length; i++)
                {
                    views[i].gameObject.layer = LayerMask.NameToLayer(_layer);
                }
            }
        }
        public string _layer = "UI";

        protected override void Awake()
        {
            base.Awake();

            _camera = new GameObject("View Camera").AddComponent<Camera>();
            _camera.transform.SetParent(transform);
            _camera.transform.position = new Vector3(0f, 0f, -10f);
            _camera.backgroundColor = Color.clear;
            _camera.orthographic = true;
            _camera.cullingMask = 1 << LayerMask.NameToLayer(_layer);
            _camera.clearFlags = CameraClearFlags.Nothing;
            _camera.depth = 100;

            UniversalAdditionalCameraData cameraData = _camera.GetUniversalAdditionalCameraData();
            cameraData.renderType = CameraRenderType.Overlay;
            cameraData.renderPostProcessing = true;

            _goo = new GameObject("View Goo").AddComponent<Goo>();
            _goo.RectTransform.SetParent(transform, false);
            _goo.gameObject.layer = LayerMask.NameToLayer(_layer);

            _canvas = _goo.gameObject.AddComponent<Canvas>();
            _canvas.worldCamera = _camera;
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;

            Camera cameraMain = Camera.main;
            if (cameraMain == null )
            {
                Debug.Log("Ensure there exists a camera tagged 'MainCamera' at time of initializing ViewManager");
                return;
            }

            UniversalAdditionalCameraData cameraDataMain = cameraMain.GetUniversalAdditionalCameraData();
            cameraDataMain.cameraStack.Add(_camera);
        }

        public void InitCamera(Camera camera)
        {
            if (_camera != null)
            {
                Camera cameraMain = Camera.main;
                if (cameraMain == null)
                {
                    Debug.Log("Ensure there exists a camera tagged 'MainCamera' at time of initializing ViewManager");
                    return;
                }
                UniversalAdditionalCameraData cameraDataMain = cameraMain.GetUniversalAdditionalCameraData();
                cameraDataMain.cameraStack.Remove(_camera);
                Destroy(_camera.gameObject);
            }

            _camera = camera;
            _canvas.worldCamera = _camera;
        }

        private void InitViewMapper(ViewMapper viewMapper)
        {
            _viewMapper = viewMapper;

            int count = 0;
            for (int i = 0; i < _viewMapper.ViewLayers.Count; i++)
            {
                ViewLayer viewLayer = _viewMapper.ViewLayers[i];

                if (i > 0) count += _viewMapper.ViewLayers[i - 1].Views.Count + 1;

                Goo goo = new GameObject(viewLayer.ID).AddComponent<Goo>();
                RectTransform gooTransform = goo.RectTransform;

                gooTransform.SetParent(_goo.RectTransform, false);
                gooTransform.SetAsLastSibling();

                goo.SizeHorizontalValue = new Value(Gooey.ValueType.PERCENTAGE, 100f);
                goo.SizeVerticalValue   = new Value(Gooey.ValueType.PERCENTAGE, 100f);
                goo.gameObject.layer    = LayerMask.NameToLayer(viewLayer.SeparateCamera ? viewLayer.ID : _layer);

                _viewLayers.Add(viewLayer.ID, goo);

                if (!viewLayer.SeparateCamera) continue;
                InitViewLayerCamera(viewLayer);
            }
        }

        private void InitViewLayerCamera(ViewLayer viewLayer)
        {
            Camera camera = new GameObject(viewLayer.ID + " Camera").AddComponent<Camera>();
            camera.transform.SetParent(transform, false);
            camera.transform.position = new Vector3(0f, 0f, -10f);
            camera.backgroundColor = Color.clear;
            camera.orthographic = true;
            camera.cullingMask = 1 << LayerMask.NameToLayer(viewLayer.ID);
            camera.clearFlags = CameraClearFlags.Nothing;
            camera.depth = 100;

            UniversalAdditionalCameraData cameraTransitionData = camera.GetUniversalAdditionalCameraData();
            cameraTransitionData.renderType = CameraRenderType.Overlay;
            cameraTransitionData.renderPostProcessing = true;

            Camera cameraMain = Camera.main;
            if (cameraMain == null)
            {
                Debug.Log("Ensure there exists a camera tagged 'MainCamera' at time of initializing ViewMapper");
                return;
            }

            UniversalAdditionalCameraData cameraDataMain = cameraMain.GetUniversalAdditionalCameraData();
            cameraDataMain.cameraStack.Add(camera);

            _camera.transform.SetAsFirstSibling();
            _goo.RectTransform.SetAsLastSibling();
        }

        private void DeinitViewMapper()
        {
            // destroy all current view layers
            for (int i = 0; i < _goo.transform.childCount; i++)
            {
                Destroy(_goo.transform.GetChild(0).gameObject);
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

            if (_views.ContainsKey(type)) return (T)_views[type];

            ViewMapper.ViewData viewData = _viewMapper[type];
            if (viewData == null)
            {
                Debug.LogError("View of type " + type + " cannot be found in ViewMapper");
                return null;
            }

            T instance = Instantiate((T)viewData.Prefab, _viewLayers[viewData.LayerID].transform);
            instance.Init();
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
                }
            }
        }
    }
}