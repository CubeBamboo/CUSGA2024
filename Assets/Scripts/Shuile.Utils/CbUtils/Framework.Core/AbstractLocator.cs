using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Shuile.Framework
{
    public abstract class AbstractLocator<T> where T : new()
    {
        protected Dictionary<Type, object> mContainer = new();

        protected AbstractLocator()
        {
            if (InitOnApplicationAwake)
            {
                OnInit();
            }
        }

        public static T Interface { get; } = new();

        // if true, will call OnInit() when application entered
        public abstract bool InitOnApplicationAwake { get; }
        public abstract void OnInit();
        public abstract void OnDeInit();

        public void Register<S>(S instance)
        {
            mContainer.Add(typeof(S), instance);
        }

        public void UnRegister<S>()
        {
            mContainer.Remove(typeof(S));
        }

        public void UnRegister(object instance)
        {
            mContainer.Remove(instance.GetType());
        }

        [DebuggerHidden]
        public S Get<S>()
        {
            if (!mContainer.TryGetValue(typeof(S), out var res))
            {
                throw new Exception($"instance of type {typeof(S)} not find.");
            }

            return (S)res;
        }

        public bool TryGet<S>(out S res)
        {
            if (!mContainer.TryGetValue(typeof(S), out var val))
            {
                res = default;
                return false;
            }

            res = (S)val;
            return true;
        }

        public void Clear()
        {
            mContainer.Clear();
        }
    }
}
