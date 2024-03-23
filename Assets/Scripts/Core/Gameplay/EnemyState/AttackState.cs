using DG.Tweening;

using System;

using UnityEngine;

using UObject = UnityEngine.Object;

namespace Shuile.Gameplay.EnemyState
{
    public class AttackState : State
    {
        private PlayerController playerCtrl;
        private State nextState = new PostAttackState();

        public override void Judge()
        {
            var dst = Math.Abs(Mathf.RoundToInt(playerCtrl.transform.position.x) - BindEnemy.Position);

            BindEnemy.transform.GetChild(0).GetComponent<SpriteRenderer>().DOColor(Color.green, 0.1f).OnComplete(
                    () => BindEnemy.transform.GetChild(0).GetComponent<SpriteRenderer>().DOColor(Color.white, 0.1f));
            if (dst <= BindEnemy.Property.attackRange)
            {
                //if (弹刀)
                if (false)
                {
                    // 通知弹刀成功
                    // 弹刀动画
                }
                else
                {
                    playerCtrl.OnAttack(BindEnemy.Property.attackPoint);
                }
                GotoState(nextState);
            }
        }

        public override void Rebind(Enemy newEnemy)
        {
            base.Rebind(newEnemy);

            playerCtrl = UObject.FindObjectOfType<PlayerController>();  // TODO: 暂时这样获取
            nextState.Rebind(newEnemy);
        }
    }
}
