using CbUtils.Unity;
using Shuile.Core.Framework;
using Shuile.Gameplay.Model;
using Shuile.ResourcesManagement.Loader;
using Shuile.Rhythm;
using UnityEngine;

namespace Shuile.Gameplay
{
    [DefaultExecutionOrder(-4000)]
    public class LevelRoot : MonoSingletons<LevelRoot>, IEntity
    {
        public static bool IsLevelActive { get; private set; } = false;
        public bool IsStart { get; private set; } = false;
        public bool needHitWithRhythm { get; private set; }
        public static LevelContext LevelContext { get; private set; }

        protected override void OnAwake()
        {
            Debug.Log("Level awake and is initializing");

            needHitWithRhythm = LevelResourcesLoader.Instance.SyncContext.levelConfig.needHitWithRhythm;
            // var scope = LevelScope.Interface;
            // LevelContext.TimingManager = scope.GetImplementation<LevelTimingManager>();

            IsStart = true;
            IsLevelActive = true;
            EntitySystem.Instance.EnableAllEntities();
            Debug.Log("Level load end, game start");
        }
        public void OnDestroy()
        {
            Debug.Log("Level end and is disposing");

            GameApplication.Level.ServiceLocator.ClearExisting();
            IsLevelActive = false;
            Debug.Log("Level dispose end and close");
        }

        public static void RequestStart(LevelContext levelContext)
        {
            LevelContext = levelContext;
        }

        public ModuleContainer GetModule() => GameApplication.Level;
    }
}
