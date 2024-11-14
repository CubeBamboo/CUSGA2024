using Shuile.Core.Framework;
using Shuile.Core.Gameplay.Common;
using Shuile.Core.Gameplay.Data;
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
    public abstract class Enemy : MonoContainer, IHurtable, IJudgeable, IPooledObject
    {
        [SerializeField] protected int MaxHealth = 100;

        protected int health;
        protected SmoothMoveCtrl moveController;
        private Player _player;

        public EnemyType CurrentType { get; protected set; }

        private EnemyHurtEvent enemyHurtEvent;

        public Action<Enemy> DieFxEnd;

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
            if (!context.TryGetValue(out GamePlayScene gamePlayScene) || !gamePlayScene.TryGetPlayer(out _player))
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

            if (Health <= 0)
            {
                BeginDie();
                TypeEventSystem.Global.Trigger<EnemyDieEvent>(new EnemyDieEvent { enemy = this });
            }
        }

        public virtual void ForceDie()
        {
            OnHurt(MaxHealth);
        }

        public abstract void Judge(int frame, bool force);

        protected virtual void OnAwake() { }

        protected abstract void OnSelfHurt(int oldVal, int newVal);
        protected abstract void BeginDie();

        protected void EndDie()
        {
            DieFxEnd?.Invoke(this);
        }

        public virtual void GetFromPool()
        {
            health = MaxHealth;
            TypeEventSystem.Global.Trigger<EnemySpawnEvent>(new EnemySpawnEvent { enemy = this });
            gameObject.SetActive(true);
        }

        public virtual void ReleaseFromPool()
        {
            gameObject.SetActive(false);
        }

        public virtual void DestroyFromPool()
        {
            Destroy(gameObject);
        }
    }
}
