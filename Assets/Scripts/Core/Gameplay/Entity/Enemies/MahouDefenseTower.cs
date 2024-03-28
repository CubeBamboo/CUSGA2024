using Shuile.Gameplay.Entity.States;

using System;

using UnityEngine;

using URandom = UnityEngine.Random;

namespace Shuile.Gameplay.Entity.Enemies
{
    public class MahouDefenseTower : Enemy
    {
        [SerializeField] private int bombCount;
        [SerializeField] private GameObject bombPrefab;

        private static Lazy<Transform> BombParent { get; } = new(() => new GameObject("Bombs").transform, false);

        protected override void RegisterState()
        {
            states.Add(EntityStateType.Spawn, new SpawnState(this));
            states.Add(EntityStateType.Idle, new EnemyIdleState(this));
            states.Add(EntityStateType.Attack, new CommonEnemyAttackState(this, Attack));
            states.Add(EntityStateType.Dead, new DeadState(this));
        }

        private bool Attack(CommonEnemyAttackState state)
        {
            for (var i = 0; i < bombCount; i++)
            {
                var pos = new Vector3(URandom.Range(0, LevelGrid.Instance.grid.Width), URandom.Range(0, LevelGrid.Instance.grid.Height));
                var bomb = Instantiate(bombPrefab, pos, Quaternion.identity, BombParent.Value);
                // bomb.GetComponent<Bomb>().delay = Property.postAttackDuration;
            }
            return false;
        }
    }
}
