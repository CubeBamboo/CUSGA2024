using CbUtils;
using CbUtils.ActionKit;
using Shuile.Framework;
using Shuile.Rhythm.Runtime;

namespace Shuile.Gameplay
{
    public class LevelStateMachine : MonoSingletons<LevelStateMachine>
    {
        public enum LevelState
        {
            Playing,
            Fail,
            Win
        }

        public event System.Action OnStart, OnFail, OnWin;

        private LevelState state;

        public LevelState State
        {
            get => state;
            set
            {
                if (state == LevelState.Fail || state == LevelState.Win) return;

                state = value;
                TriggerEvent(state);
            }
        }

        private void TriggerEvent(LevelState state)
        {
            switch (state)
            {
                case LevelState.Playing:
                    OnStart?.Invoke();
                    break;
                case LevelState.Fail:
                    OnFail?.Invoke();
                    LevelFail();
                    break;
                case LevelState.Win:
                    OnWin?.Invoke();
                    LevelWin();
                    break;
            }
        }

        private void LevelFail()
        {
            var endPanel = UICtrl.Instance.Get<EndLevelPanel>();
            endPanel.SetState(false);
            endPanel.Show();
            MonoAudioCtrl.Instance.PlayOneShot("Level_Fail", 0.6f);

            ActionCtrl.Delay(3f)
                .OnComplete(() => MonoGameRouter.Instance.ToLevelScene(MonoGameRouter.Instance.GetCurrentScene().name))
                .Start(gameObject);
        }
        private void LevelWin()
        {
            var endPanel = UICtrl.Instance.Get<EndLevelPanel>();
            endPanel.SetState(true);
            endPanel.Show();

            MusicRhythmManager.Instance.FadeOutAndStop(); // 当前音乐淡出
            MonoAudioCtrl.Instance.PlayOneShot("Level_Win", 0.6f);

            ActionCtrl.Delay(3f)
                .OnComplete(() => MonoGameRouter.Instance.ToMenu())
                .Start(gameObject);
        }

        public void Init()
        {
            state = LevelState.Playing;
        }
    }
}
