using CbUtils;
using System;

namespace Shuile
{
    public class HearableProperty<T>
    {
        public static Func<T, T, bool> Comparer = (a, b) => a.Equals(b);
        private T _value;

        /// <summary> "T - old value, T - new value" </summary>
        public EasyEvent<T, T> onValueChanged = new();

        public HearableProperty(T value = default)
        {
            _value = value;
        }

        public T Value
        {
            get => _value;
            set
            {
                if (!Comparer(_value, value))
                {
                    onValueChanged.Invoke(_value, value);
                }

                _value = value;
            }
        }

        public ICustomUnRegister Register(Action<T, T> _onValueChanged)
        {
            return onValueChanged.Register(_onValueChanged);
        }

        public void UnRegister(Action<T, T> _onValueChanged)
        {
            onValueChanged.UnRegister(_onValueChanged);
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
