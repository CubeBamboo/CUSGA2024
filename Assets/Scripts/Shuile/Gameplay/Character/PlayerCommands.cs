using Shuile.Core.Framework;
using UnityEngine;

namespace Shuile.Gameplay.Character
{
    internal class AttackCommand : ICommand
    {
        public Vector2 position;
        public float attackRadius;
        public int attackPoint;

        public void Execute()
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
