using CbUtils;

using Shuile.Framework;

using UnityEngine;
using UnityEngine.Events;

namespace Shuile.Gameplay
{
    public enum WeaponType
    {
        Sword,
        Gun
    }

    public interface IWeapon
    {
        public WeaponType Type { get; }
        public EasyEvent<WeaponHitData> OnHit { get; }
        public BaseCommand<WeaponCommandData> AttackCommand { get; }
        public BaseCommand<WeaponCommandData> AttackFinishCommand { get; }
        public BaseCommand<bool> HypermodeSwitchCommand { get; }
        public void BindToTransform(Transform target);
    }

    public abstract class BaseWeapon : MonoBehaviour, IWeapon
    {
        private bool attacking = false;

        public BaseCommand<WeaponCommandData> AttackCommand { get; private set; }
        public BaseCommand<WeaponCommandData> AttackFinishCommand { get; private set; }
        public BaseCommand<bool> HypermodeSwitchCommand { get; private set; }
        public EasyEvent<WeaponHitData> OnHit { get; } = new();
        public bool IsHypermode { get; private set; }
        public abstract WeaponType Type { get; }

        public BaseWeapon()
        {
            AttackCommand = new DelegateCommand<WeaponCommandData>(OnAttack);
            AttackFinishCommand = new DelegateCommand<WeaponCommandData>(OnAttackFinish);
            HypermodeSwitchCommand = new DelegateCommand<bool>(OnHypermodeSwitch);
        }

        protected abstract void OnAttack(WeaponCommandData data);
        protected abstract void OnAttackFinish(WeaponCommandData data);
        protected abstract void OnHypermodeSwitch(bool enabled);
        protected virtual void OnTransformRebind(Transform target) { }

        public void BindToTransform(Transform target)
        {
            transform.parent = target;
            gameObject.SetActive(target != null);
            OnTransformRebind(target);
        }
    }

    public struct WeaponCommandData
    {
        public Vector2 position;
        public Vector2 dir;

        public WeaponCommandData(Vector2 position, Vector2 dir)
        {
            this.position = position;
            this.dir = dir;
        }
    }

    public struct WeaponHitData
    {
        public IWeapon weapon;
        public IHurtable hurtable;

        public WeaponHitData(IWeapon weapon, IHurtable hurtable)
        {
            this.weapon = weapon;
            this.hurtable = hurtable;
        }
    }
}