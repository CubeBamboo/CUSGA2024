using System;
using UnityEngine;

namespace Shuile.Framework
{
    /// <summary>
    /// provide the context shared based on the hierarchy of the GameObject.
    /// if you need to access Unity's event functions, use <see cref="UnityEntryPointScheduler"/>.
    /// </summary>
    public abstract class MonoContainer : MonoBehaviour, IHasContext
    {
        public bool IsAwake { get; private set; }
        public bool IsResolved { get; protected set; }

        public readonly RuntimeContext Context;

        public MonoContainer()
        {
            Context = new RuntimeContext { Reference = this };
        }

        public virtual void Awake()
        {
            if (IsAwake) return;

            if (GetComponents<MonoContainer>().Length > 1) throw MultiMonoContainerException();

            // build self context and parent.
            var parent = FindParent();
            if (parent != null)
            {
                Context.ServiceLocator.AddParent(parent.Context.ServiceLocator);
                parent.MakeSureAwake(); // make sure awake
            }
            else if (ServiceLocator.Global != null)
            {
                Context.ServiceLocator.AddParent(ServiceLocator.Global);
            }
            BuildContext(Context.ServiceLocator);
            ResolveContext(Context.ServiceLocator);
            IsAwake = true;
        }

        private MonoContainer FindParent()
        {
            var parent = transform.parent;
            while (parent != null)
            {
                var containers = parent.GetComponents<MonoContainer>();
                switch (containers.Length)
                {
                    case 0:
                        parent = parent.parent;
                        break;
                    case 1:
                        return containers[0];
                    default:
                        throw MultiMonoContainerException();
                }
            }

            return null;
        }

        public virtual void ResolveContext(IReadOnlyServiceLocator context)
        {
            IsResolved = true;
        }

        public virtual void BuildContext(ServiceLocator context)
        {
        }

        public void MakeSureAwake()
        {
            Awake();
        }

        public static InvalidOperationException MultiMonoContainerException() => new($"cannot have more than one {nameof(MonoContainer)} in a GameObject, try use child GameObjects or plain c# classes.");
    }
}
