using System;
using System.Collections.Generic;
using UnityEngine;

namespace VED.Utilities
{
    [CreateAssetMenu(fileName = "ViewMapper", menuName = "VED/Utilities/ViewMapper")]
    public class ViewMapper : ScriptableObject
    {
        public class ViewData
        {
            public ViewData(string layerID, int layerOrder, View prefab)
            {
                _layerID = layerID;
                _layerOrder = layerOrder;
                _prefab = prefab;
            }

            public string LayerID => _layerID;
            private string _layerID = string.Empty;

            public int LayerOrder => _layerOrder;
            private int _layerOrder = 0;

            public View Prefab => _prefab;
            private View _prefab = null;
        }

        public const string PATH = "Packages/com.veryevildemons.veryevilutilities/Assets/ViewManager/ViewMapper.asset";

        public List<ViewLayer> ViewLayers => _viewLayers;
        [SerializeField] private List<ViewLayer> _viewLayers = new List<ViewLayer>();

        private Dictionary<Type, ViewData> _viewTypeDictionary = new Dictionary<Type, ViewData>();

        public ViewData this[Type type]
        {
            get
            {
                if (!_viewTypeDictionary.ContainsKey(type)) return null;
                return _viewTypeDictionary[type];
            }
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            _viewTypeDictionary.Clear();

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
#endif
    }
}