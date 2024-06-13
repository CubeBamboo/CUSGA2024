using Shuile.Core.Gameplay.Common;
using System.Linq;

using UnityEngine;

namespace Shuile.Gameplay.Weapon
{
    public class ZakoSword : BaseWeapon
    {
        [SerializeField] private float attackRadius = 2.8f;
        [SerializeField] private int attackPoint = 20;

        public override WeaponType Type => WeaponType.Sword;

        protected override void OnAttack(WeaponCommandData data)
        {
            var hits = Physics2D.OverlapCircleAll(data.position, this.attackRadius, LayerMask.GetMask("Player"));
            var hurts = hits.Select(hit => hit.GetComponent<IHurtable>());
            foreach (var hurt in hurts)
                hurt.OnHurt(this.attackPoint);
        }

        protected override void OnAttackFinish(WeaponCommandData data)
        {
        }

        protected override void OnHypermodeSwitch(bool enabled)
        {
        }
    }
}
