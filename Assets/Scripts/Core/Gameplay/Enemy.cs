using Shuile.Gameplay.EnemyState;

using System;

using UnityEngine;

namespace Shuile.Gameplay
{
    public class Enemy : MonoBehaviour, IAttackable
    {
        [SerializeField] private EnemyPropertySO property;
        private int health;
        private State currentState;

        public readonly State dieState;
        public readonly State idleState;
        public readonly State attackState;

        public int Health => health;
        public State CurrentState => currentState;
        public EnemyPropertySO Property => property;

        public Enemy()
        {
            dieState = null;
            idleState = new IdleState(this);
            attackState = new AttackState(this);
        }

        private void Awake()
        {
            health = property.healthPoint;
        }

        private void Start()
        {
            GotoState(idleState);
        }

        public void OnAttack(int attackPoint)
        {
            if (health <= 0)
                return;

            health = Mathf.Max(0, health - attackPoint);
            if (health == 0)
            {
                GotoState(dieState);
            }
        }

        public void GotoState(State state)
        {
            if (state == null)
            {
                OnAttack(health);  // 寄掉
#if DEBUG
                throw new ArgumentNullException(nameof(state));
#else
                return;
#endif
            }    
            currentState.ExitState();
            currentState = state;
            currentState.EnterState();
        }
    }
}
