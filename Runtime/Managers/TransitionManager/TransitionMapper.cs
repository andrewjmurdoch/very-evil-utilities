using System;
using System.Collections.Generic;
using UnityEngine;

namespace VED.Utilities
{
    [CreateAssetMenu(fileName = "TransitionMapper", menuName = "VED/Utilities/TransitionMapper")]
    public class TransitionMapper : ScriptableObject
    {
        public const string PATH = "Packages/com.veryevildemons.veryevilutilities/Assets/TransitionManager/TransitionMapper.asset";

        [SerializeField] private List<Transition> _transitions = new List<Transition>();

        private Dictionary<Type, Transition> _transitionDictionary = new Dictionary<Type, Transition>();

        public Transition this[Type type]
        {
            get
            {
                if (!_transitionDictionary.ContainsKey(type)) return null;
                return _transitionDictionary[type];
            }
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            _transitionDictionary.Clear();
            foreach (Transition transition in _transitions)
            {
                if (transition == null) continue;

                Type type = transition.GetType();

                if (_transitionDictionary.ContainsKey(type)) continue;

                _transitionDictionary.Add(type, transition);
            }
        }
#endif
    }
}