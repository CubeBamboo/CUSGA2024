namespace Shuile.Framework
{
    public abstract class SceneContainer : MonoContainer
    {
        public static SceneContainer Instance { get; private set; }

        public override void Awake()
        {
            if (IsAwake) return;
            base.Awake();

            if (Instance != null)
            {
                throw new System.InvalidOperationException("SceneContainer should be singleton in a scene.");
            }
            Instance = this;
        }
    }
}

// using System;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace Shuile.Framework
// {
//     [DefaultExecutionOrder(-1000)]
//     public abstract class SceneContainer : GameObjectContainer
//     {
//         private readonly List<GameObjectContainer> _gameObjectCache = new(20);
//
//         // inject on awake
//         public override void Awake()
//         {
//             // base.Awake(); -> no need to call base.Awake() here
//
//             var currentScene = gameObject.scene;
//
//             // scene context will be auto-injected into the top game objects under the scene container
//             var contextChain = new List<RuntimeContext>(5) { Context };
//
//             foreach (var rootGameObject in currentScene.GetRootGameObjects())
//             {
//                 SetupChild(rootGameObject.transform);
//             }
//
//
//             BuildContext(Context.ServiceLocator);
//             foreach (var monoContainer in _gameObjectCache)
//             {
//                 monoContainer.BuildContext(monoContainer.Context.ServiceLocator);
//             }
//
//             ResolveContext(Context.ServiceLocator);
//             foreach (var monoContainer in _gameObjectCache)
//             {
//                 monoContainer.ResolveContext(monoContainer.Context.ServiceLocator);
//             }
//
//             return;
//
//             void SetupChild(Transform current)
//             {
//                 if (current == null)
//                 {
//                     return;
//                 }
//
//                 GameObjectContainer container = null;
//                 var containers = current.GetComponents<GameObjectContainer>();
//                 if (containers.Length == 1 && containers[0] is not SceneContainer)
//                 {
//                     container = containers[0];
//                     var locator = container.Context.ServiceLocator;
//                     if (contextChain.Count > 0)
//                     {
//                         locator.AddParent(contextChain[^1].ServiceLocator); // last context in the chain
//                     }
//
//                     _gameObjectCache.Add(container);
//                     contextChain.Add(container.Context);
//                 }
//                 else if (containers.Length > 1)
//                 {
//                     throw new InvalidOperationException("cannot have more than one GameObjectContainer in a GameObject, try use child GameObjects or plain c# classes.");
//                 }
//
//                 for (var i = 0; i < current.childCount; i++)
//                 {
//                     var child = current.GetChild(i);
//                     SetupChild(child);
//                 }
//
//                 if (container != null)
//                 {
//                     contextChain.Remove(container.Context);
//                 }
//             }
//         }
//     }
// }
