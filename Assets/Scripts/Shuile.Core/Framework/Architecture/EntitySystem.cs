using CbUtils;
using System.Collections.Generic;

namespace Shuile.Core.Framework
{
    public class EntitySystem : CSharpLazySingletons<EntitySystem>
    {
        private readonly List<IEntity> _entities = new();

        public bool ContainsEntity(IEntity entity) => _entities.Contains(entity);
        public void AddEntity(IEntity entity) => _entities.Add(entity);
        public bool RemoveEntity(IEntity entity) => _entities.Remove(entity);
        public void Clear() => _entities.Clear();

        public void EnableAllEntities()
        {
            foreach (var entity in _entities)
            {
                entity.SelfEnable = true;
            }
        }

        public T FindEntityOfType<T>() where T : class, IEntity
        {
            foreach (var entity in _entities)
            {
                if (entity is T t)
                {
                    return t;
                }
            }
            return null;
        }
    }
}
