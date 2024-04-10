using UnityEngine;
using DG.Tweening;

namespace Shuile
{
    public class PlayerAnimCtrl
    {
        private GameObject _target;
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
            Die,
            Hurt
        }

        public PlayerAnimCtrl(GameObject go)
        {
            _target = go;
            _spriteRenderer = go.GetComponentInChildren<SpriteRenderer>();
            _animator = go.GetComponentInChildren<Animator>();
        }

        public void AnimControlUpdate()
        {

        }

        public void Trigger(AnimTrigger trigger)
        {
            // TODO: shit
            if (trigger == AnimTrigger.Hurt)
            {
                // 效果反馈
                _spriteRenderer.color = Color.white;
                _spriteRenderer.DOColor(new Color(230f / 255f, 73f / 255f, 73f / 255f), 0.2f).OnComplete(() =>
                    _spriteRenderer.DOColor(Color.white, 0.2f));
                var initPos = _target.transform.position;
                _target.transform.DOShakePosition(0.2f, strength: 0.2f).OnComplete(() =>
                        _target.transform.position = initPos);
                return;
            }

            _animator.SetTrigger(trigger.ToString());
        }
    }
}
