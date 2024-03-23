using UnityEngine;

using UObject = UnityEngine.Object;

namespace Shuile.Gameplay.EnemyState
{
    public class PreAttackState : State
    {
        private int duration;
        private State nextState = new AttackState();

        public override void Judge()
        {
            if (duration == 0)
                BindEnemy.transform.GetChild(0).GetComponent<SpriteRenderer>().DOColor(Color.yellow, 0.1f);

            duration++;
            if (duration >= BindEnemy.Property.preAttackDuration)
                GotoState(nextState);
        }

        public override void EnterState()
        {
            base.EnterState();

            duration = 0;
            if (BindEnemy.Property.preAttackDuration == 0)
                GotoState(nextState);
        }

        public override void Rebind(Enemy newEnemy)
        {
            base.Rebind(newEnemy);
            nextState.Rebind(newEnemy);
        }
    }
}
