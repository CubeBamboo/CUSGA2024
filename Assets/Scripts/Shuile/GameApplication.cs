using CbUtils.Kits.Tasks;
using Shuile.Core.Framework;
using Shuile.ResourcesManagement.Loader;
using UnityEngine;

namespace Shuile
{
    public static class GameApplication
    {
        public static ModuleContainer Global { get; } = new();
        public static ModuleContainer Level { get; } = new(); // nearly obsolete, use LevelScope instead

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void PreInitialize()
        {
            // Initialize the game
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void PostInitialize()
        {
            // Post initialize the game
            var globalGameObject = Global.InstantiateGlobalGameObject();
            TaskBus.Instance.InitializeBusyScreenByGlobalGameObject(globalGameObject);

            _ = GameResourcesLoader.Instance.PreCacheAsync();
        }
    }
}
