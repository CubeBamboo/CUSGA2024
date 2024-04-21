using System;
using System.Collections.Generic;

//inspired by qframework
namespace CbUtils
{
    public interface ISimepleEvent
    {
    }

    public interface ICustomUnRegister
    {
        void OnDo();
        public ICustomUnRegister UnRegisterWhenGameObjectDestroyed(UnityEngine.GameObject gameObject)
        {
            if (!gameObject.TryGetComponent<UnRegisterOnDestroyTrigger>(out var trigger))
                trigger = gameObject.AddComponent<UnRegisterOnDestroyTrigger>();
            trigger.Add(this);
            return this;
        }
    }

    /* Example:
     * new SimpleEvent().Register(() => { })
     *                  .UnRegisterWhenGameObjectDestroyed(gameObject);
     * new UnityEvent().AddListenerWithCustomUnRegister(() => { })
     *                 .UnRegisterWhenGameObjectDestroyed(gameObject);
     */
    public class EasyEvent
    {
        private Action _action = ()=> { };
        public ICustomUnRegister Register(Action action)
        {
            _action += action;
            return new CustomUnRegister(() => _action -= action);
        }
        public void UnRegister(Action action) => _action -= action;
        public void Invoke() => _action.Invoke();
    }

    public class SimpleEvent<T>
    {
        private Action<T> _action = _ => { };
        public ICustomUnRegister Register(Action<T> action)
        {
            _action += action;
            return new CustomUnRegister(() => _action -= action);
        }
        public void UnRegister(Action<T> action) => _action -= action;
        public void Invoke(T para1) => _action.Invoke(para1);
    }

    public class SimpleEvent<T1, T2>
    {
        private Action<T1, T2> _action = (_, _) => { };
        public ICustomUnRegister Register(Action<T1, T2> action)
        {
            _action += action;
            return new CustomUnRegister(() => _action -= action);
        }
        public void UnRegister(Action<T1, T2> action) => _action -= action;
        public void Invoke(T1 para1, T2 para2) => _action.Invoke(para1, para2);
    }
    public class SimpleEvent<T1, T2, T3>
    {
        private Action<T1, T2, T3> _action = (_, _, _) => { };
        public ICustomUnRegister Register(Action<T1, T2, T3> action)
        {
            _action += action;
            return new CustomUnRegister(() => _action -= action);
        }
        public void UnRegister(Action<T1, T2, T3> action) => _action -= action;
        public void Invoke(T1 para1, T2 para2, T3 para3) => _action.Invoke(para1, para2, para3);
    }

    public class CustomUnRegister : ICustomUnRegister
    {
        private Action _OnCustomUnRegister;
        public CustomUnRegister(Action onCustomUnRegister) => _OnCustomUnRegister = onCustomUnRegister;

        public void OnDo()
        {
            _OnCustomUnRegister?.Invoke();
            _OnCustomUnRegister = null;
        }
    }

    public class UnRegisterOnDestroyTrigger : UnityEngine.MonoBehaviour
    {
        private HashSet<ICustomUnRegister> unRegisters = new HashSet<ICustomUnRegister>();

        public void Add(ICustomUnRegister add) => unRegisters.Add(add);
        public void Remove(ICustomUnRegister remove) => unRegisters.Remove(remove);

        private void OnDestroy()
        {
            foreach(var unRegister in unRegisters)
            {
                unRegister.OnDo();
            }
        }
    }
}
