using CbUtils.Unity;
using Shuile.Core.Framework;
using Shuile.Gameplay.Model;
using Shuile.ResourcesManagement.Loader;
using UnityEngine;

namespace Shuile.Gameplay
{
    [DefaultExecutionOrder(-4000)]
    public class LevelRoot : MonoSingletons<LevelRoot>
    {
        public static bool IsLevelActive { get; private set; }
        public bool IsStart { get; private set; }
        public bool needHitWithRhythm { get; private set; }
        public static LevelContext LevelContext { get; private set; }

        public void OnDestroy()
        {
            Debug.Log("Level end and is disposing");
            IsLevelActive = false;
            Debug.Log("Level dispose end and close");
        }

        protected override void OnAwake()
        {
            Debug.Log("Level awake and is initializing");

            needHitWithRhythm = LevelResourcesLoader.Instance.SyncContext.levelConfig.needHitWithRhythm;

            IsStart = true;
            IsLevelActive = true;
            EntitySystem.Instance.EnableAllEntities();
            Debug.Log("Level load end, game start");
        }

        public static void RequestStart(LevelContext levelContext)
        {
            LevelContext = levelContext;
        }
    }
}
