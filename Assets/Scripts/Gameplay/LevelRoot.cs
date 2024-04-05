using CbUtils;
using Shuile.Framework;
using Shuile.Gameplay;
using Shuile.UI;
using Shuile.Rhythm;

using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

namespace Shuile
{
    /* link to all objects which want to depend on unity level scene (like lifeTime control)
     * Contain:
     * whole GameplayService
     * UICtrl's GameplayPart
     */
    //it also contains the level state callback
    public class LevelRoot : MonoSingletons<LevelRoot>, IGameRoot
    {
        public enum LevelState
        {
            Playing,
            End
        }

        public event System.Action OnStart, OnEnd;

        private LevelState state;
        private ISceneLoader sceneLoader;

        protected override void OnAwake()
        {
            InitResource();

            // TODO: [!] debug
            //var chart = ChartConverter.LoadChart("testchart.json");
            //FooChart.Do();
            
        }
        private void OnDestroy()
        {
            DeInitResource();
        }

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

        private void Start()
        {
            UICtrl.Instance.Create<EndLevelPanel>();
            UICtrl.Instance.Get<PlayingPanel>().Show();
            UICtrl.Instance.Get<DebugPanel>().Show();
            MainGame.Interface.TryGet(out sceneLoader);
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
            sceneLoader.LoadSceneAsync(new SceneInfo() { SceneName = "Level0Test" }, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        public void InitResource()
        {
            GameplayService.Interface.OnInit();
            UICtrl.Instance.RegisterCreator<EndLevelPanel>(EndLevelPanel.Creator);
            UICtrl.Instance.RegisterCreator<HUDHpBarElement>(HUDHpBarElement.Creator);
        }
        public void DeInitResource()
        {
            GameplayService.Interface.OnDeInit();
            UICtrl.Instance.UnRegisterCreator<EndLevelPanel>();
            UICtrl.Instance.UnRegisterCreator<HUDHpBarElement>();
        }
    }

    public interface IGameRoot
    {
        void InitResource();
        void DeInitResource();
    }
}
