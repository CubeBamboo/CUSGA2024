using CbUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile
{
    public class HearableProperty<T>
    {
        public HearableProperty(T value = default) => _value = value;
        public static Func<T, T, bool> Comparer = (a, b) => a.Equals(b);
        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if(!Comparer(_value, value))
                    onValueChanged.Invoke(_value, value);
                _value = value;
            }
        }

        /// <summary> "T - old value, T - new value" </summary>
        public SimpleEvent<T, T> onValueChanged = new();
        public ICustomUnRegister Register(Action<T, T> _onValueChanged) => onValueChanged.Register(_onValueChanged);
        public void UnRegister(Action<T, T> _onValueChanged) => onValueChanged.UnRegister(_onValueChanged);
        public override string ToString() => _value.ToString();
    }
}
