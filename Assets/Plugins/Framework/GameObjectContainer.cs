using Shuile.Core.Framework;
using System;
using UnityEngine;

namespace Plugins.Framework
{
    public abstract class GameObjectContainer : MonoContainer
    {
        public bool IsInitialized { get; protected set; }

        protected virtual void Awake()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("GameObjectContainer is not initialized. Please make sure that you have SceneContainer in your unity scene. And invoke base.ResolveContext()");
            }
        }

        /// <summary>
        ///     it will be invoked by <see cref="SceneContainer" />, the GameObjectContainer with a parent transform will be added
        ///     to the parent's service locator, which shares the services.
        /// </summary>
        public virtual void ResolveContext(IReadOnlyServiceLocator context)
        {
            IsInitialized = true;
        }

        public virtual void BuildContext(ServiceLocator context)
        {
        }
    }
}
