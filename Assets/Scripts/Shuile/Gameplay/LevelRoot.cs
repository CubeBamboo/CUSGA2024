using CbUtils.Unity;
using Shuile.Gameplay.Model;
using UnityEngine;

namespace Shuile.Gameplay
{
    // old shit code
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

            needHitWithRhythm = GameApplication.BuiltInData.levelConfig.needHitWithRhythm;

            IsStart = true;
            IsLevelActive = true;
            Debug.Log("Level load end, game start");
        }

        public static void RequestStart(LevelContext levelContext)
        {
            LevelContext = levelContext;
        }
    }
}
