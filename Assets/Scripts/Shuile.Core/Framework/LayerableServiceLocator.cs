namespace Shuile.Core.Framework
{
    public class LayerableServiceLocator
    {
        #region Layer

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

        #endregion

        /*private class ViewResolver
        {
            readonly List<System.Func<IEntity, IView>> funcs = new();
            public void Add(System.Func<IEntity, IView> func) => funcs.Add(func);
            public void Resolve(IEntity entity)
            {
                foreach (var done in funcs)
                {
                    done(entity);
                }
            }
        }

        private readonly Dictionary<System.Type, ViewResolver> viewLinkedToEntityDict = new();

        public void LinkViewToEntity<TEntity, TView>(System.Func<TEntity, TView> func) where TEntity : IEntity where TView : IView
        {
            var entityType = typeof(TEntity);
            if (!viewLinkedToEntityDict.ContainsKey(entityType))
            {
                viewLinkedToEntityDict.Add(entityType, new());
            }
            viewLinkedToEntityDict[entityType].Add(ie => func((TEntity)ie));
        }
        public bool TryResolveView<TEntity>(IEntity entity) where TEntity : IEntity
        {
            var entityType = typeof(TEntity);
            if (!viewLinkedToEntityDict.ContainsKey(entityType)) return false;
            var res = viewLinkedToEntityDict[entityType];
            res.Resolve(entity);
            return true;
        }*/
    }
}
