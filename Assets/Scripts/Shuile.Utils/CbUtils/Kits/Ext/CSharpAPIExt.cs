namespace CbUtils.Extension
{
    public static class CSharpAPIExt
    {
        public static string ArrayToString<T>(this T[] self, System.Func<T, string> formatter = null, string title = null, string separator = "\n")
        {
            title ??= typeof(T).Name;
            formatter ??= (item) => item.ToString();
            System.Text.StringBuilder sb = new($"{title}:\n");
            foreach (var item in self)
            {
                sb.Append($"{formatter(item)}{separator}");
            }
            return sb.ToString();
        }
    }
}
