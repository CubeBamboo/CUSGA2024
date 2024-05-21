using CbUtils.Extension;
using CbUtils.Kits.Tasks;
using Shuile.ResourcesManagement.Loader;
using UnityEngine;

namespace Shuile.Core
{
    public static class GameApplication
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void PreInitialize()
        {
            // Initialize the game
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