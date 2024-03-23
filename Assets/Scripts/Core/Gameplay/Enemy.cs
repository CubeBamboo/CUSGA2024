using DG.Tweening;

using Shuile.Gameplay.EnemyState;

using System;

using UnityEngine;

namespace Shuile.Gameplay
{
    public class Enemy : MonoBehaviour, IAttackable
    {
        [SerializeField] private EnemyPropertySO property;
        [SerializeField] private bool isPreset = false;
        private int health;
        private State currentState;
        private int position;

        public State dieState;
        public State idleState;
        public State attackState;

        public int Health => health;
        public State CurrentState => currentState;
        public EnemyPropertySO Property => property;
        public bool IsAlive => health > 0;
        public int Position
        {
            get => position;
            set
            {
                if (position == value || !EnemyManager.Instance.IsValidPosition(value))
                    return;
                EnemyManager.Instance.UpdateEnemyPosition(this, value);
                transform.DOMoveX(value, 0.1f);
                position = value;
            }
        }

        private void Awake()
        {
            health = property.healthPoint;
            idleState = EnemyStateHelper.CreateAndBind<IdleState>(this);
            attackState = EnemyStateHelper.CreateAndBind<PreAttackState>(this);
            dieState = EnemyStateHelper.CreateAndBind<DieState>(this);
        }

        private void Start()
        {
            if (isPreset)
            {
                position = Mathf.RoundToInt(transform.position.x);
                EnemyManager.Instance.UpdateEnemyPosition(this, position);
            }
            GotoState(EnemyStateHelper.CreateAndBind<SpawnState>(this));
        }

        public void JudgeUpdate()
        {
            CurrentState?.Judge();
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
            currentState?.ExitState();
            currentState = state;
            currentState.EnterState();
        }
    }
}
