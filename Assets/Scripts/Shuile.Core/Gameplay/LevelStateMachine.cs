using Shuile.Core.Framework.Unity;
using System;

namespace Shuile.Core.Gameplay
{
    public class LevelStateMachine
    {
        public enum LevelState
        {
            Playing,
            Fail,
            Win
        }

        private LevelState _state;

        public LevelStateMachine()
        {
            Initialize();
        }

        public LevelState State
        {
            get => _state;
            set
            {
                if (_state == value)
                {
                    return;
                }

                _state = value;
                TriggerEvent(_state);
            }
        }

        private void Initialize()
        {
            _state = LevelState.Playing;
            TriggerEvent(LevelState.Playing);
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
