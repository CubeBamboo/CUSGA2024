using CbUtils;

using DG.Tweening;

using UnityEngine;

namespace Shuile.Gameplay.Entity
{
    public class Bomb : MonoBehaviour
    {
        private void Start()
        {
            transform.GetChild(0).transform.DOScale(0f, 0.2f).From();
        }

        public void Explode(int attackPoint)
        {
            var gridPosition = LevelGrid.Instance.grid.WorldToCell(transform.position);
            var grid = LevelGrid.Instance.grid;
            grid.Get(gridPosition)?.GetComponent<IAttackable>()?.OnAttack(attackPoint);
            var player = GameplayService.Interface.Get<PlayerController>();
            if (grid.WorldToCell(player.transform.position) == gridPosition)  // 因为现在玩家不在grid里，所以要这么做
                player.OnAttack(attackPoint);

            // play anim
            transform.GetChild(0).transform.DOScale(6f, 0.2f).From().OnComplete(() => Destroy(this.gameObject));
        }

        public void Interrupt()
        {
            // play anim
            transform.GetChild(0).transform.DOScale(6f, 0.2f).From().OnComplete(() => Destroy(this.gameObject));
        }
    }
}
