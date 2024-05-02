namespace Shuile.Framework
{
    public class SimpleDeltaTimer
    {
        protected enum State
        {
            Sleep, Running
        }

        private State state = State.Sleep;
        private float timer = 0f;
        private float duration = 0f;
        private System.Action onComplete;

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
        public SimpleDeltaTimer RegisterComplete(System.Action onComplete)
        {
            this.onComplete += onComplete;
            return this;
        }
        public SimpleDeltaTimer UnRegisterComplete(System.Action onComplete)
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
            if (state == State.Sleep) return;

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
    }

    public class SimpleDurationTimer
    {
        public float StartTime { get; set; }
        public float MaxDuration { get; set; }
        public bool HasReachTime(float time) => time - StartTime > MaxDuration;
    }
}
