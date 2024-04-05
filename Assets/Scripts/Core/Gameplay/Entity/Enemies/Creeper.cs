using Shuile.Gameplay.Entity.States;

using UnityEngine;

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

            if (Vector3.Distance(player.transform.position, Position) <= Property.attackRange)
                player.OnAttack(Property.attackPoint);
            GotoState(EntityStateType.Dead);  // Or OnAttack(Health);
            stateBehaviour.Judge();  // die了
            return true;
        }
    }
}
