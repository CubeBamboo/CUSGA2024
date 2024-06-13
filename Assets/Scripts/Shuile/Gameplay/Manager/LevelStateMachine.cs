using Shuile.Core.Framework.Unity;
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

        private LevelState state;

        public LevelStateMachine(IGetableScope scope)
        {
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
