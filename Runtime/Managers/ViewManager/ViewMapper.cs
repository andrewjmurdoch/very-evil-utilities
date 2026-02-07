using System;
using System.Collections.Generic;
using UnityEngine;
using Gooey;
using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VED.Utilities
{
    [CreateAssetMenu(fileName = "ViewMapper", menuName = "VED/Utilities/ViewMapper")]
    public class ViewMapper : ScriptableObject
    {
        public class ViewData
        {
            public ViewData(string layerID, int layerOrder, View prefab)
            {
                _layerID    = layerID;
                _layerOrder = layerOrder;
                _prefab     = prefab;
            }

            public  string  LayerID => _layerID;
            private string _layerID = string.Empty;

            public  int  LayerOrder => _layerOrder;
            private int _layerOrder = 0;

            public  View  Prefab => _prefab;
            private View _prefab = null;
        }

        public List<ViewLayer> ViewLayers => _viewLayers;
        [SerializeField] private List<ViewLayer> _viewLayers = new List<ViewLayer>();

        [SerializeField] private Dictionary<Type, ViewData> _viewTypeDictionary = null;

        public ViewData this[Type type]
        {
            get
            {
                if (_viewTypeDictionary == null) InitViewTypeDictionary();
                if (_viewTypeDictionary.TryGetValue(type, out ViewData viewData)) return viewData;
                return null;
            }
        }

        private void InitViewTypeDictionary()
        {
            _viewTypeDictionary = new Dictionary<Type, ViewData>();

            foreach (ViewLayer viewLayer in _viewLayers)
            {
                int order = 0;
                foreach (View view in viewLayer.Views)
                {
                    if (view == null) continue;

                    Type type = view.GetType();

                    if (_viewTypeDictionary.ContainsKey(type)) continue;

                    _viewTypeDictionary.Add(type, new ViewData(viewLayer.ID, order, view));
                    order++;
                }
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ViewMapper))]
    public class EditorViewMapper : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);

            if (GUILayout.Button("Create Instance"))
                CreateInstance();
        }

        private void CreateInstance()
        {

            Goo goo = InitViewManager();
            InitViewLayers(goo);
        }

        private Goo InitViewManager()
        {
            Transform transform = new GameObject("ViewManager Instance").transform;
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;

            Camera camera = new GameObject("View Camera").AddComponent<Camera>();
            camera.transform.SetParent(transform);
            camera.transform.position = new Vector3(0f, 0f, -10f);
            camera.backgroundColor = Color.clear;
            camera.orthographic = true;
            camera.cullingMask = 1 << LayerMask.NameToLayer("UI");
            camera.clearFlags = CameraClearFlags.Nothing;
            camera.depth = 100;

            UniversalAdditionalCameraData cameraData = camera.GetUniversalAdditionalCameraData();
            cameraData.renderType = CameraRenderType.Overlay;
            cameraData.renderPostProcessing = true;

            Goo goo = new GameObject("View Goo").AddComponent<Goo>();
            goo.RectTransform.SetParent(transform, false);
            goo.gameObject.layer = LayerMask.NameToLayer("UI");

            Canvas canvas = goo.gameObject.AddComponent<Canvas>();
            canvas.worldCamera = camera;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;

            Camera cameraMain = Camera.main;
            if (cameraMain == null )
            {
                Debug.Log("Ensure there exists a camera tagged 'MainCamera' at time of initializing ViewManager");
                return goo;
            }

            UniversalAdditionalCameraData cameraDataMain = cameraMain.GetUniversalAdditionalCameraData();
            cameraDataMain.cameraStack.Add(camera);

            return goo;
        }

        private void InitViewLayers(Goo gooViewManager)
        {
            ViewMapper viewMapper = target as ViewMapper;
            
            int count = 0;
            for (int i = 0; i < viewMapper.ViewLayers.Count; i++)
            {
                ViewLayer viewLayer = viewMapper.ViewLayers[i];

                if (i > 0)
                    count += viewMapper.ViewLayers[i - 1].Views.Count + 1;

                Goo gooViewLayer = new GameObject(viewLayer.ID).AddComponent<Goo>();
                RectTransform gooTransform = gooViewLayer.RectTransform;

                gooTransform.SetParent(gooViewManager.RectTransform, false);
                gooTransform.SetAsLastSibling();

                gooViewLayer.SizeHorizontalValue = new Value(Gooey.ValueType.PERCENTAGE, 100f);
                gooViewLayer.SizeVerticalValue   = new Value(Gooey.ValueType.PERCENTAGE, 100f);
                gooViewLayer.gameObject.layer    = LayerMask.NameToLayer("UI");

                for (int j = 0; j < viewLayer.Views.Count; j++)
                    PrefabUtility.InstantiatePrefab(viewLayer.Views[j], gooViewLayer.RectTransform);
            }
        }

    }
#endif
}