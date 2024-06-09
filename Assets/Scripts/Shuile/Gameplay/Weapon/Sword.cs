using Shuile.Gameplay.Character;
using System.Linq;

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

        protected override void OnAttack(WeaponCommandData data)
        {
            if (playerCtrl != null) playerCtrl.AttackingLock = true;
        }

        protected override void OnAttackFinish(WeaponCommandData data)
        {
            if (playerCtrl != null) playerCtrl.AttackingLock = false;
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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //Debug.Log(collision.name);
            if (!playerCtrl.AttackingLock || collision.CompareTag("Player"))  // 防止紫砂
                return;

            var hurtable = collision.GetComponent<IHurtable>();
            if (hurtable != null)
                hurtable.OnHurt(IsHypermode ? hyperAttackPoint : attackPoint);
            OnHit.Invoke(new WeaponHitData(this, hurtable));
        }
    }
}
