using UnityEngine;

namespace Shuile.Gameplay
{
    public static class PlayerCommands
    {
        public static void Move(float xInput, IMoveController moveController)
        {
            moveController.XMove(xInput);
        }
        public static void Attack(Vector2 position, float attackRadius, int attackPoint)
        {
            var hits = Physics2D.OverlapCircleAll(position, attackRadius, LayerMask.GetMask("Enemy"));
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IHurtable>(out var hurt))
                    hurt.OnHurt(attackPoint);
            }
        }
    }
}
