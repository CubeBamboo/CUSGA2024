using CbUtils.Kits.Tasks;
using Shuile.Core.Framework;
using Shuile.Core.Global;
using Shuile.Framework;
using Shuile.ResourcesManagement.Loader;
using UnityEngine;

namespace Shuile
{
    public static class GameApplication
    {
        public static readonly ServiceLocator GlobalService = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void PreInitialize()
        {
            GlobalService.AddImplemenation(() => new SceneTransitionManager());

            ServiceLocator.Global = GlobalService;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void PostInitialize()
        {
            // Post initialize the game
            var globalGameObject = GlobalCommands.InstantiateGlobalGameObject();
            TaskBus.Instance.InitializeBusyScreenByGlobalGameObject(globalGameObject);

            _ = GameResourcesLoader.Instance.PreCacheAsync();
        }
    }
}
