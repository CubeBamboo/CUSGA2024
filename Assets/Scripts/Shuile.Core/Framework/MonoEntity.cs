using System;
using UnityEngine;

namespace Shuile.Core.Framework.Unity
{
    [Obsolete("it is recommended to use the IEntity interface instead of MonoEntity")]
    public abstract class MonoEntity : MonoBehaviour, IEntity
    {
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

        public virtual bool SelfEnable { get => enabled; set => enabled = value; }

        public abstract ModuleContainer GetModule();
        protected virtual void AwakeOverride() { }
        protected virtual void OnDestroyOverride() { }
    }
}
