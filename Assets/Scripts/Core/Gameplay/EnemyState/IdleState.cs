using System;

using UnityEngine;

using UObject = UnityEngine.Object;

namespace Shuile.Gameplay.EnemyState
{
    public class IdleState : State
    {
        private PlayerController playerCtrl;
        private int moveSleep;

        public override void Judge()
        {
            var dst = Mathf.Abs(playerCtrl.transform.position.x - BindEnemy.Position);
            if (dst <= BindEnemy.Property.attackRange)
            {
                BindEnemy.GotoState(BindEnemy.attackState);
                return;
            }

            if (moveSleep != 0)
            {
                moveSleep--;
                return;
            }
            if (dst <= BindEnemy.Property.viewRange)
            {
                BindEnemy.Position += Math.Sign(Mathf.RoundToInt(playerCtrl.transform.position.x) - BindEnemy.Position);
                moveSleep = BindEnemy.Property.moveInterval;
            }
        }

        public override void EnterState()
        {
            moveSleep = 0;
            // moveSleep = BindEnemy.Property.moveInterval;
        }

        public override void Rebind(Enemy newEnemy)
        {
            base.Rebind(newEnemy);

            playerCtrl = UObject.FindObjectOfType<PlayerController>();  // TODO: 暂时这样获取
        }
    }
}
