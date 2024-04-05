using System;
using System.Collections.Generic;

//inspired by qframework
namespace CbUtils
{
    public interface ISimpleEvent
    {
        ICustomUnRegister Register(Action action);
        void UnRegister(Action action);
        void Invoke();
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
    public class SimpleEvent : ISimpleEvent
    {
        private Action _action;
        public ICustomUnRegister Register(Action action)
        {
            _action += action;
            return new CustomUnRegister(() => _action -= action);
        }
        public void UnRegister(Action action) => _action -= action;
        public void Invoke() => _action?.Invoke();
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
        public void Remove(ICustomUnRegister add) => unRegisters.Remove(add);

        private void OnDestroy()
        {
            foreach(var unRegister in unRegisters)
            {
                unRegister.OnDo();
            }
        }
    }
}
