using UnityEngine;

namespace Shuile.Core.Framework.Unity
{
    [System.Obsolete("it is recommended to use the IEntity interface instead of MonoEntity")]
    public abstract class MonoEntity : MonoBehaviour, IEntity
    {
        public virtual bool SelfEnable { get => this.enabled; set => this.enabled = value; }
        protected void Awake()
        {
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

        public abstract ModuleContainer GetModule();
    }
}
