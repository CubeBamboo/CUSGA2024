using CbUtils;

namespace Shuile.Framework
{
    /*public class GlobalEventCtrl
    {
        public static void Register<T>(System.Action<T> action)
        {
            //GlobalEventSystem.Instance.Register(action);
        }
        public static void UnRegister<T>(System.Action<T> action)
        {
            //GlobalEventSystem.Instance.UnRegister(action);
        }
        public static void Trigger<T>(T arg)
        {
            //GlobalEventSystem.Instance.Trigger(arg);
            
        }
    }*/

    public abstract class GlobalEvent<TTarget> where TTarget : GlobalEvent<TTarget>
    {
        static System.Action _onAction;
        public static void Register(System.Action action) => _onAction += action;
        public static void UnRegister(System.Action action) => _onAction -= action;
        public static void Trigger() => _onAction?.Invoke();
        public static void Clear() => _onAction = null;
    }

    public abstract class GlobalEvent<TTarget, T1> where TTarget : GlobalEvent<TTarget, T1>
    {
        static System.Action<T1> _onAction;
        public static void Register(System.Action<T1> action) => _onAction += action;
        public static void UnRegister(System.Action<T1> action) => _onAction -= action;
        public static void Trigger(T1 para1) => _onAction?.Invoke(para1);
        public static void Clear() => _onAction = null;
    }

    public abstract class GlobalEvent<TTarget, T1, T2> where TTarget : GlobalEvent<TTarget, T1, T2>
    {
        static System.Action<T1, T2> _onAction;
        public static void Register(System.Action<T1, T2> action) => _onAction += action;
        public static void UnRegister(System.Action<T1, T2> action) => _onAction -= action;
        public static void Trigger(T1 para1, T2 para2) => _onAction?.Invoke(para1, para2);
        public static void Clear() => _onAction = null;
    }
}
