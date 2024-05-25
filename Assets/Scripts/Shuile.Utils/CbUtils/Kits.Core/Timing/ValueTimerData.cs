using System;

namespace CbUtils.Timing
{
    public struct ValueTimerData
    {
        public TimeSpan delay { get; private set; }
        public Action onComplete { get; private set; }

        public static ValueTimerData Create(double secondsDelay, Action onComplete) => new()
        {
            delay = TimeSpan.FromSeconds(secondsDelay),
            onComplete = onComplete
        };
        public static ValueTimerData Create(TimeSpan delay, Action onComplete) => new()
        {
            delay = delay,
            onComplete = onComplete
        };
    }

    public static class ValueTimerDataExtension
    {
        /// <summary> multiple call is ok. </summary>
        public static ValueTimerData Start(this ValueTimerData timer, bool canBeCancelled = true)
        {
            TimingCtrl.Instance.CurrentTimerScheduler.Schedule(timer, canBeCancelled);
            return timer;
        }

        public static ValueTimerData SetLink(this ValueTimerData timer, UnityEngine.GameObject go)
        {
            throw new NotImplementedException();
            //go.GetOrAddComponent<MonoSpawnEventTrigger>().OnDestroye_Evt += timer.ForceStopForExternal;
            //return timer;
        }
    }

    public class RefTimerData
    {
        public TimeSpan delay { get; private set; }
        public Action onComplete { get; private set; }
        public int debounceMilliseconds { get; private set; } = -1;

        public static RefTimerData Create(double secondsDelay, Action onComplete) => new()
        {
            delay = TimeSpan.FromSeconds(secondsDelay),
            onComplete = onComplete
        };

        public void Start(bool canBeCancelled = true)
        {
            throw new NotImplementedException();
            //TimingCtrl.Instance.Schedule(ValueTimerData.Create(delay, onComplete), canBeCancelled);
        }

        public RefTimerData Debounce(int milliseconds)
        {
            debounceMilliseconds = milliseconds;
            return this;
        }
    }
}
