using CbUtils;

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
        [SerializeField] private float explodeRadius = 2f;
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

        protected override void RegisterState(FSM<EntityStateType> fsm)
        {
            fsm.AddState(EntityStateType.Spawn, new SpawnState(this));
            var attackState = new CommonEnemyAttackState(this, Attack, InterruptAttack);
            fsm.AddState(EntityStateType.Idle, attackState);
            fsm.AddState(EntityStateType.Attack, attackState);
            fsm.AddState(EntityStateType.Dead, new DeadState(this));
        }

        private bool Attack(CommonEnemyAttackState state)
        {
            if (state.counter == 1)
            {
                for (var i = 0; i < bombCount; i++)
                {
                    var pos = LevelZoneManager.Instance.RandomValidPosition();
                    var bomb = Instantiate(bombPrefab, pos, Quaternion.identity, BombParent).GetComponent<Bomb>();
                    bombs.Add(bomb);
                }
            }
            if (state.counter < explosionDelay)
                return true;

            foreach (var bomb in bombs)
                bomb.Explode(Property.attackPoint, explodeRadius);

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
