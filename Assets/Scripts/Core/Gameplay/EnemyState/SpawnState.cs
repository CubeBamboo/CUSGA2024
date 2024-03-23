using DG.Tweening;

using UnityEngine;

namespace Shuile.Gameplay.EnemyState
{
    public class SpawnState : State
    {
        private bool spawned;

        public override void Judge()
        {
            if (!spawned)
            {
                spawned = true;
                BindEnemy.transform.GetChild(0).DOScale(Vector3.one, 0.1f);
                return;
            }
            BindEnemy.GotoState(BindEnemy.idleState);
            BindEnemy.CurrentState.Judge();
        }

        public override void EnterState()
        {
            BindEnemy.transform.GetChild(0).localScale = Vector3.zero;
            spawned = false;
        }
    }
}
