using CbUtils;

using Shuile.Gameplay.Entity.States;

using UnityEngine;

namespace Shuile.Gameplay.Entity.Enemies
{
    public class ZakoMachine : Enemy
    {
        protected override void RegisterState(FSM<EntityStateType> fsm)
        {
            fsm.AddState(EntityStateType.Spawn, new SpawnState(this));
            fsm.AddState(EntityStateType.Idle, new EnemyIdleState(this));
            fsm.AddState(EntityStateType.Attack, new CommonEnemyAttackState(this, Attack));
            fsm.AddState(EntityStateType.Dead, new DeadState(this));
        }

        private bool Attack(CommonEnemyAttackState state)
        {
            var player = GameplayService.Interface.Get<Player>();

            if (Vector3.Distance(player.transform.position, MoveController.Position) <= Property.attackRange)
                player.OnHurt(Property.attackPoint);
            return false;
        }
    }
}
