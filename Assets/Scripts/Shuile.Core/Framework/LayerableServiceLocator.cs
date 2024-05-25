namespace Shuile.Core.Framework
{
    public class LayerableServiceLocator // if you dont want to use sublayer, choose singleton
    {
        ServiceLocator container = new();

        public void AddModelCreator<T>(System.Func<T> creator) where T : IModel
        {
            container.RegisterCreator(creator);
        }
        public void AddSystemCreator<T>(System.Func<T> creator) where T : ISystem
        {
            container.RegisterCreator(creator);
        }

        public T GetModel<T>() where T : IModel
        {
            return container.GetService<T>();
        }
        public T GetSystem<T>() where T : ISystem
        {
            return container.GetService<T>();
        }

        public void ClearExsiting() => container.ClearAllServices();
    }
}
