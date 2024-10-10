using System;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Shuile.Utils
{
    public static class Utils
    {
        public static bool IsNull(this object obj)
        {
            return obj == null;
        }
    }

    public static class UnityUtils
    {
        public static Task<Object> AsTask(this ResourceRequest op)
        {
            var tcs = new TaskCompletionSource<Object>();
            op.completed += operation => tcs.TrySetResult(op.asset);
            return tcs.Task;
        }

        public static Task<T> AsTask<T>(this ResourceRequest op) where T : Object
        {
            var tcs = new TaskCompletionSource<T>();
            op.completed += operation => tcs.TrySetResult(op.asset as T);
            return tcs.Task;
        }
    }

    public static class ExceptionUtils
    {
        public static void UnityCatch(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Debug.LogError($"Some exception were captured: {e}");
            }
        }

        public static Task Catch(this Task task, Action<Exception> action)
        {
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    action(t.Exception);
                }
            });
            return task;
        }

        public static void ThrowIfObjectNull(object obj, string message = null)
        {
            if (obj == null)
            {
                throw new NullReferenceException(message);
            }
        }

        /// <summary>
        /// simplify the null check
        /// </summary>
        public static T ThrowIfNull<T>(this T obj, string message = null)
        {
            if (obj == null)
            {
                throw new NullReferenceException(message);
            }

            return obj;
        }
    }
}
