using UnityEngine;

namespace Shuile
{
    public class PlayerAnimCtrl
    {
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;

        public bool FlipX
        {
            set
            {
                _spriteRenderer.flipX = value;
            }
        }

        public float XVelocity
        {
            set
            {
                _animator.SetFloat("XAbsSpeed", value * (value >= 0 ? 1 : -1));
            }
        }

        public enum AnimTrigger
        {
            Attack,
            Jump,
            Land,
            Die
        }

        public PlayerAnimCtrl(GameObject go)
        {
            _spriteRenderer = go.GetComponentInChildren<SpriteRenderer>();
            _animator = go.GetComponentInChildren<Animator>();
        }

        public void AnimControlUpdate()
        {

        }

        public void Trigger(AnimTrigger trigger)
        {
            _animator.SetTrigger(trigger.ToString());
        }
    }
}
