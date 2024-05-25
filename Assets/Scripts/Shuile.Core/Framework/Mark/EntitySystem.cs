using CbUtils;
using System.Collections.Generic;

namespace Shuile.Core.Framework
{
    public class EntitySystem : CSharpLazySingletons<EntitySystem>
    {
        private readonly List<IEntity> entities = new();

        public void AddEntity(IEntity entity) => entities.Add(entity);
        public bool RemoveEntity(IEntity entity) => entities.Remove(entity);
        public void Clear() => entities.Clear();
        public bool Contains(IEntity entity) => entities.Contains(entity);

        public void EnableAllEntities()
        {
            foreach (var entity in entities)
            {
                entity.OnSelfEnable();
            }
        }
    }
}
