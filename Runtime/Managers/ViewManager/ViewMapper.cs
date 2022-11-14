using System;
using System.Collections.Generic;
using UnityEngine;

namespace VED.Utilities
{
    [CreateAssetMenu(fileName = "ViewMapper", menuName = "VED/Utilities/ViewMapper")]
    public class ViewMapper : ScriptableObject
    {
        public const string PATH = "Packages/com.veryevildemons.veryevilutilities/Assets/ViewManager/ViewMapper.asset";

        [SerializeField] private List<View> _views = new List<View>();

        private Dictionary<Type, View> _viewDictionary = new Dictionary<Type, View>();

        public View this[Type type]
        {
            get
            {
                if (!_viewDictionary.ContainsKey(type)) return null;
                return _viewDictionary[type];
            }
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            foreach (View view in _views)
            {
                if (view == null) continue;

                Type type = view.GetType();

                if (_viewDictionary.ContainsKey(type)) continue;

                _viewDictionary.Add(type, view);
            }
        }
#endif
    }
}