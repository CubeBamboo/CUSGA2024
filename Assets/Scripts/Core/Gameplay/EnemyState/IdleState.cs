using System;

using UnityEngine;

using UObject = UnityEngine.Object;

namespace Shuile.Gameplay.EnemyState
{
    public class IdleState : State
    {
        private PlayerController playerCtrl;
        private int moveSleep;

        public IdleState(Enemy enemy) : base(enemy)
        {
            playerCtrl = UObject.FindObjectOfType<PlayerController>();  // TODO: 暂时这样获取
        }

        public override void Judge()
        {
            var dst = Mathf.Abs(playerCtrl.transform.position.x - enemy.Position);
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
                enemy.Position += Math.Sign(Mathf.RoundToInt(playerCtrl.transform.position.x) - enemy.Position);
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
