namespace Shuile.Gameplay.Feel
{
    [System.Obsolete("dont use")]
    public class ViewEntityProperty<T>
    {
        private bool _isDirty;
        private bool InternalCheckDirty(T RawValue, T TargetValue) => EnableDirtyCheck && DirtyCheck(RawValue, TargetValue);
        private System.Func<T, T> onValueDirty = v => v;
        private T targetValue;

        public bool EnableDirtyCheck { get; set; } = true;
        public System.Func<T, T, bool> DirtyCheck { get; set; } = (a, b) => !a.Equals(b);
        /// <summary> "recive T as target value, return T as new Target Value" </summary>
        public void OnValueDirty(System.Func<T, T> func) => onValueDirty = func;

        public T RawValue { get; private set; } = default;
        public T TargetValue
        {
            get => targetValue;
            set
            {
                targetValue = value;
                if (InternalCheckDirty(RawValue, targetValue))
                {
                    _isDirty = true;
                    TryUpdateDirtValue();
                }
            }
        }

        public bool TryUpdateDirtValue()
        {
            if (!_isDirty) return false;
            RawValue = onValueDirty(TargetValue);
            _isDirty = InternalCheckDirty(RawValue, TargetValue);
            return true;
        }

        public override string ToString() => RawValue.ToString();
        public ViewEntityProperty(T value = default) => RawValue = value;

    }
}
