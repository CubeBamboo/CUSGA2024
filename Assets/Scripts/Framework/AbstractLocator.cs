using System;
using System.Collections.Generic;

namespace Shuile.Framework
{
    // easy ioc container
    public abstract class AbstractLocator<T> where T : new()
    {
        private static T mInterface = new();
        public static T Interface => mInterface;

        protected Dictionary<Type, object> mContainer = new();

        protected AbstractLocator()
        {
            OnInit();
        }

        protected abstract void OnInit();

        public void Register<S>(S instance) => mContainer.Add(typeof(S), instance);

        public void UnRegister<S>() => mContainer.Remove(typeof(S));

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
