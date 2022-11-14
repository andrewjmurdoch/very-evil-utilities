namespace VED.Utilities
{
    public class Singleton<T> where T : Singleton<T>, new()
    {
        private static T _instance = null;

        public static T Instance
        {
            get => _instance ??= new T();
            private set => _instance = value;
        }

        protected Singleton()
        {
            if (_instance == null) Instance = (T)this;
        }
    }
}