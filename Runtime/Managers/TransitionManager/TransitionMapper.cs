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

        private Dictionary<Type, Transition> _transitionsDictionary = new Dictionary<Type, Transition>();

        public Transition this[Type type]
        {
            get
            {
                if (!_transitionsDictionary.ContainsKey(type)) return null;
                return _transitionsDictionary[type];
            }
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            foreach (Transition transition in _transitions)
            {
                if (transition == null) continue;

                Type type = transition.GetType();

                if (_transitionsDictionary.ContainsKey(type)) continue;

                _transitionsDictionary.Add(type, transition);
            }
        }
#endif
    }
}