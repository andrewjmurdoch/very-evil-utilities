using UnityEngine;

namespace VED.Utilities
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        public new bool DontDestroyOnLoad = true;
        private static T _instance = null;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                
                T inScene = FindAnyObjectByType<T>();
                if (inScene != null)
                {
                    _instance = inScene;
                    return _instance;
                }

                _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                return _instance;
            }

            protected set { _instance = value; }
        }

        protected virtual void Awake()
        {
            if (_instance != null)
            {
                if (_instance == this) return;
                DestroyImmediate(gameObject);
                return;
            }

            _instance = GetComponent<T>();

            ResetTransform();

            if (DontDestroyOnLoad)
            {
                Transform target = transform;
                while (target.parent != null)
                {
                    target = target.parent;
                }

                DontDestroyOnLoad(target); 
            }
        }

        private void ResetTransform()
        {
            if (GameManager.Instance != _instance) transform.SetParent(GameManager.Instance.transform);
            transform.position   = Vector3.zero;
            transform.rotation   = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
    }
}