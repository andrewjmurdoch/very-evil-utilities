using System;
using System.Collections.Generic;
using UnityEngine;

namespace VED.Utilities
{
    [CreateAssetMenu(fileName = "TransitionMapper", menuName = "VED/Utilities/TransitionMapper")]
    public class TransitionMapper : ScriptableObject
    {
        [SerializeField] private List<Transition> _transitions = new List<Transition>();

        private Dictionary<Type, Transition> _transitionTypeDictionary = null;

        public Transition this[Type type]
        {
            get
            {
                if (_transitionTypeDictionary == null) InitTransitionTypeDictionary();
                if (!_transitionTypeDictionary.ContainsKey(type)) return null;
                return _transitionTypeDictionary[type];
            }
        }

        private void InitTransitionTypeDictionary()
        {
            _transitionTypeDictionary = new Dictionary<Type, Transition>();

            foreach (Transition transition in _transitions)
            {
                Type type = transition.GetType();

                if (_transitionTypeDictionary.ContainsKey(type)) continue;

                _transitionTypeDictionary.Add(type, transition);
            }
        }
    }
}