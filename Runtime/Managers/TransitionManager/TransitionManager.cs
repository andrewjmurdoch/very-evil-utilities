using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace VED.Utilities
{
    public class TransitionManager : SingletonMonoBehaviour<TransitionManager>
    {
        public TransitionMapper TransitionMapper => _transitionMapper;
        private TransitionMapper _transitionMapper = null;

        private const float DEFAULT_DURATION = 0.5f;
        private const float DEFAULT_TIMEOUT = 200f;

        private Timer _timer = null;
        private Awaiter _awaiter = null;

        private Dictionary<Type, Transition> _transitions = new Dictionary<Type, Transition>();

        private Canvas _canvas = null;

        protected override void Awake()
        {
            base.Awake();

            // load via addressables physics manager settings
            AsyncOperationHandle<TransitionMapper> transitionMapperHandle = Addressables.LoadAssetAsync<TransitionMapper>(TransitionMapper.PATH);
            _transitionMapper = transitionMapperHandle.WaitForCompletion();

            _timer = new Timer(DEFAULT_DURATION);
            _awaiter = new Awaiter(DEFAULT_TIMEOUT);
            _canvas  = ViewManager.Instance.GetView<TransitionView>().Canvas;
        }

        public void Stop()
        {
            foreach (Transition transition in _transitions.Values)
            {
                transition.Stop();
            }
        }

        public void TransitionOut<T>(Action callback = null, float duration = DEFAULT_DURATION) where T : Transition
        {
            Stop();

            T transition = GetTransition<T>();

            if (transition == null)
            {
                callback?.Invoke();
                return;
            }

            transition.Out(callback, duration);
        }

        public void TransitionIn<T>(Action callback = null, float duration = DEFAULT_DURATION)
            where T : Transition
        {
            Stop();

            T transition = GetTransition<T>();

            if (transition == null)
            {
                callback?.Invoke();
                return;
            }

            transition.In(callback, duration);
        }

        public void Transition<T, U>(Action outCallback = null, Action midCallback = null, Action inCallback = null, float outDuration = DEFAULT_DURATION, float midDuration = 0f, float inDuration = DEFAULT_DURATION)
            where T : Transition
            where U : Transition
        {
            void OutCallback()
            {
                void MidCallback() => TransitionIn<U>(inCallback, inDuration);

                midCallback += MidCallback;
                _timer.Set(callback: midCallback, duration: midDuration);
            }

            outCallback += OutCallback;
            TransitionOut<T>(outCallback, outDuration);
        }

        public void Transition<T, U>(Func<bool> inCondition, Action outCallback = null, Action midCallback = null, Action inCallback = null, float outDuration = DEFAULT_DURATION, float inDuration = DEFAULT_DURATION)
            where T : Transition
            where U : Transition
        {
            void OutCallback()
            {
                void MidCallback() => TransitionIn<U>(inCallback, inDuration);

                midCallback += MidCallback;
                _awaiter.Set(inCondition, midCallback);
            }

            outCallback += OutCallback;
            TransitionOut<T>(outCallback, outDuration);
        }

        public void TransitionScene<T, U>(string scene, Action outCallback = null, Action midCallback = null, Action inCallback = null, float outDuration = DEFAULT_DURATION, float midDuration = 0f, float inDuration = DEFAULT_DURATION)
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
                        TransitionIn<U>(inCallback, inDuration);
                    }

                    SceneManager.sceneLoaded += SceneLoadedCallback;
                    SceneManager.LoadScene(scene);
                }

                midCallback += MidCallback;
                _timer.Set(callback: midCallback, duration: midDuration);
            }

            outCallback += OutCallback;
            TransitionOut<T>(outCallback, outDuration);
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

            Transition original = _transitionMapper[type];
            if (original == null)
            {
                Debug.LogError("Transition of type " + type + " cannot be found in TransitionMapper");
                return null;
            }

            T instance = Instantiate((T)original, _canvas.transform);
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