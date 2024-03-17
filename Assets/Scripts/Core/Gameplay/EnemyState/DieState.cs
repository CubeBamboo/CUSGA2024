using DG.Tweening;

using UnityEngine;

namespace Shuile.Gameplay.EnemyState
{
    public class DieState : State
    {
        public DieState(Enemy enemy) : base(enemy)
        {
        }

        public override void Judge()
        {
            enemy.transform.GetChild(0).DOScale(Vector3.zero, 0.1f);
            // Do something
            // e.g 加分
            EnemyManager.Instance.RemoveEnemy(enemy);
        }
    }
}
