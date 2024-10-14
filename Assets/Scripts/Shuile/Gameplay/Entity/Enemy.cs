using Shuile.Core.Framework;
using Shuile.Core.Gameplay.Common;
using Shuile.Framework;
using Shuile.Gameplay.Character;
using Shuile.Gameplay.Event;
using Shuile.Gameplay.Move;
using System;
using UnityEngine;

namespace Shuile.Gameplay.Entity
{
    /// <summary>
    /// handle enemy's common dependencies. use event system to communicate with outer world.
    /// </summary>
    public abstract class Enemy : MonoContainer, IHurtable, IJudgeable
    {
        [SerializeField] protected int MaxHealth = 100;

        protected int health;
        protected SmoothMoveCtrl moveController;
        private Player _player;

        private EnemyHurtEvent enemyHurtEvent;

        public override void BuildSelfContext(RuntimeContext context)
        {
            context.RegisterInstance(GetComponent<Rigidbody2D>());
            context.RegisterInstance(transform);
            context.RegisterMonoScheduler(this);
            context.RegisterInstance(moveController);
        }

        public override void LoadFromParentContext(IReadOnlyServiceLocator context)
        {
            base.LoadFromParentContext(context);
            if (!context.TryGet(out GamePlayScene gamePlayScene) || !gamePlayScene.TryGetPlayer(out _player))
            {
                Debug.Log("Player not found. player feature will be disabled.");
            }
        }

        /// <summary>
        ///  player can be null
        /// </summary>
        protected bool TryGetPlayer(out Player player)
        {
            player = _player;
            return _player;
        }

        protected Player GetPlayerOrThrow() => TryGetPlayer(out var player) ? player : throw new InvalidOperationException("Player not found but you are trying to access it.");

        public int Health => health;
        public bool IsAlive => health > 0;
        public SmoothMoveCtrl MoveController => moveController;

        public override void Awake()
        {
            base.Awake();
            moveController = new SmoothMoveCtrl(Context);
            enemyHurtEvent = new EnemyHurtEvent { enemy = this };
            health = MaxHealth;
            OnAwake();
            TypeEventSystem.Global.Trigger<EnemySpawnEvent>(new EnemySpawnEvent { enemy = this });
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

        protected virtual void OnAwake() { }

        private void HandleDieEvent()
        {
            TypeEventSystem.Global.Trigger<EnemyDieEvent>(new EnemyDieEvent { enemy = this });
        }

        protected abstract void OnSelfHurt(int oldVal, int newVal);
        protected abstract void OnSelfDie();
    }
}
