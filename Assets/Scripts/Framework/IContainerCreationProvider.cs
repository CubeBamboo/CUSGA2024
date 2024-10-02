namespace Shuile.Framework
{
    public interface IContainerCreationProvider
    {
        void InitParent();
        void BuildContext(ServiceLocator context);
        void ResolveContext(IReadOnlyServiceLocator context);
    }

    public class MonoContainerCreationProvider : IContainerCreationProvider
    {
        public void InitParent()
        {
        }

        public void BuildContext(ServiceLocator context)
        {
            throw new System.NotImplementedException();
        }

        public void ResolveContext(IReadOnlyServiceLocator context)
        {
            throw new System.NotImplementedException();
        }
    }
}
