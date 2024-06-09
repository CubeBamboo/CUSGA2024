/*using CbUtils.Event;
using DG.Tweening;

using UnityEngine;

namespace Shuile.Gameplay.Entity.States
{
    public class DeadState : EntityState
    {
        public DeadState(BehaviourEntity entity) : base(entity)
        {
        }

        public override void Judge()
        {
            entity.transform.GetChild(0).DOScale(Vector3.zero, 0.1f).OnComplete(() => GameObject.Destroy(entity?.gameObject));
            entity.gameObject.SetOnDestroy(() => entity.transform.DOKill(), "transform");
            // Do something
            // e.g 加分
            EntityManager.Instance.Remove(entity);
        }
    }
}*/
