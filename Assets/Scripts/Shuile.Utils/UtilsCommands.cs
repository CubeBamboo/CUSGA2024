using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Shuile.Utils
{
    public static class UtilsCommands
    {
        public static async void SetTimer(int millisecondsDelay, System.Action action, CancellationToken token = default)
        {
            try
            {
                await UniTask.Delay(millisecondsDelay, cancellationToken: token);
                action();
            }
            catch (OperationCanceledException)
            {
            }
        }

        public static async void SetTimer(double secondsDelay, System.Action action, CancellationToken token = default)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(secondsDelay), cancellationToken: token);
                action();
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
