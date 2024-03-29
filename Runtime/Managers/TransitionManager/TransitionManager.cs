using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VED.Utilities
{
    public class TransitionManager : SingletonMonoBehaviour<TransitionManager>
    {
        public TransitionMapper TransitionMapper
        {
            get
            {
                return _transitionMapper;
            }
            set
            {
                if (_transitionMapper != null) DeinitTransitionMapper();
                _transitionMapper = value;
            }
        }
        private TransitionMapper _transitionMapper = null;

        private const float DEFAULT_DURATION = 000.000f;
        private const float DEFAULT_TIMEOUT  = 200.000f;

        private TimerRealtime _timer = new TimerRealtime(DEFAULT_DURATION);
        private AwaiterRealtime _awaiter = new AwaiterRealtime(DEFAULT_TIMEOUT);

        private Dictionary<Type, Transition> _transitions = new Dictionary<Type, Transition>();

        private Transform _root = null;

        protected override void Awake()
        {
            base.Awake();

            TransitionView transitionView = ViewManager.Instance.GetView<TransitionView>();
            transitionView.Show();
            _root = transitionView.transform;
        }

        private void DeinitTransitionMapper()
        {
            // destroy all current transitions
            int count = _root.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                Destroy(_root.transform.GetChild(0).gameObject);
            }
        }

        public void Stop()
        {
            foreach (Transition transition in _transitions.Values)
            {
                transition.Stop();
            }
        }

        public void TransitionOut<T>(Action callback = null) where T : Transition
        {
            Stop();

            T transition = GetTransition<T>();

            if (transition == null)
            {
                callback?.Invoke();
                return;
            }

            transition.Out(callback);
        }

        public void TransitionIn<T>(Action callback = null)
            where T : Transition
        {
            Stop();

            T transition = GetTransition<T>();

            if (transition == null)
            {
                callback?.Invoke();
                return;
            }

            transition.In(callback);
        }

        public void Transition<T, U>(Action outCallback = null, Action midCallback = null, Action inCallback = null, float midDuration = 0f)
            where T : Transition
            where U : Transition
        {
            void OutCallback()
            {
                void MidCallback() => TransitionIn<U>(inCallback);

                midCallback += MidCallback;
                _timer.Set(callback: midCallback, duration: midDuration);
            }

            outCallback += OutCallback;
            TransitionOut<T>(outCallback);
        }

        public void Transition<T, U>(Func<bool> inCondition, Action outCallback = null, Action midCallback = null, Action inCallback = null)
            where T : Transition
            where U : Transition
        {
            void OutCallback()
            {
                void MidCallback() => TransitionIn<U>(inCallback);

                midCallback += MidCallback;
                _awaiter.Set(inCondition, midCallback);
            }

            outCallback += OutCallback;
            TransitionOut<T>(outCallback);
        }

        public void TransitionScene<T, U>(string scene, Action outCallback = null, Action midCallback = null, Action inCallback = null, float midDuration = 0f)
            where T : Transition
            where U : Transition
        {
            if (SceneManager.GetSceneByName(scene) == null)
            {
                Debug.LogError("Cannot get scene: " + scene);
            }

            void OutCallback()
            {
                void MidCallback()
                {
                    void SceneLoadedCallback(Scene scene, LoadSceneMode mode)
                    {
                        TransitionIn<U>(inCallback);
                    }

                    SceneManager.sceneLoaded += SceneLoadedCallback;
                    SceneManager.LoadScene(scene);
                }

                midCallback += MidCallback;
                _timer.Set(callback: midCallback, duration: midDuration);
            }

            outCallback += OutCallback;
            TransitionOut<T>(outCallback);
        }

        public T GetTransition<T>() where T : Transition
        {
            Type type = typeof(T);

            if (_transitions.ContainsKey(type)) return (T)_transitions[type];

            return LoadTransition<T>();
        }

        public T LoadTransition<T>() where T : Transition
        {
            Type type = typeof(T);

            if (_transitions.ContainsKey(type)) return (T)_transitions[type];

            Transition original = _transitionMapper[type];
            if (original == null)
            {
                Debug.LogError("Transition of type " + type + " cannot be found in TransitionMapper");
                return null;
            }

            T instance = Instantiate((T)original, _root);
            _transitions.Add(type, instance);

            return instance;
        }

        public void UnloadTransition<T>() where T : Transition
        {
            Type type = typeof(T);

            if (!_transitions.ContainsKey(type)) return;

            T instance = (T)_transitions[type];
            _transitions.Remove(type);
            Destroy(instance);
        }
    }
}