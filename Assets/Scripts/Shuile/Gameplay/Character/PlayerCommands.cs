using Shuile.Core.Framework;
using Shuile.Core.Gameplay.Common;
using UnityEngine;

namespace Shuile.Gameplay.Character
{
    internal class AttackCommand : ICommand
    {
        public int attackPoint;
        public float attackRadius;
        public Vector2 position;

        public void Execute()
        {
            var hits = Physics2D.OverlapCircleAll(position, attackRadius, LayerMask.GetMask("Enemy"));
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IHurtable>(out var hurt))
                {
                    hurt.OnHurt(attackPoint);
                }
            }
        }
    }
}
