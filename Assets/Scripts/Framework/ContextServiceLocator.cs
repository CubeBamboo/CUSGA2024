using System;

namespace Shuile.Framework
{
    /// <summary>
    /// automatically initialize the container after the instance creation.
    /// </summary>
    public class ContextServiceLocator : ServiceLocator
    {
        protected override void RegisterInstanceInternal(Type type, object service)
        {
            base.RegisterInstanceInternal(type, service);
            if (service is PlainContainer container)
            {
                ContainerHelper.AwakeContainer(container);
            }
        }
    }
}
