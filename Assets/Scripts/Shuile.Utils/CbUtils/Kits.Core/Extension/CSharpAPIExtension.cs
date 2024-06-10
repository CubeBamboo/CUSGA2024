using System.Collections.Generic;

namespace CbUtils.Extension
{
    public static class CSharpAPIExtension
    {
        public static string IEnumerableToString<T>(this IEnumerable<T> self, System.Func<T, string> formatter = null, string title = null, string separator = "\n")
        {
            title ??= typeof(T).Name + ":\n";
            formatter ??= (item) => item.ToString();
            System.Text.StringBuilder sb = new($"{title}");
            foreach (var item in self)
            {
                sb.Append($"{formatter(item)}{separator}");
            }
            return sb.ToString();
        }

        public static void SafeInvoke(this System.Action self, System.Action<System.Exception> onCatch = null)
        {
            if (self == null) return;
            try { self(); } catch (System.Exception e)
            {
                UnityEngine.Debug.LogException(e);
                onCatch?.Invoke(e);
            }
        }
    }

    public static class MathUtils
    {
        public static int RoundToInt(float value) => (int)System.Math.Round(value);
    }

    public static class CollectionsExtension
    {
    }
}
