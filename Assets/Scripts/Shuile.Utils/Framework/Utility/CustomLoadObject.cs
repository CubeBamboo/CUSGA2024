namespace Shuile.Framework
{
    [System.Serializable]
    public class CustomLoadObject<T>
    {
        private T _value;
        private readonly System.Func<T> factory;
        private readonly System.Func<T, bool> check;

        /// <summary> call factoryFunc when checkFunc returns false </summary>
        /// <param name="check"> if is null, check will be set to o => o != null </param>
        public CustomLoadObject(System.Func<T> factory, System.Func<T, bool> check = null)
        {
            this.factory = factory;
            check ??= o => o != null;
            this.check = check;
        }

        public T Value
        {
            get => check(_value) ? _value : _value = factory();
        }

        public T GetValueWithoutEvent() => _value;
        public void SetValueWithoutEvent(T value) => _value = value;
    }
}
