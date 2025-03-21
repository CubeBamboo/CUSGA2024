using Shuile.Core.Gameplay.Common;
using Shuile.Gameplay.Character;
using System;
using UnityEngine;

namespace Shuile.Gameplay.Weapon
{
    public class Sword : BaseWeapon
    {
        public BoxCollider2D attackRange;
        public int attackPoint;
        public BoxCollider2D hyperAttackRange;
        public int hyperAttackPoint;
        private NormalPlayerCtrl playerCtrl;

        public override WeaponType Type => WeaponType.Sword;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //Debug.Log(collision.name);
            // if (!playerCtrl.AttackingLock || collision.CompareTag("Player")) // 防止紫砂
            // {
            //     return;
            // }
            //
            // var hurtable = collision.GetComponent<IHurtable>();
            // if (hurtable != null)
            // {
            //     hurtable.OnHurt(IsHypermode ? hyperAttackPoint : attackPoint);
            // }
            //
            // OnHit.Invoke(new WeaponHitData(this, hurtable));
        }

        protected override void OnAttack(WeaponCommandData data)
        {
            if (playerCtrl != null)
            {
                // playerCtrl.AttackingLock = true;
                throw new NotSupportedException();
            }
        }

        protected override void OnAttackFinish(WeaponCommandData data)
        {
            if (playerCtrl != null)
            {
                // playerCtrl.AttackingLock = false;
                throw new NotSupportedException();
            }
        }

        protected override void OnTransformRebind(Transform target)
        {
            playerCtrl = target?.GetComponentInParent<NormalPlayerCtrl>();
        }

        protected override void OnHypermodeSwitch(bool enabled)
        {
            if (enabled)
            {
                attackRange.enabled = false;
                hyperAttackRange.enabled = true;
            }
            else
            {
                attackRange.enabled = true;
                hyperAttackRange.enabled = false;
            }
        }
    }
}
