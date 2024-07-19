using System;

namespace Shuile.Framework
{
    [Serializable]
    public class CustomLoadObject<T>
    {
        private readonly Func<T, bool> check;
        private readonly Func<T> factory;
        private T _value;

        /// <summary> call factoryFunc when checkFunc returns false </summary>
        /// <param name="check"> if is null, check will be set to o => o != null </param>
        public CustomLoadObject(Func<T> factory, Func<T, bool> check = null)
        {
            this.factory = factory;
            check ??= o => o != null;
            this.check = check;
        }

        public T Value => check(_value) ? _value : _value = factory();

        public T GetValueWithoutEvent()
        {
            return _value;
        }

        public void SetValueWithoutEvent(T value)
        {
            _value = value;
        }
    }
}
