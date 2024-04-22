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
            endPanel.TimeTextUGUI.text = "SurviveTime: " + MusicRhythmManager.Instance.CurrentTime.ToString("0.0");
            endPanel.Show();
            ActionCtrl.Instance.Delay(3f)
                .OnComplete(() => MonoGameRouter.Instance.ToLevelScene(MonoGameRouter.Instance.GetCurrentScene().name))
                .Start(gameObject);
        }
        private void LevelWin()
        {
            var endPanel = UICtrl.Instance.Get<EndLevelPanel>();
            endPanel.TimeTextUGUI.text = "UseTime: " + MusicRhythmManager.Instance.CurrentTime.ToString("0.0");
            endPanel.TitleUGUI.text = "Stage Clear";
            endPanel.Show();

            MusicRhythmManager.Instance.FadeOutAndStop(); // 当前音乐淡出

            ActionCtrl.Instance.Delay(3f)
                .OnComplete(() => MonoGameRouter.Instance.ToMenu())
                .Start(gameObject);
        }

        public void Init()
        {
            state = LevelState.Playing;
        }
    }
}
