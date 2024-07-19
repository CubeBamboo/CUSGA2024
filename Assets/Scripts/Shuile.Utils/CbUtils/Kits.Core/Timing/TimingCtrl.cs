using CbUtils.Timing.Scheduler;
using System;

namespace CbUtils.Timing
{
    public class TimingCtrl : CSharpLazySingletons<TimingCtrl>
    {
        public ITimerScheduler CurrentTimerScheduler { get; set; } = new UniTaskTimerScheduler();

        public ValueTimerData Timer(double secondsDelay, Action onComplete)
        {
            return ValueTimerData.Create(secondsDelay, onComplete);
        }
    }

    public static class TimingCtrlExtension
    {
        public static void StopAllTimer(this TimingCtrl self)
        {
            self.CurrentTimerScheduler.StopAllTimer();
        }
    }
}
