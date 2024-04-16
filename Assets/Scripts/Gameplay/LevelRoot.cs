using CbUtils;
using Shuile.Framework;
using Shuile.UI;
using Shuile.Rhythm.Runtime;

using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using UDebug = UnityEngine.Debug;
using UnityEngine.SceneManagement;
using CbUtils.ActionKit;
using DG.Tweening;

namespace Shuile
{
    /* link to all objects which want to depend on unity level scene (like lifeTime control)
     * Contain:
     * whole GameplayService
     * UICtrl's GameplayPart
     */
    //it also contains the level state callback
    public class LevelRoot : MonoSingletons<LevelRoot>
    {
        public enum LevelState
        {
            Playing,
            Fail,
            Win
        }

        public event System.Action OnStart, OnFail, OnWin;

        private LevelState state;

        public bool needHitWithRhythm { get; private set; }
        public LevelState State
        {
            get => state;
            set
            {
                if (state == LevelState.Fail) return;

                state = value;
                TriggerEvent(state);
            }
        }

        protected override void OnAwake()
        {
            UICtrl.Instance.RegisterCreator<EndLevelPanel>(EndLevelPanel.Creator);
            UICtrl.Instance.RegisterCreator<HUDHpBarElement>(HUDHpBarElement.Creator);
            needHitWithRhythm = LevelResources.Instance.debugSettings.needHitWithRhythm;

            InitSingletons();
        }
        private void OnDestroy()
        {
            UICtrl.Instance.UnRegisterCreator<EndLevelPanel>();
            UICtrl.Instance.UnRegisterCreator<HUDHpBarElement>();
        }
        private void Start()
        {
            UICtrl.Instance.Create<EndLevelPanel>().Hide();
            UICtrl.Instance.Get<PlayingPanel>().Show();
            UICtrl.Instance.Get<DebugPanel>().Show();
        }

        private void Update()
        {
            // TODO: [!] for debug
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                SceneManager.LoadScene("MainMenu");
            }
        }

        private void InitSingletons()
        {
            MusicRhythmManager.Instance.enabled = true;
            PlayerChartManager.Instance.enabled = true;
            AutoPlayChartManager.Instance.enabled = true;
            LevelChartManager.Instance.enabled = true;
            EnemyRoundManager.Instance.enabled = true;
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
                .OnComplete(() => MonoGameRouter.Instance.ToLevelScene(0))
                .Start();
        }
        private void LevelWin()
        {
            var endPanel = UICtrl.Instance.Get<EndLevelPanel>();
            endPanel.TimeTextUGUI.text = "UseTime: " + MusicRhythmManager.Instance.CurrentTime.ToString("0.0");
            endPanel.TitleUGUI.text = "Stage Clear";
            endPanel.Show();

            // 当前音乐淡出
            AudioManager.Instance.OtherSource.DOFade(0, 0.8f).OnComplete(() =>
            {
                AudioManager.Instance.OtherSource.Stop();
            }); // TODO: [!!] NOOOOOOOOO IT'S SHITTTTTTTTTTT FengZhuang YIxia zhe ge musicPlayer!!!!

            ActionCtrl.Instance.Delay(3f)
                .OnComplete(() => MonoGameRouter.Instance.ToMenu())
                .Start();
        }
    }
}
