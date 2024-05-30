using System;

namespace Shuile.Framework
{
    #region GlobalEvent

    public class GlobalEvent<TTarget> where TTarget : GlobalEvent<TTarget>
    {
        static Action _onAction;
        public static void Register(Action action) => _onAction += action;
        public static void UnRegister(Action action) => _onAction -= action;
        public static void Trigger() => _onAction?.Invoke();
        public static void Clear() => _onAction = null;
    }

    public class GlobalEvent<TTarget, T1> where TTarget : GlobalEvent<TTarget, T1>
    {
        static Action<T1> _onAction;
        public static void Register(Action<T1> action) => _onAction += action;
        public static void UnRegister(Action<T1> action) => _onAction -= action;
        public static void Trigger(T1 para1) => _onAction?.Invoke(para1);
        public static void Clear() => _onAction = null;
    }

    public class GlobalEvent<TTarget, T1, T2> where TTarget : GlobalEvent<TTarget, T1, T2>
    {
        static Action<T1, T2> _onAction;
        public static void Register(Action<T1, T2> action) => _onAction += action;
        public static void UnRegister(Action<T1, T2> action) => _onAction -= action;
        public static void Trigger(T1 para1, T2 para2) => _onAction?.Invoke(para1, para2);
        public static void Clear() => _onAction = null;
    }

    #endregion

    #region GlobalEvent

    public class GlobalEvent_AutoClear<TTarget> where TTarget : GlobalEvent_AutoClear<TTarget>
    {
        private static Action _onAction;
        public static void Register(Action action) => _onAction += action;
        public static void UnRegister(Action action) => _onAction -= action;
        public static void Trigger() { _onAction?.Invoke(); Clear(); }
        public static void Clear() => _onAction = null;
    }

    public class GlobalEvent_AutoClear<TTarget, T1> where TTarget : GlobalEvent_AutoClear<TTarget, T1>
    {
        private static Action<T1> _onAction;
        public static void Register(Action<T1> action) => _onAction += action;
        public static void UnRegister(Action<T1> action) => _onAction -= action;
        public static void Trigger(T1 para1) { _onAction?.Invoke(para1); Clear(); }
        public static void Clear() => _onAction = null;
    }

    public class GlobalEvent_AutoClear<TTarget, T1, T2> where TTarget : GlobalEvent_AutoClear<TTarget, T1, T2>
    {
        private static Action<T1, T2> _onAction;
        public static void Register(Action<T1, T2> action) => _onAction += action;
        public static void UnRegister(Action<T1, T2> action) => _onAction -= action;
        public static void Trigger(T1 para1, T2 para2) { _onAction?.Invoke(para1, para2); Clear(); }
        public static void Clear() => _onAction = null;
    }

    #endregion
}
