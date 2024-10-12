using System.Collections.Generic;

namespace Shuile.Framework
{
    /// <summary>
    ///     plain c# class with context.
    ///     Notice that if you create the object by yourself, you need to call <see cref="ContainerHelper.InitContainer" />
    ///     to inject into the container.
    /// </summary>
    public abstract class PlainContainer : IHasContext
    {
        public readonly RuntimeContext Context;

        /// <summary>
        /// after init <see cref="ContainerHelper.InitContainer"/> and invoke <see cref="LoadFromParentContext"/>, it will be true.
        /// </summary>
        public bool IsInit { get; set; }

        private List<PlainContainer> parents;
        public IReadOnlyList<PlainContainer> Parents => parents;

        protected PlainContainer()
        {
            Context = new RuntimeContext(new ContextServiceLocator()) { Reference = this };
        }

        public void AddParent(PlainContainer parent)
        {
            parents ??= new List<PlainContainer>();
            parents.Add(parent); // register

            Context.AddParent(parent.Context);
        }

        public void MakeSureParentsInit()
        {
            if (!this.HasParent())
            {
                return;
            }

            foreach (var parent in parents)
            {
                if (!parent.IsInit)
                {
                    ContainerHelper.InitContainer(parent);
                }
            }
        }

        public virtual void LoadFromParentContext(IReadOnlyServiceLocator context)
        {
        }

        public virtual void BuildSelfContext(RuntimeContext context)
        {
        }
    }
}
