using Shuile.Core.Gameplay.Common;
using System.Linq;
using UnityEngine;

namespace Shuile.Gameplay.Entity
{
    public partial class Bomb
    {
        private class ExplodeHandler
        {
            public Vector3 OriginPosition;
            public float Radius;
            public int AttackPoint;
            public LayerMask HurtMask;

            public void Explode()
            {
                var hurtables = Physics2D.OverlapCircleAll(OriginPosition, Radius, HurtMask)
                    .Select(collider => collider.GetComponent<IHurtable>())
                    .Where(hurtable => hurtable != null);

                foreach (var hurtable in hurtables)
                {
                    hurtable.OnHurt(AttackPoint);
                }
            }
        }
    }
}
