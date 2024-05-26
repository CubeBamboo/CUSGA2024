using UnityEngine;

namespace Shuile.Core.Framework.Unity
{
    public abstract class MonoEntity : MonoBehaviour, IEntity
    {
        public virtual void OnSelfEnable() => this.enabled = true;
        protected void Awake()
        {
            this.enabled = false;
            EntitySystem.Instance.AddEntity(this);
            AwakeOverride();
        }
        protected void OnDestroy()
        {
            EntitySystem.Instance.RemoveEntity(this);
            OnDestroyOverride();
        }
        protected virtual void AwakeOverride() { }
        protected virtual void OnDestroyOverride() { }

        public abstract LayerableServiceLocator GetLocator();
    }
}
