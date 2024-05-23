using CbUtils.Timing.Scheduler;

namespace CbUtils.Timing
{
    public class TimingCtrl : CSharpLazySingletons<TimingCtrl>
    {
        public ITimerScheduler CurrentTimerScheduler { get; set; } = new UniTaskTimerScheduler();
        public void StopAllTimer() => CurrentTimerScheduler.StopAllTimer();
        public void Schedule(ValueTimerData timerData, bool canBeCancelled = true) => CurrentTimerScheduler.Schedule(timerData, canBeCancelled);

        public ValueTimerData Timer(int millisecondsDelay, System.Action onComplete)
            => ValueTimerData.Create(millisecondsDelay, onComplete);
    }

    public static class TimingCtrlExtension
    {
        public static ValueTimerData Timer(this TimingCtrl timingCtrl, float secondsDelay, System.Action onComplete)
            => timingCtrl.Timer((int)System.Math.Round(secondsDelay * 1000f), onComplete);
    }
}
