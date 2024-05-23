using CbUtils.Event;
using CbUtils.Extension;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace CbUtils.Timing.Scheduler
{
    public interface ITimerScheduler
    {
        void Schedule(ValueTimerData timerData, bool canBeCancelled = true);
        void StopAllTimer();
    }

    public class UniTaskTimerScheduler : ITimerScheduler
    {
        private CancellationTokenSource globalCts = new();

        public void Schedule(ValueTimerData timerData)
        {
            InternalSchedule(timerData, globalCts.Token).Forget();
        }

        public void Schedule(ValueTimerData timerData, bool canBeCancelled = true)
        {
            InternalSchedule(timerData, canBeCancelled ? globalCts.Token : default).Forget();
        }

        public void StopAllTimer()
        {
            globalCts.Cancel();
            globalCts.Dispose();
            globalCts = new();
        }

        private async UniTask InternalSchedule(ValueTimerData timerData, CancellationToken ct = default)
        {
            await UniTask.Delay(timerData.millisecondsDelay, cancellationToken: ct);
            timerData.onComplete();
        }
    }

    public class CoroutineTimerScheduler : ITimerScheduler
    {
        private readonly UnityEngine.MonoBehaviour _monoBehaviour;

        public CoroutineTimerScheduler()
        {
            _monoBehaviour = new UnityEngine.GameObject("CbUtils.Timing.CoroutineTimerScheduler")
                .SetDontDestroyOnLoad()
                .AddComponent<EmptyMonoBehavior>();
        }

        public void Schedule(ValueTimerData timerData, bool canBeCancelled = true)
        {
            throw new System.NotImplementedException();
            //_monoBehaviour.StartCoroutine(InternalSchedule(timerData));
        }

        public void StopAllTimer()
        {
            _monoBehaviour.StopAllCoroutines();
        }

        private System.Collections.IEnumerator InternalSchedule(ValueTimerData timerData)
        {
            yield return new UnityEngine.WaitForSeconds(timerData.millisecondsDelay / 1000f);
            timerData.onComplete();
        }
    }
}
