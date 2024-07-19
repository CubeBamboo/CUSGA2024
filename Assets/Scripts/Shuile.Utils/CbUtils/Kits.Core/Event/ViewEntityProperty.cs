using System;

namespace Shuile.Gameplay.Feel
{
    [Obsolete("dont use")]
    public class ViewEntityProperty<T>
    {
        private bool _isDirty;
        private Func<T, T> onValueDirty = v => v;
        private T targetValue;

        public ViewEntityProperty(T value = default)
        {
            RawValue = value;
        }

        public bool EnableDirtyCheck { get; set; } = true;
        public Func<T, T, bool> DirtyCheck { get; set; } = (a, b) => !a.Equals(b);

        public T RawValue { get; private set; }

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

        private bool InternalCheckDirty(T RawValue, T TargetValue)
        {
            return EnableDirtyCheck && DirtyCheck(RawValue, TargetValue);
        }

        /// <summary> "recive T as target value, return T as new Target Value" </summary>
        public void OnValueDirty(Func<T, T> func)
        {
            onValueDirty = func;
        }

        public bool TryUpdateDirtValue()
        {
            if (!_isDirty)
            {
                return false;
            }

            RawValue = onValueDirty(TargetValue);
            _isDirty = InternalCheckDirty(RawValue, TargetValue);
            return true;
        }

        public override string ToString()
        {
            return RawValue.ToString();
        }
    }
}
