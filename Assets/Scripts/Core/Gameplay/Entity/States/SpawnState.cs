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
            //entity.transform.GetChild(0).DOScale(0f, 0.3f).From().SetEase(Ease.OutBounce);
            entity.GotoState(EntityStateType.Idle);
        }

        public override void EnterState()
        {
            //entity.transform.GetChild(0).localScale = Vector3.zero;
            entity.transform.GetChild(0).DOScale(0f, 0.3f).From().SetEase(Ease.OutBounce);
        }

        public override void ExitState() { }
    }
}
