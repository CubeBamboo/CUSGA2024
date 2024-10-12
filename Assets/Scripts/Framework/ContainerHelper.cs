using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Shuile.Framework
{
    public static class ContainerHelper
    {
        /// <summary>
        /// awake itself and all its parents.
        /// plz construct object outside of this method.
        /// </summary>
        public static void InitContainer(PlainContainer container)
        {
            if (container.IsInit)
            {
                Debug.LogWarning("trying to init a container multiple times.");
                return;
            }

            // init parent first
            container.MakeSureParentsInit();
            container.LoadFromParentContext(container.Context); // contains parents
            container.IsInit = true;

            container.BuildSelfContext(container.Context);
        }

        /// <summary>
        /// child container will inherit parent's context.
        /// </summary>
        public static void SetChildContainer(this PlainContainer parent, PlainContainer child)
        {
            child.AddParent(parent);
        }
    }

    public class MonoPlainContainerAdapter : PlainContainer
    {
        public Action<IReadOnlyServiceLocator> OnLoadFromContext;
        public Action<RuntimeContext> OnBuildChildContext;

        [CanBeNull] public object Reference
        {
            get => Context.Reference;
            set => Context.Reference = value;
        }

        public override void BuildSelfContext(RuntimeContext context)
        {
            base.BuildSelfContext(context);
            OnBuildChildContext?.Invoke(context);
        }

        public override void LoadFromParentContext(IReadOnlyServiceLocator context)
        {
            base.LoadFromParentContext(context);
            OnLoadFromContext?.Invoke(context);
        }
    }
}
