using System.Collections.Generic;
using UnityEngine;

namespace Plugins.Framework
{
    [DefaultExecutionOrder(-1000)]
    public abstract class SceneContainer : GameObjectContainer
    {
        private readonly List<GameObjectContainer> _gameObjectCache = new(20);

        protected override void Awake()
        {
            var currentScene = gameObject.scene;

            // scene context will be auto-injected into the top game objects under the scene container
            var contextChain = new List<RuntimeContext>(5) { Context };

            foreach (var rootGameObject in currentScene.GetRootGameObjects())
            {
                SetupChild(rootGameObject.transform);
            }

            BuildContext(Context.ServiceLocator);
            foreach (var monoContainer in _gameObjectCache)
            {
                monoContainer.BuildContext(monoContainer.Context.ServiceLocator);
            }

            ResolveContext(Context.ServiceLocator);
            foreach (var monoContainer in _gameObjectCache)
            {
                monoContainer.ResolveContext(monoContainer.Context.ServiceLocator);
            }

            return;

            void SetupChild(Transform current)
            {
                if (current == null)
                {
                    return;
                }

                if (current.TryGetComponent(out GameObjectContainer container) && container is not SceneContainer)
                {
                    var locator = container.Context.ServiceLocator;
                    if (contextChain.Count > 0)
                    {
                        locator.AddParent(contextChain[^1].ServiceLocator); // last context in the chain
                    }

                    _gameObjectCache.Add(container);
                    contextChain.Add(container.Context);
                }

                for (var i = 0; i < current.childCount; i++)
                {
                    var child = current.GetChild(i);
                    SetupChild(child);
                }

                if (container != null)
                {
                    contextChain.Remove(container.Context);
                }
            }
        }
    }
}
