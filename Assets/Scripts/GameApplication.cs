using CbUtils.Extension;
using CbUtils.Kits.Tasks;
using Shuile.Core.Framework;
using Shuile.Model;
using Shuile.ResourcesManagement.Loader;
using Shuile.Rhythm;
using Shuile.Rhythm.Runtime;
using UnityEngine;

namespace Shuile
{
    public static class GameApplication
    {
        public static LayerableServiceLocator ServiceLocator { get; } = new LayerableServiceLocator();
        public static LayerableServiceLocator LevelServiceLocator { get; } = new LayerableServiceLocator();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void PreInitialize()
        {
            // Initialize the game
            LevelServiceLocator.AddModelCreator<LevelModel>(() => new LevelModel());
            LevelServiceLocator.AddModelCreator<PlayerModel>(() => new PlayerModel());
            LevelServiceLocator.AddSystemCreator<MusicRhythmManager>(() => new MusicRhythmManager());
            LevelServiceLocator.AddSystemCreator<PlayerChartManager>(() => new PlayerChartManager());
            LevelServiceLocator.AddSystemCreator<LevelTimingManager>(() => new LevelTimingManager());
            LevelServiceLocator.AddSystemCreator<AutoPlayChartManager>(() => new AutoPlayChartManager());
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void PostInitialize()
        {
            // Post initialize the game
            var globalGO = Object.Instantiate(Resources.Load<GameObject>("GlobalGameObject"));
            Object.DontDestroyOnLoad(globalGO);
            var busyScreen = globalGO.GetChildByPath<IBusyScreen>("GlobalCanvas/TaskBusyScreen");
            Debug.Log("Use BusyScreen: " + busyScreen);
            TaskBus.Instance.Initialize(busyScreen);

            _ = GameResourcesLoader.Instance.PreCacheAsync();
        }
    }
}
