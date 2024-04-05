using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;

using System;
using System.Threading;

using UnityEngine;

using UObject = UnityEngine.Object;

namespace Shuile.Gameplay.Entity.States
{
    public class EnemyIdleState : EntityState
    {
        private int moveSleep;
        private CancellationTokenSource moveCts;
        private Player player;
        private IRouteFinder routeFinder;

        public EnemyIdleState(BehaviourEntity entity) : base(entity)
        {
            if (entity is not Enemy)
                throw new ArgumentException($"entity is {entity.GetType()} not {nameof(Enemy)}", nameof(entity));
        }

        public async UniTaskVoid MoveUpdate()
        {
            await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate(PlayerLoopTiming.Update).WithCancellation(moveCts.Token))
            {
                var path = routeFinder.FindRoute(entity.transform.position, player.transform.position);
                if (path.Length == 0)
                    continue;

                // move to path
                var dir = (path[0] - entity.transform.position).normalized;
                entity.Position += dir * ((Enemy)entity).Property.moveSpeed * Time.deltaTime;
            }
        }

        public override void Judge()
        {
            var playerPos = GameplayService.Interface.Get<Player>().transform.position;
            var dst = Vector3.Distance(playerPos, entity.transform.position);

            if (dst > ((Enemy)entity).Property.attackRange)
                return;

            entity.GotoState(EntityStateType.Attack);
        }

        public override void EnterState()
        {
            if (moveCts != null)
                return;

            moveCts = new();
            player = GameplayService.Interface.Get<Player>();
            routeFinder = GameplayService.Interface.Get<IRouteFinder>();
            MoveUpdate().Forget();
        }

        public override void ExitState()
        {
            if (moveCts != null)
            {
                moveCts.Cancel();
                moveCts = null;
            }
        }
    }
}
