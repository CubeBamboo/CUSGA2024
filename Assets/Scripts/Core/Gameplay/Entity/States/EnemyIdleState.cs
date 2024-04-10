using System;

using UnityEngine;

namespace Shuile.Gameplay.Entity.States
{
    public class EnemyIdleState : EntityState
    {
        private int moveSleep;
        private Player player;
        private IRouteFinder routeFinder;
        private Enemy enemy;

        public EnemyIdleState(BehaviourEntity entity) : base(entity)
        {
            if (entity is not Enemy enemy)
                throw new ArgumentException($"entity is {entity.GetType()} not {nameof(Enemy)}", nameof(entity));
            this.enemy = enemy;
        }

        public override void Update()
        {
            var path = routeFinder.FindRoute(entity.transform.position, player.transform.position);
            if (path.Length == 0)
                return;

            // move to path
            var dir = Mathf.Sign((path[0] - entity.transform.position).x);
            enemy.MoveController.XAddForce(dir * enemy.Property.acceleration);
        }

        public override void FixedUpdate()
        {
            if (UnityEngine.Random.Range(0, 10) == 0)
                enemy.MoveController.TryJump(enemy.Property.jumpForce);
        }

        public override void Judge()
        {
            var playerPos = GameplayService.Interface.Get<Player>().transform.position;
            var dst = Vector3.Distance(playerPos, entity.transform.position);

            if (dst > ((Enemy)entity).Property.attackRange)
                return;

            entity.State = EntityStateType.Attack;
        }

        public override void Enter()
        {
            player = GameplayService.Interface.Get<Player>();
            routeFinder = GameplayService.Interface.Get<IRouteFinder>();
        }
    }
}
