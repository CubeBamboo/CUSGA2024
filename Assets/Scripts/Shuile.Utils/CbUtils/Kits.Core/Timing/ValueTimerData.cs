using CbUtils.Event;
using CbUtils.Extension;

namespace CbUtils.Timing
{
    public struct ValueTimerData
    {
        public int millisecondsDelay { get; private set; }
        public System.Action onComplete { get; private set; }

        public static ValueTimerData Create(int millisecondsDelay, System.Action onComplete)
        {
            return new()
            {
                millisecondsDelay = millisecondsDelay,
                onComplete = onComplete
            };
        }
    }

    public static class ValueTimerDataExtension
    {
        /// <summary> multiple call is ok. </summary>
        public static ValueTimerData Start(this ValueTimerData timer, bool canBeCancelled = true)
        {
            TimingCtrl.Instance.Schedule(timer, canBeCancelled);
            return timer;
        }

        public static ValueTimerData SetLink(this ValueTimerData timer, UnityEngine.GameObject go)
        {
            throw new System.NotImplementedException();
            //go.GetOrAddComponent<MonoSpawnEventTrigger>().OnDestroye_Evt += timer.ForceStopForExternal;
            //return timer;
        }
    }
}
