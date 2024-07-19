using System.Collections.Generic;

namespace Shuile
{
    public static class HashSetExtension
    {
        public static void AddRange<T>(this HashSet<T> set, params T[] items)
        {
            foreach (var item in items)
            {
                set.Add(item);
            }
        }
    }
}
