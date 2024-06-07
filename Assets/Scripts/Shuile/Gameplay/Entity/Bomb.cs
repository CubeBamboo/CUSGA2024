using CbUtils.ActionKit;
using CbUtils.Event;
using CbUtils.Extension;
using DG.Tweening;

using System.Linq;

using UnityEngine;

namespace Shuile.Gameplay.Entity
{
    public class Bomb : MonoBehaviour
    {
        [SerializeField] private LayerMask hurtMask;
        //private Animator animator;

        private Vector3 initScale;
        //float exitAnimDuration = 0.5f;

        private void Start()
        {
            //animator = GetComponentInChildren<Animator>();
            initScale = transform.localScale;
            transform.GetChild(0).transform.DOScale(0f, 0.2f).From();
            gameObject.SetOnDestroy(() => transform.DOKill(), "transform");
        }

        public void Explode(int attackPoint, float radius)
        {
            var hurtables = Physics2D.OverlapCircleAll(transform.position, radius, hurtMask)
                .Select(collider => collider.GetComponent<IHurtable>())
                .Where(hurtable => hurtable != null);

            foreach (var hurtable in hurtables)
                hurtable.OnHurt(attackPoint);

            // play effect
            DOTween.Sequence()
                .Append(transform.DOScale(initScale * 2f, 0.1f)
                    .SetLoops(2, LoopType.Yoyo))
                .AppendInterval(0.5f)
                .Append(transform.DOScale(0f, 0.6f)
                        .OnComplete(() =>
                        {
                            gameObject.Destroy();
                        }));
        }

        public void Interrupt()
        {
            // play anim
            //transform.GetChild(0).transform.DOScale(6f, 0.2f).From()
            //    .OnComplete(() => Destroy(this.gameObject));
            //gameObject.SetOnDestroy(() => transform.DOKill(), "transform");
            //animator.SetTrigger("Exit");

            transform.DOScale(0f, 0.6f)
                .OnComplete(() =>
                {
                    gameObject.Destroy();
                });
        }

    }
}
