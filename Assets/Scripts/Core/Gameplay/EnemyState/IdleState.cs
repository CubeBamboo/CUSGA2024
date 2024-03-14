using UnityEngine;

namespace Shuile.Gameplay.EnemyState
{
    public class IdleState : State
    {
        private PlayerController playerCtrl;
        private int moveSleep;

        public IdleState(Enemy enemy) : base(enemy)
        {
            playerCtrl = Object.FindObjectOfType<PlayerController>();  // TODO: 暂时这样获取
        }

        public override void Judge()
        {
            var dst = Mathf.Abs(playerCtrl.transform.position.x - enemy.transform.position.x);
            if (dst <= enemy.Property.attackRange)
            {
                enemy.GotoState(enemy.attackState);
                return;
            }

            if (moveSleep != 0)
            {
                moveSleep--;
                return;
            }
            if (dst <= enemy.Property.viewRange)
            {
                enemy.transform.Translate((playerCtrl.transform.position - enemy.transform.position).normalized, Space.World);
                moveSleep = enemy.Property.moveInterval;
            }
        }

        public override void EnterState()
        {
            moveSleep = 0;
            // moveSleep = enemy.Property.moveInterval;
        }
    }
}
