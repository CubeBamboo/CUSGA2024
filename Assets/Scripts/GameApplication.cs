using CbUtils.Extension;
using CbUtils.Kits.Tasks;
using Shuile.Core.Framework;
using Shuile.Gameplay;
using Shuile.Model;
using Shuile.ResourcesManagement.Loader;
using Shuile.Rhythm;
using Shuile.Rhythm.Runtime;
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
                .AddModelImplemenation<LevelModel>(() => new LevelModel(Level))
                .AddModelImplemenation<PlayerModel>(() => new PlayerModel())
                .AddSystemImplemenation<MusicRhythmManager>(() => new MusicRhythmManager())
                .AddSystemImplemenation<PlayerChartManager>(() => new PlayerChartManager())
                .AddSystemImplemenation<LevelTimingManager>(() => new LevelTimingManager())
                .AddSystemImplemenation<AutoPlayChartManager>(() => new AutoPlayChartManager())
                .AddSystemImplemenation<LevelFeelManager>(() => new LevelFeelManager())
                .AddSystemImplemenation<LevelStateMachine>(() => new LevelStateMachine())
                .AddSystemImplemenation<EnemySpawnManager>(() => new EnemySpawnManager());
            
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void PostInitialize()
        {
            // Post initialize the game
            var globalGO = Object.Instantiate(Resources.Load<GameObject>("GlobalGameObject"))
                .SetDontDestroyOnLoad();
            var busyScreen = globalGO.GetChildByPath<IBusyScreen>("GlobalCanvas/TaskBusyScreen");
            Debug.Log("Use BusyScreen: " + busyScreen);
            TaskBus.Instance.Initialize(busyScreen);

            _ = GameResourcesLoader.Instance.PreCacheAsync();
        }
    }
}
