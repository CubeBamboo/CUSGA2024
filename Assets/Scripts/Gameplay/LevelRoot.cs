using CbUtils;
using Shuile.Framework;
using Shuile.UI;
using Shuile.Gameplay;
using Shuile.Gameplay.Event;
using Shuile.ResourcesManagement.Loader;

namespace Shuile.Root
{
    /* link to all objects which want to depend on unity level scene (like lifeTime control)
     * Contain:
     * whole GameplayService
     * UICtrl's GameplayPart
     */
    //it also contains the level state callback
    public class LevelRoot : MonoNonAutoSpawnSingletons<LevelRoot>
    {
        public bool IsStart { get; private set; } = false;
        public bool needHitWithRhythm { get; private set; }

        protected override void OnAwake()
        {
            GameplayService.Interface.OnInit();
            LevelDataBinder.Instance.Initialize();

            LevelStateMachine.Instance.enabled = true;

            UICtrl.Instance.RegisterCreator<EndLevelPanel>(EndLevelPanel.Creator);
            UICtrl.Instance.RegisterCreator<HUDHpBarElement>(HUDHpBarElement.Creator);
            needHitWithRhythm = LevelResourcesLoader.Instance.SyncContext.levelConfig.needHitWithRhythm;

            LevelStartEvent_AutoClear.Register(name =>
            {
                IsStart = true;
            });
        }
        private void OnDestroy()
        {
            UICtrl.Instance.UnRegisterCreator<EndLevelPanel>();
            UICtrl.Instance.UnRegisterCreator<HUDHpBarElement>();

            LevelDataBinder.Instance.DeInitialize();
            GameplayService.Interface.OnDeInit();
        }
        private void Start()
        {
            UICtrl.Instance.Create<EndLevelPanel>().Hide();
            UICtrl.Instance.Get<PlayingPanel>().Show();
            //UICtrl.Instance.Get<DebugPanel>().Show();
        }
    }
}
