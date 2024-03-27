using System;

using UnityEngine;

using UObject = UnityEngine.Object;

namespace Shuile.Gameplay.Entity.States
{
    public class EnemyIdleState : EntityState
    {
        private PlayerController playerCtrl;
        private int moveSleep;

        public EnemyIdleState(BehaviourEntity entity) : base(entity)
        {
            if (entity is not Enemy)
                throw new InvalidCastException($"entity is {entity.GetType()} not {nameof(Enemy)}");
        }

        public override void Judge()
        {
            var dst = Vector3.Distance(playerCtrl.transform.position, entity.transform.position);
            var attackRange = (LevelGrid.Instance.grid.CellSize.x + LevelGrid.Instance.grid.CellGap.x) * ((Enemy)entity).Property.attackRange;
            if (dst <= attackRange)
            {
                entity.GotoState(EntityStateType.Attack);
                return;
            }

            if (moveSleep != 0)
            {
                moveSleep--;
                return;
            }


            entity.GridPosition += new Vector3Int(Math.Sign(playerCtrl.transform.position.x - entity.transform.position.x), 0, 0);
            moveSleep = ((Enemy)entity).Property.moveInterval;
        }

        public override void EnterState()
        {
            moveSleep = 0;
            // moveSleep = BindEnemy.Property.moveInterval;
        }

        public override void ExitState()
        {
        }
    }
}
