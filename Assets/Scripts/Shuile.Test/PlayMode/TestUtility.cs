using System;
using System.Collections;
using System.Threading.Tasks;

namespace Shuile.Test.PlayMode
{
    public static class TestUtility
    {
        public static IEnumerator YieldWaitTaskEndWithThrow(this Task task, Action<Exception> onException = null)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.IsFaulted)
            {
                var exception = task.Exception.InnerException ?? task.Exception;
                if (onException != null)
                {
                    onException(exception);
                }
                else
                {
                    throw exception;
                }
            }

            yield return null;
        }

        public static IEnumerator Task2Crt(Task task)
        {
            return task.YieldWaitTaskEndWithThrow();
        }

        public static IEnumerator Task2Crt(Func<Task> task)
        {
            return Task2Crt(task());
        }
    }
}
