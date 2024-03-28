using CbUtils;

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
            var playerCollider = Physics2D.OverlapCircle(LevelGrid.Instance.grid.CellToWorld(GridPosition),
                LevelGrid.Instance.grid.ToWorldSize(Property.attackRange).x,
                LayerMask.NameToLayer("Player"));

            if (playerCollider != null)
            {
                playerCollider.GetComponent<PlayerController>().OnAttack(Property.attackPoint);
                GotoState(EntityStateType.Dead);  // Or base.OnAttack(114514?);
            }
            return false;
        }
    }
}
