using Shuile.Gameplay.Entity.States;

using System;

namespace Shuile.Gameplay.Entity.Enemies
{
    public class Creeper : Enemy
    {
        protected override void RegisterState()
        {
            states.Add(EntityStateType.Spawn, new SpawnState(this));
            states.Add(EntityStateType.Idle, new EnemyIdleState(this));
            states.Add(EntityStateType.Attack, new CommonEnemyAttackState(this, Attack));
            states.Add(EntityStateType.Dead, new DeadState(this));
        }

        private bool Attack(CommonEnemyAttackState state)
        {
            var player = GameplayService.Interface.Get<Player>();
            var playerPos = LevelGrid.Instance.grid.WorldToCell(player.transform.position);

            if (playerPos.y == GridPosition.y && Math.Abs(playerPos.x - GridPosition.x) <= Property.attackRange)
                player.OnAttack(Property.attackPoint);
            GotoState(EntityStateType.Dead);  // Or OnAttack(Health);
            currentState.Judge();  // die了
            return false;
        }
    }
}
