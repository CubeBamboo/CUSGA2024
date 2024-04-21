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

        private void Start()
        {
            transform.GetChild(0).transform.DOScale(0f, 0.2f).From();
            gameObject.SetOnDestroy(() => transform.DOKill(), "transform");
        }

        public void Explode(int attackPoint, float radius)
        {
            //var gridPosition = LevelGrid.Instance.grid.WorldToCell(transform.position);
            //var grid = LevelGrid.Instance.grid;
            //grid.Get(gridPosition)?.GetComponent<IHurtable>()?.OnAttack(attackPoint);

            var hurtables = Physics2D.OverlapCircleAll(transform.position, radius, hurtMask)
                .Select(collider => collider.GetComponent<IHurtable>())
                .Where(hurtable => hurtable != null);

            foreach (var hurtable in hurtables)
                hurtable.OnAttack(attackPoint);

            // play anim
            transform.GetChild(0).transform.DOScale(2f, 0.2f).From()
                .OnComplete(() => Destroy(this.gameObject));
            gameObject.SetOnDestroy(() => transform.DOKill(), "transform");
            //MonoAudioCtrl.Instance.PlayOneShot("Enemy_Bomb", 0.6f);
        }

        public void Interrupt()
        {
            // play anim
            transform.GetChild(0).transform.DOScale(6f, 0.2f).From()
                .OnComplete(() => Destroy(this.gameObject));
            gameObject.SetOnDestroy(() => transform.DOKill(), "transform");
        }
    }
}
