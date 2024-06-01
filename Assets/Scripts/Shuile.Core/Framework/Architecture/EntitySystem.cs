using CbUtils;
using System.Collections.Generic;

namespace Shuile.Core.Framework
{
    public class EntitySystem : CSharpLazySingletons<EntitySystem>
    {
        private readonly List<IEntity> entities = new();

        public bool ContainsEntity(IEntity entity) => entities.Contains(entity);
        public void AddEntity(IEntity entity) => entities.Add(entity);
        public bool RemoveEntity(IEntity entity) => entities.Remove(entity);
        public void Clear() => entities.Clear();

        public void EnableAllEntities()
        {
            foreach (var entity in entities)
            {
                entity.SelfEnable = true;
            }
        }

        public T FindEntityOfType<T>() where T : class, IEntity
        {
            foreach (var entity in entities)
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
