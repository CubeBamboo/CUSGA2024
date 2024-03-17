using System.Collections.Generic;

namespace Shuile
{
    public static class ListExtension
    {
        public static void UnorderedRemoveAt<T>(this List<T> list, int index)
        {
            // out of bound 就 out of bound 吧，不用自己检查了，list内部会检查
            list[index] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);  // 上面exception了这里也执行不到
        }

        public static void UnorderedRemove<T>(this List<T> list, T element)
        {
            var index = list.IndexOf(element);
            if (index == -1)
                return;
            UnorderedRemoveAt(list, index);
        }
    }
}
