using CbUtils.Event;
using CbUtils.Extension;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace CbUtils.Timing.Scheduler
{
    public interface ITimerScheduler
    {
        void Schedule(ValueTimerData timerData, bool canBeCancelled = true);
        void StopAllTimer();
    }

    public class MonoBehaviorTimerScheduler : ITimerScheduler
    {
        private readonly MonoBehaviour _monoBehaviour;
        private readonly List<RefTimerData> refTimerList = new();

        private readonly List<ValueTimerData> valueTimerList = new();
        private float baseTimer;

        public MonoBehaviorTimerScheduler()
        {
            _monoBehaviour = new GameObject("Framework.Timing.MonoBehaviorTimerScheduler")
                .SetDontDestroyOnLoad()
                .AddComponent<EmptyMonoBehavior>();
            _monoBehaviour.gameObject.GetOrAddComponent<MonoUpdateEventTrigger>().UpdateEvt += OnUpdate;
        }

        public void Schedule(ValueTimerData timerData, bool canBeCancelled = true)
        {
            valueTimerList.Add(timerData);
        }

        public void StopAllTimer()
        {
            valueTimerList.Clear();
            refTimerList.Clear();
        }

        ~MonoBehaviorTimerScheduler()
        {
            _monoBehaviour.gameObject.GetOrAddComponent<MonoUpdateEventTrigger>().UpdateEvt -= OnUpdate;
        }

        private void OnUpdate()
        {
            baseTimer += Time.deltaTime;

            foreach (var timer in valueTimerList)
            {
                if (baseTimer > (float)timer.delay.TotalSeconds)
                {
                    timer.onComplete.SafeInvoke();
                    valueTimerList.Remove(timer);
                }
            }

            foreach (var timer in refTimerList)
            {
                if (baseTimer > (float)timer.delay.TotalSeconds)
                {
                    timer.onComplete.SafeInvoke();
                    refTimerList.Remove(timer);
                }
            }
        }

        public void Schedule(RefTimerData timerData, bool canBeCancelled = true)
        {
            refTimerList.Add(timerData);
        }
    }

    public class UniTaskTimerScheduler : ITimerScheduler
    {
        private CancellationTokenSource globalCts = new();

        public void Schedule(ValueTimerData timerData, bool canBeCancelled = true)
        {
            InternalSchedule(timerData, canBeCancelled ? globalCts.Token : default).Forget();
        }

        public void StopAllTimer()
        {
            globalCts.Cancel();
            globalCts.Dispose();
            globalCts = new CancellationTokenSource();
        }

        public void Schedule(ValueTimerData timerData)
        {
            InternalSchedule(timerData, globalCts.Token).Forget();
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
        private List<Coroutine> valueTimerCoroutines = new();

        public CoroutineTimerScheduler()
        {
            _monoBehaviour = new UnityEngine.GameObject("CbUtils.Timing.CoroutineTimerScheduler")
                .SetDontDestroyOnLoad()
                .AddComponent<EmptyMonoBehavior>();
        }

        public void Schedule(ValueTimerData timerData, bool canBeCancelled = true)
        {
            _monoBehaviour.StartCoroutine(InternalSchedule(timerData));
        }

        public void Schedule(RefTimerData timerData, bool canBeCancelled = true)
        {
            throw new System.NotImplementedException();
        }

        public void StopAllTimer()
        {
            _monoBehaviour.StopAllCoroutines();
        }

        private System.Collections.IEnumerator InternalSchedule(ValueTimerData timerData)
        {
            yield return new UnityEngine.WaitForSeconds((float)timerData.delay.TotalSeconds);
            timerData.onComplete.SafeInvoke();
        }
    }*/
}
