using System;

namespace VED.Utilities
{
    public class Toggler<T>
    {
        protected T _one;
        protected T _two;

        public T Value => _value;
        protected T _value;

        public T Other => _value.Equals(_one) ? _two : _one;

        public Action<T> OnToggle = null;

        public Toggler(T one, T two)
        {
            _one = one;
            _two = two;

            _value = one;
        }

        public void Toggle()
        {
            _value = Other;

            OnToggle?.Invoke(_value);
        }
    }
}