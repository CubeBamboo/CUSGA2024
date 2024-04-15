using CbUtils;
using Shuile.Framework;
using Shuile.UI;
using Shuile.Rhythm.Runtime;

using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using UDebug = UnityEngine.Debug;

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
            End
        }

        public event System.Action OnStart, OnEnd;
        public ChartData CurrentChart { get; set; }

        private LevelState state;
        //private ISceneLoader sceneLoader;

        public bool needHitWithRhythm { get; private set; }
        public LevelState State
        {
            get => state;
            set
            {
                if (state == LevelState.End) return;

                state = value;
                TriggerEvent(state);
            }
        }

        protected override void OnAwake()
        {
            CurrentChart = LevelResources.Instance.currentChart;

            UICtrl.Instance.RegisterCreator<EndLevelPanel>(EndLevelPanel.Creator);
            UICtrl.Instance.RegisterCreator<HUDHpBarElement>(HUDHpBarElement.Creator);
            LevelChartManager.Instance.enabled = true;
            needHitWithRhythm = LevelResources.Instance.debugSettings.needHitWithRhythm;
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
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }

        private void TriggerEvent(LevelState state)
        {
            switch (state)
            {
                case LevelState.Playing:
                    OnStart?.Invoke();

                    break;
                case LevelState.End:
                    OnEnd?.Invoke();
                    LevelEnd();

                    break;
            }
        }

        private async void LevelEnd()
        {
            var endPanel = UICtrl.Instance.Get<EndLevelPanel>();
            endPanel.TimeTextUGUI.text = "SurviveTime: " + MusicRhythmManager.Instance.CurrentTime.ToString("0.0");
            endPanel.Show();

            await UniTask.Delay(System.TimeSpan.FromSeconds(3f));
            GameRoot.Instance.LoadLevel();
            //sceneLoader.LoadSceneAsync(new SceneInfo() { SceneName = "Level0Test" }, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
