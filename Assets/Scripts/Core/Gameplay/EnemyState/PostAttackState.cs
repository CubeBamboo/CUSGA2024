using DG.Tweening;

using System;

using UnityEngine;

using UObject = UnityEngine.Object;

namespace Shuile.Gameplay.EnemyState
{
    public class PostAttackState : State
    {
        private int duration;

        public override void Judge()
        {
            if (BindEnemy.Property.postAttackDuration == 0)
            {
                GotoState(BindEnemy.idleState);
                return;
            }

            if (duration == 0)
                BindEnemy.transform.GetChild(0).GetComponent<SpriteRenderer>().DOColor(Color.yellow, 0.1f);

            duration++;
            if (duration >= BindEnemy.Property.postAttackDuration)
                GotoState(BindEnemy.idleState);
        }

        public override void EnterState()
        {
            base.EnterState();
            duration = 0;
            if (BindEnemy.Property.preAttackDuration == 0)
                GotoState(BindEnemy.idleState);
        }

        public override void Rebind(Enemy newEnemy)
        {
            base.Rebind(newEnemy);

            duration = 0;
        }
    }
}
