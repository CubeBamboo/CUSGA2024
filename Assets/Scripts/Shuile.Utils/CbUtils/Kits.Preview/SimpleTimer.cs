using System;

namespace Shuile.Framework
{
    public class SimpleDeltaTimer
    {
        private float duration;
        private Action onComplete;

        private State state = State.Sleep;
        private float timer;

        public float Duration
        {
            get => duration;
            set
            {
                duration = value;
                state = State.Running;
            }
        }

        public float LastDeltaTime { get; private set; }

        public SimpleDeltaTimer SetDuration(float duration)
        {
            Duration = duration;
            return this;
        }

        public SimpleDeltaTimer RegisterComplete(Action onComplete)
        {
            this.onComplete += onComplete;
            return this;
        }

        public SimpleDeltaTimer UnRegisterComplete(Action onComplete)
        {
            this.onComplete -= onComplete;
            return this;
        }

        public void RestartTick()
        {
            timer = 0f;
            state = State.Running;
        }

        public void Reset()
        {
            Duration = 0f;
            onComplete = null;
            state = State.Sleep;
        }

        public void Tick(float deltaTime)
        {
            if (state == State.Sleep)
            {
                return;
            }

            LastDeltaTime = deltaTime;

            timer += deltaTime;
            if (timer > Duration)
            {
                EndTiming();
            }
        }

        public void ForceAwake()
        {
            state = State.Running;
        }

        public void ForceSleep()
        {
            state = State.Sleep;
        }

        public void ForceComplete()
        {
            EndTiming();
        }

        private void EndTiming()
        {
            onComplete?.Invoke();
            state = State.Sleep;
        }

        protected enum State
        {
            Sleep, Running
        }
    }

    public class SimpleDurationTimer
    {
        public float StartTime { get; set; }
        public float MaxDuration { get; set; }

        public bool HasReachTime(float time)
        {
            return time - StartTime > MaxDuration;
        }
    }
}
