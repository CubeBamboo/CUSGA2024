namespace Shuile.Core.Framework
{
    public class LayerableServiceLocator
    {
        private readonly ServiceLocator serviceLocator = new ();
        
        public void AddModelCreator<T>(System.Func<T> creator) where T : IModel
        {
            serviceLocator.RegisterCreator(creator);
        }
        public void AddSystemCreator<T>(System.Func<T> creator) where T : ISystem
        {
            serviceLocator.RegisterCreator(creator);
        }

        public T GetModel<T>() where T : IModel
        {
            return serviceLocator.GetService<T>();
        }
        public T GetSystem<T>() where T : ISystem
        {
            return serviceLocator.GetService<T>();
        }

        public void ClearExsiting() => serviceLocator.ClearAllServices();
    }
}
