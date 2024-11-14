using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CbUtils.Extension
{
    public static class CSharpAPIExtension
    {
        public static string IEnumerableToString<T>(this IEnumerable<T> self, Func<T, string> formatter = null,
            string title = null, string separator = "\n")
        {
            title ??= typeof(T).Name + ":\n";
            formatter ??= item => item.ToString();
            StringBuilder sb = new($"{title}");
            foreach (var item in self)
            {
                sb.Append($"{formatter(item)}{separator}");
            }

            return sb.ToString();
        }

        public static void SafeInvoke(this Action self, Action<Exception> onCatch = null)
        {
            if (self == null)
            {
                return;
            }

            try { self(); }
            catch (Exception e)
            {
                Debug.LogException(e);
                onCatch?.Invoke(e);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static T With<T>(this T self, Action<T> action)
        {
            action(self);
            return self;
        }
    }

    public static class MathUtils
    {
        public static int RoundToInt(float value)
        {
            return (int)Math.Round(value);
        }
    }

    public static class CollectionsExtension
    {
    }
}
