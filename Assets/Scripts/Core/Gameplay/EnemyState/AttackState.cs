using UnityEngine;

namespace Shuile.Gameplay.EnemyState
{
    public class AttackState : State
    {
        private PlayerController playerCtrl;
        private bool isPrepareAttack;

        public AttackState(Enemy enemy) : base(enemy)
        {
            playerCtrl = Object.FindObjectOfType<PlayerController>();  // TODO: 暂时这样获取
        }

        public override void Judge()
        {
            var dst = Mathf.Abs(playerCtrl.transform.position.x - enemy.transform.position.x);

            /* 当前是攻击前摇
             *   当前在攻击范围内
             *     当前是否弹刀 -> (弹刀 or 扣血) and 攻击动画
             *   当前不在攻击范围内
             *     攻击动画
             * 当前不是攻击前摇
             *   当前在攻击范围内
             *   
             *   可以继续拆成状态机
             */

            if (isPrepareAttack)
            {
                isPrepareAttack = false;
                // 播放攻击动画
                if (dst <= enemy.Property.attackRange)
                {
                    //if (弹刀)
                    if (false)
                    {
                        // 通知弹刀成功
                        // 弹刀动画
                    }
                    else
                    {
                        playerCtrl.OnAttack(enemy.Property.attackPoint);
                    }
                }
            }
            else
            {
                if (dst <= enemy.Property.attackRange)
                    PrepareAttack();
                else
                    enemy.GotoState(enemy.idleState);
            }
        }

        public override void EnterState()
        {
            PrepareAttack();
        }

        private void PrepareAttack()
        {
            isPrepareAttack = true;
            // TODO: 攻击前摇动画
        }
    }
}
