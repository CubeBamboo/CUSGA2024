using CbUtils.Kits.Tasks;
using Shuile.Core.Global;
using Shuile.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile
{
    public static class GameApplication
    {
        public static readonly RuntimeContext GlobalContext = new();
        public static BuiltInData BuiltInData { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void PreInitialize()
        {
            GlobalContext.RegisterFactory(() => new SceneTransitionManager());

            MonoContainer.GlobalExtraParents ??= new List<RuntimeContext>();
            MonoContainer.GlobalExtraParents.Add(GlobalContext);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void PostInitialize()
        {
            // Post initialize the game
            var globalGameObject = GlobalCommands.InstantiateGlobalGameObject();
            TaskBus.Instance.InitializeBusyScreenByGlobalGameObject(globalGameObject);

            var resourceLoader = new ResourceLoader();
            BuiltInData = resourceLoader.Load<BuiltInData>("Assets/Data/BuiltInData.asset");
        }
    }
}
