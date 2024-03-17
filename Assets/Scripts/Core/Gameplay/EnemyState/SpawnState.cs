using DG.Tweening;

using UnityEngine;

namespace Shuile.Gameplay.EnemyState
{
    public class SpawnState : State
    {
        private bool spawned;

        public SpawnState(Enemy enemy) : base(enemy)
        {
        }

        public override void Judge()
        {
            if (!spawned)
            {
                spawned = true;
                enemy.transform.GetChild(0).DOScale(Vector3.one, 0.1f);
                return;
            }
            enemy.GotoState(enemy.idleState);
            enemy.CurrentState.Judge();
        }

        public override void EnterState()
        {
            enemy.transform.GetChild(0).localScale = Vector3.zero;
            spawned = false;
        }
    }
}
