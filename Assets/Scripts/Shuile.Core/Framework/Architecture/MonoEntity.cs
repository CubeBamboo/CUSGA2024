using CbUtils.Unity;
using UnityEngine;

namespace Shuile.Core.Framework.Unity
{
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

        public virtual void OnInitData(object data) { }

    }
}
