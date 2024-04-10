using Shuile.Gameplay.Entity.States;

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

            if (Vector3.Distance(player.transform.position, MoveController.Position) <= Property.attackRange)
                player.OnAttack(Property.attackPoint);
            return false;
        }
    }
}