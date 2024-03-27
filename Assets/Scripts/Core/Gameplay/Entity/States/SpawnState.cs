using DG.Tweening;

using UnityEngine;

namespace Shuile.Gameplay.Entity.States
{
    public class SpawnState : EntityState
    {
        public SpawnState(BehaviourEntity entity) : base(entity)
        {
        }

        public override void Judge()
        {
            entity.transform.GetChild(0).DOScale(Vector3.one, 0.1f);
            entity.GotoState(EntityStateType.Idle);
        }

        public override void EnterState()
        {
            entity.transform.GetChild(0).localScale = Vector3.zero;
        }

        public override void ExitState() { }
    }
}
