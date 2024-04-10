using CbUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile
{
    public class EventProperty<T>
    {
        public EventProperty(T value = default) => _value = value;
        public static Func<T, T, bool> Comparer = (a, b) => a.Equals(b);
        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if(!Comparer(_value, value))
                    onValueChanged.Invoke(value);
                _value = value;
            }
        }
        public SimpleEvent<T> onValueChanged = new();
        public ICustomUnRegister Register(Action<T> _onValueChanged) => onValueChanged.Register(_onValueChanged);
        public void UnRegister(Action<T> _onValueChanged) => onValueChanged.UnRegister(_onValueChanged);
        public override string ToString() => _value.ToString();
    }
}
