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

    public class GlobalEvent
    {
        static System.Action _onAction;

        public static void Register(System.Action action)
        {
            _onAction += action;
        }
        public static void UnRegister(System.Action action)
        {
            _onAction -= action;
        }
        public static void Trigger()
        {
            _onAction?.Invoke();
        }
        public static void Clear()
        {
            _onAction = null;
        }
    }
}
