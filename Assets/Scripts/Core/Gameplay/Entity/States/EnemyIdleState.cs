using System;

using UnityEngine;

using UObject = UnityEngine.Object;

namespace Shuile.Gameplay.Entity.States
{
    public class EnemyIdleState : EntityState
    {
        private int moveSleep;

        public EnemyIdleState(BehaviourEntity entity) : base(entity)
        {
            if (entity is not Enemy)
                throw new ArgumentException($"entity is {entity.GetType()} not {nameof(Enemy)}", nameof(entity));
        }

        public override void Judge()
        {
            var player = GameplayService.Interface.Get<Player>();
            var playerPos = LevelGrid.Instance.grid.WorldToCell(player.transform.position);
            var gridDistance = Vector3.Distance(playerPos, entity.GridPosition);

            if (gridDistance <= ((Enemy)entity).Property.attackRange)
            {
                entity.GotoState(EntityStateType.Attack);
                return;
            }

            if (moveSleep != 0)
            {
                moveSleep--;
                return;
            }

            // TODO: clever to find path
            var moveTo = entity.GridPosition + new Vector3Int(Math.Sign(player.transform.position.x - entity.transform.position.x), 0, 0);
            if (LevelGrid.Instance.grid.TryGet(moveTo, out var destGameObject))
            {
                var judgeable = destGameObject.GetComponent<IJudgeable>();
                if (judgeable == null)  // not judgeable
                    return;
                judgeable.Judge(entity.LastJudgeFrame, false);  // try trigger it's judge
                if (LevelGrid.Instance.grid.HasContent(moveTo))
                    return;  // still can't move to
            }
            entity.GridPosition = moveTo;
            moveSleep = ((Enemy)entity).Property.moveInterval;
        }

        public override void EnterState()
        {
            moveSleep = 0;
        }

        public override void ExitState()
        {
        }
    }
}
