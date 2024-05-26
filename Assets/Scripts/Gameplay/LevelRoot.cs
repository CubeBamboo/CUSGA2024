using Shuile.Framework;
using Shuile.Gameplay;
using Shuile.Gameplay.Event;
using Shuile.ResourcesManagement.Loader;
using UnityEngine;
using Shuile.Core.Framework;
using Shuile.Rhythm;
using CbUtils.Unity;

namespace Shuile.Root
{
    /* link to all objects which want to depend on unity level scene (like lifeTime control)
     * Contain:
     * whole GameplayService
     * UICtrl's GameplayPart
     */
    //it also contains the level state callback
    public class LevelRoot : MonoSingletons<LevelRoot>, IEntity
    {
        public static bool IsLevelActive { get; private set; } = false;
        public bool IsStart { get; private set; } = false;
        public bool needHitWithRhythm { get; private set; }
        public LevelContext LevelContext { get; private set; }

        protected override void OnAwake()
        {
            Debug.Log("Level awake and is initializing");
            LevelDataBinder.Instance.Initialize();

            UICtrl.Instance.RegisterCreator<EndLevelPanel>(EndLevelPanel.Creator);
            UICtrl.Instance.RegisterCreator<HUDHpBarElement>(HUDHpBarElement.Creator);
            needHitWithRhythm = LevelResourcesLoader.Instance.SyncContext.levelConfig.needHitWithRhythm;
            LevelContext = new();
            LevelContext.timingManager = this.GetSystem<LevelTimingManager>();

            LevelStartEvent_AutoClear.Register(name =>
            {
                Debug.Log("Level load end, game start");
                IsStart = true;
                IsLevelActive = true;
                EntitySystem.Instance.EnableAllEntities();
            });

            UICtrl.Instance.Create<EndLevelPanel>().Hide();
            //UICtrl.Instance.Get<PlayingPanel>().Show();
            //UICtrl.Instance.Get<DebugPanel>().Show();
        }
        public void OnDestroy()
        {
            Debug.Log("Level end and is disposing");
            UICtrl.Instance.UnRegisterCreator<EndLevelPanel>();
            UICtrl.Instance.UnRegisterCreator<HUDHpBarElement>();

            LevelDataBinder.Instance.DeInitialize();
            GameApplication.LevelServiceLocator.ClearExsiting();
            IsLevelActive = false;
            Debug.Log("Level dispose end and close");
        }

        public void OnSelfEnable()
        {
        }

        public LayerableServiceLocator GetLocator() => GameApplication.LevelServiceLocator;
    }
}
