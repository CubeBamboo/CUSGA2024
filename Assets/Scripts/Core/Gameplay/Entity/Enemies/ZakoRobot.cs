using CbUtils;

using Shuile.Gameplay.Entity.States;

using System;

using UnityEngine;

namespace Shuile.Gameplay.Entity.Enemies
{
    public class ZakoMachine : Enemy
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
            return false;
        }
    }
}
