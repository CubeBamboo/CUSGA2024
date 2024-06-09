using Shuile.Gameplay.Event;

using System;

using UnityEngine;

using UObject = UnityEngine.Object;
using Cysharp.Threading.Tasks;
using Shuile.Core.Framework;
using Shuile.Gameplay.Entity;
using Shuile.Gameplay.Move;
using Shuile.UI.Gameplay;

namespace Shuile.Gameplay
{
    /// <summary> base class for enemy </summary>
    public abstract class Enemy : MonoBehaviour, IHurtable, IJudgeable, IEntity
    {
        [SerializeField] protected int MaxHealth = 100;
        protected SmoothMoveCtrl moveController;
        protected int health;

        public int Health => health;
        public bool IsAlive => health > 0;
        public SmoothMoveCtrl MoveController => moveController;

        EnemyHurtEvent enemyHurtEvent;

        public event Action<int> OnHpChangedEvent = _ => { };
        private HUDHpBarElement hpBarUI;

        protected void Awake()
        {
            this.TriggerEvent<EnemySpawnEvent>(new() { enemy = gameObject });
            enemyHurtEvent = new() { enemy = gameObject };
            health = MaxHealth;
            moveController = GetComponent<SmoothMoveCtrl>();

            OnAwake();
        }

        protected virtual void OnDestroy()
        {
            if (hpBarUI) UObject.Destroy(hpBarUI.gameObject);
        }

        protected virtual void OnAwake() { }
        public virtual void OnHurt(int attackPoint)
        {
            if (Health <= 0)
                return;

            var oldVal = health;
            health = Mathf.Max(0, health - attackPoint);
            OnSelfHurt(oldVal, health);
            this.TriggerEvent<EnemyHurtEvent>(enemyHurtEvent);

            if (Health == 0)
            {
                OnSelfDie();
                HandleDieEvent();
            }
        }
        private void HandleDieEvent()
        {
            this.TriggerEvent<EnemyDieEvent>(new() { enemy = gameObject });
        }
        protected abstract void OnSelfHurt(int oldVal, int newVal);
        protected abstract void OnSelfDie();
        public abstract void Judge(int frame, bool force);

        public ModuleContainer GetModule() => GameApplication.Level;
    }
}
