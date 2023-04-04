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
}