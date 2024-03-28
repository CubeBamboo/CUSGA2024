using CbUtils;
using Shuile.Framework;
using Shuile.Gameplay;
using Shuile.UI;

using Cysharp.Threading.Tasks;
using Shuile.Rhythm;

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

        private LevelState state;
        private ISceneLoader sceneLoader;

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
            GameplayService.Interface.OnInit();
            UICtrl.Instance.InitGameplay();
        }

        private void Start()
        {
            UICtrl.Instance.Get<PlayingPanel>().Show();
            MainGame.Interface.TryGet(out sceneLoader);
        }

        private void OnDestroy()
        {
            GameplayService.Interface.OnDeInit();
            UICtrl.Instance.DeInitGameplay();
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
            var endPanel = UICtrl.Instance.Get<EndGamePanel>();
            endPanel.TimeTextUGUI.text = "SurviveTime: " + MusicRhythmManager.Instance.CurrentTime.ToString("0.0");
            endPanel.Show();

            await UniTask.Delay(System.TimeSpan.FromSeconds(3f));
            sceneLoader.LoadSceneAsync(new SceneInfo() { SceneName = "Level0Test" }, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
