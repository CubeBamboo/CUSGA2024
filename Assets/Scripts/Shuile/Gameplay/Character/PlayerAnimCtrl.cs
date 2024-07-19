using DG.Tweening;
using Shuile.Gameplay.Weapon;
using Shuile.MonoGadget;
using UnityEngine;

namespace Shuile.Gameplay.Character
{
    public class PlayerAnimCtrl
    {
        public enum AnimTrigger
        {
            Die,
            Hurt,

            Run
            //AttackStart,
            //AttackStop,
            //Jump,
            //Land,
        }

        private readonly Animator _animator;

        private PlayerModel _playerModel;
        private readonly SpriteRenderer _spriteRenderer;
        private GameObject _target;

        public PlayerAnimCtrl(GameObject go, PlayerModel playerModel)
        {
            _target = go;
            _spriteRenderer = go.GetComponentInChildren<SpriteRenderer>();
            _animator = go.GetComponentInChildren<Animator>();
            if (_animator.gameObject.GetComponent<AttackingUnlocker>() is AttackingUnlocker unlocker)
            {
                unlocker.ctrl = go.GetComponent<NormalPlayerCtrl>();
            }

            _playerModel = playerModel;
        }

        public bool FlipX
        {
            set => _spriteRenderer.flipX = value;
        }

        public float XVelocity
        {
            set => _animator.SetFloat("XAbsSpeed", value * (value >= 0 ? 1 : -1));
        }

        public bool Inviciable
        {
            set => _animator.SetBool("IsInviciable", value);
        }

        public void AnimControlUpdate()
        {
        }

        public void Trigger(AnimTrigger trigger)
        {
            switch (trigger)
            {
                //case AnimTrigger.AttackStart:
                //    Debug.LogWarning($"Use {nameof(TriggerAttackWithType)} to trigger attack animation");
                //    TriggerAttackWithType(true, WeaponType.Sword);
                //    break;
                //case AnimTrigger.AttackStop:
                //    Debug.LogWarning($"Use {nameof(TriggerAttackWithType)} to trigger attack animation");
                //    TriggerAttackWithType(true, WeaponType.Sword);
                //    break;
                case AnimTrigger.Hurt:
                    TriggerHurt();
                    break;
                case AnimTrigger.Run:
                    if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
                    {
                        _animator.SetTrigger("ForceRun");
                    }

                    break;
                default:
                    _animator.SetTrigger(trigger.ToString());
                    break;
            }
        }

        private void TriggerHurt()
        {
            _spriteRenderer.color = Color.white;
            _spriteRenderer.DOColor(new Color(230f / 255f, 73f / 255f, 73f / 255f), 0.2f).OnComplete(() =>
                _spriteRenderer.DOColor(Color.white, 0.2f));
        }

        public void TriggerAttackWithType(bool start, WeaponType type)
        {
            _animator.SetInteger("WeaponType", (int)type);
            _animator.SetBool("Attack", start);
        }
    }
}
