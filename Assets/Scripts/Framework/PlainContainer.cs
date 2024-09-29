using System;

namespace Shuile.Framework
{
    public abstract class PlainContainer : IHasContext
    {
        public virtual void ResolveContext(IReadOnlyServiceLocator context)
        {
        }

        public void BuildContext(ServiceLocator context)
        {
            throw new NotSupportedException();
        }
    }
}
