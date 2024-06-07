using CbUtils;

using Shuile.Framework;

using UnityEngine;
using UnityEngine.Events;

namespace Shuile.Gameplay.Weapon
{
    public sealed class NoWeapon : IWeapon
    {
        private Transform transform;  // To debug

        private Transform Transform
        {
            get
            {
                if (transform == null)
                    transform = new GameObject($"{nameof(NoWeapon)}").transform;
                return transform;
            }
        }

        public BaseCommand<WeaponCommandData> AttackCommand { get; } = new EmptyCommand<WeaponCommandData>();
        public BaseCommand<WeaponCommandData> AttackFinishCommand { get; } = new EmptyCommand<WeaponCommandData>();
        public BaseCommand<bool> HypermodeSwitchCommand { get; } = new EmptyCommand<bool>();
        public WeaponType Type => WeaponType.Sword;
        public EasyEvent<WeaponHitData> OnHit { get; } = new();

        public void BindToTransform(Transform target)
        {
            Transform.parent = target;
            Transform.gameObject.SetActive(target != null);
        }

        public class EmptyCommand<TData> : BaseCommand<TData> where TData : struct
        {
            public override void OnExecute()
            {
            }
        }
    }
}