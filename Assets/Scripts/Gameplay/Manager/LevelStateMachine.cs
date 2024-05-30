using CbUtils;
using CbUtils.Timing;
using CbUtils.Unity;
using Shuile.Core.Framework;
using Shuile.Framework;
using Shuile.Rhythm.Runtime;

namespace Shuile.Gameplay
{
    public class LevelStateMachine : ISystem
    {
        public enum LevelState
        {
            Loading,
            Playing,
            Fail,
            Win
        }

        public event System.Action OnStart, OnFail, OnWin;

        private MusicRhythmManager _musicRhythmManager;
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

        public LevelStateMachine()
        {
            _musicRhythmManager = this.GetSystem<MusicRhythmManager>();
        }

        private void LevelFail()
        {
            var endPanel = UICtrl.Instance.Get<EndLevelPanel>();
            endPanel.SetState(false);
            endPanel.Show();
            MonoAudioCtrl.Instance.PlayOneShot("Level_Fail", 0.6f);

            TimingCtrl.Instance
                .Timer(3f, () => MonoGameRouter.Instance.ToLevelScene(MonoGameRouter.Instance.GetCurrentScene().name))
                .Start();
            //ActionCtrl.Delay(3f)
            //    .OnComplete(() => MonoGameRouter.Instance.ToLevelScene(MonoGameRouter.Instance.GetCurrentScene().name))
            //    .Start();
        }
        private void LevelWin()
        {
            var endPanel = UICtrl.Instance.Get<EndLevelPanel>();
            endPanel.SetState(true);
            endPanel.Show();

            _musicRhythmManager.FadeOutAndStop(); // 当前音乐淡出
            MonoAudioCtrl.Instance.PlayOneShot("Level_Win", 0.6f);

            TimingCtrl.Instance
                .Timer(3f, () => MonoGameRouter.Instance.ToLevelScene(MonoGameRouter.Instance.GetCurrentScene().name))
                .Start();
            //ActionCtrl.Delay(3f)
            //    .OnComplete(() => MonoGameRouter.Instance.ToMenu())
            //    .Start();
        }

        public void Init()
        {
            state = LevelState.Loading;
        }

        public void OnSelfEnable()
        {
        }

        public LayerableServiceLocator GetLocator() => GameApplication.LevelServiceLocator;
    }
}
