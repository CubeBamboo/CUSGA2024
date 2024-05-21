using System.Threading.Tasks;

namespace Shuile.Utils
{
    public static class Utils
    {
        public static bool IsNull(this object obj) => obj == null;
    }

    public static class UnityUtils
    {
        public static Task<UnityEngine.Object> AsTask(this UnityEngine.ResourceRequest op)
        {
            var tcs = new TaskCompletionSource<UnityEngine.Object>();
            op.completed += operation => tcs.TrySetResult(op.asset);
            return tcs.Task;
        }
        public static Task<T> AsTask<T>(this UnityEngine.ResourceRequest op) where T : UnityEngine.Object
        {
            var tcs = new TaskCompletionSource<T>();
            op.completed += operation => tcs.TrySetResult(op.asset as T);
            return tcs.Task;
        }
    }
}
