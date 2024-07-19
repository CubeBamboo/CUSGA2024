using Shuile.Core.Framework;
using Shuile.Core.Gameplay.Common;
using Shuile.Gameplay.Event;
using Shuile.Gameplay.Move;
using System;
using UnityEngine;

namespace Shuile.Gameplay.Entity
{
    /// <summary> base class for enemy </summary>
    public abstract class Enemy : MonoBehaviour, IHurtable, IJudgeable
    {
        [SerializeField] protected int MaxHealth = 100;

        private EnemyHurtEvent enemyHurtEvent;
        protected int health;
        protected SmoothMoveCtrl moveController;

        public int Health => health;
        public bool IsAlive => health > 0;
        public SmoothMoveCtrl MoveController => moveController;

        protected void Awake()
        {
            TypeEventSystem.Global.Trigger<EnemySpawnEvent>(new EnemySpawnEvent { enemy = gameObject });
            enemyHurtEvent = new EnemyHurtEvent { enemy = gameObject };
            health = MaxHealth;
            moveController = GetComponent<SmoothMoveCtrl>();

            OnAwake();
        }

        public virtual void OnHurt(int attackPoint)
        {
            if (Health <= 0)
            {
                return;
            }

            var oldVal = health;
            health = Mathf.Max(0, health - attackPoint);
            OnSelfHurt(oldVal, health);
            TypeEventSystem.Global.Trigger(enemyHurtEvent);

            if (Health == 0)
            {
                OnSelfDie();
                HandleDieEvent();
            }
        }

        public abstract void Judge(int frame, bool force);

        public event Action<int> OnHpChangedEvent = _ => { };

        protected virtual void OnAwake() { }

        private void HandleDieEvent()
        {
            TypeEventSystem.Global.Trigger<EnemyDieEvent>(new EnemyDieEvent { enemy = gameObject });
        }

        protected abstract void OnSelfHurt(int oldVal, int newVal);
        protected abstract void OnSelfDie();
    }
}
