/*using CbUtils;

using Shuile.Framework;
using Shuile.Gameplay.Weapon;
using Shuile.Rhythm.Runtime;
using Shuile.Root;

using UnityEngine;

namespace Shuile.Gameplay
{
    public class NormalPlayerAttackCtrl : MonoBehaviour
    {
        [SerializeField] private Transform handTransform;
        private NormalPlayerInput mPlayerInput;
        private PlayerModel playerModel;

        private NormalPlayerAttackCtrl()
        {
            AttackCommand = new DelegateCommand<WeaponCommandData>(WeaponAttackWrapper);
            AttackFinishCommand = new DelegateCommand<WeaponCommandData>(WeaponAttackFinishWrapper);
        }


        public bool FlipX { get; set; }
        public bool CheckRhythm =>
            MusicRhythmManager.Instance.CheckBeatRhythm(
                MusicRhythmManager.Instance.CurrentTime, out playerModel.currentHitOffset);
        public Vector2 AttackDir => FlipX ? Vector2.left : Vector2.right;

        private void Awake()
        {
            CurrentWeapon = new NoWeapon();
            mPlayerInput = GetComponent<NormalPlayerInput>();
            playerModel = GameplayService.Interface.Get<PlayerModel>();
        }

        private void OnEnable()
        {
            mPlayerInput.OnAttack += Attack;
            mPlayerInput.OnAttackFinish += AttackFinish;
        }

        private void OnDisable()
        {
            mPlayerInput.OnAttack -= Attack;
            mPlayerInput.OnAttackFinish -= AttackFinish;
        }

        private void Attack()
        {
            if (LevelRoot.Instance.needHitWithRhythm && !CheckRhythm) return;

            AttackCommand
                .Bind(new(transform.position, AttackDir))
                .Execute();
        }

        // set public to allow animator callback
        public void AttackFinish()
        {
            AttackFinishCommand
                .Bind(new(transform.position, AttackDir))
                .Execute();
        }

        private void WeaponAttackWrapper(WeaponCommandData data)
            => CurrentWeapon.AttackCommand.Bind(data).Execute();
        private void WeaponAttackFinishWrapper(WeaponCommandData data)
            => CurrentWeapon.AttackFinishCommand.Bind(data).Execute();

        public BaseCommand<WeaponCommandData> AttackCommand { get; private set; }
        public BaseCommand<WeaponCommandData> AttackFinishCommand { get; private set; }
    }
}*/


