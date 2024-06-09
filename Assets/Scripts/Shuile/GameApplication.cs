using CbUtils.Kits.Tasks;
using Shuile.Core.Framework;
using Shuile.Gameplay.Feel;
using Shuile.Gameplay.Manager;
using Shuile.Model;
using Shuile.ResourcesManagement.Loader;
using Shuile.Rhythm;
using UnityEngine;

namespace Shuile
{
    public static class GameApplication
    {
        public static ModuleContainer Global { get; } = new();
        public static ModuleContainer Level { get; } = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void PreInitialize()
        {
            // Initialize the game
            Level
                .AddUtilityImplemenation<LevelFeelManager>(() => new LevelFeelManager())

                .AddModelImplemenation<LevelModel>(() => new LevelModel(Level))
                .AddModelImplemenation<PlayerModel>(() => new PlayerModel())

                .AddSystemImplemenation<LevelTimingManager>(() => new LevelTimingManager())
                .AddSystemImplemenation<LevelStateMachine>(() => new LevelStateMachine());
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void PostInitialize()
        {
            // Post initialize the game
            var globalGO = Global.InstantiateGlobalGameObject();
            TaskBus.Instance.InitializeBusyScreenByGlobalGameObject(globalGO);

            _ = GameResourcesLoader.Instance.PreCacheAsync();
        }
    }
}
