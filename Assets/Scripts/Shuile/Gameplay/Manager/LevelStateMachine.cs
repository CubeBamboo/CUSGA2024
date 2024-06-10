using Shuile.Rhythm;
using System;

namespace Shuile.Gameplay.Manager
{
    public class LevelStateMachine
    {
        public enum LevelState
        {
            Playing,
            Fail,
            Win
        }

        private readonly MusicRhythmManager _musicRhythmManager;
        private LevelState state;

        public LevelStateMachine()
        {
            _musicRhythmManager = MusicRhythmManager.Instance;

            Initialize();
        }

        private void Initialize()
        {
            state = LevelState.Playing;
            TriggerEvent(LevelState.Playing);
        }

        public LevelState State
        {
            get => state;
            set
            {
                if (state == value)
                {
                    return;
                }

                state = value;
                TriggerEvent(state);
            }
        }
        
        public event Action OnStart, OnFail, OnWin;

        private void TriggerEvent(LevelState state)
        {
            switch (state)
            {
                case LevelState.Playing:
                    OnStart?.Invoke();
                    break;
                case LevelState.Fail:
                    OnFail?.Invoke();
                    break;
                case LevelState.Win:
                    OnWin?.Invoke();
                    break;
            }
        }
    }
}
