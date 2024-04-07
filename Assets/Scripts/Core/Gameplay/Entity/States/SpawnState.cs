using CbUtils;

using DG.Tweening;

using UnityEngine;

namespace Shuile.Gameplay.Entity.States
{
    public class SpawnState : IState
    {
        public Enemy enemy;

        public SpawnState(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public void Enter()
        {
            enemy.transform.GetChild(0).localScale = Vector3.zero;
        }

        public void Exit()
        {
        }

        public void FixedUpdate()
        {
        }

        public void Update()
        {
        }

        public void OnGUI()
        {
        }

        public bool Condition() => true;

        public void Custom()
        {
            enemy.transform.GetChild(0).DOScale(1f, 0.3f).SetEase(Ease.OutBounce);
            enemy.State = EntityStateType.Idle;
        }
    }
}
