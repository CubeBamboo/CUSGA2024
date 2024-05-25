using CbUtils.Event;
using CbUtils.Extension;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace CbUtils.Timing.Scheduler
{
    public interface ITimerScheduler
    {
        void Schedule(ValueTimerData timerData, bool canBeCancelled = true);
        void Schedule(RefTimerData timerData, bool canBeCancelled = true);
        void StopAllTimer();
    }

    public class MonoBehaviorTimerScheduler : ITimerScheduler
    {
        private readonly UnityEngine.MonoBehaviour _monoBehaviour;
        private float globalTimer = 0f;

        private List<ValueTimerData> valueTimers = new();

        public MonoBehaviorTimerScheduler()
        {
            _monoBehaviour = new UnityEngine.GameObject("CbUtils.Timing.MonoBehaviorTimerScheduler")
                .SetDontDestroyOnLoad()
                .AddComponent<EmptyMonoBehavior>();
            _monoBehaviour.gameObject.GetOrAddComponent<MonoUpdateEventTrigger>().UpdateEvt += OnUpdate;
        }

        private void OnUpdate()
        {
            globalTimer += UnityEngine.Time.deltaTime;

            foreach (var timer in valueTimers)
            {
                if (globalTimer > (float)timer.delay.TotalSeconds)
                {
                    timer.onComplete.SafeInvoke();
                }
            }
        }

        public void Schedule(ValueTimerData timerData, bool canBeCancelled = true)
        {
            
            throw new System.NotImplementedException();
        }

        public void Schedule(RefTimerData timerData, bool canBeCancelled = true)
        {
            throw new System.NotImplementedException();
        }

        public void StopAllTimer()
        {
            throw new System.NotImplementedException();
        }
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

        public void Schedule(RefTimerData timerData, bool canBeCancelled = true)
        {
            throw new System.NotImplementedException();
        }

        public void StopAllTimer()
        {
            globalCts.Cancel();
            globalCts.Dispose();
            globalCts = new();
        }

        private async UniTask InternalSchedule(ValueTimerData timerData, CancellationToken ct = default)
        {
            await UniTask.Delay(timerData.delay, cancellationToken: ct);
            timerData.onComplete();
        }
    }

    /*public class CoroutineTimerScheduler : ITimerScheduler
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
    }*/
}
