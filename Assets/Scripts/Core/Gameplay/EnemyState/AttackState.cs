using DG.Tweening;

using System;

using UnityEngine;

using UObject = UnityEngine.Object;

namespace Shuile.Gameplay.EnemyState
{
    public class AttackState : State
    {
        private PlayerController playerCtrl;
        private bool isPrepareAttack;

        public AttackState(Enemy enemy) : base(enemy)
        {
            playerCtrl = UObject.FindObjectOfType<PlayerController>();  // TODO: 暂时这样获取
        }

        public override void Judge()
        {
            var dst = Math.Abs(Mathf.RoundToInt(playerCtrl.transform.position.x) - enemy.Position);

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
                enemy.transform.GetChild(0).GetComponent<SpriteRenderer>().DOColor(Color.green, 0.1f).OnComplete(
                    () => enemy.transform.GetChild(0).GetComponent<SpriteRenderer>().DOColor(Color.white, 0.1f));
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
            enemy.transform.GetChild(0).GetComponent<SpriteRenderer>().DOColor(Color.yellow, 0.1f);
        }
    }
}
