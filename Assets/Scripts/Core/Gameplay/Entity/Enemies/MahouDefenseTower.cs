using Shuile.Gameplay.Entity.States;

using System.Collections.Generic;

using UnityEngine;

using URandom = UnityEngine.Random;

namespace Shuile.Gameplay.Entity.Enemies
{
    public class MahouDefenseTower : Enemy
    {
        [SerializeField] private int explosionDelay = 1;
        [SerializeField] private int bombCount;
        [SerializeField] private GameObject bombPrefab;
        private static Transform bombParent;
        private List<Bomb> bombs = new();

        public int ExplosionDelay
        {
            get => explosionDelay;
            set => explosionDelay = value;
        }

        public static Transform BombParent
        {
            get
            {
                if (bombParent == null)
                    bombParent = new GameObject("Bombs").transform;
                return bombParent;
            }
        }

        protected override void RegisterState()
        {
            states.Add(EntityStateType.Spawn, new SpawnState(this));
            var attackState = new CommonEnemyAttackState(this, Attack, InterruptAttack);
            states.Add(EntityStateType.Idle, attackState);
            states.Add(EntityStateType.Attack, attackState);
            states.Add(EntityStateType.Dead, new DeadState(this));
        }

        private bool Attack(CommonEnemyAttackState state)
        {
            if (state.counter == 1)
            {
                for (var i = 0; i < bombCount; i++)
                {
                    var pos = LevelGrid.Instance.grid.CellToWorld(new Vector3Int(URandom.Range(0, LevelGrid.Instance.grid.Width), URandom.Range(0, LevelGrid.Instance.grid.Height)));
                    var bomb = Instantiate(bombPrefab, pos, Quaternion.identity, BombParent).GetComponent<Bomb>();
                    bombs.Add(bomb);
                }
            }
            if (state.counter < explosionDelay)
                return true;

            foreach (var bomb in bombs)
                bomb.Explode(Property.attackPoint);

            bombs.Clear();
            return false;
        }

        private void InterruptAttack(CommonEnemyAttackState state)
        {
            foreach (var bomb in bombs)
                bomb.Interrupt();
            bombs.Clear();
        }
    }
}
