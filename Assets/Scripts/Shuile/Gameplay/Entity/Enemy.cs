using Shuile.Core.Framework;
using Shuile.Core.Gameplay.Common;
using Shuile.Gameplay.Event;
using Shuile.Gameplay.Move;
using Shuile.UI.Gameplay;
using System;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace Shuile.Gameplay.Entity
{
    /// <summary> base class for enemy </summary>
    public abstract class Enemy : MonoBehaviour, IHurtable, IJudgeable
    {
        [SerializeField] protected int MaxHealth = 100;
        protected SmoothMoveCtrl moveController;
        protected int health;

        public int Health => health;
        public bool IsAlive => health > 0;
        public SmoothMoveCtrl MoveController => moveController;

        EnemyHurtEvent enemyHurtEvent;

        public event Action<int> OnHpChangedEvent = _ => { };

        protected void Awake()
        {
            TypeEventSystem.Global.Trigger<EnemySpawnEvent>(new() { enemy = gameObject });
            enemyHurtEvent = new() { enemy = gameObject };
            health = MaxHealth;
            moveController = GetComponent<SmoothMoveCtrl>();

            OnAwake();
        }

        protected virtual void OnAwake() { }
        public virtual void OnHurt(int attackPoint)
        {
            if (Health <= 0)
                return;

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
        private void HandleDieEvent()
        {
            TypeEventSystem.Global.Trigger<EnemyDieEvent>(new() { enemy = gameObject });
        }
        protected abstract void OnSelfHurt(int oldVal, int newVal);
        protected abstract void OnSelfDie();
        public abstract void Judge(int frame, bool force);
    }
}
